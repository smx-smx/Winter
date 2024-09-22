namespace Smx.Winter.Cbs.Native;

public enum _CbsInstallState
{
	CbsInstallStatePartiallyInstalled = -19,
	CbsInstallStateCancel = -18,
	CbsInstallStateSuperseded = -17,
	CbsInstallStateDefault = -16,
	CbsInvalidStatePermanent = -8,
	CbsInvalidStateInstalled = -7,
	CbsInvalidStateStaged = -4,
	CbsInvalidStateResolved = -2,
	CbsInstallStateUnknown = -1,
	CbsInstallStateAbsent = 0,
	CbsInstallStateResolving = 1,
	CbsInstallStateResolved = 2,
	CbsInstallStateStaging = 3,
	CbsInstallStateStaged = 4,
	CbsInstallStateUninstallRequested = 5,
	CbsInstallStateInstallRequested = 6,
	CbsInstallStateInstalled = 7,
	CbsInstallStatePermanent = 8,
	CbsInstallStateInvalid = int.MaxValue
}
