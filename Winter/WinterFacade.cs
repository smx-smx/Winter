#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    public class WinterFacadeOptions
    {
        public string? SystemRoot { get; set; }
    }

    public class WinterFacade
    {
        public static void ConfigureServices(IServiceCollection sc, WinterFacadeOptions? options = default)
        {
            sc.AddSingleton(new WindowsSystem(options?.SystemRoot));
            sc.AddSingleton<WindowsRegistryAccessor>();
            sc.AddSingleton<ElevationService>();
            sc.AddSingleton<ComponentStoreService>();
            sc.AddSingleton<ComponentFactory>();
            sc.AddSingleton<WcpLibraryAccessor>();
            sc.AddSingleton<Program>();
        }
    }
}
