using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("D2F9F360-5BC7-4B1E-AAD5-DD151733C7C8")]
public interface IEnumCbsFeaturePackage
{
	void Next([In] uint celt, [MarshalAs(UnmanagedType.Interface)] out ICbsFeaturePackage rgpCol, out uint pbFetched);
	void Skip([In] uint celt);
	void Reset();
	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumCbsUpdate __MIDL__IEnumCbsFeaturePackage0000);
}
