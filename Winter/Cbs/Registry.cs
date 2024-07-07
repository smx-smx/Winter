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
    public class Registry
    {
        public const string KEY_CATALOGS = @"HKEY_LOCAL_MACHINE\COMPONENTS\CanonicalData\Catalogs";
        public const string KEY_COMPONENTS = @"HKEY_LOCAL_MACHINE\COMPONENTS\DerivedData\Components";
        public const string KEY_DEPLOYMENTS = @"HKEY_LOCAL_MACHINE\COMPONENTS\CanonicalData\Deployments";
        public const string KEY_PACKAGES = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\Packages";

        public static CatalogNode OpenCatalog(string id)
        {
            using var hkey = ManagedRegistryKey.Open($@"{KEY_CATALOGS}\{id}");
            var catalog = Catalog.FromRegistryKey(hkey);
            return new CatalogNode(catalog);
        }

        public static ComponentNode OpenComponent(string id)
        {
            using var hkey = ManagedRegistryKey.Open($@"{KEY_COMPONENTS}\{id}");
            var component = Component.FromRegistryKey(hkey);
            return new ComponentNode(component);
        }

        public static DeploymentNode OpenDeployment(string id)
        {
            using var hkey = ManagedRegistryKey.Open($@"{KEY_DEPLOYMENTS}\{id}");
            var deployment = Deployment.FromRegistryKey(hkey);
            return new DeploymentNode(deployment);
        }
    }
}
