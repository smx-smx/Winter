#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    public class WinterFacade : IDisposable
    {
        public ServiceProvider Services { get; private set; }


        private static ServiceProvider BuildServices()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<WindowsSystem>();
            sc.AddSingleton<ElevationService>();
            sc.AddSingleton<ComponentStoreService>();
            sc.AddSingleton<ManifestReader>();
            sc.AddSingleton<ComponentFactory>();
            sc.AddSingleton<WcpLibraryAccessor>();
            sc.AddSingleton<Program>();
            var sp = sc.BuildServiceProvider();
            return sp;
        }

        public WinterFacade() {
            Services = BuildServices();
        }

        public IEnumerable<string> AllManifests
        {
            get
            {
                return Services.GetRequiredService<WindowsSystem>().AllManifests;
            }
        }

        public void Initialize()
        {
            // $FIXME: dependency injection
            using var ldr = new ComponentStoreLoader(Services.GetRequiredService<WindowsSystem>());
            ldr.LoadComponentStore(keepLoaded: true);

            var p = Services.GetRequiredService<Program>();
            p.Initialize();
        }

        public void Dispose()
        {
            Services.Dispose();
        }
    }
}
