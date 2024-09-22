using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("F568C899-AF4F-4EAA-B12A-B8E5F1B219DE")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsSession8 : ICbsSession7
{
	new void Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir);
	new void Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	new ICbsPackage CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	new ICbsPackage OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	new void EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	new ICbsIdentity CreateCbsIdentity();
	new void GetStatus(out uint pCurrentPhase, [ComAliasName("Smx.Winter.Cbs.Native._CbsSessionState")] out _CbsSessionState pLastSuccessfulSessionState, out int pbCompleted, [MarshalAs(UnmanagedType.Error)] out int phrStatus);
	new void Resume([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.LPWStr)]
	new string GetSessionId();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	new string GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionProperty")] _CbsSessionProperty prop);
	new void AddPhaseBreak();
	[return: ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")]
	new _CbsRequiredAction FinalizeEx([In] uint __MIDL__ICbsSession70000);
	void AddSource([In] uint options, [In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	void RegisterCbsUIHandler([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
}
