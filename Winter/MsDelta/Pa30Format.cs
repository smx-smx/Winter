#region License
/*
 * Copyright (c) 2026 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion
using System.IO;

namespace Smx.Winter.MsDelta;

/// <summary>
/// Variable-length integer format using static Huffman encoding (252 codes, lengths ≤ 16).
/// Layout: three 8-bit counts (numFixed/numRun/numSpecial), 4-bit code lengths for the
/// fixed/run sections, then derived fills for codes numFixed..125 and 126+numRun..251.
/// </summary>
public class Pa30IntFormat
{
    private const int MaxCodes = 252; // 0xFC

    public StaticHuffmanCodes Codes { get; }
    public StaticHuffmanDecoderTable DecoderTable { get; private set; }

    public Pa30IntFormat(StaticHuffmanCodes codes)
    {
        Codes = codes;
        DecoderTable = null!;
    }

    public static Pa30IntFormat FromBitReader(Pa30BitReader reader)
    {
        var numFixed = (int)reader.Read(8);
        if (numFixed > 0x7E)
            throw new InvalidOperationException($"IntFormat: numFixed {numFixed} > 126");
        var numRun = (int)reader.Read(8);
        if (numRun > 0x7E)
            throw new InvalidOperationException($"IntFormat: numRun {numRun} > 126");
        var numSpecial = (int)reader.Read(8);
        if (252 - numRun - numFixed < numSpecial)
            throw new InvalidOperationException(
                $"IntFormat: special run {numSpecial} exceeds remaining {252 - numRun - numFixed}");

        var lengths = new byte[MaxCodes];

        // Fixed section: lengths[0..numFixed-1] = Read(4)+1
        for (int i = 0; i < numFixed; i++)
        {
            var len = (int)reader.Read(4) + 1;
            if (len > 0x10)
                throw new InvalidOperationException($"IntFormat: fixed length {len} > 16");
            lengths[i] = (byte)len;
        }

        // Run section: numRun+1 reads; the first numRun set lengths[126+j],
        // the final extra read seeds currentLen (native reads Read(4) before the break check).
        var lastRead = 0;
        for (int j = 0; ; j++)
        {
            lastRead = (int)reader.Read(4);
            if (numRun <= j) break;
            var len = lastRead + 1;
            if (len > 0x10)
                throw new InvalidOperationException($"IntFormat: run length {len} > 16");
            lengths[j + 126] = (byte)len;
        }
        var currentLen = lastRead + 1;
        if (currentLen > 0x10)
            throw new InvalidOperationException($"IntFormat: length {currentLen} > 16");

        var special = numSpecial;

        // Fill lengths[numFixed..125]: countdown on 'special' decrements currentLen
        if (numFixed < 0x7E)
        {
            int f = numFixed;
            int idx = numFixed;
            do
            {
                int oldSpecial = special--;
                if (oldSpecial == 0)
                {
                    currentLen--;
                    special = 252 - f - numRun;
                }
                lengths[idx] = (byte)currentLen;
                f++;
                idx++;
            } while (f < 0x7E);
        }

        // Fill lengths[126+numRun..251]
        if (numRun < 0x7E)
        {
            int r = numRun;
            int idx = numRun + 126;
            do
            {
                int nextSpecial = special - 1;
                int slotLength = currentLen - 1;
                int savedRun = r;
                if (special != 0) slotLength = currentLen;
                r++;
                lengths[idx++] = (byte)slotLength;
                currentLen = slotLength;
                int prevSpecial = special;
                special = 126 - savedRun;
                if (prevSpecial != 0) special = nextSpecial;
            } while (r < 0x7E);
        }

        var codes = new StaticHuffmanCodes(MaxCodes, 16);
        codes.SetLengths(lengths);

        var format = new Pa30IntFormat(codes);
        format.DecoderTable = new StaticHuffmanDecoderTable(codes);
        return format;
    }

    /// <summary>
    /// Signed-magnitude delta decoder: symbols below 126 are positive deltas,
    /// symbols at/above 126 are negative (bitwise-NOT).
    /// </summary>
    public long ReadNumber(Pa30BitReader reader)
    {
        var sym = DecoderTable.Decode(reader, out _);
        if (sym == uint.MaxValue)
            throw new InvalidOperationException("IntFormat.ReadNumber: invalid symbol");

        long magnitude = sym < 126 ? sym : sym - 126;
        long value;
        if (magnitude >= 4)
        {
            int bitCount = (int)((magnitude >> 1) - 1);
            long baseValue = ((magnitude & 1) + 2L) << (int)((magnitude >> 1) - 1);
            ulong extra;
            if (bitCount > 0x20)
                extra = reader.Read(32) | ((ulong)reader.Read(bitCount - 32) << 32);
            else
                extra = reader.Read(bitCount);
            value = (long)(extra | (ulong)baseValue);
        }
        else
        {
            value = magnitude;
        }
        return sym < 126 ? value : ~value;
    }
}

/// <summary>
/// The 872 Huffman code lengths shared by a CompressionFormat's three decoder tables:
/// bytes 0-599 (main, 600 codes), 600-855 (length, 256 codes), 856-871 (symbol, 16 codes).
/// Decoded from the bitstream with the 39-code precursor table, delta-chained against the
/// previous lengths (872 zeros for the first subformat).
/// </summary>
public class Pa30CompressionLengths
{
    internal const int TotalLength = 872; // 0x368 = 600 + 256 + 16
    public byte[] Lengths { get; set; } = [];

    public static Pa30CompressionLengths FromBitReader(
        Pa30BitReader reader, StaticHuffmanDecoderTable precursorDecoder,
        byte[]? prevLengths = null)
    {
        if (prevLengths == null) prevLengths = new byte[TotalLength];

        var result = new Pa30CompressionLengths();
        var lengths = new byte[TotalLength];
        int idx = 0;

        while (idx < TotalLength)
        {
            var sym = precursorDecoder.Decode(reader, out _);

            if (sym == uint.MaxValue)
                throw new InvalidOperationException($"CompressionLengths: sentinel at idx={idx}");

            if (sym < 17)
            {
                lengths[idx++] = (byte)sym;
            }
            else if (sym < 23)
            {
                var offset = (byte)(sym - 17);
                var repl = prevLengths[idx];
                if (offset < 3)
                    lengths[idx] = (byte)(repl + offset + 1);
                else
                    lengths[idx] = (byte)(repl - offset + 2);
                idx++;
            }
            else
            {
                var cat = (int)(sym - 23);
                var extraBits = cat & 7;
                uint runLen;
                if (extraBits >= 3)
                    runLen = (1u << (extraBits - 1)) | reader.Read(extraBits - 1);
                else
                    runLen = (uint)(extraBits + 1);

                // Bound check: reject overflow and runs past the 872-byte table end.
                if (idx > runLen + idx || runLen + idx >= TotalLength)
                    throw new InvalidOperationException(
                        $"CompressionLengths: run length {runLen} exceeds bounds");

                if (cat < 8)
                {
                    // Repeat previous CURRENT byte (constant fill).
                    if (idx == 0)
                        throw new InvalidOperationException("CompressionLengths: can't repeat with no previous");
                    var repeatByte = lengths[idx - 1];
                    for (uint i = 0; i < runLen; i++)
                        lengths[idx++] = repeatByte;
                }
                else
                {
                    // Copy the PREVIOUS lengths sequence (advancing, NOT frozen).
                    for (uint i = 0; i < runLen; i++)
                    {
                        lengths[idx] = prevLengths[idx];
                        idx++;
                    }
                }
            }
        }

        result.Lengths = lengths;
        return result;
    }
}

/// <summary>
/// One decompression table set: main (600 codes), length (256) and symbol (16) Huffman
/// decoders, all with max code length 16. Decoder tables are built eagerly on ApplyLengths.
/// </summary>
public class Pa30CompressionFormat
{
    private const int MainCodeCount = 600;
    private const int LengthCodeCount = 256;
    private const int SymbolCodeCount = 16;

    public StaticHuffmanCodes MainCodes { get; private set; }
    public StaticHuffmanCodes LengthCodes { get; private set; }
    public StaticHuffmanCodes SymbolCodes { get; private set; }
    public StaticHuffmanDecoderTable MainDecoder { get; private set; }
    public StaticHuffmanDecoderTable LengthDecoder { get; private set; }
    public StaticHuffmanDecoderTable SymbolDecoder { get; private set; }
    public byte[] SymbolLengths { get; private set; }
    public byte[] LengthLengths { get; private set; }

    public Pa30CompressionFormat()
    {
        // Table geometry: main 600 codes, length 256, symbol 16; max code length 16 each.
        MainCodes = new StaticHuffmanCodes(MainCodeCount, 16);
        LengthCodes = new StaticHuffmanCodes(LengthCodeCount, 16);
        SymbolCodes = new StaticHuffmanCodes(SymbolCodeCount, 16);
        SymbolLengths = [];
        LengthLengths = [];
        MainDecoder = null!;
        LengthDecoder = null!;
        SymbolDecoder = null!;
    }

    public static Pa30CompressionFormat CreateDefault()
    {
        // Diagnostic shortcut: a previously hook-captured lengths blob from staging/
        // for single-file experiments (--direct). Production parsing never uses this —
        // multi-format deltas parse lengths from the stream, single-format deltas use
        // CreateSingleDefault() below.
        var capturePath = Pa30Paths.StagingRead("compression_lengths.bin");
        if (File.Exists(capturePath))
        {
            var bytes = File.ReadAllBytes(capturePath);
            if (bytes.Length >= 872)
            {
                var fmt = new Pa30CompressionFormat();
                fmt.ApplyLengths(new Pa30CompressionLengths { Lengths = bytes });
                return fmt;
            }
        }

        return CreateSingleDefault();
    }

    /// <summary>
    /// Single-format defaults: no bits are read; the 888-byte lengths object is
    /// memset at +8: main[0..423]=9, main[424..599]=10, length[0..255]=8,
    /// symbol[0..15]=4, trailing byte 880 = 0.
    /// </summary>
    public static Pa30CompressionFormat CreateSingleDefault()
    {
        var format = new Pa30CompressionFormat();

        var mainLens = new byte[MainCodeCount];
        for (int i = 0; i < 424; i++) mainLens[i] = 9;
        for (int i = 424; i < 600; i++) mainLens[i] = 10;

        var mainCodes = new StaticHuffmanCodes(MainCodeCount, 16);
        mainCodes.SetLengths(mainLens);
        format.MainCodes = mainCodes;
        format.MainDecoder = new StaticHuffmanDecoderTable(mainCodes);

        var lenLens = new byte[LengthCodeCount];
        for (int i = 0; i < 256; i++) lenLens[i] = 8;

        var lengthCodes = new StaticHuffmanCodes(LengthCodeCount, 16);
        lengthCodes.SetLengths(lenLens);
        format.LengthCodes = lengthCodes;
        format.LengthDecoder = new StaticHuffmanDecoderTable(lengthCodes);
        format.LengthLengths = lenLens;

        var symLens = new byte[SymbolCodeCount];
        for (int i = 0; i < 16; i++) symLens[i] = 4;

        var symbolCodes = new StaticHuffmanCodes(SymbolCodeCount, 16);
        symbolCodes.SetLengths(symLens);
        format.SymbolCodes = symbolCodes;
        format.SymbolDecoder = new StaticHuffmanDecoderTable(symbolCodes);
        format.SymbolLengths = symLens;

        return format;
    }

    public void ApplyLengths(Pa30CompressionLengths lengths)
    {
        var lenBytes = lengths.Lengths;
        if (lenBytes.Length < MainCodeCount + LengthCodeCount + SymbolCodeCount)
            throw new InvalidOperationException($"CompressionLengths too short: {lenBytes.Length}");

        // MainCodes: bytes 0-599
        var mainLens = new byte[MainCodeCount];
        Array.Copy(lenBytes, 0, mainLens, 0, MainCodeCount);
        MainCodes.SetLengths(mainLens);
        MainDecoder = new StaticHuffmanDecoderTable(MainCodes);

        // LengthCodes: bytes 600-855
        var lenLens = new byte[LengthCodeCount];
        Array.Copy(lenBytes, 600, lenLens, 0, LengthCodeCount);
        LengthCodes.SetLengths(lenLens);
        LengthDecoder = new StaticHuffmanDecoderTable(LengthCodes);
        LengthLengths = lenLens;

        // SymbolCodes: bytes 856-871
        var symLens = new byte[SymbolCodeCount];
        Array.Copy(lenBytes, 856, symLens, 0, SymbolCodeCount);
        SymbolCodes.SetLengths(symLens);
        SymbolDecoder = new StaticHuffmanDecoderTable(SymbolCodes);
        SymbolLengths = symLens;
    }
}

/// <summary>
/// The format selector for a decompression run. Either a single default table set
/// (no bits read) or count subformats, each covering concat-space positions
/// [offset[i], offset[i+1]) with its own chained lengths. Format lookup is a
/// binary search by position.
/// </summary>
public class Pa30CompositeFormat
{
    public bool IsSingleFormat { get; }
    public Pa30CompressionFormat? SingleFormat { get; }
    public List<(ulong Offset, Pa30CompressionFormat Format)> SubFormats { get; } = [];

    public Pa30CompositeFormat(Pa30CompressionFormat singleFormat)
    {
        IsSingleFormat = true;
        SingleFormat = singleFormat;
    }

    public Pa30CompositeFormat(List<(ulong, Pa30CompressionFormat)> subFormats)
    {
        IsSingleFormat = false;
        SubFormats = subFormats;
    }

    public static Pa30CompositeFormat FromBitReader(Pa30BitReader reader)
    {
        var isSingle = reader.Read(1) != 0;
        if (isSingle)
            return new Pa30CompositeFormat(Pa30CompressionFormat.CreateSingleDefault());

        // Multi-format layout:
        //   count = ReadVarInt
        //   offsets[i] = ReadVarInt + offsets[i-1]   (cumulative concat-space starts)
        //   precursor = 39 x Read(4) -> 39-code table
        //   for each i: lengths[i] = delta-decoded with precursor, prev=lengths[i-1]
        //              (first chained against 872 zeros)
        var count = reader.ReadVarInt();
        var subFormats = new List<(ulong, Pa30CompressionFormat)>();

        ulong cumulativeOffset = 0;
        var offsets = new List<ulong>();
        for (ulong i = 0; i < count; i++)
        {
            cumulativeOffset += reader.ReadVarInt();
            offsets.Add(cumulativeOffset);
        }

        // Read shared precursor codes (39 × 4 bits)
        var precursorLenBytes = new byte[39];
        for (int i = 0; i < 39; i++)
            precursorLenBytes[i] = (byte)reader.Read(4);
        var precursorCodes = new StaticHuffmanCodes(39, 15);
        precursorCodes.SetLengths(precursorLenBytes);
        var precursorDecoder = new StaticHuffmanDecoderTable(precursorCodes);

        var prevLengths = new byte[Pa30CompressionLengths.TotalLength]; // 872 zeros
        for (int i = 0; i < offsets.Count; i++)
        {
            var compLengths = Pa30CompressionLengths.FromBitReader(reader, precursorDecoder, prevLengths);
            prevLengths = compLengths.Lengths;
            var format = new Pa30CompressionFormat();
            format.ApplyLengths(compLengths);
            subFormats.Add((offsets[i], format));
        }

        return new Pa30CompositeFormat(subFormats);
    }
}

public class Pa30RiftTable
{
    public List<(ulong SourceOffset, ulong TargetOffset)> Entries { get; } = [];

    /// <summary>
    /// Parses the delta's base RiftTable. A zero hasTable bit yields an empty table,
    /// which is all any observed manifest carries.
    /// NOTE: the native side sorts entries here and the apply factory folds
    /// this table into the OffsetRiftTable via rift-table summation — neither is wired up yet.
    /// </summary>
    public static Pa30RiftTable FromBitReader(Pa30BitReader reader)
    {
        var table = new Pa30RiftTable();

        var hasTable = reader.Read(1) != 0;
        if (!hasTable) return table;

        var sourceFormat = Pa30IntFormat.FromBitReader(reader);
        var targetFormat = Pa30IntFormat.FromBitReader(reader);

        // Entry count is read upfront as a VarInt, then source/target deltas accumulate.
        var count = reader.ReadVarInt();

        long sourceBase = 0;
        long targetBase = 0;
        for (ulong n = count; n != 0; n--)
        {
            sourceBase += sourceFormat.ReadNumber(reader);
            targetBase += targetFormat.ReadNumber(reader);
            // native Add(this, sourceBase, targetBase + sourceBase)
            table.Entries.Add(((ulong)sourceBase, (ulong)(targetBase + sourceBase)));
        }

        return table;
    }
}
