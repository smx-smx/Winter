#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.RescacheV1
{
    [XmlRoot(ElementName = "resfile", Namespace = "urn:schemas-microsoft-com:rescache.v1")]
    public class Resfile
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "affinity")]
        public string Affinity { get; set; }
        [XmlAttribute(AttributeName = "cached")]
        public string Cached { get; set; }
        [XmlAttribute(AttributeName = "sequence")]
        public string Sequence { get; set; }
    }

    [XmlRoot(ElementName = "rescache", Namespace = "urn:schemas-microsoft-com:rescache.v1")]
    public class Rescache
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }
}
