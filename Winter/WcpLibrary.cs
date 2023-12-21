using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public delegate int pfnGenerateKeyFormIntoBuffer(nint pThis);
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
    }
}
