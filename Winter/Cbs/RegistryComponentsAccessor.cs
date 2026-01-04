#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.Winter.SchemaDefinitions.AsmV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public class RegistryComponentsAccessor
    {
        private readonly WindowsRegistryAccessor _registryAccessor;
        public RegistryComponentsAccessor(WindowsRegistryAccessor registryAccessor)
        {
            _registryAccessor = registryAccessor;
        }

        public ManagedRegistryKey Catalogs => _registryAccessor.ComponentsHive.OpenChildKey(@"CanonicalData\Catalogs");
        public ManagedRegistryKey Deployments => _registryAccessor.ComponentsHive.OpenChildKey(@"CanonicalData\Deployments");
        public ManagedRegistryKey Components => _registryAccessor.ComponentsHive.OpenChildKey(@"DerivedData\Components");
        public ManagedRegistryKey Packages => _registryAccessor.SoftwareHive.OpenChildKey(@"Microsoft\Windows\CurrentVersion\Component Based Servicing\Packages");

        public CatalogNode OpenCatalog(string id)
        {
            using var parent = Catalogs;
            using var hkey = parent.OpenChildKey(id);
            var catalog = Catalog.FromRegistryKey(hkey);
            return new CatalogNode(catalog, this);
        }

        public ComponentNode OpenComponent(string id)
        {
            using var parent = Components;
            using var hkey = parent.OpenChildKey($@"DerivedData\Components\{id}");
            var component = Component.FromRegistryKey(hkey);
            return new ComponentNode(component, this);
        }

        public DeploymentNode OpenDeployment(string id)
        {
            using var parent = Deployments;
            using var hkey = parent.OpenChildKey($@"CanonicalData\Deployments\{id}");
            var deployment = Deployment.FromRegistryKey(hkey);
            return new DeploymentNode(deployment, this);
        }
    }
}
