using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("9EEAE39F-E52B-4F18-9C14-F827BB3BAF0F")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsCapability
{
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsIdentity GetIdentity();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	string GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageProperty")] _CbsPackageProperty __MIDL__ICbsCapability0000);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsUpdate EnumerateUpdates([In][ComAliasName("Smx.Winter.Cbs.Native._CbsApplicability")] _CbsApplicability pApplicability, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsSelectability")] _CbsSelectability pSelectability);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsUpdate GetUpdate([In][MarshalAs(UnmanagedType.LPWStr)] string szUpdName);
	void AddSource([In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	void RemoveSource([In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumString EnumerateSources();
	void EvaluateApplicability([In] uint option, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pApplicabilityState, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pCurrentState);
	void InitiateChanges([In] uint installOptions, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] _CbsInstallState targetState, [In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	void Status([ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pProgressState, out uint pLastError);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumString ResourcesToCheck([In][ComAliasName("Smx.Winter.Cbs.Native._CbsResourceType")] _CbsResourceType resType);
	void GetCapability([MarshalAs(UnmanagedType.LPWStr)] out string pszNamespace, [MarshalAs(UnmanagedType.LPWStr)] out string pszLang, [MarshalAs(UnmanagedType.LPWStr)] out string pszArch, out uint dwVerMajor, out uint dwVerMinor);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsCapability GetDependencies();
	void GetSources(out uint __MIDL__ICbsCapability0002);
	uint GetDownloadSize();
	uint GetInstallSize();
	[return: ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")]
	_CbsInstallState GetInstallState();
	void GetOwnerInformation([In] uint reserved, out int __MIDL__ICbsCapability0003, out uint __MIDL__ICbsCapability0004, [MarshalAs(UnmanagedType.LPWStr)] out string __MIDL__ICbsCapability0005);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsFeaturePackage EnumerateFeaturePackages();
}
