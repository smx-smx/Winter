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

namespace Smx.Winter.SchemaDefinitions.WindowsSettingsNS
{
    [XmlRoot(ElementName = "windowsSettings", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
    public class WindowsSettings
    {
        [XmlElement(ElementName = "dpiAware", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
        public string DpiAware { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "dpiAware", Namespace = "http://schemas.microsoft.com/SMI/2005/WindowsSettings")]
    public class DpiAware
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
}
