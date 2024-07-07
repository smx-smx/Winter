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
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    public class WcpLibraryAccessor
    {
        private readonly WindowsSystem windows;

        public WcpLibraryAccessor(WindowsSystem windows)
        {
            this.windows = windows;
        }

        private WcpLibrary? _theLibrary = null;

        public WcpLibrary ServicingStack
        {
            get
            {
                if (_theLibrary == null)
                {
                    _theLibrary = LoadWcpLibrary();
                }
                return _theLibrary;
            }
        }

        private string GetCurrentWCPVersion()
        {
            using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\Version");
            return hkey.ValueNames.First();
        }

        private string GetLastWCPVersionToAccessStore()
        {
            using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\COMPONENTS\ServicingStackVersions");
            var formattedVersion = hkey.GetValue<string>("LastWCPVersionToAccessStore");
            var lastVer = formattedVersion.Split('(').First().TrimEnd();
            return lastVer;
        }

        private WcpLibrary LoadWcpLibrary()
        {
            var wcpVer = GetLastWCPVersionToAccessStore();

            var servicingStackAppID = new ComponentAppId
            {
                Name = "microsoft-windows-servicingstack",
                Culture = "neutral",
                ProcessorArchitecture = "amd64",
                PublicKeyToken = Constants.MICROSOFT_PUBKEY,
                Version = wcpVer,
                VersionScope = "NonSxS"
            };

            var sxsName = servicingStackAppID.GetSxSName();
            var servicingStackRoot = Path.Combine(windows.SystemRoot, "WinSxS", sxsName);
            var wcpPath = Path.Combine(servicingStackRoot, "wcp.dll");

            Console.WriteLine(wcpPath);
            if (File.Exists(wcpPath))
            {
                Console.WriteLine("OK");
            }

            var wcp = WcpLibrary.Load(wcpPath);
            
            // $DEBUG
            // wcp.GetAppIdHash(servicingStackAppID.ToString());

            return wcp;

        }
    }
}
