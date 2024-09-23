using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;
using MinHook;

namespace Smx.Winter.Cbs
{
    public class TraceCollector : IDisposable
    {
        private static readonly Guid LogProvider_TrustedInstaller =
            Guid.Parse("6D1B249D-131B-468A-899B-FB0AD9551772");
        private static readonly Guid LogProvider_Wcp =
            Guid.Parse("696AC247-4E85-42E8-B0D2-FEC2475C76AD");
        private static readonly Guid LogProvider_CbsTelemetry =
            Guid.Parse("5FC48AED-2EB8-4CD4-9C87-54700C4B7B26");
        private static readonly Guid LogProvider_CbsSetup =
            Guid.Parse("BD12F3B8-FC40-4A61-A307-B7A013A069C1");

        private delegate void pfnOutputDebugStringW([MarshalAs(UnmanagedType.LPWStr)] string lpOutputString);
        private delegate void pfnOutputDebugStringA([MarshalAs(UnmanagedType.LPStr)] string lpOutputString);

        private TraceEventSession _trace;
        private Task _eventProcessor;
        private HookEngine _hooks;

        private void InitializeTrace()
        {
            _trace.EnableProvider(LogProvider_TrustedInstaller, TraceEventLevel.Verbose, 0);
            _trace.EnableProvider(LogProvider_Wcp, TraceEventLevel.Verbose, 0);
            _trace.EnableProvider(LogProvider_CbsTelemetry, TraceEventLevel.Verbose, 0);
            _trace.EnableProvider(LogProvider_CbsSetup, TraceEventLevel.Verbose, 0);
            _trace.Source.Dynamic.All += delegate (TraceEvent data)
            {
                Console.WriteLine($"[EV] {data.EventName}");
            };
            _trace.Source.UnhandledEvents += delegate (TraceEvent data)
            {
                Console.WriteLine($"[EV] {data.EventName}");
            };
        }

        public TraceCollector()
        {
            _hooks = new HookEngine();
            _trace = new TraceEventSession("Winter");

            InitializeTrace();
            _eventProcessor = Task.Run(_trace.Source.Process);
            _hooks.CreateHook("kernel32.dll", "OutputDebugStringA", new pfnOutputDebugStringA(str =>
            {
                Console.WriteLine($"[DBG]: {str}");
            }));
            _hooks.CreateHook("kernel32.dll", "OutputDebugStringW", new pfnOutputDebugStringW(str =>
            {
                Console.WriteLine($"[DBG]: {str}");
            }));
            _hooks.EnableHooks();
        }

        public void Dispose()
        {
            _trace.Dispose();
            _eventProcessor.Wait();
        }
    }
}