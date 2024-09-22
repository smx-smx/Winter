using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("75207397-23F2-4396-85F0-8FDB879ED0ED")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumCbsIdentity
{
	void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICbsIdentity rgpCol, out uint pbFetched);
	void Skip([In] uint celt);
	void Reset();
	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity __MIDL__IEnumCbsIdentity0000);
}
