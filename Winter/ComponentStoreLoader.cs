#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace Smx.Winter
{
    public class ComponentStoreLoader : IDisposable
    {
        private WindowsSystem windows;
        private bool wasLoaded = false;

        public ComponentStoreLoader(WindowsSystem windows)
        {
            this.windows = windows;
        }

        void LoadComponentsHive()
        {
            Util.EnablePrivilege(PInvoke.SE_BACKUP_NAME);
            Util.EnablePrivilege(PInvoke.SE_RESTORE_NAME);

            var componentsHivePath = Path.Combine(windows.SystemRoot, "System32", "Config", "components");
            var hKey = new SafeRegistryHandle((nint)RegistryHive.LocalMachine, true);
            PInvoke.RegLoadKey(hKey, "COMPONENTS", componentsHivePath);
        }

        bool IsComponentsHiveLoaded()
        {
            try
            {
                using var key = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\COMPONENTS");
                return true;
            } catch (Win32Exception ex)
            {
                var err = (WIN32_ERROR)ex.NativeErrorCode;
                if (err != WIN32_ERROR.ERROR_FILE_NOT_FOUND)
                {
                    throw;
                }

                return false;
            }
        }

        public void UnloadComponentsHive()
        {
            if (IsComponentsHiveLoaded())
            {
                var hKey = new SafeRegistryHandle((nint)RegistryHive.LocalMachine, true);
                PInvoke.RegUnLoadKey(hKey, "COMPONENTS");
            }
        }

        public void LoadComponentStore(bool keepLoaded = false)
        {
            if ((wasLoaded = IsComponentsHiveLoaded()) == false)
            {
                LoadComponentsHive();
            }
        }

        public void Dispose()
        {
            if (!wasLoaded)
            {
                UnloadComponentsHive();
            }
        }
    }
}
