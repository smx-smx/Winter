#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public class DeploymentNode
    {
        private readonly Deployment deployment;

        public DeploymentNode(Deployment deployment)
        {
            this.deployment = deployment;
        }

        public string AppId => deployment.AppId;
        public CatalogNode? Catalog => deployment.CatalogThumbprint?.Let(Registry.OpenCatalog);

        public override string ToString()
        {
            return $"DeploymentNode({deployment})";
        }
    }

    public class Deployment
    {
        public string AppId { get; set; }
        public string? CatalogThumbprint { get; set; }

        public Deployment()
        {
        }

        public static Deployment FromRegistryKey(ManagedRegistryKey key)
        {
            if (!key.TryGetValue<string>("CatalogThumbprint", out var catalogThumbprint))
            {
                catalogThumbprint = null;
            }
            return new Deployment
            {
                AppId = Encoding.UTF8.GetString(key.GetValue<byte[]>("appid")),
                CatalogThumbprint = catalogThumbprint
            };
        }

        public override string ToString()
        {
            return AppId;
        }
    }
}
