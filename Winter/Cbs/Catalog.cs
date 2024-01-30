#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public class CatalogNode
    {
        private Catalog catalog;

        public IEnumerable<DeploymentNode> Deployments
        {
            get
            {
                return catalog.ComponentNames.Select(Registry.OpenDeployment);
            }
        }

        public CatalogNode(Catalog catalog)
        {
            this.catalog = catalog;
        }

        public override string ToString()
        {
            return $"CatalogNode({catalog})";
        }
    }

    public class Catalog
    {
        public List<string> ComponentNames { get; set; } = new List<string>();
        public string Thumbprint { get; set; }

        public Catalog()
        {
        }

        public static Catalog FromRegistryKey(ManagedRegistryKey key)
        {
            return new Catalog
            {
                Thumbprint = key.Name,
                ComponentNames = ObjectRefList.FromRegistryKey(key)
                    .Where(o => o.ObjectType == ObjectType.Object)
                    .Select(o => o.ObjectName)
                    .ToList()
            };
        }

        public override string ToString()
        {
            return Thumbprint;
        }
    }
}
