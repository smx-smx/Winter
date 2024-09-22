#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO.Memory;
using System.Runtime.CompilerServices;
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

            public override string ToString()
            {
                if(Buffer == 0 || Length < 1)
                {
                    return "";
                }
                return Marshal.PtrToStringUni(Buffer, (int)Length);
            }

            public static NativeMemoryHandle CreateFromString(string str, out LUNICODE_STRING value)
            {
                var memoryHandle = MemoryNative.Alloc(Encoding.Unicode.GetByteCount(str));
                Marshal.Copy(Encoding.Unicode.GetBytes(str), 0, memoryHandle.Address, (int)memoryHandle.Size);
                value = new LUNICODE_STRING
                {
                    Buffer = memoryHandle.Address,
                    Length = (ulong)memoryHandle.Size,
                    MaximumLength = (ulong)memoryHandle.Size
                };
                return memoryHandle;
            }
        }
    }
}
