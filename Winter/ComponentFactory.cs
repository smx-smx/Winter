#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Smx.Winter.Cbs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    internal class ComponentFactory
    {
        private readonly WindowsSystem windows;
        private readonly ComponentStoreService componentStore;

        public ComponentFactory(WindowsSystem windows, ComponentStoreService componentStore)
        {
            this.windows = windows;
            this.componentStore = componentStore;
        }

        private static string? GetComponentIdFromFile(string filePath)
        {
            var dirSep = Path.DirectorySeparatorChar;
            return Util.GetHardLinks(filePath)
                .FirstOrDefault(
                    l => l.Contains($"{dirSep}WinSxS{dirSep}",
                    StringComparison.InvariantCultureIgnoreCase));
        }


        public Component GetFromManifest(string manifestPath)
        {
            //componentStore.TraverseDeployments
            return null;
        }

        public Component GetFromFile(string filePath)
        {
            return null;
        }
    }
}
