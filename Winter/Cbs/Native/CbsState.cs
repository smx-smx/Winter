namespace Smx.Winter.Cbs.Native;

public enum CbsState
{
	CbsStateResolvedInvalid = -32,
	CbsStateStagedInvalid = -64,
	CbsStateInstalledInvalid = -112,
	CbsStatePermanentInvalid = -128,
	CbsStateAbsent = 0,
	CbsStateUninstallPending = 5,
	CbsStateResolving = 16,
	CbsStateResolved = 32,
	CbsStateStaging = 48,
	CbsStateStaged = 64,
	CbsStateSuperseded = 80,
	CbsStateInstallPending = 96,
	CbsStatePartiallyInstalled = 101,
	CbsStateInstalled = 112,
	CbsStatePermanent = 128,
	CbsStateDefault = 4096,
	CbsStateDetect = 4100,
	CbsStateCanceled = 4101
}
