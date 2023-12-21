using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.MsmqV1
{
    [XmlRoot(ElementName = "MsmqWorkgroupOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqWorkgroupOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "MsmqAdIntegrationOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqAdIntegrationOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "MsmqHttpOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqHttpOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
