namespace Smx.Winter.MsDelta;

/// <summary>
/// OffsetRiftTable — maps concat-space positions to match-back offsets.
/// Entries are {SrcOffset (u64), TgtDelta (i64)}. Lookup advances the entry index
/// while entry.SrcOffset - 1 &lt; position, computed UNSIGNED (so a zero SrcOffset
/// yields UINT64_MAX and never advances).
/// </summary>
public class OffsetRiftTable
{
    /// <summary>Source page boundary offsets (u64). Entry[k].SrcOffset.</summary>
    public ulong[] SrcOffsets { get; }
    /// <summary>Target deltas (i64) for each entry. Entry[k].TgtDelta.</summary>
    public long[] TgtDeltas { get; }

    public OffsetRiftTable(ulong[] srcOffsets, long[] tgtDeltas)
    {
        SrcOffsets = srcOffsets;
        TgtDeltas = tgtDeltas;
    }

    /// <summary>
    /// Builds the OffsetRiftTable for an empty base RiftTable, which is all any
    /// observed manifest carries (hasTable=0). Verified against a native Init hook:
    /// count=2 with entry[0]={SrcOffset 0, TgtDelta -sourceSize}, i.e. every match
    /// offset resolves against a sourceSize lookback. Entry[1] is the identical
    /// sentinel that terminates the lookup loop.
    /// NOTE: the native factory Sums the base table with a preprocess rift table
    /// before Init; non-empty inputs are not implemented here.
    /// </summary>
    public static OffsetRiftTable CreateFromSource(int sourceSize)
    {
        return new OffsetRiftTable(
            [0, 0],
            [-sourceSize, -sourceSize]);
    }

    /// <summary>
    /// Advances currentIndex while the next entry's (SrcOffset - 1) is below position,
    /// including the unsigned wraparound for zero SrcOffsets. currentIndex only moves forward.
    /// </summary>
    public int FindEntry(ulong position, ref int currentIndex)
    {
        // Advance while the next entry's (SrcOffset - 1) is below position.
        int k = currentIndex + 1;
        while (k < SrcOffsets.Length && SrcOffsets[k] - 1 < position)
        {
            currentIndex++;
            k++;
        }
        return currentIndex;
    }
}
