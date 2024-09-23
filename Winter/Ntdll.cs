using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Windows.Win32.Foundation;

namespace Smx.Winter;

public enum PROCESSINFOCLASS
{
	ProcessBasicInformation = 0,
	ProcessAccessToken = 9
}

public struct PROCESS_ACCESS_TOKEN
{
	public HANDLE Token;
	public HANDLE Thread;
}

public class Ntdll
{
	[DllImport("ntdll.dll", SetLastError = true)]
	public static extern int NtSetInformationProcess(
		SafeProcessHandle hProcess,
		PROCESSINFOCLASS processInformationClass,
		nint processInformation,
		int processInformationLength);
}