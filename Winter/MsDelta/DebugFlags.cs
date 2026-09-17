namespace Smx.Winter.MsDelta;

public static class DebugFlags
{
    public static bool TraceComressionLengths;
    public static bool TraceReadSymbol;
    public static bool TraceFormat;
    public static bool TraceDecoderTable;

    public static void SetAll(bool on)
    {
        TraceComressionLengths = on;
        TraceReadSymbol = on;
        TraceFormat = on;
        TraceDecoderTable = on;
    }
}
