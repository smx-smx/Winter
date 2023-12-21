
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.CountersNS
{
    [XmlRoot(ElementName = "counterAttribute", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class CounterAttribute
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "counterAttributes", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class CounterAttributes
    {
        [XmlElement(ElementName = "counterAttribute", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public List<CounterAttribute> CounterAttribute { get; set; }
    }

    [XmlRoot(ElementName = "counter", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class Counter
    {
        [XmlElement(ElementName = "counterAttributes", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public CounterAttributes CounterAttributes { get; set; }
        [XmlAttribute(AttributeName = "defaultScale")]
        public string DefaultScale { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "descriptionID")]
        public string DescriptionID { get; set; }
        [XmlAttribute(AttributeName = "detailLevel")]
        public string DetailLevel { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "nameID")]
        public string NameID { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
        [XmlAttribute(AttributeName = "scope")]
        public string Scope { get; set; }
        [XmlAttribute(AttributeName = "aggregate")]
        public string Aggregate { get; set; }
        [XmlAttribute(AttributeName = "field")]
        public string Field { get; set; }
        [XmlAttribute(AttributeName = "struct")]
        public string Struct { get; set; }
        [XmlAttribute(AttributeName = "baseID")]
        public string BaseID { get; set; }
        [XmlAttribute(AttributeName = "perfFreqID")]
        public string PerfFreqID { get; set; }
        [XmlAttribute(AttributeName = "perfTimeID")]
        public string PerfTimeID { get; set; }
    }

    [XmlRoot(ElementName = "struct", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class Struct
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "structs", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class Structs
    {
        [XmlElement(ElementName = "struct", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public List<Struct> Struct { get; set; }
    }

    [XmlRoot(ElementName = "counterSet", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class CounterSet
    {
        [XmlElement(ElementName = "counter", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public List<Counter> Counter { get; set; }
        [XmlElement(ElementName = "structs", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public Structs Structs { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "descriptionID")]
        public string DescriptionID { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "instances")]
        public string Instances { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "nameID")]
        public string NameID { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
    }

    [XmlRoot(ElementName = "provider", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class Provider2
    {
        [XmlElement(ElementName = "counterSet", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public List<CounterSet> CounterSet { get; set; }
        [XmlAttribute(AttributeName = "applicationIdentity")]
        public string ApplicationIdentity { get; set; }
        [XmlAttribute(AttributeName = "callback")]
        public string Callback { get; set; }
        [XmlAttribute(AttributeName = "providerGuid")]
        public string ProviderGuid { get; set; }
        [XmlAttribute(AttributeName = "providerName")]
        public string ProviderName { get; set; }
        [XmlAttribute(AttributeName = "providerType")]
        public string ProviderType { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "resourceBase")]
        public string ResourceBase { get; set; }
    }

    [XmlRoot(ElementName = "counters", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
    public class Counters
    {
        [XmlElement(ElementName = "provider", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public List<Provider2> Provider2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "schemaVersion")]
        public string SchemaVersion { get; set; }
    }

    [XmlRoot(ElementName = "counters", Namespace = "http://schemas.microsoft.com/win/2004/08/counters")]
    public class Counters2
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
