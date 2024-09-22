using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("25F05277-E733-4455-80B7-FA9C2DC9E678")]
public interface IEnumCbsCapability
{
	void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICbsCapability rgpCol, out uint pbFetched);
	void Skip([In] uint celt);
	void Reset();
	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumCbsCapability __MIDL__IEnumCbsCapability0000);
}
