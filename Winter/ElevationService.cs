#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;
using Windows.Win32.System.Services;
using Windows.Win32.System.Threading;
using Windows.Win32.System.StationsAndDesktops;
using Smx.SharpIO.Memory;
using System.ComponentModel;

namespace Smx.Winter;

/**
 * Inspired by https://github.com/nfedera/run-as-trustedinstaller/blob/master/run-as-trustedinstaller/main.cpp
 **/

public class ElevationService
{
    public ElevationService()
    {
        Util.EnablePrivilege(PInvoke.SE_DEBUG_NAME);
        Util.EnablePrivilege(PInvoke.SE_IMPERSONATE_NAME);
    }

    public static SafeFileHandle DuplicateTokenEx(
        SafeHandle hExistingToken,
        TOKEN_ACCESS_MASK dwDesiredAccess,
        SECURITY_ATTRIBUTES? lpTokenAttributes,
        SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
        TOKEN_TYPE TokenType
    )
    {
        PInvoke.DuplicateTokenEx(
            hExistingToken,
            dwDesiredAccess,
            lpTokenAttributes,
            ImpersonationLevel,
            TokenType,
            out var newToken
        );
        return newToken;
    }

    public static SafeFileHandle OpenProcessToken(SafeFileHandle hProc, TOKEN_ACCESS_MASK flags)
    {
        PInvoke.OpenProcessToken(hProc, flags, out var hToken);
        return hToken;
    }

    public static SafeFileHandle ImpersonateProcess(SafeFileHandle hProc)
    {
        using var hSystemToken = OpenProcessToken(hProc, (TOKEN_ACCESS_MASK)PInvoke.MAXIMUM_ALLOWED);

        var tokenAttributes = new SECURITY_ATTRIBUTES
        {
            bInheritHandle = false,
            nLength = (uint)Marshal.SizeOf<SECURITY_ATTRIBUTES>(),
            lpSecurityDescriptor = null
        };

        var hDupToken = DuplicateTokenEx(
            hSystemToken,
            (TOKEN_ACCESS_MASK)PInvoke.MAXIMUM_ALLOWED,
            tokenAttributes,
            SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation,
            TOKEN_TYPE.TokenImpersonation
        );
        if (hDupToken == null)
        {
            throw new Win32Exception();
        }

        if (!PInvoke.ImpersonateLoggedOnUser(hDupToken))
        {
            throw new Win32Exception();
        }

        return hDupToken;
    }


    public static void RunAsToken<T>(SafeFileHandle hToken, Action action)
    {
        RunAsToken(hToken, () =>
        {
            action();
            return 0;
        });
    }

    public static T RunAsToken<T>(SafeFileHandle hToken, Func<T> action)
    {
        if (!PInvoke.ImpersonateLoggedOnUser(hToken))
        {
            throw new Win32Exception();
        }

        var res = action();

        if (!PInvoke.RevertToSelf())
        {
            throw new Win32Exception();
        }
        return res;
    }

    public static T RunAsProcess<T>(uint dwProcessId, Func<T> action)
    {
        using var handle = GetProcessHandleForImpersonation(dwProcessId);
        using var token = ImpersonateProcess(handle);

        var res = action();

        if (!PInvoke.RevertToSelf())
        {
            throw new Win32Exception();
        }
        return res;
    }

    private string? GetCurrentWindowStation()
    {
        using var hWinsta = PInvoke.GetProcessWindowStation_SafeHandle();
        if (hWinsta == null) return null;


        uint size;
        unsafe
        {
            PInvoke.GetUserObjectInformation(
                hWinsta,
                USER_OBJECT_INFORMATION_INDEX.UOI_NAME,
                null, 0, &size);
        }
        using var buf = MemoryHGlobal.Alloc(size);
        unsafe
        {
            PInvoke.GetUserObjectInformation(
                hWinsta,
                USER_OBJECT_INFORMATION_INDEX.UOI_NAME,
                buf.Address.ToPointer(), size, null);
        }

        return Marshal.PtrToStringUni(buf.Address);
    }

    public void RunAsTrustedInstaller(string commandLine)
    {
        var commandLineBuf = new char[commandLine.Length + 1];
        commandLine.CopyTo(commandLineBuf);

        var commandLineSpan = commandLineBuf.AsSpan();

        using var tiToken = ImpersonateTrustedInstaller();

        var winsta = GetCurrentWindowStation();
        var desktop = $@"{winsta}\Default";
        using var lpDesktop = MemoryHGlobal.Alloc(Encoding.Unicode.GetByteCount(desktop));
        Marshal.Copy(Encoding.Unicode.GetBytes(desktop), 0, lpDesktop.Address, (int)lpDesktop.Size);

        var si = new STARTUPINFOW();
        unsafe
        {
            si.lpDesktop = new PWSTR((char*)lpDesktop.Address.ToPointer());
            PInvoke.CreateProcessWithToken(
                tiToken,
                CREATE_PROCESS_LOGON_FLAGS.LOGON_WITH_PROFILE,
                null,
                ref commandLineSpan,
                PROCESS_CREATION_FLAGS.CREATE_UNICODE_ENVIRONMENT,
                null,
                null,
                si, out var pi
            );
        }
    }


    uint StartTrustedInstallerService()
    {
        using var hSCManager = PInvoke.OpenSCManager(
            null,
            PInvoke.SERVICES_ACTIVE_DATABASE,
            (uint)GENERIC_ACCESS_RIGHTS.GENERIC_EXECUTE);

        using var hService = PInvoke.OpenService(
            hSCManager,
            "TrustedInstaller",
            (uint)(GENERIC_ACCESS_RIGHTS.GENERIC_READ | GENERIC_ACCESS_RIGHTS.GENERIC_EXECUTE));

        var status = new SERVICE_STATUS_PROCESS();
        var pBuf = MemoryMarshal.Cast<SERVICE_STATUS_PROCESS, byte>(
            MemoryMarshal.CreateSpan(ref status, 1));

        while (PInvoke.QueryServiceStatusEx(
            hService,
            SC_STATUS_TYPE.SC_STATUS_PROCESS_INFO,
            pBuf, out var bytesNeeded))
        {
            switch (status.dwCurrentState)
            {
                case SERVICE_STATUS_CURRENT_STATE.SERVICE_STOPPED:
                    PInvoke.StartService(hService, null);
                    break;
                case SERVICE_STATUS_CURRENT_STATE.SERVICE_START_PENDING:
                case SERVICE_STATUS_CURRENT_STATE.SERVICE_STOP_PENDING:
                    Thread.Sleep((int)status.dwWaitHint);
                    break;
                case SERVICE_STATUS_CURRENT_STATE.SERVICE_RUNNING:
                    return status.dwProcessId;
            }
        }

        throw new InvalidOperationException("Failed to start TrustedInstaller");
    }

    public static SafeFileHandle GetProcessHandleForImpersonation(uint dwProcessId)
    {
        return PInvoke.OpenProcess_SafeHandle(0
            | PROCESS_ACCESS_RIGHTS.PROCESS_DUP_HANDLE
            | PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_INFORMATION,
            false, dwProcessId);
    }

    public SafeHandle ImpersonateSystem()
    {
        using var winlogon = Process.GetProcessesByName("winlogon").FirstOrDefault();
        if (winlogon == null)
        {
            throw new InvalidOperationException();
        }

        using var hProc = GetProcessHandleForImpersonation((uint)winlogon.Id);
        if (hProc == null)
        {
            throw new InvalidOperationException();
        }

        return ImpersonateProcess(hProc);
    }

    public SafeHandle ImpersonateTrustedInstaller()
    {
        using var _ = ImpersonateSystem();

        var pid = StartTrustedInstallerService();
        using var hProc = GetProcessHandleForImpersonation(pid);
        return ImpersonateProcess(hProc);
    }

}
