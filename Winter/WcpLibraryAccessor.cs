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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Smx.Winter
{
    public class ServicingStackVersion
    {
        public readonly string Version;
        public readonly string? ProductName;

        public ServicingStackVersion(string unparsed)
        {
            var firstSpace = unparsed.IndexOf(' ');
            ProductName = firstSpace < 0 ? null : unparsed.Substring(firstSpace + 1);
            Version = firstSpace < 0 ? unparsed : unparsed.Substring(0, firstSpace);
        }
    }

    public class WcpLibraryAccessor : IDisposable
    {
        private readonly WindowsSystem windows;
        private readonly WindowsRegistryAccessor _registryAccessor;

        public WcpLibraryAccessor(WindowsSystem windows, WindowsRegistryAccessor registryAccessor)
        {
            this.windows = windows;
            _registryAccessor = registryAccessor;
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

        public void UnloadServicingStack()
        {
            _theLibrary?.Dispose();
            _theLibrary = null;
        }

        private string GetCurrentWCPVersion()
        {
            using var hkey = _registryAccessor.SoftwareHive.OpenChildKey(@"Microsoft\Windows\CurrentVersion\Component Based Servicing\Version");
            return hkey.ValueNames.First();
        }

        private ServicingStackVersion GetLastWCPVersionToAccessStore()
        {
            using var hkey = _registryAccessor.ComponentsHive.OpenChildKey(@"ServicingStackVersions");
            var formattedVersion = hkey.GetValue<string>("LastWCPVersionToAccessStore");
            if(formattedVersion == null)
            {
                throw new InvalidOperationException("Cannot get Lats WCP Version to access store");
            }
            return new ServicingStackVersion(formattedVersion);
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
                Version = wcpVer.Version,
                VersionScope = "NonSxS"
            };

            var sxsName = servicingStackAppID.GetName(ComponentAppIdNameFormat.CbsLong);
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

        public void Dispose()
        {
            _registryAccessor.Dispose();
            _theLibrary?.Dispose();
        }
    }
}
