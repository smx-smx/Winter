#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Smx.PDBSharp.Thunks;
using Smx.SharpIO;
using Smx.Winter.MsDelta;
using Smx.Winter.Tools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Xml.Linq;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Security;

namespace Smx.Winter;

public record ManagedHandle(HANDLE handle) : IDisposable
{
    public void Dispose()
    {
        PInvoke.CloseHandle(handle);
    }

    public static implicit operator HANDLE(ManagedHandle h) => h.handle;
}
public record ManagedScHandle(SC_HANDLE handle) : IDisposable
{
    public void Dispose()
    {
        PInvoke.CloseServiceHandle(handle);
    }

    public static implicit operator SC_HANDLE(ManagedScHandle h) => h.handle;
}

public static class HandleExtensions
{
    public static ManagedScHandle AsManaged(this SC_HANDLE handle) => new ManagedScHandle(handle);
}

public static class DisposableMemoryExtensions
{
    public static unsafe PWSTR ToPWSTR(this DisposableMemory mem)
    {
        return new PWSTR((char *)mem.Value.ToPointer());
    }
}

public class Program
{
    private WindowsSystem windows;
    private ElevationService elevator;
    private ComponentStoreService store;
    private readonly WcpLibrary servicingStack;
    private readonly AssemblyReader asmReader;


    public Program(
        WindowsSystem windows,
        ElevationService elevator,
        ComponentStoreService store,
        WcpLibraryAccessor wcp,
        AssemblyReader decompressor
    ) {
        this.windows = windows;
        this.elevator = elevator;
        this.store = store;
        this.servicingStack = wcp.ServicingStack;
        this.asmReader = decompressor;
    }

    public void Initialize()
    {
        elevator.ImpersonateTrustedInstaller();
    }

    private void TestSAM()
    {
        using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SAM\SAM\Domains\Account\Users\Names\Administrator");
        hkey.GetValue("", out var type);
        Console.WriteLine(type);
    }

    public void TestAllManifests(bool parse)
    {
        var manifestsDir = Path.Combine(windows.SystemRoot, "WinSxS", "Manifests");
        var filesIterator = Directory.EnumerateFiles(manifestsDir, "*.manifest", new EnumerationOptions
        {
            MatchCasing = MatchCasing.CaseInsensitive,
            RecurseSubdirectories = false,
        });

        var iManifest = 0;
        var nManifests = filesIterator.Count();

        // ramdisk path
        var basePath = @"S:\out";
        if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);


        foreach (var manifest in filesIterator)
        {
            Console.WriteLine($"[{++iManifest}/{nManifests}] {manifest}");
            using var stream = new FileStream(manifest, FileMode.Open);
            if (parse)
            {
                var asm = asmReader.ReadToObject(stream);
            }
            else
            {
                var manifestData = asmReader.ReadToString(stream);
                var outPath = Path.Combine(basePath, Path.GetFileName(manifest));
                File.WriteAllText(outPath, manifestData);
            }
        }
    }

    public void TestCBS()
    {
        /*
        foreach (var cat in store.Catalogs)
        {
            foreach (var dep in cat.Deployments)
            {
                var hash = servicingStack.GetAppIdHash(dep.AppId);
                Console.WriteLine(dep.AppId);
                Console.WriteLine(hash);
            }
        }
        return;
        */

        foreach (var cmp in store.Components)
        {
            var hash = string.Format("{0:x}", servicingStack.GetAppIdHash(cmp.Identity));
            Console.WriteLine(cmp);
            Console.WriteLine(cmp.Identity);

            var formatted = ComponentAppId.FromAppId(cmp.Identity).GetSxSName(compact: true);
            var manifestPath = Path.Combine(windows.SystemRoot, "WinSxS", "Manifests", $"{formatted}.manifest");
            if (!File.Exists(manifestPath))
            {
                Console.WriteLine("!!!!!!! FAIL: " + manifestPath);
            }
        }
        return;
    }

    public void ParseAllUpdateModules()
    {
        foreach (string item in windows.AllModules)
        {
            using var stream = new FileStream(item, FileMode.Open, FileAccess.Read);
            var obj = asmReader.ReadToObject(stream);
        }
    }

    public static void TestMsdeltaNative(string[] args)
    {
        int argi = 1;
        var name = args[argi++];
        var code = File.ReadAllText(args[argi++]);

        var nInputs = args.Length - (argi + 1);
        var inputs = args.Skip(argi).Take(nInputs).Select(File.ReadAllBytes).ToArray();
        
        argi += nInputs;

        var nOutputs = args.Length - argi;
        var outputs = new byte[nOutputs][];

        var outputFiles = args.Skip(argi).Take(nOutputs).ToArray();
        
        var msdelta = new NativeMSDelta();
        msdelta.AddComponent(name, code);
        if(!msdelta.Call(name, inputs, outputs))
        {
            throw new InvalidOperationException("script call failed");
        }

        for(int i = 0; i < nOutputs; i++)
        {
            File.WriteAllBytes(outputFiles[i], outputs[i]);
        }
    }


    public static void Main(string[] args)
    {
        bool handled = true;
        switch (args[0]) {
            case "msdelta-script":
                TestMsdeltaNative(args);
                break;
            case "xml-merge":
                XmlMerger.ToolMain([@"S:\out", @"S:\merged.xml"]);
                break;
            case "patch-read":
                {
                    using var patch = MFile.Open(args[1], FileMode.Open, FileAccess.Read, FileShare.Read);
                    new PatchReader(patch).Read();
                    break;
                }
            default:
                handled = false;
                break;
        }

        if (handled)
        {
            Environment.Exit(0);
        }


        var winter = new WinterFacade();
        winter.Initialize();

        var p = winter.Services.GetRequiredService<Program>();
        p.Initialize();

        switch (args[0])
        {
            case "cmd":
                p.elevator.RunAsTrustedInstaller("cmd.exe");
                break;
            case "test-all":
                p.TestAllManifests(true);
                break;
            case "test-cbs":
                p.TestCBS();
                break;
            case "test-mum":
                p.ParseAllUpdateModules();
                break;
            case "read-asm":
                {
                    var wcpAcc = winter.Services.GetRequiredService<WcpLibraryAccessor>();

                    using var input = MFile.Open(args[1], FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var stream = new SpanStream(input);
                    var decoded = new AssemblyReader(wcpAcc).ReadToString(stream);
                    Console.WriteLine(decoded);
                    break;
                }
        }
    }
}
