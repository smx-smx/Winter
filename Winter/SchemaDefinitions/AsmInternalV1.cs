using System.Xml.Serialization;

namespace Smx.Winter.AsmInternalV1
{
    [XmlRoot(ElementName = "sequence", Namespace = "urn:schemas-microsoft-com:asm.internal.v1")]
    public class Sequence
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "languagePack", Namespace = "urn:schemas-microsoft-com:asm.internal.v1")]
    public class LanguagePack
    {
        [XmlElement(ElementName = "sequence", Namespace = "urn:schemas-microsoft-com:asm.internal.v1")]
        public Sequence Sequence { get; set; }
        [XmlElement(ElementName = "bilingualComponents", Namespace = "urn:schemas-microsoft-com:asm.internal.v1")]
        public string BilingualComponents { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "auto-ns1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Autons1 { get; set; }
    }
}
