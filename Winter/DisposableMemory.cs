#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
﻿using System.Runtime.InteropServices;

namespace Smx.Winter;

public enum DisposableMemoryKind
{
    HGlobal = 0,
    Native = 1
}

public class DisposableMemory : IDisposable
{
    private readonly nint handle;
    private readonly nint size;
    private readonly DisposableMemoryKind kind;
    private DisposableMemory(nint handle, nint size, DisposableMemoryKind kind)
    {
        this.handle = handle;
        this.size = size;
        this.kind = kind;
    }

    public void Dispose()
    {
        switch (kind)
        {
            case DisposableMemoryKind.HGlobal:
                Marshal.FreeHGlobal(handle);
                break;
            case DisposableMemoryKind.Native:
                unsafe
                {
                    NativeMemory.Free(handle.ToPointer());
                }
                break;
        }
    }

    public Span<byte> AsSpan()
    {
        Span<byte> span;
        unsafe
        {
            span = new Span<byte>(handle.ToPointer(), (int)size);
        }
        return span;
    }

    public nint Value => handle;
    public nint Size => size;

    public void Realloc(nint size)
    {
        switch (kind)
        {
            case DisposableMemoryKind.HGlobal:
                Marshal.ReAllocHGlobal(handle, size);
                break;
            case DisposableMemoryKind.Native:
                unsafe
                {
                    NativeMemory.Realloc(handle.ToPointer(), (nuint)size);
                }
                break;
        }
    }

    public static DisposableMemory AllocHGlobal(nint size)
    {
        var hMem = Marshal.AllocHGlobal(size);
        return new DisposableMemory(hMem, size, DisposableMemoryKind.HGlobal);
    }

    public static DisposableMemory AllocNative(nint size)
    {
        nint hMem;
        unsafe
        {
            hMem = new nint(NativeMemory.AllocZeroed((nuint)size));
        }
        return new DisposableMemory(hMem, size, DisposableMemoryKind.Native);
    }
}
