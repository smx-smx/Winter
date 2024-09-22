namespace Smx.Winter.Cbs.Native;

public enum _CbsOperationStage
{
	CbsOperationStageWaiting = 1,
	CbsOperationStagePlanning = 5,
	CbsOperationStageDownloading = 15,
	CbsOperationStageExtracting = 20,
	CbsOperationStageResolving = 25,
	CbsOperationStageStaging = 30,
	CbsOperationStageInstalling = 40,
	CbsOperationStageInstallingEx = 50,
	CbsOperationStageReservicingLCU = 60
}
