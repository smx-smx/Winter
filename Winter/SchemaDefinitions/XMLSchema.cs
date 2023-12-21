using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.XMLSchema
{
    [XmlRoot(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Element
    {
        [XmlAttribute(AttributeName = "default", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Default { get; set; }
        [XmlAttribute(AttributeName = "default")]
        public string Default2 { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "accessControl", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string AccessControl { get; set; }
        [XmlAttribute(AttributeName = "displayName", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "handler", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Handler { get; set; }
        [XmlAttribute(AttributeName = "scope", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Scope { get; set; }
        [XmlAttribute(AttributeName = "description", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "legacyName", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string LegacyName { get; set; }
        [XmlAttribute(AttributeName = "legacyType", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string LegacyType { get; set; }
        [XmlAttribute(AttributeName = "passes", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Passes { get; set; }
        [XmlAttribute(AttributeName = "migrate", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Migrate { get; set; }
        [XmlAttribute(AttributeName = "visible", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "dataOnly", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string DataOnly { get; set; }
        [XmlAttribute(AttributeName = "subScope", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string SubScope { get; set; }
        [XmlAttribute(AttributeName = "changeImpact", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string ChangeImpact { get; set; }
        [XmlAttribute(AttributeName = "perUserVirtualization", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string PerUserVirtualization { get; set; }
        [XmlAttribute(AttributeName = "deprecated", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Deprecated { get; set; }
        [XmlAttribute(AttributeName = "nillable")]
        public string Nillable { get; set; }
        [XmlAttribute(AttributeName = "maxOccurs")]
        public string MaxOccurs { get; set; }
        [XmlAttribute(AttributeName = "minOccurs")]
        public string MinOccurs { get; set; }
        [XmlAttribute(AttributeName = "key", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "pattern", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Pattern { get; set; }
    }

    [XmlRoot(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Enumeration
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "minInclusive", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class MinInclusive
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "maxInclusive", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class MaxInclusive
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "maxLength", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class MaxLength
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Restriction
    {
        [XmlElement(ElementName = "enumeration", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Enumeration> Enumeration { get; set; }
        [XmlElement(ElementName = "minInclusive", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public MinInclusive MinInclusive { get; set; }
        [XmlElement(ElementName = "maxInclusive", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public MaxInclusive MaxInclusive { get; set; }
        [XmlElement(ElementName = "maxLength", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public MaxLength MaxLength { get; set; }
        [XmlAttribute(AttributeName = "base")]
        public string Base { get; set; }
    }

    [XmlRoot(ElementName = "simpleType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class SimpleType
    {
        [XmlElement(ElementName = "restriction", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Restriction Restriction { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "description", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Sequence2
    {
        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Element> Element { get; set; }
    }

    [XmlRoot(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class ComplexType
    {
        [XmlElement(ElementName = "sequence", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Sequence2 Sequence2 { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "description", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
    public class Schema
    {
        [XmlElement(ElementName = "element", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<Element> Element { get; set; }
        [XmlElement(ElementName = "simpleType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<SimpleType> SimpleType { get; set; }
        [XmlElement(ElementName = "complexType", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public List<ComplexType> ComplexType { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "targetNamespace")]
        public string TargetNamespace { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "attributeFormDefault")]
        public string AttributeFormDefault { get; set; }
        [XmlAttribute(AttributeName = "elementFormDefault")]
        public string ElementFormDefault { get; set; }
        [XmlAttribute(AttributeName = "app", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string App { get; set; }
    }

}
