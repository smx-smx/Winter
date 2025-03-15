using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smx.Winter.Cbs;
using Smx.Winter.Cbs.Enumerators;
using Smx.Winter.Cbs.Native;
using Smx.Winter.Gui.Models;

namespace Smx.Winter.Gui.Controllers;

public class WinterSession : IDisposable
{
    public Guid SessionId { get; private set; }
    private ICbsSession _session;
    private StartCbsSessionCommand _para;

    public WinterSession(StartCbsSessionCommand para)
    {
        SessionId = Guid.NewGuid();
        _para = para;
        _session = Initialize(para);
    }

    private static ICbsSession Initialize(StartCbsSessionCommand para)
    {
        const bool useOfflineServicingStack = true;
        var nat = new NativeCbs(para.WinDir);
        var shim = nat.StackShim.SssBindServicingStack(useOfflineServicingStack ? para.WinDir : null);
        var cbsCorePath = shim.GetCbsCorePath();
        Console.WriteLine($"CbsCore: {cbsCorePath}");
        var core = new CbsCore(cbsCorePath);
        var session = core.Initialize();

        var currentWinDir = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System))!.FullName;
        var isOnline = string.Equals(currentWinDir, para.WinDir, StringComparison.InvariantCultureIgnoreCase);
        session.Initialize(_CbsSessionOption.CbsSessionOptionNone,
            "Winter",
            isOnline ? null : para.BootDrive,
            isOnline ? null : para.WinDir);
        return session;
    }

    public IEnumerable<string> GetPackageIds()
    {
        _session.EnumeratePackages(0x70, out var list);
        return new CbsIdentityEnumerable(list).Select(pkg => pkg.GetStringId());
    }

    public IEnumerable<ICbsPackage> GetPackages()
    {
        _session.EnumeratePackages(0x70, out var list);
        // warning: opening packages require a compatible version of the servicing stack
        return new CbsIdentityEnumerable(list).Select(pkg => _session.OpenPackage(0, pkg, null!));
    }

    public void Dispose()
    {
        Console.WriteLine("Closing CBS session");
        _session.Finalize(out var requiredAction);
    }
}
