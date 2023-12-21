using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.XMLDSig
{
    [XmlRoot(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transform
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class Transforms
    {
        [XmlElement(ElementName = "Transform", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
        public Transform Transform { get; set; }
        [XmlAttribute(AttributeName = "dsig", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dsig { get; set; }
    }


    [XmlRoot(ElementName = "DigestMethod", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestMethod
    {
        [XmlAttribute(AttributeName = "Algorithm")]
        public string Algorithm { get; set; }
        [XmlAttribute(AttributeName = "dsig", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dsig { get; set; }
    }

    [XmlRoot(ElementName = "DigestValue", Namespace = "http://www.w3.org/2000/09/xmldsig#")]
    public class DigestValue
    {
        [XmlAttribute(AttributeName = "dsig", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dsig { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
