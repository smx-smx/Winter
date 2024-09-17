using Smx.SharpIO.Memory;
using System.Runtime.InteropServices;

namespace Smx.Winter
{
    internal class NativeMemoryManager : IMemoryManager
    {
        public unsafe nint Alloc(nuint size) => new nint(NativeMemory.Alloc(size));
        public unsafe void Free(nint ptr) => NativeMemory.Free(ptr.ToPointer());
        public unsafe nint Realloc(nint ptr, nuint size) => new nint(NativeMemory.Realloc(ptr.ToPointer(), size));
    }
}
