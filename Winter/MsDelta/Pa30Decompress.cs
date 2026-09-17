#region License
/*
 * Copyright (c) 2026 Stefano Moioli
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */
#endregion

namespace Smx.Winter.MsDelta;

public struct CompressionSymbol
{
    public uint Value;
    public uint Length;
}

public class Pa30Decompress
{
    protected readonly Pa30BitReader _reader;
    private readonly Pa30CompositeFormat _format;
    private readonly byte[] _source;
    // Base RiftTable parsed from the delta stream. The native factory folds it into
    // the OffsetRiftTable via SumRiftTables; all manifests observed so far carry an
    // empty base table (hasTable=0), for which OffsetRiftTable::Init yields exactly
    // the source-size mapping built by OffsetRiftTable.CreateFromSource, so this
    // field is currently retained for diagnostics only.
    private readonly Pa30RiftTable _riftTable;
    private readonly OffsetRiftTable _offsetRift;
    private readonly ulong _sourceSize;
    private readonly ulong _targetSize;
    private readonly byte[] _output;
    // Native records lzx_range_copy entries when this is set; always false here.
    private readonly bool _reverseRequested;
    private readonly List<(ulong Dest, ulong Src, ulong Len)> _reversalCopies;

    // Concat-space position: starts at _sourceSize, ends at _sourceSize + _targetSize.
    // (The native loop likewise starts at sourceSize and must end exactly at
    // sourceSize + targetSize.)
    private ulong _position;
    private int _currentIndex;
    private int _currentFormatIndex;
    private ulong[] _formatOffsets;

    // MRU cache of the last 3 match offsets, for symbol values 344065-344067.
    // Updated after EVERY match (see UpdateMru).
    private ulong _lastOff0;
    private ulong _lastOff1;
    private ulong _lastOff2;

    public Pa30Decompress(
        Pa30BitReader reader,
        Pa30CompositeFormat format,
        byte[] source,
        Pa30RiftTable riftTable,
        OffsetRiftTable offsetRift,
        ulong sourceSize,
        ulong targetSize,
        byte[] output,
        bool reverseRequested)
    {
        _reader = reader;
        _format = format;
        _source = source;
        _riftTable = riftTable;
        _offsetRift = offsetRift;
        _sourceSize = sourceSize;
        _targetSize = targetSize;
        _output = output;
        _reverseRequested = reverseRequested;
        _reversalCopies = new List<(ulong, ulong, ulong)>();
        _position = sourceSize;
        _currentIndex = 0;
        _currentFormatIndex = 0;
        _lastOff0 = 0;
        _lastOff1 = 0;
        _lastOff2 = 0;

        // Subformat offsets are concat-space starts (the native format lookup
        // searches by concat position); the last format runs to concat end.
        _formatOffsets = _format.IsSingleFormat
            ? [targetSize]
            : _format.SubFormats.Select(f => f.Offset).Append(sourceSize + targetSize).ToArray();
    }

    public byte[] Decompress()
    {
        var end = _sourceSize + _targetSize;
        while (_position < end)
        {
            AdvanceFormat();
            var format = GetCurrentFormat();
            var symbol = ReadSymbol(format);

            // Native literal: value = symIndex (< 256), length = 1.
            if (symbol.Length == 1 && symbol.Value < 256)
            {
                _output[_position - _sourceSize] = (byte)symbol.Value;
                _position++;
                continue;
            }

            // Unreachable via ReadSymbol (literals always carry value < 256 and every
            // match length is >= 2); kept as a defensive guard.
            if (symbol.Length <= 1)
            {
                throw new InvalidOperationException(
                    $"Invalid match symbol value={symbol.Value} length={symbol.Length} at pos={_position}");
            }

            var matchLen = symbol.Length;
            var matchOffset = ResolveMatchOffset(symbol.Value);

            // Native throws (0x42201) on a zero offset or an offset beyond position.
            if (matchOffset == 0)
                throw new InvalidOperationException(
                    $"Zero match offset for symbol {symbol.Value} at pos={_position}");
            if (_position < matchOffset)
                throw new InvalidOperationException(
                    $"Match offset {matchOffset} exceeds position {_position} for symbol {symbol.Value}");

            CopyMatch(_position - matchOffset, matchLen);
            _position += matchLen;

            UpdateMru(matchOffset);
        }

        return _output;
    }

    /// <summary>
    /// Copies matchLen bytes from concat-space src into the target at the current position.
    /// Reads go through the concatenated [source ++ target] address space so matches can
    /// reference source bytes (c &lt; sourceSize) or already-written target bytes.
    /// Per-byte read-then-write preserves LZ overlap semantics.
    /// </summary>
    private void CopyMatch(ulong srcConcat, ulong matchLen)
    {
        var dstIdx = _position - _sourceSize;
        for (ulong i = 0; i < matchLen; i++)
        {
            var c = srcConcat + i;
            _output[dstIdx + i] = c < _sourceSize ? _source[c] : _output[c - _sourceSize];
        }
    }

    public List<(ulong Dest, ulong Src, ulong Len)> ReversalCopies => _reversalCopies;

    private void AdvanceFormat()
    {
        if (_format.IsSingleFormat) return;
        while (_currentFormatIndex + 1 < _formatOffsets.Length
               && _position >= _formatOffsets[_currentFormatIndex + 1])
        {
            _currentFormatIndex++;
        }
    }

    private Pa30CompressionFormat GetCurrentFormat()
    {
        if (_format.IsSingleFormat)
            return _format.SingleFormat!;
        return _format.SubFormats[_currentFormatIndex].Format;
    }

    protected virtual CompressionSymbol ReadSymbol(Pa30CompressionFormat fmt)
    {
        uint symIndex;
        try { symIndex = fmt.MainDecoder.Decode(_reader, out _); }
        catch (InvalidOperationException) { return new CompressionSymbol { Value = 0, Length = 1 }; }

        if (symIndex == uint.MaxValue) return new CompressionSymbol { Value = 0, Length = 1 };

        if (symIndex < 256) return new CompressionSymbol { Value = symIndex, Length = 1 };

        var code = symIndex - 256;
        var lengthCategory = code & 7;
        var offsetCategory = code >> 3;

        // Offset first, length second (native order).
        var offsetValue = DecodeOffset(fmt, offsetCategory);
        // lengthCategory != 0: length = lc+1 (2..8); else extended length via LengthDecoder.
        var matchLength = lengthCategory != 0 ? (ulong)(lengthCategory + 1) : DecodeMatchLength(fmt);

        return new CompressionSymbol { Value = (uint)offsetValue, Length = (uint)matchLength };
    }

    /// <summary>
    /// Resolves a match symbol value to a concat-space backward offset.
    ///   value == 344064       → rift lookup: -tgtDelta of current entry
    ///   value &lt; 344064      → value - 172032 - tgtDelta of current entry
    ///   value in 344065..67   → MRU cache[value - 344065]
    ///   value &gt; 344067      → value - 344067
    /// </summary>
    private ulong ResolveMatchOffset(uint symbolValue)
    {
        if (symbolValue == 344064)
        {
            _offsetRift.FindEntry(_position, ref _currentIndex);
            return (ulong)(-_offsetRift.TgtDeltas[_currentIndex]);
        }
        else if (symbolValue < 344064)
        {
            _offsetRift.FindEntry(_position, ref _currentIndex);
            return (ulong)((long)symbolValue - 172032L - _offsetRift.TgtDeltas[_currentIndex]);
        }
        else if (symbolValue <= 344067)
        {
            var slot = symbolValue - 344065;
            return slot switch
            {
                0 => _lastOff0,
                1 => _lastOff1,
                _ => _lastOff2,
            };
        }
        return symbolValue - 344067;
    }

    /// <summary>
    /// MRU-3 update, runs after EVERY match:
    /// if (mru[0] != off) { t = mru[1]; mru[1] = mru[0]; if (t != off) mru[2] = t; mru[0] = off; }
    /// </summary>
    private void UpdateMru(ulong matchOffset)
    {
        if (_lastOff0 != matchOffset)
        {
            var t = _lastOff1;
            _lastOff1 = _lastOff0;
            if (t != matchOffset) _lastOff2 = t;
            _lastOff0 = matchOffset;
        }
    }

    /// <summary>
    /// Decodes the match offset class.
    /// Categories 0-2 use fixed-bit reads; 3-6 are rift markers 344064-344067;
    /// 7+ enter long-offset decoding with loc = category - 7.
    /// </summary>
    protected virtual ulong DecodeOffset(Pa30CompressionFormat fmt, ulong category)
    {
        if (category < 3)
        {
            return category switch
            {
                // Read(14) - 0x2000 + 172032
                0 => (ulong)_reader.Read(14) - 0x2000 + 172032,
                // Read(16) biased by +/- 0x2000 around 172032
                1 => DecodeCategory1(),
                // Read(18) biased around 212992 / 172032 - 40960
                2 => DecodeCategory2(),
                _ => 0
            };
        }
        // Categories 3-6 map to 344064-344067, no bits consumed.
        if (category - 3 < 4)
            return category - 3 + 344064;

        return DecodeLongOffset(fmt, (int)(category - 7));
    }

    private ulong DecodeCategory1()
    {
        var v = _reader.Read(16);
        var sv = (int)v - 0x8000;
        return sv < 0x8000 ? (ulong)(sv + 0x2000 + 172032) : (ulong)(sv - 0x2000 + 172032);
    }

    private ulong DecodeCategory2()
    {
        var v = _reader.Read(18);
        var sv = (int)v - 0x20000;
        return sv < 0x20000 ? (ulong)(sv + 212992) : (ulong)(sv - 40960 + 172032);
    }

    /// <summary>
    /// Long-offset decoder.
    /// loc == 0 redirects through the bit-reading special (36..63) and jumps back
    /// to the range checks; loc 1..3 decode bare (no bits consumed).
    /// </summary>
    private ulong DecodeLongOffset(Pa30CompressionFormat fmt, int loc)
    {
        while (true)
        {
            // Range check: loc >= 64 throws.
            if ((uint)loc >= 64)
                throw new InvalidOperationException($"Invalid long offset category {loc}");

            if (loc == 0)
            {
                loc = ReadLongOffsetSpecialLoc();
                continue;
            }

            // loc < 4 decodes bare: value = loc + 344067.
            if (loc < 4)
                return (ulong)loc + 344067;

            return DecodeLongOffsetComplex(fmt, loc);
        }
    }

    private int ReadLongOffsetSpecialLoc()
    {
        // loc == 0 special: one flag bit selects a 2-bit (+36), 3-bit (+40),
        // or 4-bit (+48) follow-up.
        var bit1 = _reader.Read(1);
        if (bit1 == 0)
            return (int)_reader.Read(2) + 36;

        var bit2 = _reader.Read(1);
        if (bit2 == 0)
            return (int)_reader.Read(3) + 40;

        // 4-bit path folds as Read(4)+8, +4, +36 = Read(4)+48.
        return (int)_reader.Read(4) + 48;
    }

    /// <summary>
    /// Complex long-offset body:
    /// subCategory = (loc &gt;&gt; 1) - 1, baseVal = (loc &amp; 1) + 2.
    /// </summary>
    private ulong DecodeLongOffsetComplex(Pa30CompressionFormat fmt, int loc)
    {
        int subCategory = (loc >> 1) - 1;
        int baseVal = (loc & 1) + 2;

        ulong result;
        if (subCategory < 4)
        {
            var raw = _reader.Read(subCategory);
            result = raw | ((ulong)baseVal << subCategory);
        }
        else if (subCategory == 4)
        {
            var sym = fmt.SymbolDecoder.Decode(_reader, out _);
            result = sym | ((ulong)baseVal << 4);
        }
        else
        {
            int extraBits = subCategory - 4;
            var raw = _reader.Read(extraBits);
            var combined = raw | ((ulong)baseVal << extraBits);
            var sym = fmt.SymbolDecoder.Decode(_reader, out _);
            result = sym | (combined << 4);
        }

        return result + 344067;
    }

    /// <summary>
    /// Extended match-length decoder.
    /// A zero length index falls back to ReadNumber; the final length is lenIdx + 8.
    /// </summary>
    protected virtual ulong DecodeMatchLength(Pa30CompressionFormat fmt)
    {
        try
        {
            var lenIdx = fmt.LengthDecoder.Decode(_reader, out _);
            if (lenIdx == 0)
            {
                // Native uses ReadNumber (not ReadVarInt) for the extended match length
                lenIdx = (uint)_reader.ReadNumber();
                if (lenIdx > 0xFFFFFFF7)
                    throw new InvalidOperationException("Invalid match length");
            }
            // Native: HIDWORD(Length) = (lenIdx + 7) + 1 = lenIdx + 8
            return (ulong)(lenIdx + 8);
        }
        catch (InvalidOperationException)
        {
            // Bitstream exhausted: native throws 0x40801 here. Fall back to the minimum
            // match length so truncated/desynced streams still terminate the loop.
            return 8;
        }
    }
}
