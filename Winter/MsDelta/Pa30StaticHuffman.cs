#region License
/*
 * Copyright (c) 2026 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.Runtime.CompilerServices;

namespace Smx.Winter.MsDelta;

/// <summary>
/// Canonical Huffman code table. Holds code lengths and computed code values for a set of symbols.
/// </summary>
public class StaticHuffmanCodes
{
    public int NumCodes { get; }
    public byte[] Lengths { get; }
    public uint[] Codes { get; }
    public int MaxBitLength { get; }

    public StaticHuffmanCodes(int numCodes, int maxBitLength)
    {
        NumCodes = numCodes;
        MaxBitLength = maxBitLength;
        Lengths = new byte[numCodes];
        Codes = new uint[numCodes];
    }

    public void SetLengths(byte[] lengths)
    {
        Array.Copy(lengths, Lengths, Math.Min(lengths.Length, NumCodes));
        CalculateCodes();
    }

    public void SetLengths(ReadOnlySpan<byte> lengths)
    {
        lengths[..Math.Min(lengths.Length, NumCodes)].CopyTo(Lengths.AsSpan());
        CalculateCodes();
    }

    private void CalculateCodes()
    {
        var bitLengthCount = new int[MaxBitLength + 1];
        var nextCode = new uint[MaxBitLength + 1];

        for (int i = 0; i < NumCodes; i++)
        {
            if (Lengths[i] > MaxBitLength)
                throw new InvalidOperationException($"Code length {Lengths[i]} exceeds max {MaxBitLength}");
            if (Lengths[i] > 0)
                bitLengthCount[Lengths[i]]++;
        }

        // Native computes starting codes from HIGHEST bit length to LOWEST.
        // Native assigns nextCode[bits] = code BEFORE folding count[bits] into code:
        //   for (idx = maxBitLength; idx >= 0; idx--) {
        //       nextCode[idx] = code;
        //       code = (count[idx] + code) >> 1;
        //   }
        uint code = 0;
        for (int bits = MaxBitLength; bits >= 1; bits--)
        {
            nextCode[bits] = code;
            code = (code + (uint)bitLengthCount[bits]) >> 1;
        }

        for (int i = 0; i < NumCodes; i++)
        {
            var len = Lengths[i];
            if (len > 0)
            {
                var raw = nextCode[len]++;
                // Native reverses the bits of the code value
                var reversed = 0u;
                for (int b = 0; b < len; b++)
                {
                    reversed = (reversed << 1) | (raw & 1);
                    raw >>= 1;
                }
                Codes[i] = reversed;
            }
            else
                Codes[i] = 0xFFFFFFFF;
        }
    }
}

/// <summary>
/// Multi-level Huffman decoder table with bit-position-based level indexing
/// instead of a flat prefix table.
/// </summary>
public class StaticHuffmanDecoderTable
{
    private readonly StaticHuffmanCodes _codes;
    private readonly ushort[] _table;
    private readonly uint[] _levelBase;
    private readonly uint[] _levelMask;

    private static readonly byte[] LowestLookup =
    // Lowest-set-bit lookup shared with the bit-reader varint paths;
    // indexed by (bit-spread word) % 37.
    [
        32, 27, 23,  2, 26,  1,  0, 32, 18, 19,
         8, 20,  5,  9, 14, 21, 32,  6, 12, 10,
        29, 15, 31, 22, 25, 32, 17,  7,  4, 13,
        32, 11, 28, 30, 24, 16,  3
    ];

    public StaticHuffmanDecoderTable(StaticHuffmanCodes codes)
    {
        _codes = codes;
        (_table, _levelBase, _levelMask) = BuildNativeTable();
    }

    public int TableLength => _table.Length;
    // Raw level/table accessors for diagnostics (--probe-table).
    public uint GetLevelBase(int i) => _levelBase[i];
    public uint GetLevelMask(int i) => _levelMask[i];
    public ushort GetTableEntry(int i) => _table[i];

    private (ushort[] table, uint[] levelBase, uint[] levelMask) BuildNativeTable()
    {
        // Replicates the native decoder-table build.
        var levelMaxExtra = new uint[32];
        int firstCodeZero = _codes.NumCodes; // index of first code==0 symbol (sentinel)
        bool seenZero = false;
        int maxLevel = 0; // highest lowest-set-bit over code!=0 symbols: only levels 0..maxLevel get slots

        // First pass: compute max extra bits per level.
        for (int sym = 0; sym < _codes.NumCodes; sym++)
        {
            var len = _codes.Lengths[sym];
            if (len == 0) continue;

            var code = _codes.Codes[sym];
            if (code == 0xFFFFFFFF) continue;

            // The first code==0 symbol becomes the table sentinel; a second one
            // is rejected, matching the native builder.
            if (code == 0)
            {
                if (seenZero)
                    throw new InvalidOperationException(
                        $"DecoderTable build: second zero code at symbol {sym}");
                seenZero = true;
                firstCodeZero = sym;
                continue;
            }

            // Find lowest set bit via spread + mod-37 lookup
            var w = code;
            w |= (w << 1); w |= (w << 2); w |= (w << 4); w |= (w << 8); w |= (w << 16);
            var lowestBit = LowestLookup[w % 37u];
            if (lowestBit >= 32) continue;

            var extraBits = (int)(len - lowestBit - 1u);
            if (extraBits < 0) continue; // invalid code: bit position exceeds code length
            if (lowestBit > maxLevel) maxLevel = lowestBit;
            if (levelMaxExtra[lowestBit] < (uint)extraBits)
                levelMaxExtra[lowestBit] = (uint)extraBits;
        }

        // Levels 0..maxLevel get table slots; levels above maxLevel point at the total
        // (sentinel slot), so out-of-range bit positions decode to the sentinel.
        var levelBase = new uint[32];
        var levelMask = new uint[32];
        uint offset = 0;
        for (int i = 0; i <= maxLevel; i++)
        {
            var entries = 1u << (int)levelMaxExtra[i];
            levelBase[i] = offset;
            levelMask[i] = entries - 1;
            offset += entries;
        }
        for (int i = maxLevel + 1; i < 32; i++)
        {
            levelBase[i] = offset;
            levelMask[i] = 0;
        }

        // Allocate table with one extra slot for sentinel
        var tableLen = offset > int.MaxValue - 2 ? int.MaxValue : (int)Math.Max(1, offset + 1);
        var table = new ushort[tableLen];

        // Fill with zero (native zero-fills all table slots)
        for (int i = 0; i < tableLen; i++)
            table[i] = 0;

        // Second pass: fill valid symbols (skip code==0)
        for (int sym = 0; sym < _codes.NumCodes; sym++)
        {
            var len = _codes.Lengths[sym];
            if (len == 0) continue;
            var code = _codes.Codes[sym];
            if (code == 0xFFFFFFFF || code == 0) continue;

            var w = code;
            w |= (w << 1); w |= (w << 2); w |= (w << 4); w |= (w << 8); w |= (w << 16);
            var lowestBit = LowestLookup[w % 37u];
            if (lowestBit >= 32) continue;

            var extraBits = (int)(len - lowestBit - 1u);
            if (extraBits < 0) continue;
            var index = levelBase[lowestBit] + ((code >> ((int)lowestBit + 1)) & levelMask[lowestBit]);
            if (index >= tableLen - 1) continue;
            var step = 1u << extraBits;
            var count = 1u << (int)(levelMaxExtra[lowestBit] - (uint)extraBits);

            for (uint j = 0; j < count; j++)
            {
                var idx = (int)(index + j * step);
                if ((uint)idx >= (uint)(tableLen - 1)) break;
                table[idx] = (ushort)sym;
            }
        }

        // Place sentinel at the very last entry
        if (firstCodeZero < _codes.NumCodes)
            table[tableLen - 1] = (ushort)firstCodeZero;

        // Diagnostic dump (enabled via DebugFlags.TraceDecoderTable).
        if (DebugFlags.TraceDecoderTable && tableLen > 1)
        {
            Console.Error.Write($"  [MGD] numCodes={_codes.NumCodes} levels: ");
            for (int i = 0; i < Math.Min(8, 32); i++)
            {
                var mask = levelMask[i]; var bse = levelBase[i];
                if (mask != 0 || bse != 0)
                    Console.Error.Write($" L{i}:m={mask}b={bse}");
            }
            Console.Error.WriteLine();
            Console.Error.Write($"  [MGD] table[0..20]: [");
            for (int i = 0; i < Math.Min(20, tableLen); i++)
                Console.Error.Write($"{table[i]},");
            Console.Error.WriteLine("]");
            Console.Error.Write($"  [MGD] lengths[0..20]: [");
            for (int i = 0; i < Math.Min(20, _codes.NumCodes); i++)
                Console.Error.Write($"{_codes.Lengths[i]},");
            Console.Error.WriteLine("]");
        }

        return (table, levelBase, levelMask);
    }

    /// <summary>
    /// Decodes one symbol, consuming its code length in bits.
    /// Returns uint.MaxValue (without consuming bits) when the lookup yields no
    /// valid symbol: an out-of-range slot or a zero-length code. Callers treat
    /// that as a literal NUL, matching the native zero-filled table behavior.
    /// Throws InvalidOperationException when the decoded length exceeds the
    /// available bits (native 0x40801).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Decode(Pa30BitReader reader, out int bitLength)
    {
        // Native reads the buffer word directly with no 32-bit availability check;
        // only the decoded length is validated (Skip throws when insufficient).
        var buf32 = reader.PeekRaw32();
        var w = buf32 | 0x80000000u;
        w |= (w << 1); w |= (w << 2); w |= (w << 4); w |= (w << 8); w |= (w << 16);
        var bitPos = LowestLookup[w % 37u];
        if (bitPos >= 32)
            throw new InvalidOperationException("Huffman decode: invalid bit position");

        var index = (int)(_levelBase[bitPos] + ((buf32 >> ((int)bitPos + 1)) & _levelMask[bitPos]));
        var sym = _table[index];
        if (sym >= _codes.NumCodes)
        {
            bitLength = 0;
            return uint.MaxValue;
        }
        bitLength = _codes.Lengths[sym];
        if (bitLength == 0)
            return uint.MaxValue;
        reader.Skip(bitLength);
        return sym;
    }
}
