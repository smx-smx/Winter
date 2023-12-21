using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Security;
using Windows.Win32.Foundation;

namespace Smx.Winter;

public class Util
{
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
        using var buf = DisposableMemory.AllocHGlobal((nint)(sizeof(char) * cchName));
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
                            buf.Realloc((nint)(sizeof(char) * cchName));
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
            Privileges = new __LUID_AND_ATTRIBUTES_1
            {
                _0 = new LUID_AND_ATTRIBUTES
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
                tp, (uint)Marshal.SizeOf(tp),
                null, null))
            {
                throw new InvalidOperationException();
            }
        }

    }
}
