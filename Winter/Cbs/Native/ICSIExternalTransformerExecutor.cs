using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("1C8ADB85-982E-47F9-999F-B0C3BF9D0449")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICSIExternalTransformerExecutor
{
	void Commit([In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0000, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0001, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0002, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0003);
	void Initialize([In] uint __MIDL__ICSIExternalTransformerExecutor0004, [In] ulong __MIDL__ICSIExternalTransformerExecutor0005, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0006, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0007, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0008);
	void Install([In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0009, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICSIExternalTransformerExecutor0010);
}
