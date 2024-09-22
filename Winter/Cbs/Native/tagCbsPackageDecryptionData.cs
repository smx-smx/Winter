using System.Runtime.InteropServices;

namespace Smx.Winter.Cbs.Native;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct tagCbsPackageDecryptionData
{
	public int Data1;

	public int Data2;
}
