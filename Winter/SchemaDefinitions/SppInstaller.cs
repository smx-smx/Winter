using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.SppInstallerNS
{
    [XmlRoot(ElementName = "sppInstaller", Namespace = "urn:schemas-microsoft-com:spp:installer")]
    public class SppInstaller
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
