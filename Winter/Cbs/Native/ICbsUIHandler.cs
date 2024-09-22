using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("75207392-23F2-4396-85F0-8FDB879ED0ED")]
public interface ICbsUIHandler
{
	void Initiate([In][MarshalAs(UnmanagedType.Interface)] IEnumCbsUpdate pUpdList, [In] ref int __MIDL__ICbsUIHandler0000);
	void Terminate();
	void Error([MarshalAs(UnmanagedType.Error)] int hr, [In][MarshalAs(UnmanagedType.LPWStr)] string szUnk, [In] ref int unkArg);
	void ResolveSource([In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsUIHandler0001, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pIdent, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsUIHandler0002, [In][Out][MarshalAs(UnmanagedType.LPWStr)] ref string __MIDL__ICbsUIHandler0003, out int __MIDL__ICbsUIHandler0004);
	void Progress([In][ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] _CbsInstallState stIns, [In] uint curProg, [In] uint totProg, out int __MIDL__ICbsUIHandler0005);
	void EnteringStage([In] uint __MIDL__ICbsUIHandler0006, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsOperationStage")] _CbsOperationStage __MIDL__ICbsUIHandler0007, [In] int __MIDL__ICbsUIHandler0008, [In] int __MIDL__ICbsUIHandler0009);
	void ProgressEx([In][ComAliasName("Smx.Winter.Cbs.Native._CbsInstallState")] _CbsInstallState stIns, [In] uint curProg, [In] uint totProg, [In] uint __MIDL__ICbsUIHandler0010, out int __MIDL__ICbsUIHandler0011);
}
