using Smx.SharpIO.Memory;
using Smx.Winter.MsDelta;

namespace Smx.Winter
{
    internal static class MemoryNative
    {
        public static readonly MemoryAllocator Allocator = new MemoryAllocator(new NativeMemoryManager());

        public static TypedMemoryHandle<T> Alloc<T>(nuint? size = null) where T : unmanaged
        {
            return Allocator.Alloc<T>(size);
        }

        public static NativeMemoryHandle Alloc(nuint size, bool owned = true)
        {
            return Allocator.Alloc(size, owned);
        }

        public static NativeMemoryHandle Alloc(nint size, bool owned = true)
        {
            return Alloc((nuint)size, owned);
        }
    }
}
