#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Memory;
using static Smx.Winter.ComponentStoreService;
using static Smx.Winter.IRtlAppIdAuthority;

namespace Smx.Winter
{

    public struct IRtlWOFOStream
    {
        public delegate int pfnRelease(nint pThis, int flag);
        public pfnRelease Release;
    }

    public struct IRtlAppIdAuthority
    {
        public delegate int pfnRelease(nint pThis);
        public delegate int pfnCreateInterface(nint pThis);
        public delegate int pfnCreateReferenceAppId(nint pThis);
        public delegate int pfnCreateDefinitionAppId(nint pThis);
        public delegate int pfnParseReferenceAppId(nint pThis);
        public delegate int pfnParseDefinitionAppId(nint pThis,
            uint Flags,
            LUNICODE_STRING String,
            out nint AppId);
        public delegate int pfnFormat(
            nint pThis,
            uint Flags,
            nint AppId,
            nint FormattedAppId
        );
        public delegate int pfnFormatIntoBuffer(nint pThis,
            uint Flags,
            nint Identity,
            ref LUNICODE_STRING Buffer,
            ref nint BufferSize
        );
        public delegate int pfnGenerateKeyFormIntoBuffer(nint pThis,
            uint Flags,
            nint AppId,
            ref LUNICODE_STRING Buffer
        );
        public delegate int pfnAreEqualReferenceAppId(nint pThis,
            uint Flags,
            nint pReference1,
            nint pReference2,
            ref bool Equal);
        public delegate int pfnAreEqualDefinitionAppId(nint pThis,
            uint Flags,
            nint pDefinition1,
            nint pDefinition2,
            ref bool Equal);
        public delegate int pfnMatches(nint pThis,
            uint Flags,
            nint AppId1,
            nint AppId2,
            ref bool Matches
        );
        public delegate int pfnHash(nint pThis,
            uint Flags,
            nint AppId,
            out ulong PseudoKey);
        public delegate int pfnIsDescendantReferenceAppId(nint pThis);
        public delegate int pfnIsDescendantDefinitionAppId(nint pThis);

        /*  0 */
        public pfnRelease Release;
        /*  8 */
        public pfnCreateInterface CreateInterface;
        /* 16 */
        public pfnCreateReferenceAppId CreateReferenceAppId;
        /* 24 */
        public pfnCreateDefinitionAppId CreateDefinitionAppId;
        /* 32 */
        public pfnParseReferenceAppId ParseReferenceAppId;
        /* 40 */
        public pfnParseDefinitionAppId ParseDefinitionAppId;
        /* 48 */
        public pfnFormat Format;
        /* 56 */
        public pfnFormatIntoBuffer FormatIntoBuffer;
        /* 64 */
        public pfnGenerateKeyFormIntoBuffer GenerateKeyFormIntoBuffer;
        /* 72 */
        public pfnAreEqualReferenceAppId AreEqualReferenceAppId;
        /* 80 */
        public pfnAreEqualDefinitionAppId AreEqualDefinitionAppId;
        /* 88 */
        public pfnMatches Matches;
        /* 96 */
        public pfnHash Hash;
        public pfnIsDescendantReferenceAppId IsDescendantReferenceAppId;
        public pfnIsDescendantDefinitionAppId IsDescendantDefinitionAppId;
    }

    public class ManagedAppId : IDisposable
    {
        public nint Value { get; private set; }
        
        private IRtlWOFOStream wofoStream;

        public ManagedAppId(nint instance)
        {
            if (instance == 0) throw new ArgumentException(nameof(instance));

            var pVtbl = Marshal.ReadIntPtr(instance);
            wofoStream = Marshal.PtrToStructure<IRtlWOFOStream>(pVtbl);
            Value = instance;
        }

        public void Dispose()
        {
            wofoStream.Release(Value, 1);
            Value = 0;
        }
    }

    public class WcpLibrary : IDisposable
    {
        public string LibraryPath { get; private set; }
        private readonly FreeLibrarySafeHandle handle;
        private Memory<byte> dictionary;

        private HMODULE hModule => new HMODULE(handle.DangerousGetHandle());

        private const int WCP_DICTIONARY_RESOURCE_ID = 0x266;

        private delegate uint RtlGetAppIdAuthority(uint Flags, ref nint Authority);

        private nint pAuthority = 0;
        private nint pVtbl = 0;
        private IRtlAppIdAuthority intfAuthority;

        public nint Authority => pAuthority;

        private WcpLibrary(string libPath, FreeLibrarySafeHandle hLib)
        {
            LibraryPath = libPath;

            handle = hLib;
            var RtlGetAppIdAuthority = PInvoke.GetProcAddress(hLib, "RtlGetAppIdAuthority").CreateDelegate<RtlGetAppIdAuthority>();

            var res = RtlGetAppIdAuthority(0, ref pAuthority);
            if (res != 0) throw new InvalidOperationException();

            pVtbl = Marshal.ReadIntPtr(pAuthority);
            intfAuthority = Marshal.PtrToStructure<IRtlAppIdAuthority>(pVtbl);

            dictionary = GetPatchDictionary().ToArray();
        }


        /// <summary>
        /// wraps a function pointer in a trampoline that stalls the call
        /// until a debugger removes the endless loop
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address"></param>
        /// <returns></returns>
        private static T MakeDebuggerTrap<T>(nint address)
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

        public T GetDebugWrappedDelegate<T>(string functionName)
        {
            var fn = Marshal.ReadIntPtr(pVtbl + Marshal.OffsetOf<IRtlAppIdAuthority>(functionName));
            return MakeDebuggerTrap<T>(fn);
        }

        private delegate void pfnAppIdRelease(nint instance, int flag);

        public ManagedAppId? AppIdParseDefinition(string appId)
        {
            using var strMem = LUNICODE_STRING.CreateFromString(appId, out var lunicodeAppId);
            var res = intfAuthority.ParseDefinitionAppId(pAuthority, 0, lunicodeAppId, out var appIdInstance);
            if (res != 0) return null;
            return new ManagedAppId(appIdInstance);
        }

        public int HashAppId(uint flags, ManagedAppId appId, out ulong hash)
        {
            return intfAuthority.Hash(pAuthority, flags, appId.Value, out hash);
        }

        public int GenerateKeyForm(uint flags, ManagedAppId appId, out string keyForm)
        {
            using var mem = LUNICODE_STRING.CreateFromString(new string(' ', 256), out var buf);
            var res = intfAuthority.GenerateKeyFormIntoBuffer(pAuthority, flags, appId.Value, ref buf);
            keyForm = mem.ToPWSTR().ToString();
            return res;
        }

        public Memory<byte> GetPatchDictionary()
        {
            PCWSTR resourceType;
            PCWSTR resourceName; // used as index
            unsafe
            {
                resourceType = new PCWSTR((char*)WCP_DICTIONARY_RESOURCE_ID);
                resourceName = new PCWSTR((char*)1); // index
            }

            ushort? firstLanguage = null;

            var wcpHModule = this.hModule;

            if (!PInvoke.EnumResourceLanguagesEx(
                wcpHModule, resourceType, resourceName,
                (hModule, lpType, lpName, wLanguage, lParam) =>
                {
                    if (firstLanguage == null)
                    {
                        firstLanguage = wLanguage;
                    }
                    return true;
                },
                0, PInvoke.RESOURCE_ENUM_LN, 0) || firstLanguage == null) throw new InvalidOperationException();

            var hResourceInfo = PInvoke.FindResourceEx(wcpHModule, resourceType, resourceName, firstLanguage.Value);
            if (hResourceInfo.Value == 0) throw new InvalidOperationException();

            var dictSize = PInvoke.SizeofResource(wcpHModule, hResourceInfo);
            var hResorce = PInvoke.LoadResource(wcpHModule, hResourceInfo);

            Memory<byte> dictBlob;
            unsafe
            {
                var pBytes = new nint(PInvoke.LockResource(hResorce));
                dictBlob = pBytes.AsSpan<byte>((int)dictSize).ToArray();
            }

            return dictBlob;
        }

        private const uint DCM_MAGIC = 0x44434D01;
        private const uint UTF8_BOM = 0xEFBBBF;

        private enum ManifestFileType
        {
            Unknown,
            Uncompressed,
            MsPatch
        }

        public string DecompressManifest_ToString(string manifestPath)
        {
            var manifestData = File.ReadAllBytes(manifestPath);
            
            var magic = BinaryPrimitives.ReadUInt32BigEndian(manifestData);
            ManifestFileType fileType = ManifestFileType.Unknown;
            do
            {
                if (magic == DCM_MAGIC)
                {
                    fileType = ManifestFileType.MsPatch;
                    break;
                }
                var bomHead = (magic & 0xFFFFFF00) >> 8;
                if (bomHead == UTF8_BOM)
                {
                    fileType = ManifestFileType.Uncompressed;
                    break;
                }
                if(Encoding.UTF8.GetString(manifestData, 0, 5) == "<?xml")
                {
                    fileType = ManifestFileType.Uncompressed;
                    break;
                }


                throw new InvalidDataException();
            } while (false);

            switch (fileType)
            {
                default:
                    throw new NotSupportedException();
                case ManifestFileType.MsPatch:
                    {
                        // skip DCM magic
                        var manifestSpan = manifestData.AsSpan();
                        var output = MsPatch.ApplyPatch(dictionary.Span, manifestSpan.Slice(sizeof(uint)));
                        using var reader = new StreamReader(new MemoryStream(output));
                        return reader.ReadToEnd() ?? "";
                    }
                case ManifestFileType.Uncompressed:
                    {
                        using var reader = new StreamReader(new MemoryStream(manifestData));
                        return reader.ReadToEnd() ?? "";
                    }
                    
            }
        }

        static T? DeserializeObject<T>(XmlReader xmlReader, XmlDeserializationEvents events)
        {
            var serializer = new XmlSerializer(typeof(T)/*, CreateXmlOverrides<T>(defaultNamespace)*/);

            //using var reader = new StringReader(xml);
            //using var xmlReader = XmlReader.Create(reader);
            return (T?)serializer.Deserialize(xmlReader, events);
        }

        public object? DecompressManifest(string manifestPath)
        {
            var xmlData = DecompressManifest_ToString(manifestPath);

            var dumpAndDie = (string errorMessage) =>
            {
                var tmp = Path.Combine(
                    Path.GetTempPath(),
                    "winternals-assembly-manifest.xml"
                    //"winternals-" + Path.GetFileName(manifestPath)
                );
                File.WriteAllText(tmp, xmlData);
                Console.Error.WriteLine(errorMessage);
                Console.Error.WriteLine($"Assembly Manifest dumped to {tmp}");
                throw new InvalidOperationException();
            };

            var events = new XmlDeserializationEvents
            {
                OnUnknownElement = (sender, args) =>
                {
                    dumpAndDie(
                        $"Error on line {args.LineNumber}, {args.LinePosition}." +
                        Environment.NewLine +
                        $"unknown element: {args.Element.Name}. " +
                        Environment.NewLine +
                        $"{args}"
                    );
                },
                OnUnknownNode = (sender, args) =>
                {
                    dumpAndDie(
                        $"Error on line {args.LineNumber}, {args.LinePosition}." +
                        Environment.NewLine +
                        $"unknown node: {args.Name} in {args.ObjectBeingDeserialized}. " +
                        Environment.NewLine +
                        $"{args}"
                    );
                },
                OnUnknownAttribute = (sender, args) =>
                {
                    dumpAndDie(
                        $"Error on line {args.LineNumber}, {args.LinePosition}." +
                        Environment.NewLine +
                        $"unknown attribute: {args.Attr.Name}. " +
                        Environment.NewLine +
                        $"{args}"
                    );
                }
            };

            using var xmlStream = new StringReader(xmlData);
            using var rdr = XmlReader.Create(xmlStream);
            rdr.MoveToContent();

            switch (rdr.NamespaceURI)
            {
                case "urn:schemas-microsoft-com:asm.v3":
                    return DeserializeObject<SchemaDefinitions.AsmV3.Assembly>(rdr, events);
                case "urn:schemas-microsoft-com:asm.v1":
                    return DeserializeObject<SchemaDefinitions.AsmV1.Assembly>(rdr, events);
                default:
                    throw new NotSupportedException(rdr.NamespaceURI);
            }
        }

        public static WcpLibrary Load(string wcpPath)
        {
            var hWcp = PInvoke.LoadLibrary(wcpPath);
            return new WcpLibrary(wcpPath, hWcp);
        }

        public void Dispose()
        {
            if(pAuthority != 0)
            {
                intfAuthority.Release(pAuthority);
                pVtbl = 0;
                pAuthority = 0;
            }
            handle.Dispose();
        }

        public string GetFormattedAppId(string appIdString)
        {
            // $FIXME: doesn't call the correct one (need LH format)

            using var appId = AppIdParseDefinition(appIdString);
            if (appId == null) throw new InvalidOperationException();

            using var strMem = LUNICODE_STRING.CreateFromString(
                new string(' ', 256), out var lunicodeBuffer
            );

            var pfnFormatWrapped = GetDebugWrappedDelegate<pfnGenerateKeyFormIntoBuffer>("GenerateKeyFormIntoBuffer");
            pfnFormatWrapped(Authority, 0, appId.Value, ref lunicodeBuffer);
            Console.WriteLine(lunicodeBuffer);
            return strMem.ToPWSTR().ToString();

            uint flags = 0;
            // flags |= 1; // to include version
            GenerateKeyForm(flags, appId, out var keyForm);
            return keyForm;

        }

        public ulong GetAppIdHash(string appIdString)
        {
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

            var pfnHashWrapped = GetDebugWrappedDelegate<pfnHash>("Hash");

            bool COMPARE_ORIGINAL = false;
            bool USE_DEBUGGER_TRAP = false;
            var SOFT_BP = false;

            ulong hashValue = 0;

#if false
            if(appIdString == "NAME, Culture=XXX, Type=XXX, Version=XXX, PublicKeyToken=XXX, ProcessorArchitecture=amd64")
            {
                //USE_DEBUGGER_TRAP = true;
                SOFT_BP = true;
            }
#endif


            int res = 0;
            if (COMPARE_ORIGINAL)
            {
                using var appId = AppIdParseDefinition(appIdString);
                if (appId == null) throw new InvalidOperationException();
                // $FIXME: free managed resource

                if (USE_DEBUGGER_TRAP)
                {
                    var trapAddr = Marshal.GetFunctionPointerForDelegate(pfnHashWrapped);
                    Console.WriteLine($"BT AT {trapAddr:X}");
                    res = pfnHashWrapped(Authority, 0, appId.Value, out hashValue);
                } else
                {
                    res = HashAppId(0, appId, out hashValue);
                }
            }

            var componentAppId = ComponentAppId.Parse(appIdString);

            var nameHashFlags = 0
                | NameHashFlags.Name
                | NameHashFlags.PublicKeyToken
                | NameHashFlags.Version
                | NameHashFlags.ProcessorArchitecture;

            nameHashFlags |= componentAppId.Culture switch
            {
                null => 0,
                "neutral" => 0,
                _ => NameHashFlags.Culture
            };

            nameHashFlags |= componentAppId.VersionScope switch
            {
                null => 0,
                _ => NameHashFlags.VersionScope
            };

            nameHashFlags |= componentAppId.Type switch
            {
                null => 0,
                _ => NameHashFlags.Type
            };

            var ourHash = componentAppId.GetHash(nameHashFlags);

            if (!COMPARE_ORIGINAL)
            {
                hashValue = ourHash;
            }

            Trace.WriteLine($"{res:X}, hash is {hashValue:X}");
            if (SOFT_BP)
            {
                hashValue.ToString(); //$DEBUG
            }
            
            Trace.WriteLine($"ours: {ourHash:X}");

            if (ourHash != hashValue)
            {
                Trace.WriteLine("!!!! DISCREPANCY");
            }

            return hashValue;
        }

    }
}
