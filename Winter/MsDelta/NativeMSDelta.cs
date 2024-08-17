using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;
using Windows.Win32.System.Diagnostics.Debug;
using static Smx.Winter.MsDelta.NativeApi;

namespace Smx.Winter.MsDelta
{
    internal struct NativeApi
    {
        public delegate void pfnInitDelta();
        public delegate void pfnInitEnv();
        public delegate nint pfnComponentGet(
            nint stringPool,
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int nameLength
        );
        public delegate void pfnComponentPut(
            nint stringPool,
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            int nameLength,
            nint compo
        );
        public delegate void pfnComponentInit(nint compo, nint compo_factory);
        public delegate nint pfnInputBufferGet(ref DELTA_INPUT input);
        public delegate void pfnInputBufferPut(nint obj, ref DELTA_OUTPUT output);
        public delegate void pfnComponentFactoriesPutAll(nint env);
        public delegate nint pfnCdpNew(nint size);
        public delegate nint pfnFlowComponentFactory(nint cdp);
        public delegate void pfnFlowComponentInit(
            nint compo, 
            nint env,
            [MarshalAs(UnmanagedType.LPStr)]
            string code,
            int code_len);
        public delegate void pfnComponentProcess(
            nint factory,
            int numInputs, nint inputArgs,
            int numOutputs, nint results
        );
        public delegate void pfnComponentInitFactory(
            nint compo, nint env,
            int numInputs, nint inputTypes,
            int numOutputs, nint outputTypes
        );
        public delegate nint pfnComponentFactoryObtainComponent(nint factory);

        public pfnInitDelta init;
        public pfnInitEnv init_env;
        public pfnComponentGet compo_get;
        public pfnComponentPut compo_put;
        public pfnComponentInit compo_init;
        public pfnComponentInitFactory compo_init_factory;
        public pfnCdpNew cdp_new;
        public pfnFlowComponentFactory flow_component_factory;
        public pfnFlowComponentInit flow_component_init;
        public pfnInputBufferGet input_buffer_get;
        public pfnInputBufferPut output_buffer_put;
        public pfnComponentProcess component_process;
        public pfnComponentFactoriesPutAll component_factories_put_all;
        public pfnComponentFactoryObtainComponent component_factory_obtain_component;
        public nint gp_environment;
        public nint gp_applyPatchFactory;
    }

    internal enum ComponentType : uint
    {
        Number = 2,
        Buffer = 3,
        BitWriterObject = 4,
        BitReaderObject = 5,
        BufferList = 6,
        BufferWriterObject = 7,
        BufferReaderObject = 8,
        IniReader = 9,
        RiftTable = 10,
        PortableExecutable = 11,
        StaticHuffman_CompressionFormat = 12,
        CompositeFormat = 13,
        PseudoLxzSearch = 14,
        CliMetadata = 15,
        Cli4Map = 16,
        StaticHuffman_IntFormat = 17,
        PortableExecutableInfo = 18,
    }

    internal struct ComponentOutputInfo
    {
        public ComponentType Type;
    }

    internal struct ComponentInputInfo
    {
        public ComponentType Type;
        public byte _u0; // bool IsInput?
    }

    internal struct ComponentInputInfoArray(nint ptr)
    {
        public ComponentInputInfo this[int index]
        {
            get => Marshal.PtrToStructure<ComponentInputInfo>(ptr + (nint.Size * index));
        }
    }

    internal struct ComponentOutputInfoArray(nint ptr) {
        public ComponentOutputInfo this[int index]
        {
            get => Marshal.PtrToStructure<ComponentOutputInfo>(ptr + (4 * index));
        }
    }


    internal struct ComponentInfo
    {
        public ulong _u0;
        public ulong _u1;
        public ulong _u2;
        private ulong numInputs;
        public ComponentInputInfoArray InputInfo;
        private ulong numOutputs;
        public ComponentOutputInfoArray OutputInfo;

        public int NumInputs => (int)numInputs;
        public int NumOutputs => (int)numOutputs;
    }

    internal struct ComponentObject
    {
        public uint _u0;
        public uint _u1;
        public bool initialized;
        public ComponentType Type;

        public ComponentObject(nint ptr)
        {
            this = Marshal.PtrToStructure<ComponentObject>(ptr);
        }
    }

    internal record CachedStruct<T>(T Value) where T : struct { }

    internal struct TypedPointer<T> where T : struct
    {
        public nint ptr;
        public T Value => Marshal.PtrToStructure<T>(ptr);
    }

    internal struct ComponentData
    {
        public nint vtbl;
        private TypedPointer<ComponentInfo> info;
        private nint inputs;
        private nint outputs;

        public ComponentObject GetInput(int i)
        {
            var ptr = Marshal.ReadIntPtr(inputs + (nint.Size * i));
            return new ComponentObject(ptr);
        }

        public ComponentObject GetOutput(int i)
        {
            var ptr = Marshal.ReadIntPtr(outputs + (nint.Size * i));
            return new ComponentObject(ptr);
        }

        public ComponentData(nint ptr)
        {
            this = Marshal.PtrToStructure<ComponentData>(ptr);
        }

        public ComponentInfo Info => info.Value;
    }

    internal class NativeMSDelta
    {
        private FreeLibrarySafeHandle msdelta;
        private const string SYM_SEARCH_PATH = "SRV*C:\\WINDOWS\\TEMP*https://msdl.microsoft.com/download/symbols";

        private readonly long MAX_SYM_BUF;
        private readonly NativeApi api;

        private void LoadSymbols()
        {
            PInvoke.SymSetOptions(PInvoke.SYMOPT_DEFERRED_LOADS | PInvoke.SYMOPT_DEBUG);
            var currentProcess = PInvoke.GetCurrentProcess_SafeHandle();
            if (!PInvoke.SymInitialize(currentProcess, SYM_SEARCH_PATH, false))
            {
                throw new Exception("SymInitialize() failed");
            }
            if(PInvoke.SymLoadModuleEx(
                currentProcess,
                null, "msdelta.dll",
                null,
                (ulong)msdelta.DangerousGetHandle(),
                0, null, 0
            ) == 0)
            {
                throw new Exception("SymLoadModuleEx() failed");
            }
        }

        private nint _env => Marshal.ReadIntPtr(api.gp_environment);
        private nint _privateData => Marshal.ReadIntPtr(_env + nint.Size);
        private nint _stringPool => Marshal.ReadIntPtr(_env + nint.Size + nint.Size);

        private bool _needInit
        {
            get => Marshal.ReadByte(_privateData + 8248) == 1;
            set => Marshal.WriteByte(_privateData + 8248, (byte)(value ? 1 : 0));
        }

        private nint GetSymbolAddress(string mangledName)
        {
            var memName = Marshal.StringToHGlobalAnsi(mangledName);
            try
            {
                using var mem = DisposableMemory.AllocHGlobal((nint)MAX_SYM_BUF);
                unsafe
                {
                    SYMBOL_INFO* psi = (SYMBOL_INFO*)mem.Value.ToPointer();
                    psi->SizeOfStruct = (uint)sizeof(SYMBOL_INFO);
                    psi->MaxNameLen = PInvoke.MAX_SYM_NAME;

                    var res = PInvoke.SymFromName(
                        PInvoke.GetCurrentProcess(),
                        new PCSTR((byte*)memName.ToPointer()), psi);

                    if (!res)
                    {
                        throw new Exception($"SymFromName(\"{mangledName}\") failed");
                    }

                    var addr = (nint)psi->Address;
                    if(addr == 0)
                    {
                        throw new Exception($"Failed to resolve \"{mangledName}\"");
                    }
                    return addr;
                }
            } finally
            {
                Marshal.FreeHGlobal(memName);
            }
        }

        private T GetFunction<T>(string mangledName)
        {
            var addr = GetSymbolAddress(mangledName);
            var func = Marshal.GetDelegateForFunctionPointer<T>(addr);
            return func;
        }

        private nint NewFlowComponent()
        {
            const int size = 0xC8;
            var node = api.cdp_new(size);
            node.AsSpan<byte>(size).Clear();
            return api.flow_component_factory(node);
        }


        public void AddComponent(string name, string code)
        {
            _needInit = true;
            var compo = NewFlowComponent();
            api.compo_put(_stringPool, name, name.Length, compo);
            api.flow_component_init(compo, _env, code, code.Length);
            _needInit = false;
        }

        private DisposableMemory GetInputBuffer(byte[] data, out nint buffer)
        {
            var mem = DisposableMemory.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, mem.Value, data.Length);
            var deltaInput = new DELTA_INPUT
            {
                uSize = (nuint)mem.Size,
                Editable = false
            };
            unsafe
            {
                deltaInput.Anonymous.lpcStart = mem.Value.ToPointer();
            }
            buffer = api.input_buffer_get(ref deltaInput);
            return mem;
        }

        private byte[] GetOutputBuffer(nint ptr)
        {
            DELTA_OUTPUT res = new DELTA_OUTPUT();
            api.output_buffer_put(ptr, ref res);

            byte[] data = new byte[res.uSize];
            unsafe
            {
                if (res.lpStart != null)
                {
                    Marshal.Copy(new nint(res.lpStart), data, 0, data.Length);
                    NativeMemory.Free(res.lpStart);
                }
            }
            return data;
        }

        private void Invoke(nint factory, ref byte[][] outputs, params byte[][] argv)
        {
            var compo = Marshal.PtrToStructure<ComponentData>(api.component_factory_obtain_component(factory));
            if (compo.Info.NumInputs != argv.Length)
            {
                throw new ArgumentOutOfRangeException($"Expected {compo.Info.NumInputs} arguments, got {argv.Length}");
            }
            /*if (compo.Info.NumOutputs != outputs.Length)
            {
                throw new ArgumentOutOfRangeException($"Expected {compo.Info.NumOutputs} output slots, got {outputs.Length}");
            }*/
            outputs = new byte[compo.Info.NumOutputs][];

            // NOTE: MUST be Native and not HGlobal due to the heap used by MSDelta
            using var argsList = DisposableMemory.AllocNative(nint.Size * argv.Length);
            using var outputsList = DisposableMemory.AllocNative(nint.Size * outputs.Length);

            var handles = new DisposableMemory[argv.Length];
            for(int i=0; i<argv.Length; i++)
            {
                if (compo.Info.InputInfo[i].Type != ComponentType.Buffer)
                {
                    throw new NotImplementedException();
                }
                handles[i] = GetInputBuffer(argv[i], out var buffer);
                Marshal.WriteIntPtr(argsList.Value + (nint.Size * i), buffer);
            }
            try
            {
                api.component_process(factory,
                    argv.Length, argsList.Value,
                    outputs.Length, outputsList.Value
                );
                for(int i=0; i<outputs.Length; i++)
                {
                    var ptr = Marshal.ReadIntPtr(outputsList.Value + (nint.Size * i));
                    var name = Enum.GetName(compo.Info.OutputInfo[i].Type) ?? ("Unknown 0x" + ((uint)compo.Info.OutputInfo[i].Type).ToString("X8"));

                    var sb = new StringBuilder();
                    sb.AppendFormat("output {0} ({1}) ", i, name);

                    switch (compo.Info.OutputInfo[i].Type)
                    {
                        case ComponentType.Buffer:
                            outputs[i] = GetOutputBuffer(ptr);
                            sb.AppendLine(Convert.ToHexString(outputs[i]));
                            break;
                        case ComponentType.Number:
                            var val = Marshal.ReadIntPtr(ptr + nint.Size + nint.Size);
                            outputs[i] = BitConverter.GetBytes(val);
                            sb.AppendFormat("Value: 0x{0:X}", val);
                            sb.AppendLine();
                            //outputs[i] = Encoding.ASCII.GetBytes($"Value: 0x{val:X}");
                            break;
                        default:
                            sb.AppendLine();
                            sb.AppendFormat("WARNING: Type {0} in output {1} not implemented", name, i);
                            sb.AppendLine();
                            Trace.WriteLine($"Type {name} in output {i} not implemented");
                            break;
                            //throw new NotImplementedException($"Type {name} in output {i} not implemented");
                    }

                    Console.Write(sb.ToString());
                    
                }
            } finally
            {
                foreach (var item in handles)
                {
                    item.Dispose();
                }
            }
        }

        public bool Call(string name, byte[][] inputs, ref byte[][] outputs)
        {
            nint compo = api.compo_get(_stringPool, name, name.Length);
            if (compo == 0) return false;
            Invoke(compo, ref outputs, inputs);
            return true;
        }

        public NativeMSDelta()
        {
            msdelta = PInvoke.LoadLibrary("msdelta.dll");
            if (msdelta.IsInvalid)
            {
                throw new InvalidOperationException("failed to load msdelta.dll");
            }
            MAX_SYM_BUF = (0
                + Marshal.SizeOf<SYMBOL_INFO>() 
                + (PInvoke.MAX_SYM_NAME * sizeof(char))
                + sizeof(ulong) - 1
            );
            LoadSymbols();
            api = new NativeApi
            {
                init = GetFunction<pfnInitDelta>("?InitDelta@@YAHXZ"),
                init_env = GetFunction<pfnInitEnv>("?Init@Environment@compo@@QEAAXXZ"),
                compo_get = GetFunction<pfnComponentGet>("?Get@?$StringMap@VComponentFactory@compo@@@compo@@QEBAPEAVComponentFactory@2@PEBD_K_N@Z"),
                compo_put = GetFunction<pfnComponentPut>("?Put@?$StringMap@VComponentFactory@compo@@@compo@@QEAAPEAVComponentFactory@2@PEBD_KPEAV32@_N@Z"),
                compo_init = GetFunction<pfnComponentInit>("?Init@Component@compo@@IEAAXPEAVComponentFactory@2@@Z"),
                compo_init_factory = GetFunction<pfnComponentInitFactory>("?Init@ComponentFactory@compo@@IEAAXPEAVEnvironment@2@_KPEBUInputPortSpec@12@1PEBK@Z"),
                cdp_new = GetFunction<pfnCdpNew>("?_New@cdp@@YAPEAX_K@Z"),
                flow_component_factory = GetFunction<pfnFlowComponentFactory>("??0FlowComponentFactory@compo@@QEAA@XZ"),
                flow_component_init = GetFunction<pfnFlowComponentInit>("?Init@FlowComponentFactory@compo@@QEAAXPEAVEnvironment@2@PEBD_K@Z"),
                component_process = GetFunction<pfnComponentProcess>("?ProcessComponent@ComponentFactory@compo@@QEAAX_KPEAPEAVObject@2@01@Z"),
                input_buffer_get = GetFunction<pfnInputBufferGet>("?GetInputBuffer@PullcapiContext@compo@@SAPEAVBufferObject@2@PEBU_DELTA_INPUT@@@Z"),
                output_buffer_put = GetFunction<pfnInputBufferPut>("?PutOutputBuffer@PullcapiContext@compo@@SAXPEAVBufferObject@2@PEAU_DELTA_OUTPUT@@@Z"),
                component_factories_put_all = GetFunction<pfnComponentFactoriesPutAll>("?PutAllComponentFactories@Environment@compo@@QEAAXXZ"),
                component_factory_obtain_component = GetFunction<pfnComponentFactoryObtainComponent>("?ObtainComponent@ComponentFactory@compo@@QEAAPEAVComponent@2@XZ"),
                gp_environment = GetSymbolAddress("?g_environment@PullcapiContext@compo@@0PEAVEnvironment@2@EA"),
                gp_applyPatchFactory = GetSymbolAddress("?g_applyPatchFactory@PullcapiContext@compo@@0PEAVComponentFactory@2@EA")
            };
            PInvoke.SymCleanup(PInvoke.GetCurrentProcess_SafeHandle());
        }
    }
}
