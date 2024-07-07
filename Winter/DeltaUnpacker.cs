#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.PDBSharp;
using Smx.PDBSharp.PE;
using Smx.PDBSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;

namespace Smx.Winter
{

    public class DeltaUnpacker
    {
        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate nint ComponentGetDelegate(
            nint env, nint lpName, long index);

        private nint g_environment;
        public ComponentGetDelegate ComponentGet;

        public DeltaUnpacker(WindowsSystem windows)
        {
            var msDeltaPath = Path.Combine(windows.SystemRoot, "System32", "msdelta.dll");

            var hLib = PInvoke.LoadLibrary("msdelta.dll");
            var libLoadAddr = hLib.DangerousGetHandle();

            if (!File.Exists("msdelta.pdb"))
            {
                using var fileStream = new FileStream("msdelta.pdb", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileStream.SetLength(0);

                var res = new PdbDownloader().DownloadForExecutable(msDeltaPath)?.Result;
                if (res == null) throw new Exception();
                var cts = new CancellationTokenSource();
                res.CopyTo(fileStream, null, cts.Token);
            }

            using var pdb = PDBFile.Open("msdelta.pdb");
            var facade = pdb.Services.GetService<PDBFacade>();
            var peSession = facade.OpenPE(msDeltaPath);

            var symbols = new Dictionary<string, ulong>()
            {
                { "cdp::New", 0 }
            };

            foreach(var p in symbols)
            {
                if (!peSession.TryGetSymbolByName(p.Key, out var addr))
                {
                    throw new Exception($"Symbol {p.Key} not found");
                }
                symbols[p.Key] = addr;
            }
        }
    }
}
