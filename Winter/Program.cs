using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Smx.Winter.Tools;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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
        }
        catch (Win32Exception ex)
        {
            var err = (WIN32_ERROR)ex.NativeErrorCode;
            if (err != WIN32_ERROR.ERROR_FILE_NOT_FOUND)
            {
                throw;
            }

            return false;
        }   
    }

    void MaybeLoadComponentsHive()
    {
        if (!IsComponentsHiveLoaded())
        {
            LoadComponentsHive();
            //AppDomain.CurrentDomain.ProcessExit += UnloadComponentsHive;
        }
    }

    private static void UnloadComponentsHive(object? sender, EventArgs e)
    {
        var hKey = new SafeRegistryHandle((nint)RegistryHive.LocalMachine, true);
        PInvoke.RegUnLoadKey(hKey, "COMPONENTS");
    }

    private WindowsSystem windows;
    private ElevationService elevator;
    private ComponentStoreService store;
    private readonly WcpLibrary wcp;
    private readonly ManifestReader decompressor;

    public Program(
        WindowsSystem windows,
        ElevationService elevator,
        ComponentStoreService store,
        WcpLibrary wcp,
        ManifestReader decompressor
    ) {
        this.windows = windows;
        this.elevator = elevator;
        this.store = store;
        this.wcp = wcp;
        this.decompressor = decompressor;
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
                var asm = decompressor.DecompressToObject(stream);
            }
            else
            {
                var manifestData = decompressor.DecompressToString(stream);
                var outPath = Path.Combine(basePath, Path.GetFileName(manifest));
                File.WriteAllText(outPath, manifestData);
            }
        }
    }

    public void Test()
    {
        //cs.TraverseDeployments();
        using var wcp = store.GetServicingStack();

        //var componentName = Path.GetFileName(Path.GetDirectoryName(wcp.LibraryPath));
        var componentName = "wow64_microsoft-windows-o..euapcommonproxystub_31bf3856ad364e35_10.0.19041.3636_none_11aa075e69fefa11";

        var manifestPath = Path.Combine(windows.SystemRoot,
            "WinSxS", "Manifests", $"{componentName}.manifest");

        var asm = wcp.DecompressManifest(manifestPath);
        Console.WriteLine(asm);
    }

    private static ServiceProvider BuildServices()
    {
        var sc = new ServiceCollection();

        var windowsSystem = new WindowsSystem();
        var elevationService = new ElevationService();
        var componentStore = new ComponentStoreService(windowsSystem);
        var servicingStack = componentStore.GetServicingStack();
        var manifestDecompressor = new ManifestReader(servicingStack.GetPatchDictionary());

        var componentFactory = new ComponentFactory(windowsSystem, componentStore);

        sc.AddSingleton(windowsSystem);
        sc.AddSingleton(elevationService);
        sc.AddSingleton(componentStore);
        sc.AddSingleton(servicingStack);
        sc.AddSingleton(manifestDecompressor);
        sc.AddSingleton(componentFactory);

        sc.AddSingleton<Program>();
        return sc.BuildServiceProvider();
    }

    public static void Main(string[] args)
    {
        using var services = BuildServices();

        var p = services.GetRequiredService<Program>();
        p.MaybeLoadComponentsHive();
        p.Initialize();
        //p.elevator.RunAsTrustedInstaller("cmd.exe");
        p.TestAllManifests(true);
        //p.Test();

        //XmlMerger.ToolMain([@"S:\out", @"S:\merged.xml"]);
    }
}
