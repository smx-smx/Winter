using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[ComImport]
[Guid("F112757A-565B-4260-BD05-9FA34417349A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ICbsSession10 : ICbsSession8
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
	new void AddSource([In] uint options, [In][MarshalAs(UnmanagedType.LPWStr)] string basePath);
	new void RegisterCbsUIHandler([In][MarshalAs(UnmanagedType.Interface)] ICbsUIHandler pUIHandler);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICbsPackage CreateWindowsUpdatePackage([In] uint __MIDL__ICbsSession100000, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100001, [In] Guid __MIDL__ICbsSession100002, [In] uint __MIDL__ICbsSession100003, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsPackageType")] _CbsPackageType __MIDL__ICbsSession100004, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100005, [In][MarshalAs(UnmanagedType.LPWStr)] string __MIDL__ICbsSession100006, [In] uint __MIDL__ICbsSession100007, [In][ComAliasName("Smx.Winter.Cbs.Native.tagCbsPackageDecryptionData")] ref tagCbsPackageDecryptionData __MIDL__ICbsSession100008, [In][ComAliasName("Smx.Winter.Cbs.Native.tagCbsPackageEncryptionEnum")] tagCbsPackageEncryptionEnum __MIDL__ICbsSession100009);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsCapability EnumerateCapabilities([In][ComAliasName("Smx.Winter.Cbs.Native._CbsCapabilitySourceFilter")] _CbsCapabilitySourceFilter sourceFilter, [In][MarshalAs(UnmanagedType.LPWStr)] string szNamespace, [In][MarshalAs(UnmanagedType.LPWStr)] string szLang, [In][MarshalAs(UnmanagedType.LPWStr)] string szArch, [In] uint dwMajor, [In] uint dwMinor);
	void InitializeEx([In] uint sessionOptions, [In][MarshalAs(UnmanagedType.LPWStr)] string sourceName, [In][MarshalAs(UnmanagedType.LPWStr)] string bootDrive, [In][MarshalAs(UnmanagedType.LPWStr)] string winDir, [In][MarshalAs(UnmanagedType.LPWStr)] string externalDir);
	[return: MarshalAs(UnmanagedType.Interface)]
	ICSIExternalTransformerExecutor CreateExternalTransformerExecutor();
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsSession ObserveSessions([In] uint options, [In][MarshalAs(UnmanagedType.Interface)] ICbsSessionObserverListener pListener);
	[return: MarshalAs(UnmanagedType.Interface)]
	IEnumCbsActivity GetActivities([In] long options);
	void SetEnhancedOptions([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionEnhancedOption")] _CbsSessionEnhancedOption enhancedOptions);
	void SetProperty([In][ComAliasName("Smx.Winter.Cbs.Native._CbsSessionConfigurableProperty")] _CbsSessionConfigurableProperty prop, [In][MarshalAs(UnmanagedType.LPWStr)] string value);
	void PerformOperation([In] uint reserved, [In][ComAliasName("Smx.Winter.Cbs.Native._CbsOperationType")] _CbsOperationType type);
	void SetClientToken([In] long token);
}
