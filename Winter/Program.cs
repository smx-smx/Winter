#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Microsoft.Extensions.DependencyInjection;
using Smx.SharpIO;
using Smx.SharpIO.Memory;
using Smx.Winter.MsDelta;
using Smx.Winter.Tools;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Services;
using Smx.Winter.Cbs.Native;
using Smx.Winter.Cbs;
using Windows.Win32.System.Com;
using Smx.Winter.Cbs.Enumerators;

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
    public static unsafe PWSTR ToPWSTR(this NativeMemoryHandle mem)
    {
        return new PWSTR((char*)mem.Address.ToPointer());
    }
}

public class Program
{
    private readonly WindowsSystem windows;
    private readonly ElevationService elevator;
    private readonly ComponentStoreService store;
    private readonly WindowsRegistryAccessor _registryAccessor;

    public Program(
        WindowsSystem windows,
        ElevationService elevator,
        ComponentStoreService store,
        WindowsRegistryAccessor registryAccessor
    )
    {
        this.windows = windows;
        this.elevator = elevator;
        this.store = store;
        _registryAccessor = registryAccessor;
    }

    public void Initialize()
    {
    }

    private void TestSAM()
    {
        using var hkey = ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SAM\SAM\Domains\Account\Users\Names\Administrator");
        hkey.GetValue("", out var type);
        Console.WriteLine(type);
    }

    private AssemblyReader NewAssemblyReader()
    {
        var acc = new WcpLibraryAccessor(windows, _registryAccessor);
        return new AssemblyReader(acc.ServicingStack);
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

        var asmReader = NewAssemblyReader();

        foreach (var manifest in filesIterator)
        {
            Console.WriteLine($"[{++iManifest}/{nManifests}] {manifest}");
            using var stream = new FileStream(manifest, FileMode.Open);
            if (parse)
            {
                var asm = asmReader.ReadToObject(stream);
            } else
            {
                var manifestData = asmReader.ReadToString(stream);
                var outPath = Path.Combine(basePath, Path.GetFileName(manifest));
                File.WriteAllText(outPath, manifestData);
            }
        }
    }

    private delegate void EnumPackages(uint a, out IEnumCbsIdentity b);

    private void TestCbsSession(string bootDrive, string winDir, bool useOfflineServicingStack)
    {
        var nat = new NativeCbs(winDir);
        var shim = nat.StackShim.SssBindServicingStack(useOfflineServicingStack ? winDir : null);
        var cbsCorePath = shim.GetCbsCorePath();
        Console.WriteLine($"CbsCore: {cbsCorePath}");
        var core = new CbsCore(cbsCorePath);
        var session = core.Initialize();

        var currentWinDir = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System))!.FullName;
        var isOnline = string.Equals(currentWinDir, winDir, StringComparison.InvariantCultureIgnoreCase);

        session.Initialize(_CbsSessionOption.CbsSessionOptionNone,
            "Winter",
            isOnline ? null : bootDrive,
            isOnline ? null : winDir);
        session.EnumeratePackages(0x70, out var list);
        session.Finalize(out var requiredAction);
    }

    private void TestOnline()
    {
        using var nat = new NativeCbs();

        if (!PInvoke.CoCreateInstance<ICbsSession>(
            typeof(CbsSessionClass).GUID,
            null,
            CLSCTX.CLSCTX_LOCAL_SERVER,
            out var localSession
        ).Succeeded)
        {
            throw new InvalidOperationException("Failed to create CbsSession");
        }
        localSession.Initialize(_CbsSessionOption.CbsSessionOptionNone, "Winter", null, null);
        localSession.EnumeratePackages(0x70, out var list1);
        while (true)
        {
            list1.Next(1, out var item, out var fetched);
            if (fetched == 0) break;
            Console.WriteLine(item.GetStringId());
        }
        localSession.Finalize(out var requiredAction1);
    }

    public void TestCbsNative()
    {
        TestCbsSession(@"C:", @"C:\Windows", useOfflineServicingStack: false);
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

        var acc = new WcpLibraryAccessor(windows, _registryAccessor);
        var servicingStack = acc.ServicingStack;

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

    public void ParseAllPackages()
    {
        var asmReader = NewAssemblyReader();
        foreach (string item in windows.AllPackages)
        {
            using var stream = new FileStream(item, FileMode.Open, FileAccess.Read);
            var obj = asmReader.ReadToObject(stream);
        }
    }

    static bool TryTake(IEnumerator<string> it, [MaybeNullWhen(false)] out string arg)
    {
        if (!it.MoveNext())
        {
            arg = null;
            return false;
        }

        arg = it.Current;
        return true;
    }

    public static void TestMsdeltaNative(IEnumerator<string> args)
    {
        if (!TryTake(args, out var name)
            || !TryTake(args, out var codeFile))
        {
            throw new InvalidEnumArgumentException();
        }

        var inputs = new List<byte[]>();
        var outputFiles = new List<string>();

        bool inOutSel = true;

        while (args.MoveNext())
        {
            var arg = args.Current;
            switch (arg)
            {
                case "-i":
                    inOutSel = true;
                    break;
                case "-o":
                    inOutSel = false;
                    break;
                default:
                    if (inOutSel)
                    {
                        inputs.Add(File.ReadAllBytes(arg));
                    } else
                    {
                        outputFiles.Add(arg);
                    }
                    break;
            }
        }

        var code = File.ReadAllText(codeFile);
        var outputs = new byte[outputFiles.Count][];

        var msdelta = new NativeMSDelta();
        msdelta.AddComponent(name, code);
        if (!msdelta.Call(name, inputs.ToArray(), ref outputs))
        {
            throw new InvalidOperationException("script call failed");
        }

        for (int i = 0; i < outputFiles.Count; i++)
        {
            using var fh = new FileStream(outputFiles[i], FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            fh.SetLength(0);

            if (outputs[i] == null) continue;
            fh.Write(outputs[i]);
            fh.Close();
        }
    }


    public static void Main(string[] args)
    {
        var it = ((IEnumerable<string>)args).GetEnumerator();
        if(!TryTake(it, out var mode))
        {
            Environment.Exit(1);
        }

        bool handled = true;
        switch (mode)
        {
            case "msdelta-script":
                TestMsdeltaNative(it);
                break;
            case "xml-merge":
                XmlMerger.ToolMain([@"S:\out", @"S:\merged.xml"]);
                break;
            case "patch-read":
                {
                    if(!TryTake(it, out var patchPath))
                    {
                        Environment.Exit(1);
                    }

                    using var patch = MFile.Open(patchPath, FileMode.Open, FileAccess.Read, FileShare.Read);
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

        var opts = new WinterFacadeOptions();

        string? arg1 = default;

        if(TryTake(it, out arg1))
        {
            if(arg1 == "--sysroot")
            {
                if(!TryTake(it, out var sysroot))
                {
                    Environment.Exit(1);
                }
                opts.SystemRoot = sysroot;

                TryTake(it, out arg1);
            }
        }



        var winter = new WinterFacade(options:opts);
        winter.Initialize();

        var p = winter.Services.GetRequiredService<Program>();
        p.Initialize();

        switch (mode)
        {
            case "cmd":
                p.elevator.RunAsTrustedInstaller(["cmd.exe"]);
                break;
            case "test-all":
                p.TestAllManifests(true);
                break;
            case "test-cbs-native":
                /**
                  * need to start a new thread to "consolidate" the current Token
                  * otherwise CBS will call `ImpersonateSelf(SecurityImpersonation)`
                  * which reverts all `AdjustTokenPrivilege` calls made so far
                  */
                new Thread(() =>
                {
                    p.TestCbsNative();
                }).Start();
                break;
            case "test-cbs":
                p.TestCBS();
                break;
            case "test-mum":
                p.ParseAllPackages();
                break;
            case "read-asm":
                {
                    var wcpAcc = winter.Services.GetRequiredService<WcpLibraryAccessor>();

                    ArgumentNullException.ThrowIfNull(arg1);

                    using var input = MFile.Open(arg1, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var stream = new SpanStream(input);
                    var decoded = new AssemblyReader(wcpAcc.ServicingStack).ReadToString(stream);
                    Console.WriteLine(decoded);
                    break;
                }
        }
    }
}
