using System.Runtime.ExceptionServices;
using Microsoft.Extensions.Logging;
using Smx.Winter.Cbs;
using Smx.Winter.Cbs.Enumerators;
using Smx.Winter.Cbs.Native;
using Smx.Winter.Gui.Models;

namespace Smx.Winter.Gui.Controllers;

public class WinterSession : IDisposable
{
    public Guid SessionId { get; private set; }
    private readonly ICbsSession _session;
    private readonly ILogger? _logger;
    private CbsCore? _core;
    private NativeCbs? _nativeCbs;

    public WinterSession(StartCbsSessionCommand para, ILogger? logger = null)
    {
        SessionId = Guid.NewGuid();
        _logger = logger;
        _logger?.LogInformation("Creating CBS session: BootDrive={BootDrive}, WinDir={WinDir}", para.BootDrive, para.WinDir);
        _session = Initialize(para);
        _logger?.LogInformation("CBS session initialized, sessionId={SessionId}", SessionId);
    }

    private ICbsSession Initialize(StartCbsSessionCommand para)
    {
        const bool useOfflineServicingStack = true;
        _logger?.LogDebug("Loading NativeCbs from {WinDir}", para.WinDir);
        _nativeCbs = new NativeCbs(para.WinDir);
        var shim = _nativeCbs.StackShim.SssBindServicingStack(useOfflineServicingStack ? para.WinDir : null);
        var cbsCorePath = shim.GetCbsCorePath();
        _logger?.LogInformation("CbsCore path: {CbsCorePath}", cbsCorePath);
        Console.WriteLine($"CbsCore: {cbsCorePath}");
        _core = new CbsCore(cbsCorePath);
        var session = _core.Initialize();

        var currentWinDir = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System))!.FullName;
        var isOnline = string.Equals(currentWinDir, para.WinDir, StringComparison.InvariantCultureIgnoreCase);
        _logger?.LogInformation("CBS mode: {Mode}, BootDrive={BootDrive}", isOnline ? "Online" : "Offline", isOnline ? null : para.BootDrive);
        session.Initialize(_CbsSessionOption.CbsSessionOptionNone,
            "Winter",
            isOnline ? null : para.BootDrive,
            isOnline ? null : para.WinDir);
        return session;
    }

    public IEnumerable<string> GetPackageIds()
    {
        _logger?.LogDebug("Enumerating package IDs");
        _session.EnumeratePackages(0x70, out var list);
        return new CbsIdentityEnumerable(list).Select(pkg => pkg.GetStringId());
    }

    public IEnumerable<ICbsPackage> GetPackages()
    {
        _session.EnumeratePackages(0x70, out var list);
        return new CbsIdentityEnumerable(list).Select(pkg => _session.OpenPackage(0, pkg, null!));
    }

    public async Task<IReadOnlyList<CbsPackageViewModel>> GetPackagesAsync(
        Action<int, int>? onProgress = null,
        CancellationToken ct = default)
    {
        _logger?.LogInformation("Starting async package enumeration");
        return await Task.Run(() =>
        {
            _session.EnumeratePackages(0x70, out var list);
            var identities = new CbsIdentityEnumerable(list).ToList();
            _logger?.LogInformation("Enumerated {Count} package identities", identities.Count);
            return LoadPackagesCore(identities, onProgress, ct);
        }, ct);
    }

    [HandleProcessCorruptedStateExceptions]
    private IReadOnlyList<CbsPackageViewModel> LoadPackagesCore(
        IReadOnlyList<ICbsIdentity> identities,
        Action<int, int>? onProgress,
        CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var total = identities.Count;
        onProgress?.Invoke(0, total);
        var packages = new List<CbsPackageViewModel>(total);
        var errorCount = 0;

        for (int i = 0; i < total; i++)
        {
            ct.ThrowIfCancellationRequested();
            var identity = identities[i];
            var idStr = SafeGetId(identity);
            if (i % 250 == 0)
                _logger?.LogInformation("Opening package {Index}/{Total}, current: {PackageId}", i + 1, total, idStr);
            try
            {
                var pkg = _session.OpenPackage(0, identities[i], null!);
                packages.Add(new CbsPackageViewModel(pkg));
            }
            catch (Exception ex)
            {
                errorCount++;
                _logger?.LogWarning(ex, "Failed to open package: {PackageId}", idStr);
            }
            onProgress?.Invoke(i + 1, total);
        }

        sw.Stop();
        _logger?.LogInformation("Loaded {Count} packages ({Errors} errors) in {Elapsed:F1}s",
            packages.Count, errorCount, sw.Elapsed.TotalSeconds);
        return packages;
    }

    private static string SafeGetId(ICbsIdentity identity)
    {
        try { return identity.GetStringId(); }
        catch { return "?"; }
    }

    public void Dispose()
    {
        _logger?.LogInformation("Closing CBS session {SessionId}", SessionId);
        Console.WriteLine("Closing CBS session");
        _session.Finalize(out var requiredAction);
        _core?.Dispose();
        _nativeCbs?.Dispose();
    }
}
