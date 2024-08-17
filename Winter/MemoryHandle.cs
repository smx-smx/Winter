#region License
/*
 * Copyright (c) 2024 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using Smx.SharpIO;
using Smx.Winter.MsDelta;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Smx.Winter.MemoryAllocator;

namespace Smx.Winter;

public enum MemoryHandleType
{
    HGlobal = 0,
    Native = 1,
    Custom = 2
}

public record struct TypedPointer<T>(nint Address) where T : struct
{
    public ref T Value
    {
        get
        {
            unsafe
            {
                return ref Unsafe.AsRef<T>(Address.ToPointer());
            }
        }
    }
}

public class TypedMemoryHandle<T> : IDisposable where T : struct
{
    public MemoryHandle Memory { get; private set; }
    public TypedPointer<T> Pointer;

    private T tmpValue = default;

    public ref T Value {
        get {
            if (_isPacked)
            {
                return ref tmpValue;
            } else
            {
                return ref Pointer.Value;
            }
        }
    }

    public nint Address => Pointer.Address;

    private readonly bool _isPacked;

    public void Write()
    {
        if (!_isPacked) return;
        Marshal.StructureToPtr<T>(Value, Memory.Address, false);
    }

    public TypedMemoryHandle(MemoryHandle memory)
    {
        var structLayout = typeof(T).StructLayoutAttribute;
        _isPacked = structLayout != null && structLayout.Pack < nint.Size;

        Memory = memory;
        Pointer = new TypedPointer<T>(Memory.Address);
    }

    public void Dispose()
    {
        Memory.Dispose();
    }
}

public class NativePointerList<T> where T : struct
{
    private List<nint> array;

    public NativePointerList(int? initialCapacity = null)
    {
        array = (initialCapacity != null)
            ? new List<nint>(initialCapacity.Value)
            : new List<nint>();
    }

    public void Add(TypedPointer<T> ptr)
    {
        array.Add(ptr.Address);
    }
    public void DangerousAdd(nint ptr)
    {
        array.Add(ptr);
    }

    public MemoryHandle Build()
    {
        var mem = MemoryHandle.AllocNative(nint.Size * this.array.Count);
        for(int i=0; i<array.Count; i++)
        {
            Marshal.WriteIntPtr(mem.Address + (nint.Size * i), array[i]);
        }
        return mem;
    }
}


public class MemoryAllocator
{
    private pfnAlloc allocator;
    private pfnFree deleter;

    public delegate nint pfnAlloc(nint size);
    public delegate void pfnFree(nint handle, nint size);

    public MemoryAllocator(pfnAlloc allocator, pfnFree deleter)
    {
        this.allocator = allocator;
        this.deleter = deleter;
    }

    public TypedMemoryHandle<T> Alloc<T>(nint? size = null) where T : struct
    {
        var allocSize = (size == null) ? Unsafe.SizeOf<T>() : size.Value;
        return new TypedMemoryHandle<T>(Alloc(allocSize));
    }

    public MemoryHandle Alloc(nint size, bool owned = true)
    {
        var mem = this.allocator(size);
        mem.AsSpan<byte>((int)size).Clear();
        return new MemoryHandle(mem, size, MemoryHandleType.Custom, owned: owned, pfnFree: this.deleter);
    }
}

public class MemoryHandle : IDisposable
{
    private nint handle;
    private nint size;
    private readonly MemoryHandleType kind;
    private pfnFree? freeFn;
    private readonly bool owned;
    private bool disposed = false;

    public MemoryHandle(nint handle, nint size, MemoryHandleType kind, bool owned = true, pfnFree ? pfnFree = null)
    {
        this.handle = handle;
        this.size = size;
        this.kind = kind;
        this.owned = owned;
        if(kind == MemoryHandleType.Custom)
        {
            ArgumentNullException.ThrowIfNull(pfnFree);
        }
        this.freeFn = pfnFree;
    }

    public void Dispose()
    {
        if (this.disposed) return;
        this.disposed = true;
        if (!owned) return;

        switch (kind)
        {
            case MemoryHandleType.HGlobal:
                Marshal.FreeHGlobal(handle);
                break;
            case MemoryHandleType.Native:
                unsafe
                {
                    NativeMemory.Free(handle.ToPointer());
                }
                break;
            case MemoryHandleType.Custom:
                ArgumentNullException.ThrowIfNull(this.freeFn);
                this.freeFn(this.handle, this.size);
                break;
        }
    }

    public Span<T> GetSpan<T>(int offset = 0, int index = 0) where T : unmanaged
    {
        return Span.Slice(offset)
            .Cast<T>()
            .Slice(index);
    }

    public Span<byte> Span
    {
        get
        {
            Span<byte> span;
            unsafe
            {
                span = new Span<byte>(handle.ToPointer(), (int)size);
            }
            return span;
        }
    }

    public nint Address => handle;
    public nint Size => size;

    public void Realloc(nint size)
    {
        nint newHandle = 0;
        switch (kind)
        {
            case MemoryHandleType.HGlobal:
                newHandle = Marshal.ReAllocHGlobal(handle, size);
                if(newHandle == 0)
                {
                    throw new InvalidOperationException("realloc failed");
                }
                this.handle = newHandle;
                this.size = size;
                break;
            case MemoryHandleType.Native:
                unsafe
                {
                    newHandle = new nint(NativeMemory.Realloc(handle.ToPointer(), (nuint)size));
                }
                if(newHandle == 0)
                {
                    throw new InvalidOperationException("realloc failed");
                }
                this.handle = newHandle;
                this.size = size;
                break;
            case MemoryHandleType.Custom:
                throw new NotImplementedException();
        }
    }

    public static TypedMemoryHandle<T> AllocHGlobal<T>(nint? size = null, bool owned = true) where T : struct
    {
        var allocSize = (size == null) ? Unsafe.SizeOf<T>() : size.Value;
        return new TypedMemoryHandle<T>(AllocHGlobal(allocSize, owned: owned));
    }

    public static TypedMemoryHandle<T> AllocNative<T>(nint? size = null, bool owned = true) where T : struct
    {
        var allocSize = (size == null) ? Unsafe.SizeOf<T>() : size.Value;
        return new TypedMemoryHandle<T>(AllocNative(allocSize, owned: owned));
    }

    public static MemoryHandle AllocHGlobal(nint size, bool owned = true)
    {
        var hMem = Marshal.AllocHGlobal(size);
        return new MemoryHandle(hMem, size, MemoryHandleType.HGlobal);
    }

    public static MemoryHandle AllocNative(nint size, bool owned = true)
    {
        nint hMem;
        unsafe
        {
            hMem = new nint(NativeMemory.AllocZeroed((nuint)size));
        }
        return new MemoryHandle(hMem, size, MemoryHandleType.Native, owned: owned);
    }
}
