#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.Winter.SchemaDefinitions.CEPSvcNS;
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.EventsNS
{
    [XmlRoot(ElementName = "importChannel", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class ImportChannel
    {
        [XmlAttribute(AttributeName = "chid")]
        public string Chid { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlText]
        public string Text { get; set; }
    }


    [XmlRoot(ElementName = "logging", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Logging
    {
        [XmlElement(ElementName = "maxSize", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string MaxSize { get; set; }
        [XmlElement(ElementName = "retention", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string Retention { get; set; }
        [XmlElement(ElementName = "autoBackup", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string AutoBackup { get; set; }
    }

    [XmlRoot(ElementName = "publishing", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Publishing
    {
        [XmlElement(ElementName = "bufferSize", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string BufferSize { get; set; }
        [XmlElement(ElementName = "level", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string Level { get; set; }
        [XmlElement(ElementName = "keywords", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string Keywords { get; set; }
        [XmlElement(ElementName = "controlGuid", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string ControlGuid { get; set; }
        [XmlElement(ElementName = "clockType", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string ClockType { get; set; }
        [XmlElement(ElementName = "maxBuffers", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string MaxBuffers { get; set; }
    }

    [XmlRoot(ElementName = "publishing", Namespace = "http://manifests.microsoft.com/win/2004/08/windows/events")]
    public class Publishing2
    {
        [XmlElement(ElementName = "bufferSize", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string BufferSize { get; set; }
    }

    [XmlRoot(ElementName = "channel", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Channel
    {
        [XmlElement(ElementName = "logging", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Logging Logging { get; set; }
        [XmlElement(ElementName = "publishing", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Publishing Publishing { get; set; }
        [XmlElement(ElementName = "publishing", Namespace = "http://manifests.microsoft.com/win/2004/08/windows/events")]
        public Publishing2 Publishing2 { get; set; }
        [XmlAttribute(AttributeName = "chid")]
        public string Chid { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "access")]
        public string Access { get; set; }
        [XmlAttribute(AttributeName = "enabled")]
        public string Enabled { get; set; }
        [XmlAttribute(AttributeName = "isolation")]
        public string Isolation { get; set; }
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "channels", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Channels
    {
        [XmlElement(ElementName = "importChannel", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<ImportChannel> ImportChannel { get; set; }
        [XmlElement(ElementName = "channel", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Channel> Channel { get; set; }
    }

    [XmlRoot(ElementName = "event", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Event
    {
        [XmlAttribute(AttributeName = "channel")]
        public string Channel { get; set; }
        [XmlAttribute(AttributeName = "keywords")]
        public string Keywords { get; set; }
        [XmlAttribute(AttributeName = "level")]
        public string Level { get; set; }
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "opcode")]
        public string Opcode { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "task")]
        public string Task { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "template")]
        public string Template { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }

    [XmlRoot(ElementName = "events", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Events
    {
        [XmlElement(ElementName = "messageTable", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<MessageTable> MessageTable { get; set; }
        [XmlElement(ElementName = "event", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Event> Event { get; set; }
        [XmlElement(ElementName = "provider", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Provider> Provider { get; set; }
        [XmlElement(ElementName = "cmi", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string Cmi { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "win", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Win { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
        [XmlAttribute(AttributeName = "p5", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string P5 { get; set; }
    }

    [XmlRoot(ElementName = "task", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Task
    {
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "eventGUID")]
        public string EventGUID { get; set; }
    }

    [XmlRoot(ElementName = "tasks", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Tasks
    {
        [XmlElement(ElementName = "task", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Task> Task { get; set; }
    }

    [XmlRoot(ElementName = "data", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Data
    {
        [XmlAttribute(AttributeName = "inType")]
        public string InType { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "outType")]
        public string OutType { get; set; }
        [XmlAttribute(AttributeName = "map")]
        public string Map { get; set; }
    }

    [XmlRoot(ElementName = "template", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Template
    {
        [XmlElement(ElementName = "data", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Data> Data { get; set; }
        [XmlAttribute(AttributeName = "tid")]
        public string Tid { get; set; }
        [XmlElement(ElementName = "UserData", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public UserData UserData { get; set; }
    }

    [XmlRoot(ElementName = "UserData", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class UserData
    {
        [XmlElement(ElementName = "TemplateData", Namespace = "CEPSvcNS")]
        public TemplateData TemplateData { get; set; }
        [XmlElement(ElementName = "CAData", Namespace = "CEPSvcNS")]
        public CAData CAData { get; set; }
        [XmlElement(ElementName = "OIDCollection", Namespace = "CEPSvcNS")]
        public OIDCollection OIDCollection { get; set; }
    }

    [XmlRoot(ElementName = "templates", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Templates
    {
        [XmlElement(ElementName = "template", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Template> Template { get; set; }
    }

    [XmlRoot(ElementName = "opcode", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Opcode
    {
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "opcodes", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Opcodes
    {
        [XmlElement(ElementName = "opcode", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Opcode> Opcode { get; set; }
    }

    [XmlRoot(ElementName = "keyword", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Keyword
    {
        [XmlAttribute(AttributeName = "mask")]
        public string Mask { get; set; }
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
    }

    [XmlRoot(ElementName = "keywords", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Keywords
    {
        [XmlElement(ElementName = "keyword", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Keyword> Keyword { get; set; }
    }

    [XmlRoot(ElementName = "map", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Map
    {
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "valueMap", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class ValueMap
    {
        [XmlElement(ElementName = "map", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Map> Map { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "bitMap", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class BitMap
    {
        [XmlElement(ElementName = "map", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Map> Map { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "maps", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Maps
    {
        [XmlElement(ElementName = "valueMap", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<ValueMap> ValueMap { get; set; }
        [XmlElement(ElementName = "bitMap", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public BitMap BitMap { get; set; }
    }

    [XmlRoot(ElementName = "provider", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Provider
    {
        [XmlElement(ElementName = "channels", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Channels Channels { get; set; }
        [XmlElement(ElementName = "events", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Events Events { get; set; }
        [XmlElement(ElementName = "tasks", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Tasks Tasks { get; set; }
        [XmlElement(ElementName = "levels", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public string Levels { get; set; }
        [XmlElement(ElementName = "templates", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Templates Templates { get; set; }
        [XmlElement(ElementName = "opcodes", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Opcodes Opcodes { get; set; }
        [XmlElement(ElementName = "keywords", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Keywords Keywords { get; set; }
        [XmlElement(ElementName = "maps", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Maps Maps { get; set; }
        [XmlAttribute(AttributeName = "guid")]
        public string Guid { get; set; }
        [XmlAttribute(AttributeName = "message")]
        public string Message { get; set; }
        [XmlAttribute(AttributeName = "messageFileName")]
        public string MessageFileName { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "parameterFileName")]
        public string ParameterFileName { get; set; }
        [XmlAttribute(AttributeName = "resourceFileName")]
        public string ResourceFileName { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "warnOnApplicationCompatibilityError")]
        public string WarnOnApplicationCompatibilityError { get; set; }
        [XmlAttribute(AttributeName = "ppm", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ppm { get; set; }
        [XmlAttribute(AttributeName = "potrg", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Potrg { get; set; }
        [XmlAttribute(AttributeName = "thrmpoll", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Thrmpoll { get; set; }
        [XmlAttribute(AttributeName = "po", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Po { get; set; }
        [XmlAttribute(AttributeName = "popep", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Popep { get; set; }
        [XmlAttribute(AttributeName = "pots", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Pots { get; set; }
        [XmlAttribute(AttributeName = "namespace")]
        public string Namespace { get; set; }
        [XmlAttribute(AttributeName = "acpi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Acpi { get; set; }
        [XmlAttribute(AttributeName = "whealogr", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Whealogr { get; set; }
    }

    [XmlRoot(ElementName = "message", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Message
    {
        [XmlAttribute(AttributeName = "message")]
        public string _message { get; set; }
        [XmlAttribute(AttributeName = "symbol")]
        public string Symbol { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "mid")]
        public string Mid { get; set; }
    }

    [XmlRoot(ElementName = "messageTable", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class MessageTable
    {
        [XmlElement(ElementName = "message", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<Message> Message { get; set; }
    }

    [XmlRoot(ElementName = "cmi", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Cmi
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "string", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class String2
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "stringTable", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class StringTable2
    {
        [XmlElement(ElementName = "string", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public List<String2> String2 { get; set; }
    }

    [XmlRoot(ElementName = "resources", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Resources2
    {
        [XmlElement(ElementName = "stringTable", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public StringTable2 StringTable2 { get; set; }
        [XmlAttribute(AttributeName = "culture")]
        public string Culture { get; set; }
    }

    [XmlRoot(ElementName = "localization", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
    public class Localization2
    {
        [XmlElement(ElementName = "resources", Namespace = "http://schemas.microsoft.com/win/2004/08/events")]
        public Resources2 Resources2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
