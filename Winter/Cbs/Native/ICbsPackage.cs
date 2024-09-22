using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("75207393-23F2-4396-85F0-8FDB879ED0ED")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsPackage
{
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsIdentity GetIdentity();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	string GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageProperty")] _CbsPackageProperty prop);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsUpdate EnumerateUpdates([In][ComAliasName("Smx.Winter.Cbs.Native._CbsApplicability")] _CbsApplicability pApplicability, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsSelectability")] _CbsSelectability pSelectability);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsUpdate GetUpdate([In][MarshalAs(UnmanagedType.LPWStr)] string szUpdName);
	void AddSource([In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	void RemoveSource([In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumString EnumerateSources();
	void EvaluateApplicability(uint option, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pApplicabilityState, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pCurrentState);
	void InitiateChanges([In] uint installOptions, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] _CbsInstallState targetState, [In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	void Status([ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pProgressState, out uint pLastError);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumString ResourcesToCheck([In][ComAliasName("Smx.Winter.Cbs.Native._CbsResourceType")] _CbsResourceType resType);
}
