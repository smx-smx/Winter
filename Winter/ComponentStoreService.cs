using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;
using Windows.Win32.System.Memory;
using static Smx.Winter.IRtlAppIdAuthority;

namespace Smx.Winter
{
    public class Component
    {
        public readonly string FilePath;
        public string CurrentComponent { get; private set; }


        private string? GetSxSPath(string filePath)
        {
            var dirSep = Path.DirectorySeparatorChar;
            return Util.GetHardLinks(filePath)
                .FirstOrDefault(
                    l => l.Contains($"{dirSep}WinSxS{dirSep}",
                    StringComparison.InvariantCultureIgnoreCase));
        }

        public Component(string filePath)
        {
            this.FilePath = filePath;
            var sxsPath = GetSxSPath(filePath);
            if (sxsPath == null) throw new ArgumentException();

            var sxsDir = Path.GetDirectoryName(sxsPath);
            if(sxsDir == null) throw new ArgumentException();
            var sxsDirName = Path.GetFileName(sxsDir);

            CurrentComponent = sxsDirName;
        }
    }

    public record ComponentDeployment(ManagedRegistryKey baseKey)
    {
        public string AppId
        {
            get
            {
                var appId = baseKey.GetValue<byte[]>("appid");
                return Encoding.UTF8.GetString(appId);
            }
        }
    }


    public class VersionedComponent
    {
        public void GetHashList()
        {
            /**
            ZRwyuWl2KtUSn2iLqPSksZ1g0Bt9PHhTQ6z/pH9HH7U=!10.0.19041.1940#
        qdoFjqqw8awupL7h+MWizXtosAizcFm9FLrXVHlaQK4=!10.0.19041.1#
        uNI/2B6RJlVLVhmmPu/J0Mk6H2Fy6JH4JDv5qzWDg48=!10.0.19041.2180#
        ALRhufoTpebPgXqIZfSmxyJznYW8Y3ujhzptl0E2gYw=!10.0.19041.2300#
        MB4maU1zZVA18JjVV4TjPn+KCLjg+7k4z2/b2dTiNfY=!10.0.19041.2664#
        1gJ+3WpwAR0GOcONY7NKlN8r3XXg+u28bV+IW5NzMbg=!10.0.19041.2780#
        ljHFUe52vQO//YDYfNts1naRU2Pez8yn35FFgYEHFEM=!10.0.19041.2905#
        zKgfVAG28IkkyAKf0gVbf9SsM77ewB96Mu7bcdlEPTk=!10.0.19041.3025#
        ojdt0jD31mUlrEhisMrW2+0M6bx79px3wZ8Db6dvZWM=!10.0.19041.3205#
        hkF3AjhHDK/bTgf8CDYLiRIO5sGnGBZMDSxnoWqT8YM=!10.0.19041.3266#
        q8Z95vjQvpcOYhriPjfEkKRg5HWxBuxukOmG182yPKs=!10.0.19041.3385#
        bVWqtxXHTNqwve9MtWc9bhLqb9LRJE+jlkcndLYJDb4=!10.0.19041.3562#
        2d0GWuF+bnF7SqQGR3zJ2DCDqMW7eMMLnJX1XZpm3to=!10.0.19041.3684#
        O8O3pdyIsbElQvECsmsBh54qFE6Q1aQOEf4rbVMkgr0=!10.0.19041.3745#
             * 
             **/
        }
    }
    
    public class VersionedIndex : IDisposable
    {
        private readonly ManagedRegistryKey hkey;

        public VersionedIndex(string version)
        {
            hkey = ManagedRegistryKey.Open(@$"HKEY_LOCAL_MACHINE\COMPONENTS\DerivedData\VersionedIndex\{version}\ComponentFamilies");
        }

        public void Dispose()
        {
            hkey.Dispose();
        }

        public ManagedRegistryKey GetComponent(string name) => hkey[name];

        public IEnumerable<string> Components => hkey.KeyNames;
    }

    public partial class ComponentStoreService
    {
        private WindowsSystem windows;

        public ComponentStoreService(WindowsSystem windows) {
            this.windows = windows;
        }

        private string GetLastWCPVersionToAccessStore()
        {
            using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\COMPONENTS\ServicingStackVersions");
            var formattedVersion = hkey.GetValue<string>("LastWCPVersionToAccessStore");
            var lastVer = formattedVersion.Split('(').First().TrimEnd();
            return lastVer;
        }

        private void PrintHashList(string hashList)
        {
            var chunks = hashList.Split(['#']);
            foreach(var c in chunks)
            {
                var parts = c.Split(['!']);
                if (parts.Length < 2) continue;
                var hash = parts[0];
                var version = parts[1];
                Console.WriteLine(Convert.ToHexString(Convert.FromBase64String(hash)));
            }
        }

        private const uint DCM_MAGIC = 0x44434D01;
        private const uint PA30_MAGIC = 0x50413330;

        private const long DELTA_FLAG_NONE = 0x00000000;
        private const long DELTA_APPLY_FLAG_ALLOW_PA19 = 0x00000001;

        private string DecompressManifest(Span<byte> wcpDict, string manifestPath)
        {

            var manifestData = File.ReadAllBytes(manifestPath).AsSpan();
            if(BinaryPrimitives.ReadUInt32BigEndian(manifestData) != DCM_MAGIC)
            {
                throw new InvalidDataException();
            }

            var output = MsPatch.ApplyPatch(wcpDict, manifestData.Slice(4));
            return Encoding.UTF8.GetString(output);
        }

        private delegate uint RtlGetAppIdAuthority(uint Flags, ref nint Authority);

        public struct LBLOB
        {
            public ulong Length;
            public ulong MaximumLength;
            public nint Buffer;
        }

        public struct IdAuthorityData
        {
            public LBLOB String1;
            public nint AuthorityData;
            public LBLOB String2;
        }


        public delegate int pfnAppId_GetType(nint pThis);
        public delegate int pfnAppId_GetData(nint pThis, out nint pDataOut);

        private static void DebugDump(nint addr, int length)
        {
            var buf = new byte[length];
            Marshal.Copy(addr, buf, 0, length);
            Console.WriteLine($"=> {addr:X}: {Convert.ToHexString(buf)}");
        }

        private static nint WcpGetDelta(nint addr)
        {
            var wcpBase = PInvoke.GetModuleHandle("wcp").DangerousGetHandle();
            if (addr < wcpBase) throw new ArgumentException();
            return addr - wcpBase;
        }

        private ulong GetAppIdHash(WcpLibrary wcp, string appIdString)
        {
            //var appIdTest = "Microsoft-Windows-CoreSystem-RemoteFS-Client-Deployment-LanguagePack, Culture=it-IT, Version=10.0.19041.3570, PublicKeyToken=31bf3856ad364e35, ProcessorArchitecture=amd64, versionScope=NonSxS";

            using var appId = wcp.AppIdParseDefinition(appIdString);
            if (appId == null) throw new InvalidOperationException();

            /** CRtlAppIdAuthority stuff **/
#if false
            if(false){
                var appId_intf1 = Marshal.ReadIntPtr(appId - 8);
                var appId_intf2 = appId - 16;

                var appId_intf1_vtbl = Marshal.ReadIntPtr(appId_intf1);
                var appId_intf2_vtbl = Marshal.ReadIntPtr(appId_intf2 + 0x10);

                var pfnGetType = Marshal.ReadIntPtr(appId_intf1_vtbl + 24);
                var pfnGetHashData = Marshal.ReadIntPtr(appId_intf2_vtbl + 40);

                Console.WriteLine($"intf1: {appId_intf1:X}, intf2: {appId_intf2:X}");
                Console.WriteLine($"vtbl1: {appId_intf1_vtbl:X}, vtbl2: {appId_intf2_vtbl:X}");

                var appId_intf1_GetType = Marshal.GetDelegateForFunctionPointer<pfnAppId_GetType>(pfnGetType);
                var appIdType = appId_intf1_GetType(appId_intf1);

                var appId_intf2_GetData = Marshal.GetDelegateForFunctionPointer<pfnAppId_GetData>(pfnGetHashData);
                appId_intf2_GetData(appId_intf2 + 16, out nint pHashData);

                Console.WriteLine(appIdType);
                Console.WriteLine($"hd is {pHashData:X}");

                var hashData = Marshal.PtrToStructure<IdAuthorityData>(pHashData);
            }
#endif

            var pfnHashWrapped = wcp.GetDebugWrappedDelegate<pfnHash>("Hash");

            bool USE_DEBUGGER_TRAP = false;

            ulong hashValue;

            int res;
            if (USE_DEBUGGER_TRAP)
            {
                res = pfnHashWrapped(wcp.Authority, 0, appId.Value, out hashValue);
            } else
            {
                res = wcp.HashAppId(0, appId, out hashValue);
            }


            var nameHashFlags = 0
                | NameHashFlags.Name
                | NameHashFlags.PublicKeyToken
                | NameHashFlags.Version
                | NameHashFlags.ProcessorArchitecture
                | NameHashFlags.VersionScope;

            Debug.Assert((uint)nameHashFlags == 0x117);

            Console.WriteLine($"{res:X}, hash is {hashValue:X}");
            var ourHash = ComponentAppId.Parse(appIdString).GetHash(nameHashFlags);
            Console.WriteLine($"ours: {ourHash:X}");

            if(ourHash != hashValue)
            {
                Console.WriteLine("!!!! DISCREPANCY");
            }

            return hashValue;
        }

        private const string MICROSOFT_PUBKEY = "31bf3856ad364e35";


        private void Test2()
        {
            var cmp = ComponentAppId.Parse("Microsoft.Windows.Common-Controls.Resources, Culture=bg-BG, Type=win32, Version=5.82.19041.1, PublicKeyToken=6595b64144ccf1df, ProcessorArchitecture=x86");

        }

        public WcpLibrary GetServicingStack()
        {
            var wcpVer = GetLastWCPVersionToAccessStore();

            var servicingStackAppID = new ComponentAppId
            {
                Name = "microsoft-windows-servicingstack",
                Culture = "neutral",
                ProcessorArchitecture = "amd64",
                PublicKeyToken = MICROSOFT_PUBKEY,
                Version = wcpVer,
                VersionScope = "NonSxS"
            };

            var sxsName = servicingStackAppID.GetSxSName();
            var servicingStackRoot = Path.Combine(windows.SystemRoot, "WinSxS", sxsName);
            var wcpPath = Path.Combine(servicingStackRoot, "wcp.dll");

            Console.WriteLine(wcpPath);
            if (File.Exists(wcpPath))
            {
                Console.WriteLine("OK");
            }

            var wcp = WcpLibrary.Load(wcpPath);

            GetAppIdHash(wcp, servicingStackAppID.ToString());

            return wcp;
          
        }

        public void TraverseDeployments()
        {
            using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\COMPONENTS\CanonicalData\Deployments");
            foreach(var k in hkey.KeyNames)
            {
                var deployment = new ComponentDeployment(hkey[k]);
                var appId = ComponentAppId.Parse(deployment.AppId);
                Console.WriteLine(k);
                Console.WriteLine(appId);
            }
        }
    }
}
