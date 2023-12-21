﻿using Smx.Winter.SchemaDefinitions.XMLDSig;
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.AsmV2
{
    [XmlRoot(ElementName = "hash", Namespace = "urn:schemas-microsoft-com:asm.v2")]
    public class Hash
    {
        [XmlElement(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transforms Transforms { get; set; }
        [XmlElement(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestMethod DigestMethod { get; set; }
        [XmlElement(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public DigestValue DigestValue { get; set; }
        [XmlAttribute(AttributeName = "asmv2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Asmv2 { get; set; }
        [XmlAttribute(AttributeName = "dsig", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dsig { get; set; }
    }

    [XmlRoot(ElementName = "runtime", Namespace = "urn:schemas-microsoft-com:asm.v2")]
    public class Runtime
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "imageVersion")]
        public string ImageVersion { get; set; }
    }

}
