#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.Foundation;
using Windows.Win32.System.Memory;
using System.Runtime.CompilerServices;
using Smx.SharpIO.Memory;

namespace Smx.Winter;

public class Util
{

    /// <summary>
    /// wraps a function pointer in a trampoline that stalls the call
    /// until a debugger removes the endless loop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="address"></param>
    /// <returns></returns>
    public static T MakeDebuggerTrap<T>(nint address)
    {
        var mem = new byte[16];
        var wr = new BinaryWriter(new MemoryStream(mem));
        // debugger trap
        wr.Write((ushort)0xFEEB);
        wr.Write((byte)0x68);
        wr.Write((uint)(address & 0xFFFFFFFF));
        wr.Write((uint)0x042444C7);
        wr.Write((uint)(address >> 32) & 0xFFFFFFFF);
        wr.Write((byte)0xC3);

        nint pMem;
        unsafe
        {
            pMem = new nint(PInvoke.VirtualAlloc(null, (uint)mem.Length, 0
                   | VIRTUAL_ALLOCATION_TYPE.MEM_COMMIT
                   | VIRTUAL_ALLOCATION_TYPE.MEM_RESERVE,
                   PAGE_PROTECTION_FLAGS.PAGE_EXECUTE_READWRITE));
        }
        Marshal.Copy(mem, 0, pMem, mem.Length);

        return Marshal.GetDelegateForFunctionPointer<T>(pMem);
    }


    public static SafeFileHandle OpenProcessToken(SafeFileHandle hProc, TOKEN_ACCESS_MASK flags)
    {
        PInvoke.OpenProcessToken(hProc, flags, out var hToken);
        return hToken;
    }

    public static IEnumerable<string> GetHardLinks(string path)
    {
        var driveLetter = Path.GetPathRoot(path)?.TrimEnd(Path.DirectorySeparatorChar);
        if (driveLetter == null) throw new ArgumentException();

        uint cchName = 0;
        PInvoke.FindFirstFileName(path, 0, ref cchName, null);
        if (Marshal.GetLastWin32Error() != (int)WIN32_ERROR.ERROR_MORE_DATA)
        {
            throw new InvalidOperationException();
        }
        using var buf = MemoryHGlobal.Alloc((nint)(sizeof(char) * cchName));
        var pBuf = buf.ToPWSTR();
        using var hFile = PInvoke.FindFirstFileName(path, 0, ref cchName, pBuf);
        if (hFile.IsInvalid) throw new InvalidOperationException();

        bool more_links = true;
        while (more_links)
        {
            yield return $"{driveLetter}{pBuf.ToString()}";

            for (int i = 0; i < 2 && more_links; i++)
            {
                if (!PInvoke.FindNextFileName(hFile, ref cchName, pBuf))
                {
                    switch (Marshal.GetLastWin32Error())
                    {
                        case (int)WIN32_ERROR.ERROR_MORE_DATA:
                            buf.Realloc(sizeof(char) * cchName);
                            break;
                        case (int)WIN32_ERROR.ERROR_HANDLE_EOF:
                        case (int)WIN32_ERROR.ERROR_NO_MORE_FILES:
                        default:
                            more_links = false;
                            break;
                    }
                }
                else break;
            }
        }
    }

    public static void EnablePrivilege(string privilegeName)
    {
        using var hProc = PInvoke.GetCurrentProcess_SafeHandle();
        if (hProc == null) throw new InvalidOperationException();
        using var hToken = OpenProcessToken(hProc, TOKEN_ACCESS_MASK.TOKEN_QUERY | TOKEN_ACCESS_MASK.TOKEN_ADJUST_PRIVILEGES);
        if (hToken == null) throw new InvalidOperationException();

        if (!PInvoke.LookupPrivilegeValue(null, privilegeName, out var luid))
        {
            throw new InvalidOperationException();
        }

        var tp = new TOKEN_PRIVILEGES
        {
            PrivilegeCount = 1,
            Privileges = new VariableLengthInlineArray<LUID_AND_ATTRIBUTES>
            {
                e0 = new LUID_AND_ATTRIBUTES
                {
                    Luid = luid,
                    Attributes = TOKEN_PRIVILEGES_ATTRIBUTES.SE_PRIVILEGE_ENABLED
                }
            }
        };

        unsafe
        {
            if (!PInvoke.AdjustTokenPrivileges(
                hToken, false,
                &tp, (uint)Marshal.SizeOf(tp),
                null, null))
            {
                throw new InvalidOperationException();
            }
        }
    }
}
