#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.CompatibilityV1
{
    [XmlRoot(ElementName = "supportedOS", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class SupportedOS
    {
        [XmlAttribute(AttributeName = "Id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "application", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class Application2
    {
        [XmlElement(ElementName = "supportedOS", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
        public List<SupportedOS> SupportedOS { get; set; }
    }

    [XmlRoot(ElementName = "compatibility", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
    public class Compatibility
    {
        [XmlElement(ElementName = "application", Namespace = "urn:schemas-microsoft-com:compatibility.v1")]
        public Application2 Application2 { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
