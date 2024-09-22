using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("75207396-23F2-4396-85F0-8FDB879ED0ED")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsIdentity
{
	void Clear();
	int IsNull();
	int IsEqual([In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pIdentComp);
	void LoadFromAttributes([In][MarshalAs(UnmanagedType.LPWStr)] string szName, [In][MarshalAs(UnmanagedType.LPWStr)] string szPublicKeyToken, [In][MarshalAs(UnmanagedType.LPWStr)] string szProcessor, [In][MarshalAs(UnmanagedType.LPWStr)] string szLanguage, [In][MarshalAs(UnmanagedType.LPWStr)] string szVersion);
	void LoadFromStringId([In][MarshalAs(UnmanagedType.LPWStr)] string szID);
	[return: MarshalAs(UnmanagedType.LPWStr)]
	string GetStringId();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	string SaveAsStringId();
	int InternalIsEqualbyAttribute([In][MarshalAs(UnmanagedType.LPWStr)] string szName, [In][MarshalAs(UnmanagedType.LPWStr)] string szPublicKeyToken, [In][MarshalAs(UnmanagedType.LPWStr)] string szProcessor, [In][MarshalAs(UnmanagedType.LPWStr)] string szLanguage, [In][MarshalAs(UnmanagedType.LPWStr)] string szVersion);
}
