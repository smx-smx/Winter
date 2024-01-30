#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.Winter.SchemaDefinitions.AsmV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public class ComponentNode
    {
        private readonly Component component;

        public ComponentNode(Component component)
        {
            this.component = component;
        }

        public string Identity => component.Identity;

        public IEnumerable<CatalogNode> Catalogs => component.Catalogs.Select(Registry.OpenCatalog);

        public override string ToString()
        {
            return $"ComponentNode({component})";
        }
    }

    public class Component 
    {
        public string Identity { get; set; }
        public ICollection<string> Catalogs { get; set; }

        public Component()
        {
        }

        internal static Component FromRegistryKey(ManagedRegistryKey key)
        {
            var sha256 = key.GetValue<byte[]>("S256H");
            var identity = Encoding.UTF8.GetString(key.GetValue<byte[]>("identity"));
            var catalogs = ObjectRefList.FromRegistryKey(key)
                .Where(c => c.ObjectType == ObjectType.Object)
                .Select(o => o.ObjectName)
                .ToList();

            return new Component
            {
                Identity = identity,
                Catalogs = catalogs
            };
        }

        public override string ToString()
        {
            return Identity;
        }
    }
}
