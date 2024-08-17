using Smx.PDBSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;
using Windows.Win32.System.Diagnostics.Debug;
using static Smx.Winter.MsDelta.NativeApi;
using static Smx.Winter.MsDelta.vtb_ComponentFactory;

namespace Smx.Winter.MsDelta
{
    internal class NativeApi
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
        public delegate void pfnOperatorDelete(nint ptr);
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

        public delegate void pfnDtor(nint ptr, nint v);
        public delegate nint pfnFactoryInternalInstantiate(nint @this);
        public delegate void pfnComponentInternalProcess(nint @this);

        public delegate nint pfnGetExclusiveInput(nint compo, nint index);
        public delegate nint pfnGetSharedInput(nint compo, nint index);

        public delegate void pfnPutOutput(nint compo, nint index, nint outputObj);
        public delegate void pfnBufferObjectInit(nint compo, nint env, InputMemoryType bufferType, nint source, nint size);

        public delegate void pfnMemoryBlockResize(nint block, nint size, CppBool copyOld);
        public delegate nint pfnBufferObjectGetStartWritable(nint block);

        public pfnInitDelta init;
        public pfnInitEnv init_env;
        public pfnComponentGet compo_get;
        public pfnComponentPut compo_put;
        public pfnComponentInit compo_init;
        public pfnComponentInitFactory compo_init_factory;
        public pfnCdpNew cdp_new;
        public pfnOperatorDelete delete;
        public pfnFlowComponentFactory flow_component_factory;
        public pfnFlowComponentInit flow_component_init;
        public pfnInputBufferGet input_buffer_get;
        public pfnInputBufferPut output_buffer_put;
        public pfnComponentProcess component_process;
        public pfnComponentFactoriesPutAll component_factories_put_all;
        public pfnComponentFactoryObtainComponent component_factory_obtain_component;
        public pfnGetExclusiveInput get_exclusive_input;
        public pfnGetSharedInput get_shared_input;
        public pfnPutOutput put_output;
        public pfnBufferObjectInit buffer_object_init;
        public pfnMemoryBlockResize memory_block_resize;
        public pfnBufferObjectGetStartWritable buffer_object_get_start_writable;
        public nint gp_environment;
        public nint gp_applyPatchFactory;
        public nint gp_bufferObject;


        public nint _env => Marshal.ReadIntPtr(gp_environment);
        public nint _privateData => Marshal.ReadIntPtr(_env + nint.Size);
        public nint _stringPool => Marshal.ReadIntPtr(_env + nint.Size + nint.Size);
        public TypedPointer<HeapMemoryManagerInstance> _mman
        {
            get
            {
                var mman = Marshal.ReadIntPtr(_env);
                return new TypedPointer<HeapMemoryManagerInstance>(mman);
            }
        }

        private MemoryAllocator? _cppAllocator = null;

        public MemoryAllocator CppAllocator
        {
            get
            {
                if(_cppAllocator == null)
                {
                    _cppAllocator = NewCppAllocator();
                }
                return _cppAllocator;
            }
        }

        public MemoryAllocator NewHeapAllocator(TypedPointer<HeapMemoryManagerInstance> mman)
        {
            return new MemoryAllocator(
                new MemoryAllocator.pfnAlloc((size) =>
                {
                    return mman.Value.Vtbl.Value.Alloc.Func(mman.Address, size);
                }),
                new MemoryAllocator.pfnFree((ptr, size) =>
                {
                    mman.Value.Vtbl.Value.Free.Func(mman.Address, ptr);
                })
            );
        }

        private MemoryAllocator NewCppAllocator()
        {
            var pfnDelete = this.delete;
            return new MemoryAllocator(
                new MemoryAllocator.pfnAlloc(this.cdp_new),
                (nint ptr, nint size) =>
                {
                    pfnDelete(ptr);
                }
            );
        }

        public MemoryHandle Alloc(nint size, bool owned = true)
        {
            var mem = this.cdp_new(size);
            mem.AsSpan<byte>((int)size).Clear();
            
            var pfnDelete = this.delete;
            return new MemoryHandle(mem, size, MemoryHandleType.Custom, owned: owned, pfnFree: (nint ptr, nint size) =>
            {
                pfnDelete(ptr);
            });
        }

        public TypedMemoryHandle<ComponentObject> CreateComponent(nint vtbl, ComponentType type)
        {
            var size = Unsafe.SizeOf<ComponentObject>();
            if(size != 32)
            {
                throw new InvalidOperationException();
            }

            var obj = MemoryHandle.AllocNative<ComponentObject>();
            obj.Value.initialized.Value = false;
            obj.Value.vtbl = vtbl;
            obj.Value.Type = type;
            obj.Write();
            return obj;
        }
    }

    public record struct FunctionPointer<T>(nint ptr) where T : Delegate
    {
        public T Func
        {
            get => Marshal.GetDelegateForFunctionPointer<T>(ptr);
            set => ptr = Marshal.GetFunctionPointerForDelegate<T>(value);
        }
    }

    internal struct vtb_ComponentFactory
    {
        public FunctionPointer<pfnDtor> dtor;
        public FunctionPointer<pfnFactoryInternalInstantiate> internal_instantiate;
    }

    internal struct vtb_Component
    {
        public FunctionPointer<pfnDtor> dtor;
        public FunctionPointer<pfnComponentInternalProcess> internal_process;
    }

    internal struct HeapMemoryManagerVtbl
    {
        public delegate nint pfnMemoryManagerAlloc(nint pThis, nint size);
        public delegate nint pfnMemoryManagerRealloc(nint pThis, TypedPointer<nint> ppMem, nint size);
        public delegate nint pfnMemoryManagerFree(nint pThis, nint pMem);

        /*  0 */ private FunctionPointer<pfnDtor> _destructor;
        /*  8 */ public FunctionPointer<pfnMemoryManagerAlloc> Alloc;
        /* 16 */ public FunctionPointer<pfnMemoryManagerRealloc> Realloc;
        /* 24 */ public FunctionPointer<pfnMemoryManagerFree> Free;
    }

    internal struct HeapMemoryManagerInstance
    {
        public TypedPointer<HeapMemoryManagerVtbl> Vtbl;
    }

    interface INativeComponent
    {
        nint Instance { get; }
    }

    interface INativeComponentFactory
    {
        INativeComponent Create(NativeApi api);
    }

    enum InputMemoryType : byte
    {
        ReadOnly = 0,
        /// <summary>
        ///  will copy input data into an owned buffer
        /// </summary>
        Editable = 1,
        Owned = 2
    }

    class MyNativeComponent : INativeComponent, IDisposable
    {
        private readonly TypedMemoryHandle<vtb_Component> vtblComponent;
        private readonly MemoryHandle instanceData;
        private bool _disposed;
        private readonly NativeApi api;

        private void Destructor(nint ptr, nint v)
        {
            Dispose();
        }

        private TypedPointer<HeapMemoryManagerInstance> GetMemoryManager(nint pCompo)
        {
            var factory = Marshal.ReadIntPtr(pCompo + nint.Size);
            var env = Marshal.ReadIntPtr(pCompo + nint.Size + nint.Size);
            var mman = Marshal.ReadIntPtr(env);
            return new TypedPointer<HeapMemoryManagerInstance>(mman);
        }

        private void InternalProcess(nint pCompo)
        {
            var outputBuffer = api.CreateComponent(api.gp_bufferObject, ComponentType.Buffer);

            var outStr = "Hello, World";
            var outBytes = Encoding.ASCII.GetBytes(outStr);

            var compo = new ComponentData(pCompo);

            var mman = compo.Info.MemoryManager;
            var mmanInstance = mman.Value;

            var memBlock = mmanInstance.Vtbl.Value.Alloc.Func(mman.Address, outBytes.Length);
            Marshal.Copy(outBytes, 0, memBlock, outBytes.Length);
            api.buffer_object_init(outputBuffer.Address, mman.Address, InputMemoryType.Owned, memBlock, outBytes.Length);
            api.put_output(pCompo, 0, outputBuffer.Address);
        }


        public MyNativeComponent(NativeApi api)
        {
            this.api = api;
            var allocator = api.CppAllocator;

            instanceData = allocator.Alloc(0x20);
            vtblComponent = allocator.Alloc<vtb_Component>();
            vtblComponent.Value.dtor.Func = this.Destructor;
            vtblComponent.Value.internal_process.Func = this.InternalProcess;
            vtblComponent.Write();
            _disposed = false;

            // write vtable
            Marshal.WriteIntPtr(instanceData.Address, vtblComponent.Address);

            _disposed = false;
        }

        public nint Instance => instanceData.Address;

        public void Dispose()
        {
            if (_disposed) return;
            instanceData.Dispose();
            vtblComponent.Dispose();
            _disposed = true;
        }
    }

    class MyNativeComponentFactory : INativeComponentFactory
    {
        public INativeComponent Create(NativeApi api)
        {
            return new MyNativeComponent(api);
        }
    }

    class NativeComponentFactory : IDisposable
    {
        private readonly TypedMemoryHandle<vtb_ComponentFactory> vtblFactory;
        private readonly NativeApi api;
        private readonly INativeComponentFactory managedFactory;
        private readonly MemoryHandle instanceData;
        private bool _disposed;

        public nint Instance => instanceData.Address;

        private void Destructor(nint ptr, nint v)
        {
            Dispose();
        }

        private nint InternalInstantiate(nint ptrFactory)
        {
            var compo = this.managedFactory.Create(api);
            this.api.compo_init(compo.Instance, ptrFactory);
            return compo.Instance;
        }

        public void Dispose()
        {
            if (_disposed) return;
            instanceData.Dispose();
            vtblFactory.Dispose();
            _disposed = true;
        }

        public NativeComponentFactory(NativeApi api, INativeComponentFactory factory)
        {
            this.api = api;
            this.managedFactory = factory;

            var allocator = api.CppAllocator;
            instanceData = allocator.Alloc(0xC0);
            vtblFactory = allocator.Alloc<vtb_ComponentFactory>();
            vtblFactory.Value.dtor.Func = this.Destructor;
            vtblFactory.Value.internal_instantiate.Func = this.InternalInstantiate;
            vtblFactory.Write();
            _disposed = false;
            
            // write vtable
            Marshal.WriteIntPtr(instanceData.Address, vtblFactory.Address);
        }
    }

    interface IComponent
    {
        void InternalProcess();
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

    internal struct ComponentInputPortSpec
    {
        public ComponentType Type;
        // if true, the argument should not be passed in by a call
        public CppBool NotUserArgument;
    }

    internal struct ComponentInputInfoArray(nint ptr)
    {
        public ComponentInputPortSpec this[int index]
        {
            get => Marshal.PtrToStructure<ComponentInputPortSpec>(ptr + (nint.Size * index));
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
        public TypedPointer<TypedPointer<HeapMemoryManagerInstance>> memoryManager;
        private ulong numInputs;
        public ComponentInputInfoArray InputInfo;
        private ulong numOutputs;
        public ComponentOutputInfoArray OutputInfo;

        public int NumInputs => (int)numInputs;
        public int NumOutputs => (int)numOutputs;

        public TypedPointer<HeapMemoryManagerInstance> MemoryManager => memoryManager.Value;
    }

    public struct CppBool()
    {
        private byte _value;
        public bool Value
        {
            get => _value != 0;
            set => _value = (byte)((value) ? 1 : 0);
        }

        public static CppBool True => new CppBool { Value = true };
        public static CppBool False => new CppBool { Value = false };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    internal struct ComponentObject
    {
        public nint vtbl;
        public CppBool initialized;
        public ComponentType Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] _u0; // first byte is written to in ctors, apparently
        public ulong _u1;

        public ComponentObject(nint ptr)
        {
            this = Marshal.PtrToStructure<ComponentObject>(ptr);
        }
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

        public ref ComponentInfo Info => ref info.Value;
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

        private bool _needInit
        {
            get => Marshal.ReadByte(api._privateData + 8248) == 1;
            set => Marshal.WriteByte(api._privateData + 8248, (byte)(value ? 1 : 0));
        }

        private nint GetSymbolAddress(string mangledName)
        {
            var memName = Marshal.StringToHGlobalAnsi(mangledName);
            try
            {
                using var mem = MemoryHandle.AllocHGlobal((nint)MAX_SYM_BUF);
                unsafe
                {
                    SYMBOL_INFO* psi = (SYMBOL_INFO*)mem.Address.ToPointer();
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
            api.compo_put(api._stringPool, name, name.Length, compo);
            api.flow_component_init(compo, api._env, code, code.Length);
            _needInit = false;
        }

        private MemoryHandle GetInputBuffer(byte[] data, out nint buffer)
        {
            var mem = MemoryHandle.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, mem.Address, data.Length);
            var deltaInput = new DELTA_INPUT
            {
                uSize = (nuint)mem.Size,
                Editable = false
            };
            unsafe
            {
                deltaInput.Anonymous.lpcStart = mem.Address.ToPointer();
            }
            buffer = api.input_buffer_get(ref deltaInput);
            return mem;
        }

        private byte[] GetOutputBuffer(nint ptr)
        {
            // NOTE: must use msdelta allocator or we will crash on realloc 
            var mman = api._mman;

            var alloc = api.NewHeapAllocator(mman);
            using var res = alloc.Alloc<DELTA_OUTPUT>();
            
            api.output_buffer_put(ptr, ref res.Value);

            byte[] data = new byte[res.Value.uSize];
            unsafe
            {
                if (res.Value.lpStart != null)
                {
                    Marshal.Copy(new nint(res.Value.lpStart), data, 0, data.Length);
                    NativeMemory.Free(res.Value.lpStart);
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
            using var argsList = MemoryHandle.AllocNative(nint.Size * argv.Length);
            using var outputsList = MemoryHandle.AllocNative(nint.Size * outputs.Length);

            var handles = new MemoryHandle[argv.Length];
            for(int i=0; i<argv.Length; i++)
            {
                if (compo.Info.InputInfo[i].Type != ComponentType.Buffer)
                {
                    throw new NotImplementedException();
                }
                handles[i] = GetInputBuffer(argv[i], out var buffer);
                Marshal.WriteIntPtr(argsList.Address + (nint.Size * i), buffer);
            }
            try
            {
                api.component_process(factory,
                    argv.Length, argsList.Address,
                    outputs.Length, outputsList.Address
                );
                for(int i=0; i<outputs.Length; i++)
                {
                    var ptr = Marshal.ReadIntPtr(outputsList.Address + (nint.Size * i));
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
            nint compo = api.compo_get(api._stringPool, name, name.Length);
            if (compo == 0) return false;
            Invoke(compo, ref outputs, inputs);
            return true;
        }

        private static HashSet<object> keepAlive = new HashSet<object>();

        private void TestNativeComponent()
        {
            var myFac = new MyNativeComponentFactory();

            var name = "NatComp";
            var fac = new NativeComponentFactory(api, myFac);
            var compoFactory = fac.Instance;

            _needInit = true;
            api.compo_put(api._stringPool, name, name.Length, compoFactory);

            const int nInputs = 2;
            const int nOutputs = 1;

            var inputSize = Marshal.SizeOf<ComponentInputPortSpec>() * nInputs;
            var outputSize = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(ComponentType))) * nOutputs;

            using var arena = MemoryHandle.AllocHGlobal(inputSize + outputSize);
            var spanIn = arena.GetSpan<ComponentInputPortSpec>();
            spanIn[0] = new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False };
            spanIn[1] = new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False };
            

            var spanOut = arena.Span.Slice(inputSize).Cast<ComponentType>();
            spanOut[0] = ComponentType.Buffer;

            api.compo_init_factory(compoFactory, api._env,
                nInputs, arena.Address,
                nOutputs, arena.Address + inputSize);

            _needInit = false;


            var inputs = new byte[][]
            {
                Encoding.ASCII.GetBytes("Buf 1"),
                Encoding.ASCII.GetBytes("Buf 2")
            };


            AddComponent("NatWrap", @"
            buf0( PassBuffer ): input[ 0 ];
            buf1( PassBuffer ): input[ 1 ];
            output( NatComp ): buf0[ 0 ], buf1[ 0 ];
            ");

            var outputs = new byte[1][];
            Call("NatWrap", inputs, ref outputs);
        }

        private void CurrentDomain_ProcessExit(object? sender, EventArgs e)
        {
            /** flush the console so native destructors (called later) can print **/
            Console.Error.Flush();
            Console.Out.Flush();
        }

        private static void CheckMsDeltaVersion()
        {
            var dllPath = Path.Combine(new WindowsSystem().SystemRoot, "System32", "msdelta.dll");
            var ver = FileVersionInfo.GetVersionInfo(dllPath);
            var verString = string.Format("{0}.{1}.{2}.{3}", ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            if (verString != "5.0.1.1")
            {
                throw new NotSupportedException($"This msdelta version ({verString}) is unsupported. Remove this exception at your own risk");
            }
        }

        public NativeMSDelta()
        {
            CheckMsDeltaVersion();


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
                delete = GetFunction<pfnOperatorDelete>("??3@YAXPEAX@Z"),
                flow_component_factory = GetFunction<pfnFlowComponentFactory>("??0FlowComponentFactory@compo@@QEAA@XZ"),
                flow_component_init = GetFunction<pfnFlowComponentInit>("?Init@FlowComponentFactory@compo@@QEAAXPEAVEnvironment@2@PEBD_K@Z"),
                component_process = GetFunction<pfnComponentProcess>("?ProcessComponent@ComponentFactory@compo@@QEAAX_KPEAPEAVObject@2@01@Z"),
                input_buffer_get = GetFunction<pfnInputBufferGet>("?GetInputBuffer@PullcapiContext@compo@@SAPEAVBufferObject@2@PEBU_DELTA_INPUT@@@Z"),
                output_buffer_put = GetFunction<pfnInputBufferPut>("?PutOutputBuffer@PullcapiContext@compo@@SAXPEAVBufferObject@2@PEAU_DELTA_OUTPUT@@@Z"),
                component_factories_put_all = GetFunction<pfnComponentFactoriesPutAll>("?PutAllComponentFactories@Environment@compo@@QEAAXXZ"),
                component_factory_obtain_component = GetFunction<pfnComponentFactoryObtainComponent>("?ObtainComponent@ComponentFactory@compo@@QEAAPEAVComponent@2@XZ"),
                get_exclusive_input = GetFunction<pfnGetExclusiveInput>("?GetExclusiveInput@Component@compo@@IEAAPEAVObject@2@_K@Z"),
                get_shared_input = GetFunction<pfnGetSharedInput>("?GetSharedInput@Component@compo@@IEAAPEBVObject@2@_K@Z"),
                put_output = GetFunction<pfnPutOutput>("?PutOutput@Component@compo@@IEAAX_KPEAVObject@2@@Z"),
                buffer_object_init = GetFunction<pfnBufferObjectInit>("?Init@BufferObject@compo@@AEAAXPEAVMemoryManager@2@W4Type@MemoryBlock@2@PEAE_K@Z"),
                memory_block_resize = GetFunction<pfnMemoryBlockResize>("?Resize@MemoryBlock@compo@@SAXPEAPEAV12@_K_N@Z"),
                buffer_object_get_start_writable = GetFunction<pfnBufferObjectGetStartWritable>("?GetStartWriteable@BufferObject@compo@@QEAAPEAEXZ"),
                gp_environment = GetSymbolAddress("?g_environment@PullcapiContext@compo@@0PEAVEnvironment@2@EA"),
                gp_applyPatchFactory = GetSymbolAddress("?g_applyPatchFactory@PullcapiContext@compo@@0PEAVComponentFactory@2@EA"),
                gp_bufferObject = GetSymbolAddress("??_7BufferObject@compo@@6B@")
            };
            PInvoke.SymCleanup(PInvoke.GetCurrentProcess_SafeHandle());

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }
    }
}
