using Smx.Winter.SchemaDefinitions.AsmV3;
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.DefaultNS
{
    [XmlRoot(ElementName = "pattern")]
    public class Pattern
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "mode")]
        public string Mode { get; set; }
    }

    [XmlRoot(ElementName = "objectSet")]
    public class ObjectSet
    {
        [XmlElement(ElementName = "pattern")]
        public List<Pattern> Pattern { get; set; }
        [XmlElement(ElementName = "conditions")]
        public Conditions Conditions { get; set; }
        [XmlElement(ElementName = "content")]
        public List<Content> Content { get; set; }
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "content")]
    public class Content
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
        [XmlAttribute(AttributeName = "filter")]
        public string Filter { get; set; }
    }

    [XmlRoot(ElementName = "condition")]
    public class Condition
    {
        [XmlAttribute(AttributeName = "negation")]
        public string Negation { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "conditions")]
    public class Conditions
    {
        [XmlElement(ElementName = "condition")]
        public List<Condition> Condition { get; set; }
        [XmlAttribute(AttributeName = "operation")]
        public string Operation { get; set; }
    }

    [XmlRoot(ElementName = "include")]
    public class Include
    {
        [XmlElement(ElementName = "objectSet")]
        public List<ObjectSet> ObjectSet { get; set; }
        [XmlElement(ElementName = "objectset")]
        public Objectset Objectset { get; set; }
    }

    [XmlRoot(ElementName = "exclude")]
    public class Exclude
    {
        [XmlElement(ElementName = "objectSet")]
        public List<ObjectSet> ObjectSet { get; set; }
        [XmlElement(ElementName = "conditions")]
        public Conditions Conditions { get; set; }
        [XmlElement(ElementName = "pattern")]
        public List<Pattern> Pattern { get; set; }
        [XmlAttribute(AttributeName = "filter")]
        public string Filter { get; set; }
    }

    [XmlRoot(ElementName = "merge")]
    public class Merge
    {
        [XmlElement(ElementName = "objectSet")]
        public List<ObjectSet> ObjectSet { get; set; }
        [XmlElement(ElementName = "pattern")]
        public Pattern Pattern { get; set; }
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
        [XmlElement(ElementName = "include")]
        public Include Include { get; set; }
    }

    [XmlRoot(ElementName = "objecSet")]
    public class ObjecSet
    {
        [XmlElement(ElementName = "pattern")]
        public List<Pattern> Pattern { get; set; }
    }

    [XmlRoot(ElementName = "excludeAttributes")]
    public class ExcludeAttributes
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
        [XmlElement(ElementName = "objecSet")]
        public ObjecSet ObjecSet { get; set; }
        [XmlAttribute(AttributeName = "attributes")]
        public string Attributes { get; set; }
    }

    [XmlRoot(ElementName = "locationModify")]
    public class LocationModify
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
    }

    [XmlRoot(ElementName = "detect")]
    public class Detect
    {
        [XmlElement(ElementName = "condition")]
        public List<Condition> Condition { get; set; }
    }

    [XmlRoot(ElementName = "detects")]
    public class Detects
    {
        [XmlElement(ElementName = "detect")]
        public List<Detect> Detect { get; set; }
    }

    [XmlRoot(ElementName = "destinationCleanup")]
    public class DestinationCleanup
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
    }

    [XmlRoot(ElementName = "processing")]
    public class Processing
    {
        [XmlElement(ElementName = "script")]
        public List<string> Script { get; set; }
        [XmlAttribute(AttributeName = "when")]
        public string When { get; set; }
    }

    [XmlRoot(ElementName = "location")]
    public class Location
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "bytes")]
    public class Bytes
    {
        [XmlAttribute(AttributeName = "expand")]
        public string Expand { get; set; }
        [XmlAttribute(AttributeName = "string")]
        public string String { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "object")]
    public class Object
    {
        [XmlElement(ElementName = "location")]
        public Location Location { get; set; }
        [XmlElement(ElementName = "attributes")]
        public string Attributes { get; set; }
        [XmlElement(ElementName = "bytes")]
        public Bytes Bytes { get; set; }
    }

    [XmlRoot(ElementName = "addObjects")]
    public class AddObjects
    {
        [XmlElement(ElementName = "conditions")]
        public Conditions Conditions { get; set; }
        [XmlElement(ElementName = "object")]
        public List<Object> Object { get; set; }
    }

    [XmlRoot(ElementName = "externalProcess")]
    public class ExternalProcess
    {
        [XmlElement(ElementName = "commandLine")]
        public string CommandLine { get; set; }
        [XmlAttribute(AttributeName = "when")]
        public string When { get; set; }
    }

    [XmlRoot(ElementName = "unconditionalExclude")]
    public class UnconditionalExclude
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
    }

    [XmlRoot(ElementName = "rules")]
    public class Rules
    {
        [XmlElement(ElementName = "include")]
        public List<Include> Include { get; set; }
        [XmlElement(ElementName = "exclude")]
        public List<Exclude> Exclude { get; set; }
        [XmlElement(ElementName = "merge")]
        public List<Merge> Merge { get; set; }
        [XmlElement(ElementName = "conditions")]
        public Conditions Conditions { get; set; }
        [XmlElement(ElementName = "excludeAttributes")]
        public ExcludeAttributes ExcludeAttributes { get; set; }
        [XmlElement(ElementName = "locationModify")]
        public List<LocationModify> LocationModify { get; set; }
        [XmlElement(ElementName = "detects")]
        public Detects Detects { get; set; }
        [XmlElement(ElementName = "destinationCleanup")]
        public DestinationCleanup DestinationCleanup { get; set; }
        [XmlElement(ElementName = "processing")]
        public List<Processing> Processing { get; set; }
        [XmlElement(ElementName = "addObjects")]
        public AddObjects AddObjects { get; set; }
        [XmlElement(ElementName = "externalProcess")]
        public List<ExternalProcess> ExternalProcess { get; set; }
        [XmlElement(ElementName = "unconditionalExclude")]
        public UnconditionalExclude UnconditionalExclude { get; set; }
        [XmlAttribute(AttributeName = "context")]
        public string Context { get; set; }
        [XmlAttribute(AttributeName = "Context")]
        public string Context2 { get; set; }
        [XmlElement(ElementName = "contentModify")]
        public List<ContentModify> ContentModify { get; set; }
        [XmlElement(ElementName = "environment")]
        public Environment Environment { get; set; }
        [XmlAttribute(AttributeName = "rules")]
        public string _rules { get; set; }
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
        [XmlElement(ElementName = "rules")]
        public Rules _Rules { get; set; }
        [XmlElement(ElementName = "trackChanges")]
        public TrackChanges TrackChanges { get; set; }
        [XmlElement(ElementName = "destinationcleanup")]
        public Destinationcleanup Destinationcleanup { get; set; }

        /// <summary>
        /// probably a mistake? reads literally `"` in the manifest file
        /// </summary>
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "variable")]
    public class Variable
    {
        [XmlElement(ElementName = "text")]
        public string Text { get; set; }
        [XmlElement(ElementName = "script")]
        public string Script { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "remap")]
        public string Remap { get; set; }
    }

    [XmlRoot(ElementName = "environment")]
    public class Environment
    {
        [XmlElement(ElementName = "variable")]
        public List<Variable> Variable { get; set; }
        [XmlElement(ElementName = "conditions")]
        public Conditions Conditions { get; set; }
        [XmlAttribute(AttributeName = "context")]
        public string Context { get; set; }
    }

    [XmlRoot(ElementName = "plugin")]
    public class Plugin
    {
        [XmlAttribute(AttributeName = "classId")]
        public string ClassId { get; set; }
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
        [XmlAttribute(AttributeName = "critical")]
        public string Critical { get; set; }
        [XmlAttribute(AttributeName = "offlineApply")]
        public string OfflineApply { get; set; }
        [XmlAttribute(AttributeName = "threadingModel")]
        public string ThreadingModel { get; set; }
        [XmlAttribute(AttributeName = "offlineGather")]
        public string OfflineGather { get; set; }
    }

    [XmlRoot(ElementName = "transforms")]
    public class Transforms
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "migXml")]
    public class MigXml
    {
        [XmlElement(ElementName = "rules")]
        public List<Rules> Rules { get; set; }
        [XmlElement(ElementName = "environment")]
        public List<Environment> Environment { get; set; }
        [XmlElement(ElementName = "detects")]
        public Detects Detects { get; set; }
        [XmlElement(ElementName = "plugin")]
        public List<Plugin> Plugin { get; set; }
        [XmlElement(ElementName = "migrationDisplayID")]
        public string MigrationDisplayID { get; set; }
        [XmlElement(ElementName = "transforms")]
        public Transforms Transforms { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "contentModify")]
    public class ContentModify
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
    }

    [XmlRoot(ElementName = "objectset")]
    public class Objectset
    {
        [XmlElement(ElementName = "pattern")]
        public Pattern Pattern { get; set; }
    }

    [XmlRoot(ElementName = "trackChanges")]
    public class TrackChanges
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
    }

    [XmlRoot(ElementName = "machineSpecific")]
    public class MachineSpecific2
    {
        [XmlElement(ElementName = "migXml")]
        public MigXml MigXml { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponentIdentity")]
    public class SupportedComponentIdentity2
    {
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "settingsVersionRange")]
        public string SettingsVersionRange { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponent")]
    public class SupportedComponent2
    {
        [XmlElement(ElementName = "machineSpecific")]
        public MachineSpecific2 MachineSpecific2 { get; set; }
        [XmlElement(ElementName = "migXml")]
        public MigXml MigXml { get; set; }
        [XmlElement(ElementName = "supportedComponentIdentity", Namespace = "")]
        public SupportedComponentIdentity2 SupportedComponentIdentity2 { get; set; }
        [XmlElement(ElementName = "supportedComponentIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SupportedComponentIdentity SupportedComponentIdentity { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponents")]
    public class SupportedComponents2
    {
        [XmlElement(ElementName = "supportedComponent")]
        public List<SupportedComponent2> SupportedComponent2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "destinationcleanup")]
    public class Destinationcleanup
    {
        [XmlElement(ElementName = "objectSet")]
        public ObjectSet ObjectSet { get; set; }
    }

    [XmlRoot(ElementName = "migxml")]
    public class Migxml
    {
        [XmlElement(ElementName = "rules")]
        public Rules Rules { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }


}
