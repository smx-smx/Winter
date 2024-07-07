#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Xml.Serialization;

using Smx.Winter.SchemaDefinitions.EventsNS;
using Smx.Winter.SchemaDefinitions.RescacheV1;
using Smx.Winter.SchemaDefinitions.MsmqV1;
using Smx.Winter.SchemaDefinitions.SppInstallerNS;
using Smx.Winter.SchemaDefinitions.AsmV1;
using Smx.Winter.SchemaDefinitions.DefaultNS;
using Smx.Winter.SchemaDefinitions.AsmV2;
using Smx.Winter.SchemaDefinitions.WindowsSettingsNS;
using Smx.Winter.SchemaDefinitions.CompatibilityV1;
using Smx.Winter.SchemaDefinitions.CountersNS;
using Smx.Winter.AsmInternalV1;
using Smx.Winter.SchemaDefinitions.XMLSchema;
using System.Runtime.CompilerServices;

namespace Smx.Winter.SchemaDefinitions.AsmV3
{
    [XmlRoot(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class AssemblyIdentity
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "buildType")]
        public string BuildType { get; set; }
        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }
        [XmlAttribute(AttributeName = "versionScope")]
        public string VersionScope { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "deployment", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Deployment
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "dependentAssembly", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DependentAssembly
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "dependencyType")]
        public string DependencyType { get; set; }
    }

    [XmlRoot(ElementName = "localizedStrings", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class LocalizedStrings
    {
        [XmlElement(ElementName = "string", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<String> String { get; set; }
    }

    [XmlRoot(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Dependency
    {
        [XmlElement(ElementName = "capabilityIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CapabilityIdentity CapabilityIdentity { get; set; }
        [XmlElement(ElementName = "dependentAssembly", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public DependentAssembly DependentAssembly { get; set; }
        [XmlAttribute(AttributeName = "discoverable")]
        public string Discoverable { get; set; }
        [XmlAttribute(AttributeName = "resourceType")]
        public string ResourceType { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "optional")]
        public string Optional { get; set; }
        [XmlAttribute(AttributeName = "offlineInstall")]
        public string OfflineInstall { get; set; }
    }
    
    [XmlRoot(ElementName = "detectNone", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DetectNone
    {
        [XmlAttribute(AttributeName = "default")]
        public string Default { get; set; }
    }

    [XmlRoot(ElementName = "selectable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Selectable
    {
        [XmlElement(ElementName = "detectNone", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public DetectNone DetectNone { get; set; }
        [XmlAttribute(AttributeName = "disposition")]
        public string Disposition { get; set; }
        [XmlElement(ElementName = "customInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CustomInformation CustomInformation { get; set; }
    }

    [XmlRoot(ElementName = "declare", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Declare
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "applyTo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ApplyTo
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }


    [XmlRoot(ElementName = "require", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Require
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "MutualExclusionGroup", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class MutualExclusionGroup
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "MutualExclusion", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class MutualExclusion
    {
        [XmlElement(ElementName = "MutualExclusionGroup", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MutualExclusionGroup MutualExclusionGroup { get; set; }
    }

    [XmlRoot(ElementName = "satelliteInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SatelliteInfo
    {
        [XmlElement(ElementName = "declare", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Declare Declare { get; set; }
        [XmlElement(ElementName = "applyTo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ApplyTo> ApplyTo { get; set; }
        [XmlElement(ElementName = "require", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Require> Require { get; set; }
    }

    [XmlRoot(ElementName = "driverExtended", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DriverExtended
    {
        [XmlAttribute(AttributeName = "mum", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mum { get; set; }
        [XmlAttribute(AttributeName = "deviceInstallRequired")]
        public string DeviceInstallRequired { get; set; }
    }

    [XmlRoot(ElementName = "driver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Driver
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "inf")]
        public string Inf { get; set; }
        [XmlAttribute(AttributeName = "ranking")]
        public string Ranking { get; set; }
        [XmlElement(ElementName = "driverExtended", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public DriverExtended DriverExtended { get; set; }
    }


    [XmlRoot(ElementName = "phoneInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class PhoneInformation
    {
        [XmlAttribute(AttributeName = "phoneRelease")]
        public string PhoneRelease { get; set; }
        [XmlAttribute(AttributeName = "phoneOwner")]
        public string PhoneOwner { get; set; }
        [XmlAttribute(AttributeName = "phoneOwnerType")]
        public string PhoneOwnerType { get; set; }
        [XmlAttribute(AttributeName = "phoneComponent")]
        public string PhoneComponent { get; set; }
        [XmlAttribute(AttributeName = "phoneSubComponent")]
        public string PhoneSubComponent { get; set; }
        [XmlAttribute(AttributeName = "phoneGroupingKey")]
        public string PhoneGroupingKey { get; set; }
    }

    [XmlRoot(ElementName = "Update", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeploysUpdate
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "update", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Update
    {
        [XmlElement(ElementName = "customInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CustomInformation CustomInformation { get; set; }
        [XmlElement(ElementName = "driver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Driver Driver { get; set; }
        [XmlElement(ElementName = "selectable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Selectable Selectable { get; set; }
        [XmlElement(ElementName = "package", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Package Package { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlElement(ElementName = "component", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Component> Component { get; set; }
        [XmlElement(ElementName = "applicable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Applicable> Applicable { get; set; }
        [XmlAttribute(AttributeName = "restart")]
        public string Restart { get; set; }
        [XmlElement(ElementName = "capability", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Capability Capability { get; set; }
    }

    [XmlRoot(ElementName = "packageExtended", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class PackageExtended
    {
        [XmlAttribute(AttributeName = "mum2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mum2 { get; set; }
        [XmlAttribute(AttributeName = "completelyOfflineCapable")]
        public string CompletelyOfflineCapable { get; set; }
        [XmlAttribute(AttributeName = "packageSize")]
        public string PackageSize { get; set; }
    }

    [XmlRoot(ElementName = "securityDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SecurityDescriptor
    {
        [XmlAttribute(AttributeName = "buildFilter")]
        public string BuildFilter { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "offline")]
        public string Offline { get; set; }
    }

    [XmlRoot(ElementName = "gac", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Gac
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "clrversion")]
        public string Clrversion { get; set; }
        [XmlAttribute(AttributeName = "managedversion")]
        public string Managedversion { get; set; }
        [XmlAttribute(AttributeName = "clrarch")]
        public string Clrarch { get; set; }
        [XmlAttribute(AttributeName = "fileCopy")]
        public string FileCopy { get; set; }
        [XmlAttribute(AttributeName = "managedculture")]
        public string Managedculture { get; set; }
        [XmlAttribute(AttributeName = "parameter")]
        public string Parameter { get; set; }
    }

    [XmlRoot(ElementName = "ngen", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Ngen
    {
        [XmlAttribute(AttributeName = "managedversion")]
        public string Managedversion { get; set; }
        [XmlAttribute(AttributeName = "parameter")]
        public string Parameter { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "win64targetarch")]
        public string Win64targetarch { get; set; }
    }

    [XmlRoot(ElementName = "signatureDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SignatureDescriptor
    {
        [XmlAttribute(AttributeName = "PETrust")]
        public string PETrust { get; set; }
        [XmlAttribute(AttributeName = "pageHash")]
        public string PageHash { get; set; }
        [XmlAttribute(AttributeName = "DRMLevel")]
        public string DRMLevel { get; set; }
    }

    [XmlRoot(ElementName = "signatureInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SignatureInfo
    {
        [XmlElement(ElementName = "signatureDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<SignatureDescriptor> SignatureDescriptor { get; set; }
        [XmlAttribute(AttributeName = "cbb", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cbb { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "dependencies", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Dependencies
    {
        [XmlElement(ElementName = "dependentAssembly", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DependentAssembly> DependentAssembly { get; set; }
    }

    [XmlRoot(ElementName = "link", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Link
    {
        [XmlAttribute(AttributeName = "destination")]
        public string Destination { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "infFile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class InfFile
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "comClass", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ComClass
    {
        [XmlAttribute(AttributeName = "clsid")]
        public string Clsid { get; set; }
    }

    [XmlRoot(ElementName = "component", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Component
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
    }

    [XmlRoot(ElementName = "systemProtection", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SystemProtection
    {
        [XmlAttribute(AttributeName = "behavior")]
        public string Behavior { get; set; }
        [XmlAttribute(AttributeName = "perUserVirtualization")]
        public string PerUserVirtualization { get; set; }
        [XmlAttribute(AttributeName = "noJournaling")]
        public string NoJournaling { get; set; }
        [XmlAttribute(AttributeName = "noPerUserVirtualization")]
        public string NoPerUserVirtualization { get; set; }
    }

    [XmlRoot(ElementName = "file", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class File
    {
        [XmlElement(ElementName = "securityDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecurityDescriptor SecurityDescriptor { get; set; }
        [XmlElement(ElementName = "hash", Namespace = "urn:schemas-microsoft-com:asm.v2")]
        public Hash Hash { get; set; }
        [XmlAttribute(AttributeName = "hash")]
        public string _Hash { get; set; }
        [XmlElement(ElementName = "gac", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Gac Gac { get; set; }
        [XmlElement(ElementName = "ngen", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Ngen Ngen { get; set; }
        [XmlElement(ElementName = "signatureInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SignatureInfo SignatureInfo { get; set; }
        [XmlElement(ElementName = "dependencies", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Dependencies Dependencies { get; set; }
        [XmlElement(ElementName = "link", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Link> Link { get; set; }
        [XmlElement(ElementName = "infFile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public InfFile InfFile { get; set; }
        [XmlElement(ElementName = "resfile", Namespace = "urn:schemas-microsoft-com:rescache.v1")]
        public Resfile Resfile { get; set; }
        [XmlElement(ElementName = "lodctr", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Lodctr { get; set; }
        [XmlElement(ElementName = "comClass", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ComClass ComClass { get; set; }
        [XmlElement(ElementName = "lodctrDat", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string LodctrDat { get; set; }
        [XmlElement(ElementName = "systemProtection", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemProtection SystemProtection { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "destinationPath")]
        public string DestinationPath { get; set; }
        [XmlAttribute(AttributeName = "sourceName")]
        public string SourceName { get; set; }
        [XmlAttribute(AttributeName = "importPath")]
        public string ImportPath { get; set; }
        [XmlAttribute(AttributeName = "sourcePath")]
        public string SourcePath { get; set; }
        [XmlAttribute(AttributeName = "writeableType")]
        public string WriteableType { get; set; }
        [XmlAttribute(AttributeName = "compress")]
        public string Compress { get; set; }
        [XmlAttribute(AttributeName = "destinationName")]
        public string DestinationName { get; set; }
        [XmlAttribute(AttributeName = "attributes")]
        public string Attributes { get; set; }
        [XmlAttribute(AttributeName = "hashalg")]
        public string Hashalg { get; set; }
    }

    [XmlRoot(ElementName = "securityDescriptorDefinition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SecurityDescriptorDefinition
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "sddl")]
        public string Sddl { get; set; }
        [XmlAttribute(AttributeName = "operationHint")]
        public string OperationHint { get; set; }
        [XmlAttribute(AttributeName = "buildFilter")]
        public string BuildFilter { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
    }

    [XmlRoot(ElementName = "securityDescriptorDefinitions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SecurityDescriptorDefinitions
    {
        [XmlElement(ElementName = "securityDescriptorDefinition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<SecurityDescriptorDefinition> SecurityDescriptorDefinition { get; set; }
    }

    [XmlRoot(ElementName = "groupTrustee", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class GroupTrustee
    {
        [XmlElement(ElementName = "capabilities", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Capabilities { get; set; }
        [XmlElement(ElementName = "privileges", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Privileges { get; set; }
        [XmlElement(ElementName = "members", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Members { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "fullName")]
        public string FullName { get; set; }
    }

    [XmlRoot(ElementName = "trustees", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Trustees
    {
        [XmlElement(ElementName = "groupTrustee", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public GroupTrustee GroupTrustee { get; set; }
    }

    [XmlRoot(ElementName = "accessControl", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class AccessControl
    {
        [XmlElement(ElementName = "securityDescriptorDefinitions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecurityDescriptorDefinitions SecurityDescriptorDefinitions { get; set; }
        [XmlElement(ElementName = "trustees", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Trustees Trustees { get; set; }
    }

    [XmlRoot(ElementName = "requestedExecutionLevel", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RequestedExecutionLevel
    {
        [XmlAttribute(AttributeName = "level")]
        public string Level { get; set; }
        [XmlAttribute(AttributeName = "uiAccess")]
        public string UiAccess { get; set; }
        [XmlAttribute(AttributeName = "uiAutomation")]
        public string UiAutomation { get; set; }
    }

    [XmlRoot(ElementName = "requestedPrivileges", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RequestedPrivileges
    {
        [XmlElement(ElementName = "requestedExecutionLevel", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public RequestedExecutionLevel RequestedExecutionLevel { get; set; }
    }

    [XmlRoot(ElementName = "security", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Security
    {
        [XmlElement(ElementName = "accessControl", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AccessControl AccessControl { get; set; }
        [XmlElement(ElementName = "requestedPrivileges", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public RequestedPrivileges RequestedPrivileges { get; set; }
    }

    [XmlRoot(ElementName = "trustInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class TrustInfo
    {
        [XmlElement(ElementName = "security", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Security Security { get; set; }
    }

    [XmlRoot(ElementName = "string", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class String
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        /** mum string, likely conflicting... (but it's in the same NS?!) **/

        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
    }

    [XmlRoot(ElementName = "stringTable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class StringTable
    {
        [XmlElement(ElementName = "string", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<String> String { get; set; }
    }

    [XmlRoot(ElementName = "resources", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Resources
    {
        [XmlElement(ElementName = "stringTable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public StringTable StringTable { get; set; }
        [XmlAttribute(AttributeName = "culture")]
        public string Culture { get; set; }
    }

    [XmlRoot(ElementName = "localization", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Localization
    {
        [XmlElement(ElementName = "resources", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Resources Resources { get; set; }
    }


    [XmlRoot(ElementName = "registryValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RegistryValue
    {
        [XmlElement(ElementName = "systemProtection", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemProtection SystemProtection { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "valueType")]
        public string ValueType { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "operationHint")]
        public string OperationHint { get; set; }
        [XmlAttribute(AttributeName = "mutable")]
        public string Mutable { get; set; }
        [XmlAttribute(AttributeName = "IEDownlevelValue")]
        public string IEDownlevelValue { get; set; }
    }

    [XmlRoot(ElementName = "overridable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Overridable
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "scope")]
        public string Scope { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "registryKey", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RegistryKey
    {
        [XmlElement(ElementName = "registryValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<RegistryValue> RegistryValue { get; set; }
        [XmlElement(ElementName = "securityDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecurityDescriptor SecurityDescriptor { get; set; }
        [XmlElement(ElementName = "overridable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Overridable> Overridable { get; set; }
        [XmlElement(ElementName = "systemProtection", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemProtection SystemProtection { get; set; }
        [XmlAttribute(AttributeName = "keyName")]
        public string KeyName { get; set; }
        [XmlAttribute(AttributeName = "perUserVirtualization")]
        public string PerUserVirtualization { get; set; }
        [XmlAttribute(AttributeName = "owner")]
        public string Owner { get; set; }
        [XmlAttribute(AttributeName = "mapDesiredAccessToMaxAllowedAccess")]
        public string MapDesiredAccessToMaxAllowedAccess { get; set; }
    }

    [XmlRoot(ElementName = "registryKeys", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RegistryKeys
    {
        [XmlElement(ElementName = "registryKey", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<RegistryKey> RegistryKey { get; set; }
        [XmlAttribute(AttributeName = "asm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Asm { get; set; }
    }

    [XmlRoot(ElementName = "directory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Directory
    {
        [XmlElement(ElementName = "securityDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecurityDescriptor SecurityDescriptor { get; set; }
        [XmlElement(ElementName = "systemProtection", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemProtection SystemProtection { get; set; }
        [XmlAttribute(AttributeName = "destinationPath")]
        public string DestinationPath { get; set; }
        [XmlAttribute(AttributeName = "owner")]
        public string Owner { get; set; }
        [XmlAttribute(AttributeName = "compression")]
        public string Compression { get; set; }
        [XmlAttribute(AttributeName = "attributes")]
        public string Attributes { get; set; }
    }

    [XmlRoot(ElementName = "directories", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Directories
    {
        [XmlElement(ElementName = "directory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Directory> Directory { get; set; }
    }

    [XmlRoot(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v3")]
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
        [XmlAttribute(AttributeName = "buildType")]
        public string BuildType { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "versionScope")]
        public string VersionScope { get; set; }
    }

    [XmlRoot(ElementName = "linkId", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class LinkId
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "ranking")]
        public string Ranking { get; set; }
    }

    [XmlRoot(ElementName = "tile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Tile
    {
        [XmlAttribute(AttributeName = "background")]
        public string Background { get; set; }
        [XmlAttribute(AttributeName = "defaultSmallTilePath")]
        public string DefaultSmallTilePath { get; set; }
        [XmlAttribute(AttributeName = "defaultSquare70x70TilePath")]
        public string DefaultSquare70x70TilePath { get; set; }
        [XmlAttribute(AttributeName = "foreground")]
        public string Foreground { get; set; }
        [XmlAttribute(AttributeName = "hostEnvironment")]
        public string HostEnvironment { get; set; }
        [XmlAttribute(AttributeName = "itemNameDisplay")]
        public string ItemNameDisplay { get; set; }
        [XmlAttribute(AttributeName = "itemNameDisplayFlag")]
        public string ItemNameDisplayFlag { get; set; }
        [XmlAttribute(AttributeName = "logoImagePath")]
        public string LogoImagePath { get; set; }
        [XmlAttribute(AttributeName = "packageFamilyMoniker")]
        public string PackageFamilyMoniker { get; set; }
        [XmlAttribute(AttributeName = "packageInstallPath")]
        public string PackageInstallPath { get; set; }
        [XmlAttribute(AttributeName = "packageMoniker")]
        public string PackageMoniker { get; set; }
        [XmlAttribute(AttributeName = "shortItemNameDisplay")]
        public string ShortItemNameDisplay { get; set; }
        [XmlAttribute(AttributeName = "softwarePublisher")]
        public string SoftwarePublisher { get; set; }
        [XmlAttribute(AttributeName = "winStoreCategoryId")]
        public string WinStoreCategoryId { get; set; }
    }

    [XmlRoot(ElementName = "shortCut", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ShortCut
    {
        [XmlElement(ElementName = "linkId", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<LinkId> LinkId { get; set; }
        [XmlElement(ElementName = "tile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Tile Tile { get; set; }
        [XmlAttribute(AttributeName = "arguments")]
        public string Arguments { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "destinationName")]
        public string DestinationName { get; set; }
        [XmlAttribute(AttributeName = "destinationPath")]
        public string DestinationPath { get; set; }
        [XmlAttribute(AttributeName = "displayResource")]
        public string DisplayResource { get; set; }
        [XmlAttribute(AttributeName = "iconPath")]
        public string IconPath { get; set; }
        [XmlAttribute(AttributeName = "targetPath")]
        public string TargetPath { get; set; }
        [XmlAttribute(AttributeName = "windowStyle")]
        public string WindowStyle { get; set; }
        [XmlAttribute(AttributeName = "workingDirectory")]
        public string WorkingDirectory { get; set; }
        [XmlAttribute(AttributeName = "isFeatureOnDemand")]
        public string IsFeatureOnDemand { get; set; }
        [XmlAttribute(AttributeName = "appID")]
        public string AppID { get; set; }
        [XmlAttribute(AttributeName = "runAsUser")]
        public string RunAsUser { get; set; }
        [XmlAttribute(AttributeName = "winXHash")]
        public string WinXHash { get; set; }
        [XmlAttribute(AttributeName = "fileAttributes")]
        public string FileAttributes { get; set; }
        [XmlAttribute(AttributeName = "isSystemComponent")]
        public string IsSystemComponent { get; set; }
    }

    [XmlRoot(ElementName = "triggerData", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class TriggerData
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "serviceTrigger", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceTrigger
    {
        [XmlElement(ElementName = "triggerData", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<TriggerData> TriggerData { get; set; }
        [XmlAttribute(AttributeName = "action")]
        public string Action { get; set; }
        [XmlAttribute(AttributeName = "subtype")]
        public string Subtype { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "action", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Action
    {
        [XmlAttribute(AttributeName = "delay")]
        public string Delay { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "actions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Actions
    {
        [XmlElement(ElementName = "action", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Action> Action { get; set; }
    }

    [XmlRoot(ElementName = "failureActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FailureActions
    {
        [XmlElement(ElementName = "actions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Actions Actions { get; set; }
        [XmlAttribute(AttributeName = "resetPeriod")]
        public string ResetPeriod { get; set; }
        [XmlAttribute(AttributeName = "command")]
        public string Command { get; set; }
        [XmlAttribute(AttributeName = "rebootMessage")]
        public string RebootMessage { get; set; }
    }

    [XmlRoot(ElementName = "serviceData", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceData
    {
        [XmlElement(ElementName = "serviceTrigger", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ServiceTrigger> ServiceTrigger { get; set; }
        [XmlElement(ElementName = "failureActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public FailureActions FailureActions { get; set; }
        [XmlElement(ElementName = "securityDescriptor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecurityDescriptor SecurityDescriptor { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "errorControl")]
        public string ErrorControl { get; set; }
        [XmlAttribute(AttributeName = "start")]
        public string Start { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "dependOnService")]
        public string DependOnService { get; set; }
        [XmlAttribute(AttributeName = "imagePath")]
        public string ImagePath { get; set; }
        [XmlAttribute(AttributeName = "objectName")]
        public string ObjectName { get; set; }
        [XmlAttribute(AttributeName = "requiredPrivileges")]
        public string RequiredPrivileges { get; set; }
        [XmlAttribute(AttributeName = "sidType")]
        public string SidType { get; set; }
        [XmlAttribute(AttributeName = "launchProtected")]
        public string LaunchProtected { get; set; }
        [XmlAttribute(AttributeName = "group")]
        public string Group { get; set; }
        [XmlAttribute(AttributeName = "tag")]
        public string Tag { get; set; }
        [XmlAttribute(AttributeName = "startAfterInstall")]
        public string StartAfterInstall { get; set; }
        [XmlAttribute(AttributeName = "failureActionsFlag")]
        public string FailureActionsFlag { get; set; }
        [XmlAttribute(AttributeName = "dependOnGroup")]
        public string DependOnGroup { get; set; }
    }

    [XmlRoot(ElementName = "serviceGroup", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceGroup
    {
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlAttribute(AttributeName = "serviceName")]
        public string ServiceName { get; set; }
    }

    [XmlRoot(ElementName = "EnableSelectabilityForEdition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class EnableSelectabilityForEdition
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "EnableSelectabilityForEditions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class EnableSelectabilityForEditions
    {
        [XmlElement(ElementName = "EnableSelectabilityForEdition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public EnableSelectabilityForEdition EnableSelectabilityForEdition { get; set; }
    }


    [XmlRoot(ElementName = "customInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CustomInformation
    {
        [XmlAttribute(AttributeName = "FWLink")]
        public string FWLink { get; set; }
        [XmlElement(ElementName = "ServerComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ServerComponent ServerComponent { get; set; }
        [XmlElement(ElementName = "noAutoMerge", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string NoAutoMerge { get; set; }
        [XmlElement(ElementName = "OptionalFeatures", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public OptionalFeatures OptionalFeatures { get; set; }
        [XmlElement(ElementName = "Version", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Version Version { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string VersionAttribute { get; set; }
        [XmlAttribute(AttributeName = "mum2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Mum2 { get; set; }
        [XmlAttribute(AttributeName = "PackageFormat")]
        public string PackageFormat { get; set; }
        [XmlAttribute(AttributeName = "PackageSupportedFeatures")]
        public string PackageSupportedFeatures { get; set; }
        [XmlElement(ElementName = "phoneInformation")]
        public PhoneInformation PhoneInformation { get; set; }
        [XmlElement(ElementName = "EnableSelectabilityForEditions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public EnableSelectabilityForEditions EnableSelectabilityForEditions { get; set; }
        [XmlAttribute(AttributeName = "LPTargetSPLevel")]
        public string LPTargetSPLevel { get; set; }
        [XmlAttribute(AttributeName = "LPType")]
        public string LPType { get; set; }
        [XmlAttribute(AttributeName = "SoftBlockLink")]
        public string SoftBlockLink { get; set; }
    }


    [XmlRoot(ElementName = "parent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Parent
    {
        [XmlElement(ElementName = "parent")]
        public Parent ParentRef { get; set; }
        [XmlAttribute(AttributeName = "set")]
        public string Set { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<AssemblyIdentity> AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "integrate")]
        public string Integrate { get; set; }
        [XmlAttribute(AttributeName = "disposition")]
        public string Disposition { get; set; }
        [XmlAttribute(AttributeName = "buildCompare")]
        public string BuildCompare { get; set; }
        [XmlAttribute(AttributeName = "distributionCompare")]
        public string DistributionCompare { get; set; }
        [XmlAttribute(AttributeName = "revisionCompare")]
        public string RevisionCompare { get; set; }
        [XmlAttribute(AttributeName = "serviceCompare")]
        public string ServiceCompare { get; set; }
    }


    [XmlRoot(ElementName = "installerAssembly", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class InstallerAssembly
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "versionScope")]
        public string VersionScope { get; set; }
        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }
    }

    [XmlRoot(ElementName = "SystemService", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SystemService
    {
        [XmlAttribute(AttributeName = "DefaultMonitoring")]
        public string DefaultMonitoring { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "SystemServices", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SystemServices
    {
        [XmlElement(ElementName = "SystemService", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemService SystemService { get; set; }
    }


    [XmlRoot(ElementName = "detectUpdate", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DetectUpdate
    {
        [XmlElement(ElementName = "parent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Parent> Parent { get; set; }
        [XmlElement(ElementName = "update", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Update Update { get; set; }
    }

    [XmlRoot(ElementName = "updateComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class UpdateComponent
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<AssemblyIdentity> AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "elevate")]
        public string Elevate { get; set; }
    }

    [XmlRoot(ElementName = "applicable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Applicable
    {
        [XmlElement(ElementName = "detectUpdate", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DetectUpdate> DetectUpdate { get; set; }
        [XmlAttribute(AttributeName = "disposition")]
        public string Disposition { get; set; }
        [XmlElement(ElementName = "updateComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public UpdateComponent UpdateComponent { get; set; }
    }

    [XmlRoot(ElementName = "package", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Package
    {
        [XmlElement(ElementName = "localizedStrings", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public LocalizedStrings LocalizedStrings { get; set; }
        [XmlElement(ElementName = "declareCapability")]
        public DeclareCapability DeclareCapability { get; set; }
        [XmlElement(ElementName = "customInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CustomInformation CustomInformation { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "applicabilityEvaluation")]
        public string ApplicabilityEvaluation { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "integrate")]
        public string Integrate { get; set; }
        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
        [XmlAttribute(AttributeName = "releaseType")]
        public string ReleaseType { get; set; }
        [XmlElement(ElementName = "parent")]
        public List<Parent> Parent { get; set; }
        [XmlElement(ElementName = "update")]
        public List<Update> Update { get; set; }
        [XmlAttribute(AttributeName = "contained")]
        public string Contained { get; set; }
        [XmlAttribute(AttributeName = "targetPartition")]
        public string TargetPartition { get; set; }
        [XmlAttribute(AttributeName = "binaryPartition")]
        public string BinaryPartition { get; set; }
        [XmlAttribute(AttributeName = "restart")]
        public string Restart { get; set; }
        [XmlElement(ElementName = "installerAssembly")]
        public InstallerAssembly InstallerAssembly { get; set; }
        [XmlElement(ElementName = "packageExtended", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public PackageExtended PackageExtended { get; set; }
        [XmlAttribute(AttributeName = "selfUpdate")]
        public string SelfUpdate { get; set; }
        [XmlAttribute(AttributeName = "permanence")]
        public string Permanence { get; set; }
        [XmlAttribute(AttributeName = "psfName")]
        public string PsfName { get; set; }
    }

    [XmlRoot(ElementName = "filter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Filter
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
    }

    [XmlRoot(ElementName = "providerOrder", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ProviderOrder
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
    }

    [XmlRoot(ElementName = "path", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Path
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "position")]
        public string Position { get; set; }
    }

    [XmlRoot(ElementName = "categoryInstance", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CategoryInstance
    {
        [XmlElement(ElementName = "shortCut", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ShortCut ShortCut { get; set; }
        [XmlElement(ElementName = "serviceData", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ServiceData ServiceData { get; set; }
        [XmlElement(ElementName = "serviceGroup", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ServiceGroup ServiceGroup { get; set; }
        [XmlElement(ElementName = "package", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Package Package { get; set; }
        [XmlElement(ElementName = "filter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Filter Filter { get; set; }
        [XmlElement(ElementName = "providerOrder", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ProviderOrder ProviderOrder { get; set; }
        [XmlElement(ElementName = "path", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Path Path { get; set; }
        [XmlAttribute(AttributeName = "subcategory")]
        public string Subcategory { get; set; }
    }

    [XmlRoot(ElementName = "categoryMembership", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CategoryMembership
    {
        [XmlElement(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Id Id { get; set; }
        [XmlElement(ElementName = "categoryInstance", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<CategoryInstance> CategoryInstance { get; set; }
    }

    [XmlRoot(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Memberships
    {
        [XmlElement(ElementName = "categoryMembership", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<CategoryMembership> CategoryMembership { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponentIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SupportedComponentIdentity
    {
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "processorArchitecture")]
        public string ProcessorArchitecture { get; set; }
        [XmlAttribute(AttributeName = "settingsVersionRange")]
        public string SettingsVersionRange { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "buildType")]
        public string BuildType { get; set; }
        [XmlAttribute(AttributeName = "publicKeyToken")]
        public string PublicKeyToken { get; set; }
        [XmlAttribute(AttributeName = "versionScope")]
        public string VersionScope { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "versionRange")]
        public string VersionRange { get; set; }
    }


    [XmlRoot(ElementName = "machineSpecific", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class MachineSpecific
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "rules")]
        public string Rules { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "")]
        public MigXml MigXml_1 { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MigXml2 MigXml_2 { get; set; }
    }

    [XmlRoot(ElementName = "pattern", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Pattern2
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "objectSet", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ObjectSet2
    {
        [XmlElement(ElementName = "pattern", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Pattern2> Pattern2 { get; set; }
    }

    [XmlRoot(ElementName = "merge", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Merge2
    {
        [XmlElement(ElementName = "objectSet", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ObjectSet2 ObjectSet2 { get; set; }
        [XmlAttribute(AttributeName = "script")]
        public string Script { get; set; }
    }

    [XmlRoot(ElementName = "rules", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Rules2
    {
        [XmlElement(ElementName = "merge", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Merge2 Merge2 { get; set; }
        [XmlElement(ElementName = "include", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Include2 Include2 { get; set; }
        [XmlAttribute(AttributeName = "context")]
        public string Context { get; set; }
    }

    [XmlRoot(ElementName = "migXml", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class MigXml2
    {
        [XmlElement(ElementName = "rules", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Rules2> Rules2 { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SupportedComponent
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlElement(ElementName = "supportedComponentIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SupportedComponentIdentity SupportedComponentIdentity { get; set; }
        [XmlElement(ElementName = "machineSpecific", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MachineSpecific MachineSpecific { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "")]
        public MigXml MigXml_1 { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MigXml2 MigXml_2 { get; set; }
        [XmlAttribute(AttributeName = "rules")]
        public string Rules { get; set; }
    }

    [XmlRoot(ElementName = "supportedComponents", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SupportedComponents
    {
        [XmlElement(ElementName = "supportedComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<SupportedComponent> SupportedComponent { get; set; }
    }

    [XmlRoot(ElementName = "include", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Include2
    {
        [XmlElement(ElementName = "objectSet", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ObjectSet2 ObjectSet2 { get; set; }
    }

    [XmlRoot(ElementName = "plugin", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Plugin2
    {
        [XmlAttribute(AttributeName = "classId")]
        public string ClassId { get; set; }
        [XmlAttribute(AttributeName = "file")]
        public string File { get; set; }
        [XmlAttribute(AttributeName = "offlineApply")]
        public string OfflineApply { get; set; }
    }

    [XmlRoot(ElementName = "uninstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Uninstall
    {
        [XmlElement(ElementName = "migXml", Namespace = "")]
        public MigXml MigXml { get; set; }
    }

    [XmlRoot(ElementName = "migration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Migration
    {
        [XmlElement(ElementName = "supportedComponents", Namespace = "")]
        public SupportedComponents2 SupportedComponents2 { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "")]
        public List<MigXml> MigXml_1 { get; set; }
        [XmlElement(ElementName = "migxml")]
        public Migxml Migxml_2 { get; set; }
        [XmlElement(ElementName = "migxml", Namespace = "")]
        public Migxml Migxml_3 { get; set; }
        [XmlElement(ElementName = "migXml", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MigXml2 MigXml_4 { get; set; }
        [XmlElement(ElementName = "supportedComponents", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SupportedComponents SupportedComponents { get; set; }
        [XmlElement(ElementName = "machineSpecific", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<MachineSpecific> MachineSpecific { get; set; }
        [XmlElement(ElementName = "migrationDisplayID", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string MigrationDisplayID { get; set; }
        [XmlElement(ElementName = "plugin", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Plugin2 Plugin2 { get; set; }
        [XmlElement(ElementName = "uninstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Uninstall Uninstall { get; set; }
        [XmlAttribute(AttributeName = "scope")]
        public string Scope { get; set; }
        [XmlAttribute(AttributeName = "settingsVersion")]
        public string SettingsVersion { get; set; }
        [XmlAttribute(AttributeName = "alwaysProcess")]
        public string AlwaysProcess { get; set; }
        [XmlAttribute(AttributeName = "replacementSettingsVersionRange")]
        public string ReplacementSettingsVersionRange { get; set; }
        [XmlAttribute(AttributeName = "replacementVersionRange")]
        public string ReplacementVersionRange { get; set; }
        [XmlAttribute(AttributeName = "offlineApply")]
        public string OfflineApply { get; set; }
        [XmlAttribute(AttributeName = "optimizePatterns")]
        public string OptimizePatterns { get; set; }
        [XmlAttribute(AttributeName = "ignoreConfigurationSection")]
        public string IgnoreConfigurationSection { get; set; }
        [XmlAttribute(AttributeName = "offlineGather")]
        public string OfflineGather { get; set; }
    }

    [XmlRoot(ElementName = "taskScheduler", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class TaskScheduler
    {
        [XmlElement(ElementName = "Task", Namespace = "http://schemas.microsoft.com/windows/2004/02/mit/task")]
        public List<TaskNS.Task> Task { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "mvid", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Mvid
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "configurationSchema", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ConfigurationSchema
    {
        [XmlElement(ElementName = "schema", Namespace = "http://www.w3.org/2001/XMLSchema")]
        public Schema Schema { get; set; }
    }

    [XmlRoot(ElementName = "AutoProxyListItem", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class AutoProxyListItem
    {
        [XmlElement(ElementName = "Name", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Default", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Default { get; set; }
        [XmlElement(ElementName = "DllFile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string DllFile { get; set; }
        [XmlElement(ElementName = "FileExtensions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string FileExtensions { get; set; }
        [XmlElement(ElementName = "Flags", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Flags { get; set; }
    }

    [XmlRoot(ElementName = "AutoProxy", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class AutoProxy
    {
        [XmlElement(ElementName = "AutoProxyListItem", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<AutoProxyListItem> AutoProxyListItem { get; set; }
    }

    [XmlRoot(ElementName = "BL", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class BL
    {
        [XmlAttribute(AttributeName = "keyValue", Namespace = "http://schemas.microsoft.com/WMIConfig/2002/State")]
        public string KeyValue { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "GroupOrderList", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class GroupOrderList
    {
        [XmlElement(ElementName = "BL", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<BL> BL { get; set; }
    }

    [XmlRoot(ElementName = "elements", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Elements
    {
        [XmlElement(ElementName = "AutoProxy", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AutoProxy AutoProxy { get; set; }
        [XmlElement(ElementName = "GroupOrderList", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public GroupOrderList GroupOrderList { get; set; }
    }

    [XmlRoot(ElementName = "metadata", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Metadata
    {
        [XmlElement(ElementName = "elements", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Elements Elements { get; set; }
    }

    [XmlRoot(ElementName = "configuration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Configuration
    {
        [XmlElement(ElementName = "configurationSchema", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ConfigurationSchema ConfigurationSchema { get; set; }
        [XmlElement(ElementName = "metadata", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Metadata Metadata { get; set; }
        [XmlAttribute(AttributeName = "asmv2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Asmv2 { get; set; }
        [XmlAttribute(AttributeName = "wcm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wcm { get; set; }
        [XmlAttribute(AttributeName = "asmv3", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Asmv3 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "app", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string App { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "buildfilter")]
        public string Buildfilter { get; set; }
        [XmlAttribute(AttributeName = "sqm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Sqm { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }

    [XmlRoot(ElementName = "deconstructionTool", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeconstructionTool
    {
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "mof", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Mof
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "uninstallmof")]
        public string Uninstallmof { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "counterAttribute", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CounterAttribute2
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "counterAttributes", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CounterAttributes2
    {
        [XmlElement(ElementName = "counterAttribute", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CounterAttribute2 CounterAttribute2 { get; set; }
    }

    [XmlRoot(ElementName = "counter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Counter2
    {
        [XmlElement(ElementName = "counterAttributes", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CounterAttributes2 CounterAttributes2 { get; set; }
        [XmlAttribute(AttributeName = "defaultScale")]
        public string DefaultScale { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "detailLevel")]
        public string DetailLevel { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
    }

    [XmlRoot(ElementName = "counterSet", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CounterSet2
    {
        [XmlElement(ElementName = "counter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Counter2> Counter2 { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "instances")]
        public string Instances { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "uri")]
        public string Uri { get; set; }
    }

    [XmlRoot(ElementName = "provider", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Provider3
    {
        [XmlElement(ElementName = "counterSet", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CounterSet2 CounterSet2 { get; set; }
        [XmlAttribute(AttributeName = "applicationIdentity")]
        public string ApplicationIdentity { get; set; }
        [XmlAttribute(AttributeName = "providerGuid")]
        public string ProviderGuid { get; set; }
        [XmlAttribute(AttributeName = "providerName")]
        public string ProviderName { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlElement(ElementName = "channels", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Channels2 Channels2 { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "messageFileName")]
        public string MessageFileName { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "resourceFileName")]
        public string ResourceFileName { get; set; }
    }

    [XmlRoot(ElementName = "counters", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Counters3
    {
        [XmlElement(ElementName = "provider", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Provider3 Provider3 { get; set; }
    }

    [XmlRoot(ElementName = "channel", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Channel2
    {
        [XmlAttribute(AttributeName = "chid")]
        public string Chid { get; set; }
        [XmlAttribute(AttributeName = "enabled")]
        public string Enabled { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "access")]
        public string Access { get; set; }
        [XmlAttribute(AttributeName = "isolation")]
        public string Isolation { get; set; }
    }

    [XmlRoot(ElementName = "channels", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Channels2
    {
        [XmlElement(ElementName = "channel", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Channel2> Channel2 { get; set; }
    }

    [XmlRoot(ElementName = "events", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Events2
    {
        [XmlElement(ElementName = "cmi", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Cmi Cmi { get; set; }
        [XmlElement(ElementName = "provider", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Provider3 Provider3 { get; set; }
    }

    [XmlRoot(ElementName = "instrumentation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Instrumentation
    {
        [XmlElement(ElementName = "events", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Events> Events { get; set; }
        [XmlElement(ElementName = "counters", Namespace = "http://schemas.microsoft.com/win/2005/12/counters")]
        public Counters Counters { get; set; }
        [XmlElement(ElementName = "counters", Namespace = "http://schemas.microsoft.com/win/2004/08/counters")]
        public Counters2 Counters2 { get; set; }
        [XmlElement(ElementName = "counters", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Counters3 Counters3 { get; set; }
        [XmlElement(ElementName = "templates", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Templates2 { get; set; }
        [XmlElement(ElementName = "events", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Events2 Events2 { get; set; }
        [XmlAttribute(AttributeName = "el", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string El { get; set; }
        [XmlAttribute(AttributeName = "win", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Win { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
        [XmlAttribute(AttributeName = "ms", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ms { get; set; }
        [XmlAttribute(AttributeName = "trace", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Trace { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
        [XmlAttribute(AttributeName = "ut", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ut { get; set; }
        [XmlAttribute(AttributeName = "Comms", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Comms { get; set; }
        [XmlAttribute(AttributeName = "wdc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wdc { get; set; }
        [XmlAttribute(AttributeName = "fi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Fi { get; set; }
        [XmlAttribute(AttributeName = "gpio", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Gpio { get; set; }
        [XmlAttribute(AttributeName = "wcse", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wcse { get; set; }
        [XmlAttribute(AttributeName = "dccwe", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dccwe { get; set; }
        [XmlAttribute(AttributeName = "wl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wl { get; set; }
        [XmlAttribute(AttributeName = "smd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Smd { get; set; }
        [XmlAttribute(AttributeName = "fve", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Fve { get; set; }
        [XmlAttribute(AttributeName = "task", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Task { get; set; }
        [XmlAttribute(AttributeName = "eth", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Eth { get; set; }
        [XmlAttribute(AttributeName = "sercx", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Sercx { get; set; }
        [XmlAttribute(AttributeName = "setupcl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Setupcl { get; set; }
        [XmlAttribute(AttributeName = "spb", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Spb { get; set; }
        [XmlAttribute(AttributeName = "csr", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Csr { get; set; }
        [XmlAttribute(AttributeName = "smss", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Smss { get; set; }
        [XmlAttribute(AttributeName = "vs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Vs { get; set; }
        [XmlAttribute(AttributeName = "kmdf", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Kmdf { get; set; }
        [XmlAttribute(AttributeName = "umdf", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Umdf { get; set; }
    }

    [XmlRoot(ElementName = "firewallRule", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FirewallRule
    {
        [XmlAttribute(AttributeName = "Action")]
        public string Action_1 { get; set; }
        [XmlAttribute(AttributeName = "action")]
        public string Action_2 { get; set; }
        [XmlAttribute(AttributeName = "Active")]
        public string Active_1 { get; set; }
        [XmlAttribute(AttributeName = "active")]
        public string Active_2 { get; set; }
        [XmlAttribute(AttributeName = "Binary")]
        public string Binary_1 { get; set; }
        [XmlAttribute(AttributeName = "binary")]
        public string Binary_2 { get; set; }
        [XmlAttribute(AttributeName = "Desc")]
        public string Desc_1 { get; set; }
        [XmlAttribute(AttributeName = "desc")]
        public string Desc_2 { get; set; }
        [XmlAttribute(AttributeName = "Dir")]
        public string Dir_1 { get; set; }
        [XmlAttribute(AttributeName = "dir")]
        public string Dir_2 { get; set; }
        [XmlAttribute(AttributeName = "Group")]
        public string Group_1 { get; set; }
        [XmlAttribute(AttributeName = "group")]
        public string Group_2 { get; set; }
        [XmlAttribute(AttributeName = "InternalName")]
        public string InternalName1 { get; set; }
        [XmlAttribute(AttributeName = "internalName")]
        public string InternalName2 { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name_1 { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name_2 { get; set; }
        [XmlAttribute(AttributeName = "Protocol")]
        public string Protocol1 { get; set; }
        [XmlAttribute(AttributeName = "protocol")]
        public string Protocol2 { get; set; }
        [XmlAttribute(AttributeName = "lPort")]
        public string LPort_1 { get; set; }

        [XmlAttribute(AttributeName = "LPort")]
        public string LPort_2 { get; set; }
        [XmlAttribute(AttributeName = "svc")]
        public string Svc_1 { get; set; }
        [XmlAttribute(AttributeName = "Svc")]
        public string Svc_2 { get; set; }

        [XmlAttribute(AttributeName = "lport")]
        public string Lport_3 { get; set; }
        [XmlAttribute(AttributeName = "Lport")]
        public string Lport_4 { get; set; }
        [XmlAttribute(AttributeName = "Edge")]
        public string Edge_1 { get; set; }
        [XmlAttribute(AttributeName = "edge")]
        public string Edge2 { get; set; }
        [XmlAttribute(AttributeName = "edgedefer")]
        public string Edgedefer_1 { get; set; }
        [XmlAttribute(AttributeName = "Rport")]
        public string Rport { get; set; }
        [XmlAttribute(AttributeName = "Profile")]
        public string Profile_1 { get; set; }
        [XmlAttribute(AttributeName = "profile")]
        public string Profile_2 { get; set; }
        [XmlAttribute(AttributeName = "RA4")]
        public string RA4_1 { get; set; }
        [XmlAttribute(AttributeName = "RA6")]
        public string RA6_1 { get; set; }
        [XmlAttribute(AttributeName = "RPort")]
        public string RPort_1 { get; set; }
        [XmlAttribute(AttributeName = "rport")]
        public string RPort_2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "icmp4")]
        public string Icmp4_1 { get; set; }
        [XmlAttribute(AttributeName = "icmp6")]
        public string Icmp6_1 { get; set; }
        [XmlAttribute(AttributeName = "TTK")]
        public string TTK_2 { get; set; }
        [XmlAttribute(AttributeName = "LUAuth")]
        public string LUAuth_2 { get; set; }
        [XmlAttribute(AttributeName = "internalname")]
        public string Internalname { get; set; }
        [XmlAttribute(AttributeName = "ra4")]
        public string Ra4_2 { get; set; }
        [XmlAttribute(AttributeName = "ra6")]
        public string Ra6_2 { get; set; }
        [XmlAttribute(AttributeName = "EdgeDefer")]
        public string EdgeDefer_2 { get; set; }
        [XmlAttribute(AttributeName = "ICMP6")]
        public string ICMP6_2 { get; set; }
        [XmlAttribute(AttributeName = "LA6")]
        public string LA6 { get; set; }
        [XmlAttribute(AttributeName = "ICMP4")]
        public string ICMP4_2 { get; set; }
        [XmlAttribute(AttributeName = "ttk")]
        public string Ttk_1 { get; set; }
        [XmlAttribute(AttributeName = "luauth")]
        public string Luauth_1 { get; set; }
    }

    [XmlRoot(ElementName = "sysprepOrder", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SysprepOrder
    {
        [XmlAttribute(AttributeName = "order")]
        public string Order { get; set; }
    }

    [XmlRoot(ElementName = "sysprepModule", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SysprepModule
    {
        [XmlAttribute(AttributeName = "methodName")]
        public string MethodName { get; set; }
        [XmlAttribute(AttributeName = "moduleName")]
        public string ModuleName { get; set; }
        [XmlAttribute(AttributeName = "moduleType")]
        public string ModuleType { get; set; }
    }

    [XmlRoot(ElementName = "sysprepValidate", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SysprepValidate
    {
        [XmlAttribute(AttributeName = "methodName")]
        public string MethodName { get; set; }
        [XmlAttribute(AttributeName = "moduleName")]
        public string ModuleName { get; set; }
    }

    [XmlRoot(ElementName = "setValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SetValue
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "deleteKey", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeleteKey
    {
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
    }

    [XmlRoot(ElementName = "deleteValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeleteValue
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
    }

    [XmlRoot(ElementName = "registryActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class RegistryActions
    {
        [XmlElement(ElementName = "setValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SetValue SetValue { get; set; }
        [XmlElement(ElementName = "deleteKey", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DeleteKey> DeleteKey { get; set; }
        [XmlElement(ElementName = "deleteValue", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DeleteValue> DeleteValue { get; set; }
    }

    [XmlRoot(ElementName = "deleteFile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeleteFile
    {
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
    }

    [XmlRoot(ElementName = "deleteDirectory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeleteDirectory
    {
        [XmlAttribute(AttributeName = "path")]
        public string Path { get; set; }
    }

    [XmlRoot(ElementName = "fileActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FileActions
    {
        [XmlElement(ElementName = "deleteFile", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DeleteFile> DeleteFile { get; set; }
        [XmlElement(ElementName = "deleteDirectory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public DeleteDirectory DeleteDirectory { get; set; }
    }

    [XmlRoot(ElementName = "sysprepProvider", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SysprepProvider
    {
        [XmlElement(ElementName = "sysprepOrder", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SysprepOrder SysprepOrder { get; set; }
        [XmlElement(ElementName = "sysprepModule", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<SysprepModule> SysprepModule { get; set; }
        [XmlElement(ElementName = "sysprepValidate", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SysprepValidate SysprepValidate { get; set; }
        [XmlElement(ElementName = "registryActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public RegistryActions RegistryActions { get; set; }
        [XmlElement(ElementName = "fileActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public FileActions FileActions { get; set; }
        [XmlAttribute(AttributeName = "stage")]
        public string Stage { get; set; }
    }

    [XmlRoot(ElementName = "sysprepInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SysprepInformation
    {
        [XmlElement(ElementName = "sysprepProvider", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<SysprepProvider> SysprepProvider { get; set; }
    }

    [XmlRoot(ElementName = "imaging", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Imaging
    {
        [XmlElement(ElementName = "sysprepInformation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SysprepInformation SysprepInformation { get; set; }
    }

    [XmlRoot(ElementName = "feature", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Feature
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Bind
    {
        [XmlAttribute(AttributeName = "keyword")]
        public string Keyword { get; set; }
        [XmlAttribute(AttributeName = "ruleType")]
        public string RuleType { get; set; }
    }

    [XmlRoot(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Property
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "netAdapterDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class NetAdapterDriver
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "ifDescrSeed")]
        public string IfDescrSeed { get; set; }
        [XmlAttribute(AttributeName = "ifType")]
        public string IfType { get; set; }
        [XmlAttribute(AttributeName = "isVirtual")]
        public string IsVirtual { get; set; }
        [XmlAttribute(AttributeName = "mediaType")]
        public string MediaType { get; set; }
        [XmlAttribute(AttributeName = "physicalMediaType")]
        public string PhysicalMediaType { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
    }

    [XmlRoot(ElementName = "protocolDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ProtocolDriver
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "bindName")]
        public string BindName { get; set; }
        [XmlAttribute(AttributeName = "defaultDisable")]
        public string DefaultDisable { get; set; }
        [XmlAttribute(AttributeName = "displayDescription")]
        public string DisplayDescription { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
        [XmlAttribute(AttributeName = "noStartAtBoot")]
        public string NoStartAtBoot { get; set; }
        [XmlAttribute(AttributeName = "nonUserRemovable")]
        public string NonUserRemovable { get; set; }
    }

    [XmlRoot(ElementName = "filterDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FilterDriver
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "bindGuid")]
        public string BindGuid { get; set; }
        [XmlAttribute(AttributeName = "displayDescription")]
        public string DisplayDescription { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "filterClass")]
        public string FilterClass { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
        [XmlAttribute(AttributeName = "mandatory")]
        public string Mandatory { get; set; }
        [XmlAttribute(AttributeName = "noStartAtBoot")]
        public string NoStartAtBoot { get; set; }
        [XmlAttribute(AttributeName = "nonUserRemovable")]
        public string NonUserRemovable { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "unbindOnAttach")]
        public string UnbindOnAttach { get; set; }
        [XmlAttribute(AttributeName = "unbindOnDetach")]
        public string UnbindOnDetach { get; set; }
    }

    [XmlRoot(ElementName = "serviceDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceDriver
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "bindName")]
        public string BindName { get; set; }
        [XmlAttribute(AttributeName = "displayDescription")]
        public string DisplayDescription { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
        [XmlAttribute(AttributeName = "nonUserRemovable")]
        public string NonUserRemovable { get; set; }
    }

    [XmlRoot(ElementName = "netAdapter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class NetAdapter
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "hidden")]
        public string Hidden { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "ifAlias")]
        public string IfAlias { get; set; }
        [XmlAttribute(AttributeName = "ifDescr")]
        public string IfDescr { get; set; }
        [XmlAttribute(AttributeName = "ifLuid")]
        public string IfLuid { get; set; }
        [XmlAttribute(AttributeName = "ifType")]
        public string IfType { get; set; }
        [XmlAttribute(AttributeName = "isVirtual")]
        public string IsVirtual { get; set; }
        [XmlAttribute(AttributeName = "mediaType")]
        public string MediaType { get; set; }
        [XmlAttribute(AttributeName = "physicalMediaType")]
        public string PhysicalMediaType { get; set; }
    }

    [XmlRoot(ElementName = "clientDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ClientDriver
    {
        [XmlElement(ElementName = "bind", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Bind> Bind { get; set; }
        [XmlElement(ElementName = "property", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Property> Property { get; set; }
        [XmlAttribute(AttributeName = "bindName")]
        public string BindName { get; set; }
        [XmlAttribute(AttributeName = "displayDescription")]
        public string DisplayDescription { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "identifier")]
        public string Identifier { get; set; }
    }

    [XmlRoot(ElementName = "networkComponents", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class NetworkComponents
    {
        [XmlElement(ElementName = "netAdapterDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public NetAdapterDriver NetAdapterDriver { get; set; }
        [XmlElement(ElementName = "protocolDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ProtocolDriver> ProtocolDriver { get; set; }
        [XmlElement(ElementName = "filterDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<FilterDriver> FilterDriver { get; set; }
        [XmlElement(ElementName = "serviceDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ServiceDriver ServiceDriver { get; set; }
        [XmlElement(ElementName = "netAdapter", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<NetAdapter> NetAdapter { get; set; }
        [XmlElement(ElementName = "clientDriver", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ClientDriver ClientDriver { get; set; }
    }

    [XmlRoot(ElementName = "mappingRow", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class MappingRow
    {
        [XmlAttribute(AttributeName = "addressFamily")]
        public string AddressFamily { get; set; }
        [XmlAttribute(AttributeName = "protocol")]
        public string Protocol { get; set; }
        [XmlAttribute(AttributeName = "socketType")]
        public string SocketType { get; set; }
    }

    [XmlRoot(ElementName = "providerFlag", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ProviderFlag
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "serviceFlag", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceFlag
    {
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "protocol", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Protocol
    {
        [XmlElement(ElementName = "providerFlag", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ProviderFlag> ProviderFlag { get; set; }
        [XmlElement(ElementName = "serviceFlag", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ServiceFlag> ServiceFlag { get; set; }
        [XmlAttribute(AttributeName = "addressFamily")]
        public string AddressFamily { get; set; }
        [XmlAttribute(AttributeName = "byteOrder")]
        public string ByteOrder { get; set; }
        [XmlAttribute(AttributeName = "maxSockAddrLength")]
        public string MaxSockAddrLength { get; set; }
        [XmlAttribute(AttributeName = "messageSize")]
        public string MessageSize { get; set; }
        [XmlAttribute(AttributeName = "minSockAddrLength")]
        public string MinSockAddrLength { get; set; }
        [XmlAttribute(AttributeName = "protocol")]
        public string _protocol { get; set; }
        [XmlAttribute(AttributeName = "protocolMaxOffset")]
        public string ProtocolMaxOffset { get; set; }
        [XmlAttribute(AttributeName = "protocolName")]
        public string ProtocolName { get; set; }
        [XmlAttribute(AttributeName = "socketType")]
        public string SocketType { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "WinsockTransportOnlineInstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class WinsockTransportOnlineInstall
    {
        [XmlElement(ElementName = "mappingRow", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<MappingRow> MappingRow { get; set; }
        [XmlElement(ElementName = "protocol", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Protocol> Protocol { get; set; }
        [XmlAttribute(AttributeName = "HelperDllName")]
        public string HelperDllName { get; set; }
        [XmlAttribute(AttributeName = "MaxSockAddrLength")]
        public string MaxSockAddrLength { get; set; }
        [XmlAttribute(AttributeName = "MinSockAddrLength")]
        public string MinSockAddrLength { get; set; }
        [XmlAttribute(AttributeName = "ProviderGuid")]
        public string ProviderGuid { get; set; }
        [XmlAttribute(AttributeName = "TransportService")]
        public string TransportService { get; set; }
    }

    [XmlRoot(ElementName = "windowsSettings", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class WindowsSettings2
    {
        [XmlElement(ElementName = "dpiAware", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
        public DpiAware DpiAware { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "application", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Application
    {
        [XmlElement(ElementName = "windowsSettings", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
        public WindowsSettings WindowsSettings { get; set; }
        [XmlElement(ElementName = "windowsSettings", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public WindowsSettings2 WindowsSettings2 { get; set; }
        [XmlAttribute(AttributeName = "asmv3", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Asmv3 { get; set; }
    }

    [XmlRoot(ElementName = "genericCommand", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class GenericCommand
    {
        [XmlAttribute(AttributeName = "arguments")]
        public string Arguments { get; set; }
        [XmlAttribute(AttributeName = "executableName")]
        public string ExecutableName { get; set; }
        [XmlAttribute(AttributeName = "install")]
        public string Install { get; set; }
        [XmlAttribute(AttributeName = "passes")]
        public string Passes { get; set; }
        [XmlAttribute(AttributeName = "successCode")]
        public string SuccessCode { get; set; }
    }

    [XmlRoot(ElementName = "genericCommands", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class GenericCommands
    {
        [XmlElement(ElementName = "genericCommand", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<GenericCommand> GenericCommand { get; set; }
    }

    [XmlRoot(ElementName = "categoryDefinition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CategoryDefinition
    {
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
    }

    [XmlRoot(ElementName = "categoryDefinitions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CategoryDefinitions
    {
        [XmlElement(ElementName = "categoryDefinition", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CategoryDefinition CategoryDefinition { get; set; }
    }

    [XmlRoot(ElementName = "bfsvc", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Bfsvc
    {
        [XmlAttribute(AttributeName = "Flags")]
        public string Flags { get; set; }
        [XmlAttribute(AttributeName = "Source")]
        public string Source { get; set; }
    }

    [XmlRoot(ElementName = "fveUpdateAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FveUpdateAI
    {
        [XmlAttribute(AttributeName = "fveCommand")]
        public string FveCommand { get; set; }
    }

    [XmlRoot(ElementName = "WinsockNameSpaceOnlineInstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class WinsockNameSpaceOnlineInstall
    {
        [XmlAttribute(AttributeName = "DisplayString")]
        public string DisplayString { get; set; }
        [XmlAttribute(AttributeName = "LibraryPath")]
        public string LibraryPath { get; set; }
        [XmlAttribute(AttributeName = "ProviderId")]
        public string ProviderId { get; set; }
        [XmlAttribute(AttributeName = "SupportedNameSpace")]
        public string SupportedNameSpace { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "languageCategory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class LanguageCategory
    {
        [XmlElement(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Id Id { get; set; }
        [XmlAttribute(AttributeName = "discoverable")]
        public string Discoverable { get; set; }
    }

    [XmlRoot(ElementName = "satelliteCategory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SatelliteCategory
    {
        [XmlElement(ElementName = "id", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Id Id { get; set; }
        [XmlAttribute(AttributeName = "discoverable")]
        public string Discoverable { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "Param", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Param
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "TransformInvoke", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class TransformInvoke
    {
        [XmlElement(ElementName = "Param", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Param> Param { get; set; }
        [XmlAttribute(AttributeName = "Plugin")]
        public string Plugin { get; set; }
        [XmlAttribute(AttributeName = "FromSettingsVersion")]
        public string FromSettingsVersion { get; set; }
    }

    [XmlRoot(ElementName = "Transforms", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Transforms2
    {
        [XmlElement(ElementName = "TransformInvoke", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<TransformInvoke> TransformInvoke { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "SettingsVersion")]
        public string SettingsVersion { get; set; }
    }

    [XmlRoot(ElementName = "unattendAction", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class UnattendAction
    {
        [XmlAttribute(AttributeName = "executableName")]
        public string ExecutableName { get; set; }
        [XmlAttribute(AttributeName = "passes")]
        public string Passes { get; set; }
        [XmlAttribute(AttributeName = "arguments")]
        public string Arguments { get; set; }
    }

    [XmlRoot(ElementName = "unattendActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class UnattendActions
    {
        [XmlElement(ElementName = "unattendAction", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public UnattendAction UnattendAction { get; set; }
    }

    [XmlRoot(ElementName = "AddUrl", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class AddUrl
    {
        [XmlAttribute(AttributeName = "sddl")]
        public string Sddl { get; set; }
        [XmlAttribute(AttributeName = "url")]
        public string Url { get; set; }
    }

    [XmlRoot(ElementName = "HTTPAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class HTTPAI
    {
        [XmlElement(ElementName = "AddUrl", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<AddUrl> AddUrl { get; set; }
    }

    [XmlRoot(ElementName = "EdgeAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class EdgeAI
    {
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "cleanupCache", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CleanupCache
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "CacheType")]
        public string CacheType { get; set; }
        [XmlAttribute(AttributeName = "Location")]
        public string Location { get; set; }
        [XmlAttribute(AttributeName = "LocationType")]
        public string LocationType { get; set; }
    }

    [XmlRoot(ElementName = "WinsockAppPermittedLspCategories", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class WinsockAppPermittedLspCategories
    {
        [XmlAttribute(AttributeName = "Path")]
        public string Path { get; set; }
        [XmlAttribute(AttributeName = "PermittedLspCategories")]
        public string PermittedLspCategories { get; set; }
    }

    [XmlRoot(ElementName = "installer", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Installer
    {
        [XmlAttribute(AttributeName = "passes")]
        public string Passes { get; set; }
        [XmlAttribute(AttributeName = "xpath")]
        public string Xpath { get; set; }
    }

    [XmlRoot(ElementName = "installers", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Installers
    {
        [XmlElement(ElementName = "installer", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Installer> Installer { get; set; }
    }

    [XmlRoot(ElementName = "sequence", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Sequence3
    {
        [XmlAttribute(AttributeName = "after")]
        public string After { get; set; }
        [XmlAttribute(AttributeName = "pass")]
        public string Pass { get; set; }
        [XmlAttribute(AttributeName = "before")]
        public string Before { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "sequences", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Sequences
    {
        [XmlElement(ElementName = "sequence", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Sequence3> Sequence3 { get; set; }
    }

    [XmlRoot(ElementName = "installerRegistration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class InstallerRegistration
    {
        [XmlElement(ElementName = "installers", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Installers Installers { get; set; }
        [XmlElement(ElementName = "sequences", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Sequences Sequences { get; set; }
        [XmlAttribute(AttributeName = "factoryClsid")]
        public string FactoryClsid { get; set; }
    }

    [XmlRoot(ElementName = "installerRegistrations", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class InstallerRegistrations
    {
        [XmlElement(ElementName = "installerRegistration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public InstallerRegistration InstallerRegistration { get; set; }
    }

    [XmlRoot(ElementName = "SecureBoot", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SecureBoot
    {
        [XmlAttribute(AttributeName = "UpdateType")]
        public string UpdateType { get; set; }
    }

    [XmlRoot(ElementName = "FeatureSettingsOverride", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class FeatureSettingsOverride
    {
        [XmlAttribute(AttributeName = "Key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "Priority")]
        public string Priority { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "ValueName")]
        public string ValueName { get; set; }
        [XmlAttribute(AttributeName = "ValueType")]
        public string ValueType { get; set; }
    }

    [XmlRoot(ElementName = "netFxState", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class NetFxState
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "configureNetFx", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ConfigureNetFx
    {
        [XmlElement(ElementName = "netFxState", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public NetFxState NetFxState { get; set; }
    }

    [XmlRoot(ElementName = "serviceModelReg", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServiceModelReg
    {
        [XmlAttribute(AttributeName = "component")]
        public string Component { get; set; }
        [XmlAttribute(AttributeName = "frameworkVersion")]
        public string FrameworkVersion { get; set; }
    }

    [XmlRoot(ElementName = "Version", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Version
    {
        [XmlAttribute(AttributeName = "Major")]
        public string Major { get; set; }
        [XmlAttribute(AttributeName = "Minor")]
        public string Minor { get; set; }
    }


    [XmlRoot(ElementName = "Deploys", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Deploys
    {
        [XmlElement(ElementName = "Update", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<DeploysUpdate> Update { get; set; }
    }

    [XmlRoot(ElementName = "OptionalCompanionFor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class OptionalCompanionFor
    {
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "UniqueName")]
        public string UniqueName { get; set; }
    }

    [XmlRoot(ElementName = "Relationships", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Relationships
    {
        [XmlElement(ElementName = "OptionalCompanionFor", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public OptionalCompanionFor OptionalCompanionFor { get; set; }
    }

    [XmlRoot(ElementName = "NonAncestorDependencies", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class NonAncestorDependencies
    {
        [XmlElement(ElementName = "ServerComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<ServerComponent> ServerComponent { get; set; }
    }

    [XmlRoot(ElementName = "ServerComponent", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class ServerComponent
    {
        [XmlElement(ElementName = "MutualExclusion", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public MutualExclusion MutualExclusion { get; set; }
        [XmlElement(ElementName = "Version", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Version Version { get; set; }
        [XmlElement(ElementName = "Deploys", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Deploys Deploys { get; set; }
        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "DisplayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "UniqueName")]
        public string UniqueName { get; set; }
        [XmlAttribute(AttributeName = "InstallWithParentByDefault")]
        public string InstallWithParentByDefault { get; set; }
        [XmlAttribute(AttributeName = "Parent")]
        public string Parent { get; set; }
        [XmlElement(ElementName = "NonAncestorDependencies", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public NonAncestorDependencies NonAncestorDependencies { get; set; }
        [XmlElement(ElementName = "Relationships", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Relationships Relationships { get; set; }
        [XmlElement(ElementName = "SystemServices", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SystemServices SystemServices { get; set; }
    }

    [XmlRoot(ElementName = "capabilityIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class CapabilityIdentity
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "language")]
        public string Language { get; set; }
    }

    [XmlRoot(ElementName = "capability", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Capability
    {
        [XmlElement(ElementName = "capabilityIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CapabilityIdentity CapabilityIdentity { get; set; }
    }


    [XmlRoot(ElementName = "declareCapability", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class DeclareCapability
    {
        [XmlElement(ElementName = "capability", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Capability Capability { get; set; }
        [XmlElement(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Dependency Dependency { get; set; }
        [XmlElement(ElementName = "satelliteInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SatelliteInfo SatelliteInfo { get; set; }
    }

    [XmlRoot(ElementName = "SettingsPageOptions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class SettingsPageOptions
    {
        [XmlAttribute(AttributeName = "Visibility")]
        public string Visibility { get; set; }
        [XmlAttribute(AttributeName = "FeatureType")]
        public string FeatureType { get; set; }
        [XmlAttribute(AttributeName = "ManageFeatureSettings")]
        public string ManageFeatureSettings { get; set; }
    }


    [XmlRoot(ElementName = "OptionalFeatures", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class OptionalFeatures
    {
        [XmlElement(ElementName = "SettingsPageOptions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SettingsPageOptions SettingsPageOptions { get; set; }
        [XmlAttribute(AttributeName = "SchemaVersion")]
        public string SchemaVersion { get; set; }
    }


    [XmlRoot(ElementName = "assembly", Namespace = "urn:schemas-microsoft-com:asm.v3")]
    public class Assembly
    {
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public AssemblyIdentity AssemblyIdentity { get; set; }
        [XmlAttribute(AttributeName = "supportInformation")]
        public string SupportInformation { get; set; }
        [XmlElement(ElementName = "package", Namespace="urn:schemas-microsoft-com:asm.v3")]
		public Package Package { get; set; }
        [XmlElement(ElementName = "deployment", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Deployment Deployment { get; set; }
        [XmlElement(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Dependency> Dependency { get; set; }
        [XmlElement(ElementName = "file", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<File> File { get; set; }
        [XmlElement(ElementName = "trustInfo", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public TrustInfo TrustInfo { get; set; }
        [XmlElement(ElementName = "localization", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Localization Localization { get; set; }
        [XmlElement(ElementName = "runtime", Namespace = "urn:schemas-microsoft-com:asm.v2")]
        public Runtime Runtime { get; set; }
        [XmlElement(ElementName = "rescache", Namespace = "urn:schemas-microsoft-com:rescache.v1")]
        public Rescache Rescache { get; set; }
        [XmlElement(ElementName = "registryKeys", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public RegistryKeys RegistryKeys { get; set; }
        [XmlElement(ElementName = "directories", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Directories> Directories { get; set; }
        [XmlElement(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Memberships> Memberships { get; set; }
        [XmlElement(ElementName = "noInheritable", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public NoInheritable NoInheritable { get; set; }
        [XmlElement(ElementName = "assemblyIdentity", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public AsmV1.AssemblyIdentity AssemblyIdentity2 { get; set; }
        [XmlElement(ElementName = "migration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Migration Migration { get; set; }
        [XmlElement(ElementName = "taskScheduler", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<TaskScheduler> TaskScheduler { get; set; }
        [XmlElement(ElementName = "mvid", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Mvid Mvid { get; set; }
        [XmlElement(ElementName = "languagePack", Namespace = "urn:schemas-microsoft-com:asm.internal.v1")]
        public LanguagePack LanguagePack { get; set; }
        [XmlElement(ElementName = "configuration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Configuration Configuration { get; set; }
        [XmlElement(ElementName = "deconstructionTool", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public DeconstructionTool DeconstructionTool { get; set; }
        [XmlElement(ElementName = "mof", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<Mof> Mof { get; set; }
        [XmlElement(ElementName = "instrumentation", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Instrumentation Instrumentation { get; set; }
        [XmlElement(ElementName = "firewallRule", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<FirewallRule> FirewallRule { get; set; }
        [XmlElement(ElementName = "imaging", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Imaging Imaging { get; set; }
        [XmlElement(ElementName = "feature", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Feature Feature { get; set; }
        [XmlElement(ElementName = "networkComponents", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<NetworkComponents> NetworkComponents { get; set; }
        [XmlElement(ElementName = "WinsockTransportOnlineInstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<WinsockTransportOnlineInstall> WinsockTransportOnlineInstall { get; set; }
        [XmlElement(ElementName = "appxRegistration", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string AppxRegistration { get; set; }
        [XmlElement(ElementName = "application", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Application Application { get; set; }
        [XmlElement(ElementName = "genericCommands", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public GenericCommands GenericCommands { get; set; }
        [XmlElement(ElementName = "categoryDefinitions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CategoryDefinitions CategoryDefinitions { get; set; }
        [XmlElement(ElementName = "bfsvc", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Bfsvc Bfsvc { get; set; }
        [XmlElement(ElementName = "fveUpdateAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public FveUpdateAI FveUpdateAI { get; set; }
        [XmlElement(ElementName = "WinsockNameSpaceOnlineInstall", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public List<WinsockNameSpaceOnlineInstall> WinsockNameSpaceOnlineInstall { get; set; }
        [XmlElement(ElementName = "languageCategory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public LanguageCategory LanguageCategory { get; set; }
        [XmlElement(ElementName = "satelliteCategory", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SatelliteCategory SatelliteCategory { get; set; }
        [XmlElement(ElementName = "Transforms", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public Transforms2 Transforms2 { get; set; }
        [XmlElement(ElementName = "msdtc", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string Msdtc { get; set; }
        [XmlElement(ElementName = "containsSettings", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string ContainsSettings { get; set; }
        [XmlElement(ElementName = "unattendActions", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public UnattendActions UnattendActions { get; set; }
        [XmlElement(ElementName = "HTTPAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public HTTPAI HTTPAI { get; set; }
        [XmlElement(ElementName = "EdgeAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public EdgeAI EdgeAI { get; set; }
        [XmlElement(ElementName = "SetupWfsAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SetupWfsAI { get; set; }
        [XmlElement(ElementName = "cleanupCache", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public CleanupCache CleanupCache { get; set; }
        [XmlElement(ElementName = "SetIEInstalledDateAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SetIEInstalledDateAI { get; set; }
        [XmlElement(ElementName = "ConfigureIEOptionalComponentsAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string ConfigureIEOptionalComponentsAI { get; set; }
        [XmlElement(ElementName = "timezoneAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string TimezoneAI { get; set; }
        [XmlElement(ElementName = "timezoneresourceAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string TimezoneresourceAI { get; set; }
        [XmlElement(ElementName = "compatibility", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
        public Compatibility Compatibility { get; set; }
        [XmlElement(ElementName = "WinsockAppPermittedLspCategories", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public WinsockAppPermittedLspCategories WinsockAppPermittedLspCategories { get; set; }
        [XmlElement(ElementName = "installerRegistrations", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public InstallerRegistrations InstallerRegistrations { get; set; }
        [XmlElement(ElementName = "MsmqWorkgroupOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
        public MsmqWorkgroupOnlineInstall MsmqWorkgroupOnlineInstall { get; set; }
        [XmlElement(ElementName = "MsmqAdIntegrationOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
        public MsmqAdIntegrationOnlineInstall MsmqAdIntegrationOnlineInstall { get; set; }
        [XmlElement(ElementName = "MsmqHttpOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
        public MsmqHttpOnlineInstall MsmqHttpOnlineInstall { get; set; }
        [XmlElement(ElementName = "SetupMxdwAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SetupMxdwAI { get; set; }
        [XmlElement(ElementName = "SetupLprAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SetupLprAI { get; set; }
        [XmlElement(ElementName = "SetupMpdwAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string SetupMpdwAI { get; set; }
        [XmlElement(ElementName = "sppInstaller", Namespace = "urn:schemas-microsoft-com:spp:installer")]
        public SppInstaller SppInstaller { get; set; }
        [XmlElement(ElementName = "SecureBoot", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public SecureBoot SecureBoot { get; set; }
        [XmlElement(ElementName = "EVDAI", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string EVDAI { get; set; }
        [XmlElement(ElementName = "noInheritable", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public string NoInheritable2 { get; set; }
        [XmlElement(ElementName = "file", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<AsmV1.File> File2 { get; set; }
        [XmlElement(ElementName = "dependency", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public List<AsmV1.Dependency> Dependency2 { get; set; }
        [XmlElement(ElementName = "memberships", Namespace = "urn:schemas-microsoft-com:asm.v1")]
        public AsmV1.Memberships Memberships2 { get; set; }
        [XmlElement(ElementName = "FeatureSettingsOverride", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public FeatureSettingsOverride FeatureSettingsOverride { get; set; }
        [XmlElement(ElementName = "configureNetFx", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ConfigureNetFx ConfigureNetFx { get; set; }
        [XmlElement(ElementName = "localization", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Localization2 Localization2 { get; set; }
        [XmlElement(ElementName = "serviceModelReg", Namespace = "urn:schemas-microsoft-com:asm.v3")]
        public ServiceModelReg ServiceModelReg { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "manifestVersion")]
        public string ManifestVersion { get; set; }
        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName = "displayName")]
        public string DisplayName { get; set; }
        [XmlAttribute(AttributeName = "copyright")]
        public string Copyright { get; set; }
        [XmlAttribute(AttributeName = "isolated")]
        public string Isolated { get; set; }
        [XmlAttribute(AttributeName = "company")]
        public string Company { get; set; }
        [XmlAttribute(AttributeName = "creationTimeStamp")]
        public string CreationTimeStamp { get; set; }
        [XmlAttribute(AttributeName = "lastUpdateTimeStamp")]
        public string LastUpdateTimeStamp { get; set; }
        [XmlAttribute(AttributeName = "buildFilter")]
        public string BuildFilter { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "win", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Win { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "p3", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P3 { get; set; }
        [XmlAttribute(AttributeName = "wcs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Wcs { get; set; }
        [XmlAttribute(AttributeName = "e", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string E { get; set; }
        [XmlAttribute(AttributeName = "p2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P2 { get; set; }
        [XmlAttribute(AttributeName = "trace", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Trace { get; set; }
        [XmlAttribute(AttributeName = "p4", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P4 { get; set; }
        [XmlAttribute(AttributeName = "test", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Test { get; set; }
        [XmlAttribute(AttributeName = "prj", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Prj { get; set; }
        [XmlAttribute(AttributeName = "Comms", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Comms { get; set; }
        [XmlAttribute(AttributeName = "cmiv2", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Cmiv2 { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
    }
}
