using System.Runtime.InteropServices;
using System.Text;

namespace Smx.Winter
{

    public partial class ComponentStoreService
    {
        public struct LUNICODE_STRING
        {
            public ulong Length;
            public ulong MaximumLength;
            public nint Buffer;

            public static DisposableMemory CreateFromString(string str, out LUNICODE_STRING value)
            {
                var memoryHandle = DisposableMemory.AllocNative(Encoding.Unicode.GetByteCount(str));
                Marshal.Copy(Encoding.Unicode.GetBytes(str), 0, memoryHandle.Value, (int)memoryHandle.Size);
                value = new LUNICODE_STRING
                {
                    Buffer = memoryHandle.Value,
                    Length = (ulong)memoryHandle.Size,
                    MaximumLength = (ulong)memoryHandle.Size
                };
                return memoryHandle;
            }
        }
    }
}
