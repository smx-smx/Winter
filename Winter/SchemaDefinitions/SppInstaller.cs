#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.SppInstallerNS
{
    [XmlRoot(ElementName = "sppInstaller", Namespace = "urn:schemas-microsoft-com:spp:installer")]
    public class SppInstaller
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
