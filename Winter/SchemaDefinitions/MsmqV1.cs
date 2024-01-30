#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Xml.Serialization;

namespace Smx.Winter.SchemaDefinitions.MsmqV1
{
    [XmlRoot(ElementName = "MsmqWorkgroupOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqWorkgroupOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "MsmqAdIntegrationOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqAdIntegrationOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

    [XmlRoot(ElementName = "MsmqHttpOnlineInstall", Namespace = "urn:schemas-microsoft-com:msmq.v1")]
    public class MsmqHttpOnlineInstall
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
    }

}
