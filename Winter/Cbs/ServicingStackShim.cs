using Smx.SharpIO.Memory;
using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Smx.Winter.ComponentStoreService;
using Windows.Win32.Foundation;
using Windows.Win32;

namespace Smx.Winter.Cbs
{

    public struct SSS_OFFLINE_IMAGE
    {
        public uint cbSize;
        public uint dwFlags;
        public PWSTR pcwszWindir;
    }

    public struct SSS_BIND_PARAMETERS
    {
        public uint cbSize;
        public SSS_BIND_CONDITION_FLAGS dwFlags;
        public FOUR_PART_VERSION fpvVersion;
        public ulong cProcessorArchitectures;
        public TypedPointer<uint> prgusProcessorArchitectures;
        public TypedPointer<SSS_OFFLINE_IMAGE> pOfflineImage;
        public ulong cAlternateLocations;
        public nint prgpszAlternateLocations;
        public PWSTR pszTemporaryLocation;
    }

    public struct SSS_COOKIE
    {
        public FOUR_PART_VERSION fpvVersion;
        public LUNICODE_STRING ucLocation;
        public LUNICODE_STRING ucWindir;
        public uint ulProcessorArchitecture;
    }

    public enum SSS_BIND_CONDITION_FLAGS : uint
    {
        VERSION = 1 << 0,
        ARCHITECTURE = 1 << 1,
        OFFLINE_IMAGE = 1 << 2,
        ALTERNATE_LOCATION = 1 << 3,
        TEMPORARY_LOCATION = 1 << 8,
    }

    public class ServicingStackShimSession
    {
        private readonly NativeServicingStackShim _shim;
        private readonly SSS_COOKIE _cookie;
        public ServicingStackShimSession(NativeServicingStackShim shim, SSS_COOKIE cookie)
        {
            _shim = shim;
            _cookie = cookie;
        }

        public string GetCbsCorePath()
        {
            var filePath = _shim.SssGetServicingStackFilePath(_cookie);
            return filePath;
        }
    }

    public class NativeServicingStackShim : IDisposable
    {
        private FreeLibrarySafeHandle handle;
        private delegate int PSSS_BIND_SERVICING_STACK_FUNCTION(
            SSS_BIND_PARAMETERS inputParams,
            out TypedPointer<SSS_COOKIE> pCookie,
            out uint dwDisposition);

        private delegate int PSSS_GET_SERVICING_STACK_FILE_PATH_LENGTH_FUNCTION(
            int dwFlags,
            SSS_COOKIE cookie,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pszFile,
            // the count of wchar_t in pszFile, including terminating null
            out ulong length);

        private delegate int PSSS_GET_SERVICING_STACK_FILE_PATH_FUNCTION(
            int dwFlags,
            SSS_COOKIE cookie,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pszFile,
            ulong cchBuffer,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder bufferPath,
            out long gotLen);

        private PSSS_BIND_SERVICING_STACK_FUNCTION _SssBindServicingStack;
        private PSSS_GET_SERVICING_STACK_FILE_PATH_LENGTH_FUNCTION _SssGetServicingStackFilePathLength;
        private PSSS_GET_SERVICING_STACK_FILE_PATH_FUNCTION _SssGetServicingStackFilePath;


        public string SssGetServicingStackFilePath(SSS_COOKIE cookie)
        {
            const string filename = "CbsCore.dll";
            var res = _SssGetServicingStackFilePathLength(0, cookie, filename, out var length);
            if (res != 0)
            {
                throw new InvalidOperationException("Failed to get file path length");
            }

            var sb = new StringBuilder((int)length);
            res = _SssGetServicingStackFilePath(0, cookie, filename, length, sb, out var gotLen);
            if (res != 0)
            {
                throw new InvalidOperationException("Failed to get file path");
            }
            return sb.ToString();
        }

        public ServicingStackShimSession SssBindServicingStack(string? winDir = null)
        {
            uint[] arches = [(uint)ProcessorArchitectureType.AMD64];
            using var gch = DisposableGCHandle.Pin(arches);

            TypedMemoryHandle<SSS_OFFLINE_IMAGE>? offlineImage = null;
            nint winDirStr = 0;
            nint offlineImagePtr = 0;

            SSS_BIND_CONDITION_FLAGS flags = SSS_BIND_CONDITION_FLAGS.ARCHITECTURE;
            try {
                if(winDir != null){
                    flags |= SSS_BIND_CONDITION_FLAGS.OFFLINE_IMAGE;
                    offlineImage = MemoryHGlobal.Alloc<SSS_OFFLINE_IMAGE>();
                    offlineImage.Value.cbSize = (uint)Unsafe.SizeOf<SSS_OFFLINE_IMAGE>();
                    
                    winDirStr = Marshal.StringToHGlobalUni(winDir);
                    unsafe {
                        offlineImage.Value.pcwszWindir = new PWSTR((char *)winDirStr.ToPointer());
                    }
                    offlineImagePtr = offlineImage.Pointer.Address;
                }
                
                var hr = _SssBindServicingStack(
                    new SSS_BIND_PARAMETERS
                    {
                        cbSize = (uint)Unsafe.SizeOf<SSS_BIND_PARAMETERS>(),
                        dwFlags = flags,
                        cProcessorArchitectures = (ulong)arches.Length,
                        prgusProcessorArchitectures = new TypedPointer<uint>(gch.AddrOfPinnedObject()),
                        pOfflineImage = new TypedPointer<SSS_OFFLINE_IMAGE>(offlineImagePtr)
                    },
                    out var cookie,
                    out var disposition
                );
                if (hr != 0)
                {
                    throw new InvalidOperationException("SssBindServicingStack failed");
                }
                return new ServicingStackShimSession(this, cookie.Value);
            } finally {
                offlineImage?.Dispose();
                if(winDirStr != 0){
                    Marshal.FreeHGlobal(winDirStr);
                }
            }
        }

        private T GetSymbol<T>(string name) where T : Delegate
        {
            var addr = PInvoke.GetProcAddress(handle, name);
            if (addr == 0)
            {
                throw new InvalidOperationException($"Cannot find symbol {name}");
            }
            return addr.CreateDelegate<T>();
        }

        public NativeServicingStackShim(string? libPath = null)
        {
            handle = PInvoke.LoadLibrary(libPath ?? "SSShim.dll");
            if (handle.IsInvalid)
            {
                throw new InvalidOperationException("Failed to load servicing stack shim");
            }
            _SssBindServicingStack = GetSymbol<PSSS_BIND_SERVICING_STACK_FUNCTION>("SssBindServicingStack");
            _SssGetServicingStackFilePathLength = GetSymbol<PSSS_GET_SERVICING_STACK_FILE_PATH_LENGTH_FUNCTION>("SssGetServicingStackFilePathLength");
            _SssGetServicingStackFilePath = GetSymbol<PSSS_GET_SERVICING_STACK_FILE_PATH_FUNCTION>("SssGetServicingStackFilePath");
        }

        public void Dispose()
        {
            handle.Dispose();
        }
    }

}
