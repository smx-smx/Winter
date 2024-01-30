#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Smx.Winter
{
    enum ManifestDataType
    {
        Unknown,
        Uncompressed,
        MsPatch
    }

    public class ManifestReader
    {
        private Memory<byte> dictionary;

        public ManifestReader(WcpLibraryAccessor wcp) {
            this.dictionary = wcp.ServicingStack.GetPatchDictionary();
        }

        private const uint DCM_MAGIC = 0x44434D01;
        private const uint UTF8_BOM = 0xEFBBBF;

        private static ManifestDataType GetDataType(Span<byte> bytes)
        {
            var magic = BinaryPrimitives.ReadUInt32BigEndian(bytes);
            if (magic == DCM_MAGIC)
            {
                return ManifestDataType.MsPatch;
            }

            var bomHead = (magic & 0xFFFFFF00) >> 8;
            if (bomHead == UTF8_BOM)
            {
                return ManifestDataType.Uncompressed;
            }

            var search = "<?xml";
            if (Encoding.UTF8.GetString(bytes.Slice(0, search.Length)) == search)
            {
                return ManifestDataType.Uncompressed;
            }

            return ManifestDataType.Unknown;
        }


        private static T? DeserializeObject<T>(XmlReader xmlReader, XmlDeserializationEvents events)
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T?)serializer.Deserialize(xmlReader, events);
        }

        public object? DecompressToObject(Stream source)
        {
            var xmlData = DecompressToString(source);

            var dumpAndDie = (string errorMessage) =>
            {
                var tmp = Path.Combine(
                    Path.GetTempPath(),
                    "winternals-assembly-manifest.xml"
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

        public string DecompressToString(Stream stream)
        {
            var head = new byte[5];
            stream.WithSavedPosition(() =>
            {
                stream.Read(head);
            });
            var dataType = GetDataType(head);

            switch (dataType)
            {
                default:
                    throw new NotSupportedException();
                case ManifestDataType.MsPatch:
                    {
                        // skip DCM magic
                        var manifestSpan = stream.ReadToEnd().Span;
                        var output = MsPatch.ApplyPatch(dictionary.Span, manifestSpan.Slice(sizeof(uint)));
                        using var reader = new StreamReader(new MemoryStream(output));
                        return reader.ReadToEnd() ?? "";
                    }
                case ManifestDataType.Uncompressed:
                    {
                        using var reader = new StreamReader(stream);
                        return reader.ReadToEnd() ?? "";
                    }
            }
        }
    }
}
