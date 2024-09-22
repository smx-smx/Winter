using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("75207395-23F2-4396-85F0-8FDB879ED0ED")]
public interface IEnumCbsUpdate
{
	void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICbsUpdate rgpCol, out uint pbFetched);
	void Skip([In] uint celt);
	void Reset();
	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumCbsUpdate __MIDL__IEnumCbsUpdate0000);
}
