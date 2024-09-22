using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("00000101-0000-0000-C000-000000000046")]
public interface IEnumString
{
	void RemoteNext([In] uint celt, [MarshalAs(UnmanagedType.LPWStr)] out string rgelt, out uint pceltFetched);
	void Skip([In] uint celt);
	void Reset();
	void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumString ppenum);
}
