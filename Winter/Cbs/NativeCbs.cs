namespace Smx.Winter.Cbs
{
    public struct FOUR_PART_VERSION
    {
        public ushort revision;
        public ushort build;
        public ushort minor;
        public ushort major;
    }


    public enum ProcessorArchitectureType
    {
        INTEL = 0,
        MIPS = 1,
        ALPHA = 2,
        PPC = 3,
        SHX = 4,
        ARM = 5,
        IA64 = 6,
        ALPHA64 = 7,
        MSIL = 8,
        AMD64 = 9,
        IA32_ON_WIN64 = 10,
        NEUTRAL = 11,
        ARM64 = 12,
        ARM32_ON_WIN64 = 13,
        IA32_ON_ARM64 = 14,
    }

    public class NativeCbs : IDisposable
    {
        public NativeServicingStackShim StackShim {  get; private set; }

        public void Dispose()
        {
            StackShim.Dispose();
        }

        public NativeCbs(string? winDir = null)
        {
            var shimPath = winDir == null
                ? null
                : Path.Combine(winDir, "System32", "SSShim.dll");

            StackShim = new NativeServicingStackShim(shimPath);
        }
    }
}
