using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("75207391-23F2-4396-85F0-8FDB879ED0ED")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsSession
{
	void Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string? bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string? winDir);
	void Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsPackage CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsPackage OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	void EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsIdentity CreateCbsIdentity();
}
