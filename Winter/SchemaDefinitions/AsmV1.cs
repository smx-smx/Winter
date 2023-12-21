using Smx.Winter.SchemaDefinitions.AsmV2;
using Smx.Winter.SchemaDefinitions.AsmV3;
using Smx.Winter.SchemaDefinitions.XMLDSig;
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.AsmV1
{
    [XmlRoot(ElementName = "noInheritable", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class NoInheritable
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class AssemblyIdentity
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }


    [XmlRoot(ElementName = "windowClass", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class WindowClass
    {
        [XmlAttribute(AttributeName = "versioned")]
        public string Versioned { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "comInterfaceProxyStub", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class ComInterfaceProxyStub
    {
        [XmlAttribute(AttributeName = "iid")]
        public string Iid { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "tlbid")]
        public string Tlbid { get; set; }
    }

    [XmlRoot(ElementName = "typelib", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class Typelib
    {
        [XmlAttribute(AttributeName = "tlbid")]
        public string Tlbid { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "helpdir")]
        public string Helpdir { get; set; }
    }

    [XmlRoot(ElementName = "comClass", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class ComClass
    {
        [XmlElement(ElementName = "progid", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public string Progid { get; set; }
        [XmlAttribute(AttributeName = "progid")]
        public string _Progid { get; set; }
        [XmlAttribute(AttributeName = "clsid")]
        public string Clsid { get; set; }
        [XmlAttribute(AttributeName = "threadingModel")]
        public string ThreadingModel { get; set; }
        [XmlAttribute(AttributeName = "tlbid")]
        public string Tlbid { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "file", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class File
    {
        [XmlElement(ElementName = "hash", Namespace = "urn:schemas-microsoft-com:asm.v2")]
        public Hash Hash { get; set; }
        [XmlAttribute(AttributeName = "hash")]
        public string _Hash { get; set; }
        [XmlElement(ElementName = "windowClass", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<WindowClass> WindowClass { get; set; }
        [XmlElement(ElementName = "comInterfaceProxyStub", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public ComInterfaceProxyStub ComInterfaceProxyStub { get; set; }
        [XmlElement(ElementName = "typelib", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public Typelib Typelib { get; set; }
        [XmlElement(ElementName = "comClass", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<ComClass> ComClass { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "hashalg")]
        public string Hashalg { get; set; }
        [XmlAttribute(AttributeName = "importPath", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string ImportPath { get; set; }
        [XmlAttribute(AttributeName = "sourceName", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SourceName { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlElement(ElementName = "signatureInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SignatureInfo SignatureInfo { get; set; }
    }

    [XmlRoot(ElementName = "bindingRedirect", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class BindingRedirect
    {
        [XmlAttribute(AttributeName = "oldVersion")]
        public string OldVersion { get; set; }
        [XmlAttribute(AttributeName = "newVersion")]
        public string NewVersion { get; set; }
    }

    [XmlRoot(ElementName = "dependentAssembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class DependentAssembly
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlElement(ElementName = "bindingRedirect", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<BindingRedirect> BindingRedirect { get; set; }
    }

    [XmlRoot(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class Dependency
    {
        [XmlElement(ElementName = "dependentAssembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public DependentAssembly DependentAssembly { get; set; }
        [XmlAttribute(AttributeName = "optional")]
        public string Optional { get; set; }
        [XmlAttribute(AttributeName = "discoverable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Discoverable { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class Id
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }
        [XmlAttribute(AttributeName = "typeName")]
        public string TypeName { get; set; }
    }

    [XmlRoot(ElementName = "categoryMembership", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class CategoryMembership
    {
        [XmlElement(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public Id Id { get; set; }
    }

    [XmlRoot(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class Memberships
    {
        [XmlElement(ElementName = "categoryMembership", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public CategoryMembership CategoryMembership { get; set; }
        [XmlAttribute(AttributeName = "cmiv2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cmiv2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }


    [XmlRoot(ElementName = "assembly", Namespace = "urn:schemas-microsoft-com:asm.v1")]
    public class Assembly
    {
        [XmlElement(ElementName = "noInheritable", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public string NoInheritable { get; set; }
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlElement(ElementName = "file", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<File> File { get; set; }
        [XmlElement(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public Memberships Memberships { get; set; }
        [XmlElement(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AsmV3.Memberships Memberships2 { get; set; }
        [XmlAttribute(AttributeName = "manifestVersion")]
        public string ManifestVersion { get; set; }
        [XmlElement(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<Dependency> Dependency { get; set; }
        [XmlAttribute(AttributeName = "copyright", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public string Copyright { get; set; }
        [XmlAttribute(AttributeName = "copyright", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Copyright2 { get; set; }
    }
}
