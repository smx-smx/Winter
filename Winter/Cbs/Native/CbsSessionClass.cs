using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[TypeLibType(TypeLibTypeFlags.FCanCreate)]
[ClassInterface(ClassInterfaceType.None)]
[Guid("752073A1-23F2-4396-85F0-8FDB879ED0ED")]
public class CbsSessionClass : ICbsSession, CbsSession, ICbsSession7, ICbsSession8, ICbsSession10
{
	public virtual extern void Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir);
	public virtual extern void Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	public virtual extern void EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsIdentity CreateCbsIdentity();
	public virtual extern void ICbsSession7_Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir);
	public virtual extern void ICbsSession7_Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession7_CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession7_OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	public virtual extern void ICbsSession7_EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsIdentity ICbsSession7_CreateCbsIdentity();
	public virtual extern void GetStatus(out uint pCurrentPhase, [ComAliasName("Smx.Winter.Cbs.Native._CbsSessionState")] out _CbsSessionState pLastSuccessfulSessionState, out int pbCompleted, [MarshalAs(UnmanagedType.Error)] out int phrStatus);
	public virtual extern void Resume([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string GetSessionId();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionProperty")] _CbsSessionProperty prop);
	public virtual extern void AddPhaseBreak();
	[return: ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")]
	public virtual extern _CbsRequiredAction FinalizeEx([In] uint __MIDL__ICbsSession70000);
	public virtual extern void ICbsSession8_Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir);
	public virtual extern void ICbsSession8_Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession8_CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession8_OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	public virtual extern void ICbsSession8_EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsIdentity ICbsSession8_CreateCbsIdentity();
	public virtual extern void ICbsSession8_GetStatus(out uint pCurrentPhase, [ComAliasName("Smx.Winter.Cbs.Native._CbsSessionState")] out _CbsSessionState pLastSuccessfulSessionState, out int pbCompleted, [MarshalAs(UnmanagedType.Error)] out int phrStatus);
	public virtual extern void ICbsSession8_Resume([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string ICbsSession8_GetSessionId();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string ICbsSession8_GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionProperty")] _CbsSessionProperty prop);
	public virtual extern void ICbsSession8_AddPhaseBreak();
	[return: ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")]
	public virtual extern _CbsRequiredAction ICbsSession8_FinalizeEx([In] uint __MIDL__ICbsSession70000);
	public virtual extern void AddSource([In] uint options, [In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	public virtual extern void RegisterCbsUIHandler([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	public virtual extern void ICbsSession10_Initialize([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionOption")] _CbsSessionOption sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string clientID, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir);
	public virtual extern void ICbsSession10_Finalize([ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")] out _CbsRequiredAction pRequiredAction);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession10_CreatePackage([In] uint options, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType packageType, [In][MarshalAs(UnmanagedType.LPWStr)] string szPkgPath, [In][MarshalAs(UnmanagedType.LPWStr)] string szSandboxPath);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage ICbsSession10_OpenPackage([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsIdentity pPackageIdentity, [In][MarshalAs(UnmanagedType.LPWStr)] string unkArgAboutLog);
	public virtual extern void ICbsSession10_EnumeratePackages([In] uint options, [MarshalAs(UnmanagedType.Interface)] out IEnumCbsIdentity pPackageList);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsIdentity ICbsSession10_CreateCbsIdentity();
	public virtual extern void ICbsSession10_GetStatus(out uint pCurrentPhase, [ComAliasName("Smx.Winter.Cbs.Native._CbsSessionState")] out _CbsSessionState pLastSuccessfulSessionState, out int pbCompleted, [MarshalAs(UnmanagedType.Error)] out int phrStatus);
	public virtual extern void ICbsSession10_Resume([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string ICbsSession10_GetSessionId();
	[return: MarshalAs(UnmanagedType.LPWStr)]
	public virtual extern string ICbsSession10_GetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionProperty")] _CbsSessionProperty prop);
	public virtual extern void ICbsSession10_AddPhaseBreak();
	[return: ComAliasName("Smx.Winter.Cbs.Native._CbsRequiredAction")]
	public virtual extern _CbsRequiredAction ICbsSession10_FinalizeEx([In] uint __MIDL__ICbsSession70000);
	public virtual extern void ICbsSession10_AddSource([In] uint options, [In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	public virtual extern void ICbsSession10_RegisterCbsUIHandler([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICbsPackage CreateWindowsUpdatePackage([In] uint __MIDL__ICbsSession100000, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100001, [In] Guid __MIDL__ICbsSession100002, [In] uint __MIDL__ICbsSession100003, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType __MIDL__ICbsSession100004, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100005, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100006, [In] uint __MIDL__ICbsSession100007, [In][ComAliasName("Smx.Winter.Cbs.Native.tagCbsPackageDecryptionData")] ref tagCbsPackageDecryptionData __MIDL__ICbsSession100008, [In][ComAliasName("Smx.Winter.Cbs.Native.tagCbsPackageEncryptionEnum")] tagCbsPackageEncryptionEnum __MIDL__ICbsSession100009);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern IEnumCbsCapability EnumerateCapabilities([In][ComAliasName("Smx.Winter.Cbs.Native._CbsCapabilitySourceFilter")] _CbsCapabilitySourceFilter sourceFilter, [In][MarshalAs(UnmanagedType.LPWStr)] string szNamespace, [In][MarshalAs(UnmanagedType.LPWStr)] string szLang, [In][MarshalAs(UnmanagedType.LPWStr)] string szArch, [In] uint dwMajor, [In] uint dwMinor);
	public virtual extern void InitializeEx([In] uint sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string sourceName, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir, [In][MarshalAs(UnmanagedType.LPWStr)] string externalDir);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern ICSIExternalTransformerExecutor CreateExternalTransformerExecutor();
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern IEnumCbsSession ObserveSessions([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsSessionObserverListener pListener);
	[return: MarshalAs(UnmanagedType.Interface)]
	public virtual extern IEnumCbsActivity GetActivities([In] long options);
	public virtual extern void SetEnhancedOptions([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionEnhancedOption")] _CbsSessionEnhancedOption enhancedOptions);
	public virtual extern void SetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionConfigurableProperty")] _CbsSessionConfigurableProperty prop, [In][MarshalAs(UnmanagedType.LPWStr)] string value);
	public virtual extern void PerformOperation([In] uint reserved, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsOperationType")] _CbsOperationType type);
	public virtual extern void SetClientToken([In] long token);
}
