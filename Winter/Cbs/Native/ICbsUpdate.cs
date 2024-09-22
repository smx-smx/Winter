using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("75207394-23F2-4396-85F0-8FDB879ED0ED")]
public interface ICbsUpdate
{
	[return: MarshalAs(UnmanagedType.LPWStr)]
	string GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsUpdateProperty")] _CbsUpdateProperty prop);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsPackage GetPackage();
	void GetParentUpdate([In] uint index, [MarshalAs(UnmanagedType.LPWStr)] out string ppParent, [MarshalAs(UnmanagedType.LPWStr)] out string ppSet);
	void GetCapability([ComAliasName("Smx.Winter.Cbs.Native._CbsApplicability")] out _CbsApplicability pApplicability, [ComAliasName("Smx.Winter.Cbs.Native._CbsSelectability")] out _CbsSelectability pSelectability);
	void GetDeclaredSet([In] uint unk, [MarshalAs(UnmanagedType.LPWStr)] out string pDeclaredSet, [ComAliasName("Smx.Winter.Cbs.Native._CbsCardinality")] out _CbsCardinality Cardinality);
	void GetInstallState([ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pCurrentState, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pIntendedState, [ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] out _CbsInstallState pRequestState);
	void SetInstallState([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] _CbsInstallState requestedState);
	void GetRealInstallState([ComAliasName("Smx.Winter.Cbs.Native.CbsState")] out CbsState pCurrentState, [ComAliasName("Smx.Winter.Cbs.Native.CbsState")] out CbsState pIntendedState, [ComAliasName("Smx.Winter.Cbs.Native.UpdateSelection")] out UpdateSelection pRequestState);
}
