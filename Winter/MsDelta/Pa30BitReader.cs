#region License
/*
 * Copyright (c) 2026 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Smx.Winter.MsDelta;

/// <summary>
/// Bit-level reader for the PA30 binary format. Replicates the native msdelta BitReaderObject.
/// Reads bits from a byte stream, maintaining a 64-bit buffer with 32-bit refills.
/// </summary>
/// <remarks>
/// The native BitReaderObject::Init calls Seek(0) followed by Read(3),
/// pre-consuming 3 bits. This is replicated in the constructor.
/// </remarks>
public class Pa30BitReader
{
    private readonly byte[] _data;
    private int _bytePos;
    private readonly int _byteEnd;
    private ulong _buffer;
    private int _availableBits;

    /// <summary>
    /// De Bruijn lookup table for lowest-set-bit detection.
    /// Indexed by (bit-spread word) % 37. Value 16 is the ReadVarInt empty
    /// sentinel; 32 marks no set bit (only reachable for a zero word, which the
    /// callers preclude by forcing bit 16 (varint) / bit 31 (Huffman) / nonzero
    /// buffer (ReadNumber) before the lookup).
    /// </summary>
    private static readonly byte[] NibbleLookup =
    [
        32, 27, 23,  2, 26,  1,  0, 32, 18, 19,
         8, 20,  5,  9, 14, 21, 32,  6, 12, 10,
        29, 15, 31, 22, 25, 32, 17,  7,  4, 13,
        32, 11, 28, 30, 24, 16,  3
    ];

    /// <summary>
    /// Creates a BitReader starting at the given byte offset. Optionally pre-consumes
    /// 3 bits to replicate BitReaderObject::Init → Read(this, 3u).
    /// Every native BitReaderObject construction runs Init, so all stream readers
    /// (header, inner patch stream, decompression data) use preConsume3: true;
    /// only manual bit-position arithmetic uses false.
    /// </summary>
    public Pa30BitReader(byte[] data, int offset, int length, bool preConsume3 = true)
    {
        _data = data;
        _bytePos = offset;
        _byteEnd = offset + length;
        _buffer = 0;
        _availableBits = 0;
        Refill();
        if (preConsume3)
            Read(3);
    }

    public Pa30BitReader(byte[] data, bool preConsume3 = true) : this(data, 0, data.Length, preConsume3) { }

    public int BytePosition => _bytePos;
    public int AvailableBits => _availableBits;
    // Raw 64-bit buffer for diagnostics (low 32 bits = next bits in LSB-first order).
    public ulong DebugBuf => _buffer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Read(int nBits)
    {
        if (nBits == 0) return 0;
        if (nBits > _availableBits)
            throw new InvalidOperationException(
                $"Read: need {nBits} bits, have {_availableBits}");

        var result = (uint)(_buffer & ((1UL << nBits) - 1));
        _buffer >>= nBits;
        _availableBits -= nBits;

        if (_availableBits < 32)
            Refill();

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Peek(int nBits)
    {
        if (nBits > _availableBits)
            throw new InvalidOperationException(
                $"Peek: need {nBits} bits, have {_availableBits}");
        return (uint)(_buffer & ((1UL << nBits) - 1));
    }

    /// <summary>
    /// Returns the low 32 bits of the buffer without any availability check.
    /// Replicates the native Huffman Decode lookup, which reads the buffer word
    /// directly (forcing bit 31) and only validates the decoded symbol's length
    /// against available bits afterwards. Required at stream end where fewer
    /// than 32 bits remain (high bits are zero after shifts, same as native).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint PeekRaw32() => (uint)_buffer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Skip(int nBits)
    {
        if (nBits > _availableBits)
            throw new InvalidOperationException(
                $"Skip: need {nBits} bits, have {_availableBits}");
        _buffer >>= nBits;
        _availableBits -= nBits;
        if (_availableBits < 32)
            Refill();
    }

    /// <summary>
    /// Reads a number in the wider VarInt variant.
    /// Skips extraNibbles+1 header bits, then reads extraNibbles+8 value bits,
    /// returning (1UL &lt;&lt; (extraNibbles+8)) | value. Used for RiftTable deltas
    /// and extended match lengths.
    /// </summary>
    public ulong ReadNumber()
    {
        var buf32 = (uint)_buffer;
        if (buf32 == 0)
            throw new InvalidOperationException("ReadNumber: no bits available");

        var w = buf32;
        w = w | (w << 1);
        w = w | (w << 2);
        w = w | (w << 4);
        w = w | (w << 8);
        w = w | (w << 16);
        var rem = w % 37u;
        var nibbleCount = NibbleLookup[(int)rem];

        if (31u - (uint)nibbleCount < 8)
            throw new InvalidOperationException($"ReadNumber: invalid nibble count {nibbleCount}");

        Skip(nibbleCount + 1);
        var valueBits = nibbleCount + 8;
        var value = Read(valueBits);
        return (1UL << valueBits) | value;
    }

    /// <summary>
    /// Reads a nibble-encoded variable-length integer.
    /// Replicates the native variable-length integer read.
    /// </summary>
    /// <remarks>
    /// The buffer word is bit-spread to find the lowest set nibble, giving a nibble count
    /// (extraNibbles) via a mod-37 de Bruijn lookup (NibbleLookup table above).
    /// Bit consumption for a count of extraNibbles:
    ///   - extraNibbles+1 header bits first, then the value bits
    ///   - extraNibbles &lt; 8: extraNibbles*4+4 value bits from the shifted buffer
    ///   - extraNibbles ≥ 8: 32 bits + (extraNibbles*4-28) further bits
    /// </remarks>
    public ulong ReadVarInt()
    {
        var buf32 = (uint)_buffer;
        var w = buf32 | 0x10000u;
        w = w | (w << 1);
        w = w | (w << 2);
        w = w | (w << 4);
        w = w | (w << 8);
        w = w | (w << 16);
        var rem = w % 37u;
        var extraNibbles = NibbleLookup[(int)rem];

        if (extraNibbles == 16)
            throw new InvalidOperationException("ReadVarInt: no bits available");

        var nBits = extraNibbles + 1;
        if (nBits > _availableBits)
            throw new InvalidOperationException($"ReadVarInt: need {nBits} header bits, have {_availableBits}");

        _buffer >>= nBits;
        _availableBits -= nBits;

        if (extraNibbles >= 8)
        {
            if (_availableBits < 32) Refill();
            var lo = Read(32);
            var extraBits = extraNibbles * 4 - 28;
            var hi = extraBits > 0 ? Read(extraBits) : 0u;
            return lo | ((ulong)hi << 32);
        }

        var nibbleCount = extraNibbles * 4;
        var valueBits = nibbleCount + 4;
        if (valueBits > _availableBits)
            throw new InvalidOperationException($"ReadVarInt: need {valueBits} bits, have {_availableBits}");

        var value = _buffer & ((1UL << valueBits) - 1);
        _buffer >>= valueBits;
        _availableBits -= valueBits;

        if (_availableBits < 32)
            Refill();

        return value;
    }

    /// <summary>
    /// Aligns the stream up to the next byte boundary, replicating the native
    /// BitReaderObject::ReadBuffer behavior (avail masked down to a multiple of 8).
    /// </summary>
    public void AlignToByte()
    {
        var drop = _availableBits % 8;
        if (drop > 0)
            Skip(drop);
    }

    public byte[] ReadBuffer()
    {
        var size = (int)ReadVarInt();
        if (size < 0 || size > _availableBits / 8 + (_byteEnd - _bytePos) + 16)
            throw new InvalidOperationException($"ReadBuffer: unreasonable size {size}");

        // Native ReadBuffer aligns to byte boundary after the size varint
        // (see AlignToByte); the content bytes follow at the aligned position.
        AlignToByte();

        var buf = new byte[size];
        for (int i = 0; i < size; i++)
            buf[i] = (byte)Read(8);
        return buf;
    }

    /// <summary>
    /// Reads 4 bytes as a little-endian DWORD and appends them to the 64-bit buffer.
    /// Replicates the native refill in BitReaderObject::Read.
    /// </summary>
    private void Refill()
    {
        if (_bytePos >= _byteEnd)
            return;

        var remaining = (uint)(_byteEnd - _bytePos);
        uint word;
        int bitsRead;
        if (remaining >= 4)
        {
            word = BinaryPrimitives.ReadUInt32LittleEndian(
                _data.AsSpan(_bytePos, 4));
            _bytePos += 4;
            bitsRead = 32;
        }
        else
        {
            word = 0;
            for (int i = 0; i < remaining; i++)
                word |= (uint)_data[_bytePos + i] << (8 * i);
            _bytePos = _byteEnd;
            bitsRead = (int)(remaining * 8);
        }

        _buffer |= (ulong)word << _availableBits;
        _availableBits += bitsRead;
    }
}
