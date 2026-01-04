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

namespace Smx.Winter
{
    public class WindowsSystem
    {
        private readonly string _systemRoot;

        public WindowsSystem(string? systemRoot = null)
        {
            if(systemRoot == null)
            {
                var onlineSystemRoot = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue<string>("SystemRoot");
                if (onlineSystemRoot == null)
                {
                    throw new InvalidOperationException("Cannot get SystemRoot from registry");
                }
                _systemRoot = onlineSystemRoot;
            } else
            {
                _systemRoot = systemRoot;
            }
        }

        public string SystemRoot => _systemRoot;

        public IEnumerable<string> RegisteredModules
        {
            get
            {
                using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\Packages");
                return hkey.KeyNames;
            }
        }

        public IEnumerable<string> AllModules
        {
            get
            {
                var modulesDir = Path.Combine(SystemRoot, "servicing", "Packages");
                var filesIterator = Directory.EnumerateFiles(modulesDir, "*.mum", new EnumerationOptions
                {
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = false,
                });

                return filesIterator;
            }
        }

        public IEnumerable<string> AllManifests
        {
            get
            {
                var manifestsDir = Path.Combine(SystemRoot, "WinSxS", "Manifests");
                var filesIterator = Directory.EnumerateFiles(manifestsDir, "*.manifest", new EnumerationOptions
                {
                    MatchCasing = MatchCasing.CaseInsensitive,
                    RecurseSubdirectories = false,
                });

                return filesIterator;
            }
        }
    }
}
