using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("75207391-23F2-4396-85F0-8FDB879ED0ED")]
[CoClass(typeof(CbsSessionClass))]
public interface CbsSession : ICbsSession
{
}
