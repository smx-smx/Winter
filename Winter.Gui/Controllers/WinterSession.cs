using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smx.Winter.Cbs;
using Smx.Winter.Cbs.Native;
using Smx.Winter.Gui.Models;

namespace Smx.Winter.Gui.Controllers;

public class WinterSession : IDisposable
{
    public Guid SessionId { get; private set; }
    private ICbsSession _session;
    private StartCbsSessionCommand _para;

    public WinterSession(StartCbsSessionCommand para){
        SessionId = Guid.NewGuid();
        _para = para;
        _session = Initialize(para);
    }

    private static ICbsSession Initialize(StartCbsSessionCommand para){
        const bool useOfflineServicingStack = false;
        var nat = new NativeCbs(para.WinDir);
        var shim = nat.StackShim.SssBindServicingStack(useOfflineServicingStack ? para.WinDir : null);
        var cbsCorePath = shim.GetCbsCorePath();
        Console.WriteLine($"CbsCore: {cbsCorePath}");
        var core = new CbsCore(cbsCorePath);
        var session = core.Initialize();
        session.Initialize(_CbsSessionOption.CbsSessionOptionNone,
            "Winter", para.BootDrive, para.WinDir);
        return session;
    }

    public IEnumerable<string> GetPackages(){
        _session.EnumeratePackages(0x70, out var list);
        while(true){
            list.Next(1, out var item, out var fetched);
            if(fetched == 0){
                yield break;
            } else {
                yield return item.GetStringId();
            }
        }
    }

    public void Dispose()
    {
        Console.WriteLine("Closing CBS session");
        _session.Finalize(out var requiredAction);
    }
}