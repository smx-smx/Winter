using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Smx.Winter.Cbs.Native;
using Windows.Win32;
using Windows.Win32.System.Com;

namespace Smx.Winter.Cbs
{
    public class CbsCore : IDisposable
    {
        private delegate int pfnLockProc(int arg);
        private delegate void pfnUnlockProc();
        private delegate void pfnInstCreated();
        private delegate void pfnInstDestroyed();
        private delegate void pfnReqShutdownNow();
        private delegate void pfnReqShutdownProcessing();

        private delegate int pfnCbsCoreFinalize();
        private delegate int pfnCbsCoreInitialize(
            [MarshalAs(UnmanagedType.Interface)]
            IMalloc pMalloc,
            pfnLockProc? lockProc,
            pfnUnlockProc? unlockProc,
            pfnInstCreated? instCreatedProc,
            pfnInstDestroyed? instDestroyedProc,
            pfnReqShutdownNow? reqShutdownNowProc,
            pfnReqShutdownProcessing? reqShutdownProcessingProc,
            [MarshalAs(UnmanagedType.Interface)]
            out IClassFactory classFactory
        );

        private delegate void vpfnCustomLogging(int tag, [MarshalAs(UnmanagedType.LPStr)] string msg);
        private delegate void pfnCbsCoreSetCustomLogging(vpfnCustomLogging loggingFunction);

        private readonly FreeLibrarySafeHandle _handle;
        private readonly pfnCbsCoreInitialize _cbsCoreInitialize;
        private readonly pfnCbsCoreFinalize _cbsCoreFinalize;
        private readonly pfnCbsCoreSetCustomLogging _cbsCoreSetCustomLogging;

        private void CbsLogMessage(int tag, string msg){
            Console.WriteLine($"[CBS,0x{tag:X}] {msg}");
        }

        public CbsCore(string dllPath){
            var handle = PInvoke.LoadLibrary(dllPath);
            if(handle == null){
                throw new InvalidOperationException("failed to load CbsCore");
            }
            _handle = handle;

            _cbsCoreInitialize = PInvoke.GetProcAddress(handle, "CbsCoreInitialize").CreateDelegate<pfnCbsCoreInitialize>();
            _cbsCoreFinalize = PInvoke.GetProcAddress(handle, "CbsCoreFinalize").CreateDelegate<pfnCbsCoreFinalize>();
            _cbsCoreSetCustomLogging = PInvoke.GetProcAddress(handle, "CbsCoreSetCustomLogging").CreateDelegate<pfnCbsCoreSetCustomLogging>();
        }

        public ICbsSession Initialize(){
            _cbsCoreSetCustomLogging(CbsLogMessage);

            if(!PInvoke.CoGetMalloc(1, out var iMalloc).Succeeded){
                throw new InvalidOperationException("CoGetMalloc failed");
            }
            var hr = _cbsCoreInitialize(
                iMalloc,
                lockProc: (_) => 0,
                unlockProc: () => {},
                instCreatedProc: () => {},
                instDestroyedProc: () => {},
                reqShutdownNowProc: () => {},
                reqShutdownProcessingProc: () => {}, 
                out var classFactory
            );
            if(hr != 0){
                throw new InvalidOperationException("CbsInitialize failed");
            }

            classFactory.CreateInstance(null, typeof(ICbsSession).GUID, out var session);
            if(session == null || !(session is ICbsSession sess)){
                throw new InvalidOperationException("CreateInstance failed");
            }
            return sess;
        }

        public void Dispose()
        {
            _cbsCoreFinalize();
            _handle.Dispose();
        }
    }
}
