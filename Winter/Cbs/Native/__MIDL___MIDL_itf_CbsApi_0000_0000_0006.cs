namespace Smx.Winter.Cbs.Native;

public enum __MIDL___MIDL_itf_CbsApi_0000_0000_0006
{
	CbsSessionOptionNone = 0,
	CbsSessionOptionProcessChangesPostReboot = 64,
	CbsSessionOptionLoadPersisted = 128,
	CbsSessionOptionDoScavenge = 1024,
	CbsSessionOptionCancelAllPendedTransactions = 2048,
	CbsSessionOptionEnableCompression = 8192,
	CbsSessionOptionDisableCompression = 16384,
	CbsSessionOptionDetectAndRepairStoreCorruption = 49152,
	CbsSessionOptionNoPend = 65536,
	CbsSessionOptionUseLocalSourceOnly = 131072,
	CbsSessionOptionUseWindowsUpdate = 262144,
	CbsSessionOptionReportStackInfo = 1048576,
	CbsSessionOptionDoSynchronousCleanup = 4194304,
	CbsSessionOptionDoAsynchronousCleanup = 8388608,
	CbsSessionOptionDelayExecutionIfPendRequired = 33554432,
	CbsSessionOptionAnalyzeComponentStore = 134217728,
	CbsSessionOptionCancelOnlySmartPendedTransactions = int.MinValue
}
