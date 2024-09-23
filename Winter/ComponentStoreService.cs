#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.Winter.Cbs;
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

    public partial class ComponentStoreService(WindowsSystem windows)
    {
        private void PrintHashList(string hashList)
        {
            var chunks = hashList.Split(['#']);
            foreach (var c in chunks)
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
            if (BinaryPrimitives.ReadUInt32BigEndian(manifestData) != DCM_MAGIC)
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

        private void Test2()
        {
            var cmp = ComponentAppId.FromAppId("Microsoft.Windows.Common-Controls.Resources, Culture=bg-BG, Type=win32, Version=5.82.19041.1, PublicKeyToken=6595b64144ccf1df, ProcessorArchitecture=x86");

        }

        public IEnumerable<ComponentNode> Components
        {
            get
            {
                using var hkey = ManagedRegistryKey.Open(Registry.KEY_COMPONENTS);
                foreach (var k in hkey.KeyNames)
                {
                    var catalog = Cbs.Component.FromRegistryKey(hkey.OpenChildKey(k));
                    yield return new ComponentNode(catalog);
                }
            }
        }

        public IEnumerable<CatalogNode> Catalogs
        {
            get
            {
                using var hkey = ManagedRegistryKey.Open(Registry.KEY_CATALOGS);
                foreach (var k in hkey.KeyNames)
                {
                    var catalog = Catalog.FromRegistryKey(hkey.OpenChildKey(k));
                    yield return new CatalogNode(catalog);
                }
            }
        }

        public IEnumerable<DeploymentNode> Deployments
        {
            get
            {
                using var hkey = ManagedRegistryKey.Open(Registry.KEY_DEPLOYMENTS);
                foreach (var k in hkey.KeyNames)
                {
                    var deployment = Deployment.FromRegistryKey(hkey.OpenChildKey(k));
                    yield return new DeploymentNode(deployment);
                }
            }
        }

        public IEnumerable<object> Packages
        {
            get
            {
                var wcpAcc = new WcpLibraryAccessor(windows);
                var asmReader = new AssemblyReader(wcpAcc.ServicingStack);

                using var hkey = ManagedRegistryKey.Open(Registry.KEY_PACKAGES);
                foreach (var k in hkey.KeyNames)
                {
                    var path = Path.Combine(windows.SystemRoot, "servicing", "Packages", $"{k}.mum");
                    if (!File.Exists(path)) continue;

                    object? pkg;
                    using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        pkg = asmReader.ReadToObject(stream);
                    }
                    if (pkg != null)
                    {
                        yield return pkg;
                    }
                }
            }
        }
    }
}
