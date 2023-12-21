﻿
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.CEPSvcNS
{
    [XmlRoot(ElementName = "TemplateData", Namespace = "CEPSvcNS")]
    public class TemplateData
    {
        [XmlElement(ElementName = "Template", Namespace = "CEPSvcNS")]
        public string Template { get; set; }
        [XmlElement(ElementName = "TemplateDetails", Namespace = "CEPSvcNS")]
        public string TemplateDetails { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }



    [XmlRoot(ElementName = "CAData", Namespace = "CEPSvcNS")]
    public class CAData
    {
        [XmlElement(ElementName = "CA", Namespace = "CEPSvcNS")]
        public string CA { get; set; }
        [XmlElement(ElementName = "CADetails", Namespace = "CEPSvcNS")]
        public string CADetails { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "OIDCollection", Namespace = "CEPSvcNS")]
    public class OIDCollection
    {
        [XmlElement(ElementName = "OIDs", Namespace = "CEPSvcNS")]
        public string OIDs { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
