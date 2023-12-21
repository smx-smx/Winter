
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.CompatibilityV1
{
    [XmlRoot(ElementName = "supportedOS", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class SupportedOS
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "application", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class Application2
    {
        [XmlElement(ElementName = "supportedOS", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
        public List<SupportedOS> SupportedOS { get; set; }
    }

    [XmlRoot(ElementName = "compatibility", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class Compatibility
    {
        [XmlElement(ElementName = "application", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
        public Application2 Application2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
