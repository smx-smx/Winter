
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.WindowsSettingsNS
{
    [XmlRoot(ElementName = "windowsSettings", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
    public class WindowsSettings
    {
        [XmlElement(ElementName = "dpiAware", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
        public string DpiAware { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "dpiAware", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
    public class DpiAware
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
