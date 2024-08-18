using Smx.PDBSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.ApplicationInstallationAndServicing;
using Windows.Win32.System.Diagnostics.Debug;
using static Smx.Winter.MsDelta.NativeApi;
using static Smx.Winter.MsDelta.vtb_ComponentFactory;

namespace Smx.Winter.MsDelta
{
    internal struct LinkedListNode
    {
        public TypedPointer<LinkedListNode> Next;
        public nint Name;
        public nint Data;
    }


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

        public delegate void pfnDtor(nint ptr, byte flags);
        public delegate nint pfnFactoryInternalInstantiate(nint @this);
        public delegate void pfnComponentInternalProcess(nint @this);

        public delegate nint pfnGetExclusiveInput(nint compo, nint index);
        public delegate nint pfnGetSharedInput(nint compo, nint index);

        public delegate void pfnPutOutput(TypedPointer<ComponentData> compo, nint index, nint outputObj);
        public delegate void pfnBufferObjectInit(
            nint compo,
            TypedPointer<HeapMemoryManagerInstance> mman,
            InputMemoryType bufferType,
            nint source, nint size);

        public delegate void pfnMemoryBlockResize(nint block, nint size, CppBool copyOld);
        public delegate nint pfnBufferObjectGetStartWritable(nint block);

        public delegate nint pfnStringMapInternalLookup(
            nint stringMap,
            [MarshalAs(UnmanagedType.LPStr)]
            string name,
            nint nameLength,
            CppBool forWrite
        );

        public delegate TypedPointer<BufferObject> pfnRiftTableSerialize(
            TypedPointer<TypedPointer<HeapMemoryManagerInstance>> mman,
            nint pRiftTable
        );
        public delegate TypedPointer<BufferObject> pfnCompositeFormatSerialize(
            TypedPointer<TypedPointer<HeapMemoryManagerInstance>> mman,
            nint pCompositeFormat
        );

        public required pfnInitDelta init;
        public required pfnInitEnv init_env;
        public required pfnComponentGet compo_get;
        public required pfnComponentPut compo_put;
        public required pfnComponentInit compo_init;
        public required pfnComponentInitFactory compo_init_factory;
        public required pfnCdpNew cdp_new;
        public required pfnOperatorDelete delete;
        public required pfnFlowComponentFactory flow_component_factory;
        public required pfnFlowComponentInit flow_component_init;
        public required pfnInputBufferGet input_buffer_get;
        public required pfnInputBufferPut output_buffer_put;
        public required pfnComponentProcess component_process;
        public required pfnComponentFactoriesPutAll component_factories_put_all;
        public required pfnComponentFactoryObtainComponent component_factory_obtain_component;
        public required pfnGetExclusiveInput get_exclusive_input;
        public required pfnGetSharedInput get_shared_input;
        public required pfnPutOutput put_output;
        public required pfnBufferObjectInit buffer_object_init;
        public required pfnMemoryBlockResize memory_block_resize;
        public required pfnBufferObjectGetStartWritable buffer_object_get_start_writable;
        public required pfnStringMapInternalLookup stringmap_internal_lookup;
        public required pfnRiftTableSerialize rift_table_serialize;
        public required pfnCompositeFormatSerialize composite_format_serialize;
        public required nint gp_environment;
        public required nint gp_applyPatchFactory;
        public required nint gp_bufferObject;

        public void DestructObject(nint obj)
        {
            // read pointer to vtable
            nint vtbl = Marshal.ReadIntPtr(obj);
            // read first vtable entry (destructor)
            nint dtor = Marshal.ReadIntPtr(vtbl);
            var fnDtor = Marshal.GetDelegateForFunctionPointer<pfnDtor>(dtor);
            fnDtor(obj, 0xFF);
        }

        public TypedPointer<LinkedListNode> StringMapLookup(string name, bool forInsertion)
        {
            var ptr = stringmap_internal_lookup(_stringPool, name, name.Length, CppBool.FromBool(forInsertion));
            return new TypedPointer<LinkedListNode>(ptr);
        }


        public nint _env => Marshal.ReadIntPtr(gp_environment);
        public nint _privateData => Marshal.ReadIntPtr(_env + nint.Size);
        public nint _stringPoolRef => Marshal.ReadIntPtr(_env + nint.Size + nint.Size);
        public nint _stringPool => Marshal.ReadIntPtr(_stringPoolRef);

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
                    return mman.Value.Vtbl.Value.Alloc.Func(mman, size);
                }),
                new MemoryAllocator.pfnFree((ptr, size) =>
                {
                    mman.Value.Vtbl.Value.Free.Func(mman, ptr);
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

    internal struct BufferObjectInfo
    {
        public nint _u0;
        public nint _u1;
        public nint _u2;
        public nint _u3;
        public nint Data;
        public nint Size;

    }

    internal struct BufferObject
    {
        public nint _u0;
        public nint _u1;
        public nint _u2;
        public TypedPointer<BufferObjectInfo> Info;

        public byte[] ReadByteArray()
        {
            if (Info.Address == 0) throw new InvalidOperationException();
            var data = new byte[Info.Value.Size];
            Marshal.Copy(Info.Value.Data, data, 0, data.Length);
            return data;
        }

        public string ReadString(Encoding? encoding = null)
        {
            var encToUse = (encoding == null) ? Encoding.ASCII : encoding;
            return encToUse.GetString(ReadByteArray());
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
        public delegate nint pfnMemoryManagerAlloc(TypedPointer<HeapMemoryManagerInstance> pThis, nint size);
        public delegate nint pfnMemoryManagerRealloc(TypedPointer<HeapMemoryManagerInstance> pThis, TypedPointer<nint> ppMem, nint size);
        public delegate nint pfnMemoryManagerFree(TypedPointer<HeapMemoryManagerInstance> pThis, nint pMem);

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
        void InternalProcess(TypedPointer<ComponentData> pCompo);
    }

    interface INativeComponentFactory
    {
        ComponentInputPortSpec[] InputSpecs { get; }
        ComponentType[] OutputSpecs { get; }
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

    abstract class NativeComponentBase : INativeComponent, IDisposable
    {
        private readonly TypedMemoryHandle<vtb_Component> vtblComponent;
        private readonly MemoryHandle instanceData;
        private bool _disposed;
        protected readonly NativeApi api;

        public nint Instance => instanceData.Address;

        private void Destructor(nint ptr, byte flags)
        {
            Dispose();
        }

        protected TypedPointer<T> GetSharedInput<T>(int index) where T : struct
        {
            return new TypedPointer<T>(api.get_shared_input(Instance, index));
        }

        private TypedPointer<HeapMemoryManagerInstance> GetGlobalMemoryManager(nint pCompo)
        {
            var factory = Marshal.ReadIntPtr(pCompo + nint.Size);
            var env = Marshal.ReadIntPtr(pCompo + nint.Size + nint.Size);
            var mman = Marshal.ReadIntPtr(env);
            return new TypedPointer<HeapMemoryManagerInstance>(mman);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            instanceData.Dispose();
            vtblComponent.Dispose();
        }

        private void InvokeInternalProcess(nint pCompo)
        {
            var compo = new TypedPointer<ComponentData>(pCompo);
            this.InternalProcess(compo);
        }

        public NativeComponentBase(NativeApi api)
        {
            this.api = api;
            var allocator = api.CppAllocator;

            instanceData = allocator.Alloc(0x20);
            vtblComponent = allocator.Alloc<vtb_Component>();
            vtblComponent.Value.dtor.Func = this.Destructor;
            vtblComponent.Value.internal_process.Func = this.InvokeInternalProcess;
            vtblComponent.Write();
            _disposed = false;

            // write vtable
            Marshal.WriteIntPtr(instanceData.Address, vtblComponent.Address);
        }

        public abstract void InternalProcess(TypedPointer<ComponentData> pCompo);
    }

    class MyNativeComponent : NativeComponentBase
    {
        public override void InternalProcess(TypedPointer<ComponentData> pCompo)
        {
            var outputBuffer = api.CreateComponent(api.gp_bufferObject, ComponentType.Buffer);

            var outStr = "Hello, World";
            var outBytes = Encoding.ASCII.GetBytes(outStr);

            var mman = pCompo.Value.Info.MemoryManager;
            var mmanInstance = mman.Value;

            var memBlock = mmanInstance.Vtbl.Value.Alloc.Func(mman, outBytes.Length);
            Marshal.Copy(outBytes, 0, memBlock, outBytes.Length);
            api.buffer_object_init(outputBuffer.Address, mman, InputMemoryType.Owned, memBlock, outBytes.Length);
            api.put_output(pCompo, 0, outputBuffer.Address);
        }


        public MyNativeComponent(NativeApi api) : base(api)
        {
        }
    }

    class SharedUtil
    {
        private const string DEBUG_DIR = "debug";

        public static void WriteDebugFile(string fileName, byte[] data)
        {
            Directory.CreateDirectory(DEBUG_DIR);
            var path = Path.Combine(DEBUG_DIR, fileName);
            File.WriteAllBytes(path, data);
        }
    }

    class MyDebugWriteFileComponent : NativeComponentBase
    {
        public MyDebugWriteFileComponent(NativeApi api) : base(api)
        {
        }

        public override void InternalProcess(TypedPointer<ComponentData> pCompo)
        {
            var inputFilename = GetSharedInput<BufferObject>(0);
            var inputBuffer = GetSharedInput<BufferObject>(1);
            
            var fileName = inputFilename.Value.ReadString();
            var buffer = inputBuffer.Value.ReadByteArray();

            SharedUtil.WriteDebugFile(fileName, buffer);
        }
    }

    class MyDebugWriteFileRiftTableComponent : NativeComponentBase
    {
        public MyDebugWriteFileRiftTableComponent(NativeApi api) : base(api)
        {
        }

        public override void InternalProcess(TypedPointer<ComponentData> pCompo)
        {
            var inputFilename = GetSharedInput<BufferObject>(0);
            var inputRiftTable = GetSharedInput<nint>(1);

            var buffer = api.rift_table_serialize(
                pCompo.Value.Info.MemoryManagerRef,
                inputRiftTable.Address
            );
            SharedUtil.WriteDebugFile(
                inputFilename.Value.ReadString(),
                buffer.Value.ReadByteArray()
            );
            api.DestructObject(buffer.Address);
        }
    }

    class MyDebugWriteFileCompositeFormatComponent : NativeComponentBase
    {
        public MyDebugWriteFileCompositeFormatComponent(NativeApi api) : base(api)
        {
        }

        public override void InternalProcess(TypedPointer<ComponentData> pCompo)
        {
            var inputFilename = GetSharedInput<BufferObject>(0);
            var inputCompositeFormat = GetSharedInput<nint>(1);

            var buffer = api.composite_format_serialize(
                pCompo.Value.Info.MemoryManagerRef,
                inputCompositeFormat.Address
            );
            SharedUtil.WriteDebugFile(
                inputFilename.Value.ReadString(),
                buffer.Value.ReadByteArray()
            );
            api.DestructObject(buffer.Address);
        }
    }


    class MyDebugWriteFileCompositeFormatFactory : INativeComponentFactory
    {
        public ComponentInputPortSpec[] InputSpecs => [
            new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False },
            new ComponentInputPortSpec { Type = ComponentType.CompositeFormat, NotUserArgument = CppBool.False },
        ];

        public ComponentType[] OutputSpecs => [];

        public INativeComponent Create(NativeApi api)
        {
            return new MyDebugWriteFileCompositeFormatComponent(api);
        }
    }

    class MyDebugWriteFileRiftTableFactory : INativeComponentFactory
    {
        public ComponentInputPortSpec[] InputSpecs => [
            new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False },
            new ComponentInputPortSpec { Type = ComponentType.RiftTable, NotUserArgument = CppBool.False }
        ];

        public ComponentType[] OutputSpecs => [];

        public INativeComponent Create(NativeApi api)
        {
            return new MyDebugWriteFileRiftTableComponent(api);
        }
    }

    class MyDebugWriteFileFactory : INativeComponentFactory
    {
        public ComponentInputPortSpec[] InputSpecs => [
            new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False },
            new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False }
        ];

        public ComponentType[] OutputSpecs => [];

        public INativeComponent Create(NativeApi api)
        {
            return new MyDebugWriteFileComponent(api);
        }
    }

    class MyNativeComponentFactory : INativeComponentFactory
    {
        public ComponentInputPortSpec[] InputSpecs
        {
            get
            {
                return
                [
                    new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False },
                    new ComponentInputPortSpec { Type = ComponentType.Buffer, NotUserArgument = CppBool.False }
                ];
            }
        }

        public ComponentType[] OutputSpecs
        {
            get
            {
                return [
                    ComponentType.Buffer
                ];
            }
        }

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

        private void Destructor(nint ptr, byte flags)
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

            InitializeFactory();
        }

        private void InitializeFactory()
        {
            var nInputs = managedFactory.InputSpecs.Length;
            var nOutputs = managedFactory.OutputSpecs.Length;

            var inputSize = Marshal.SizeOf<ComponentInputPortSpec>() * nInputs;
            var outputSize = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(ComponentType))) * nOutputs;

            using var arena = MemoryHandle.AllocHGlobal(inputSize + outputSize);
            
            var spanIn = arena.GetSpan<ComponentInputPortSpec>();
            for(int i=0; i<nInputs; i++)
            {
                spanIn[i] = managedFactory.InputSpecs[i];
            }

            var spanOut = arena.Span.Slice(inputSize).Cast<ComponentType>();
            for(int i=0; i<nOutputs; i++)
            {
                spanOut[i] = managedFactory.OutputSpecs[i];
            }

            api.compo_init_factory(Instance, api._env,
                nInputs, arena.Address,
                nOutputs, arena.Address + inputSize);
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
        public TypedPointer<TypedPointer<HeapMemoryManagerInstance>> MemoryManagerRef;
        private ulong numInputs;
        public ComponentInputInfoArray InputInfo;
        private ulong numOutputs;
        public ComponentOutputInfoArray OutputInfo;

        public int NumInputs => (int)numInputs;
        public int NumOutputs => (int)numOutputs;

        public TypedPointer<HeapMemoryManagerInstance> MemoryManager => MemoryManagerRef.Value;
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

        public static CppBool FromBool(bool v)
        {
            return v ? CppBool.True : CppBool.False;
        }
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

        public nint MakeFlowComponent(string code)
        {
            var compo = NewFlowComponent();
            api.flow_component_init(compo, api._env, code, code.Length);
            return compo;
        }


        public void AddComponent(string name, string code)
        {
            _needInit = true;
            var compo = MakeFlowComponent(code);
            api.compo_put(api._stringPoolRef, name, name.Length, compo);
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
            nint compo = api.compo_get(api._stringPoolRef, name, name.Length);
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
            api.compo_put(api._stringPoolRef, name, name.Length, compoFactory);
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

        private void OverwriteFactoryVtable(nint dst, nint src)
        {
            unsafe
            {
                NativeMemory.Copy(src.ToPointer(), dst.ToPointer(), 0xC0);
            }
        }

        private bool StringMapGetNode(string name, bool forWrite, out TypedPointer<LinkedListNode> nodeOut)
        {
            nodeOut = new TypedPointer<LinkedListNode>(0);

            var nameLoc = api.StringMapLookup(name, forWrite);
            if (nameLoc.Address == 0) return false;

            var listHead = Marshal.ReadIntPtr(api._stringPoolRef + nint.Size);
            if (listHead == 0) return false;

            var node = new TypedPointer<LinkedListNode>(listHead);
            while (node.Value.Name != nameLoc.Address)
            {
                node = node.Value.Next;
            }

            nodeOut = node;
            return true;
        }

        private void PatchBuiltin(string name, INativeComponentFactory factory)
        {
            if(!StringMapGetNode(name, false, out var node))
            {
                throw new InvalidOperationException($"Cannot get builtin with name \"{name}\"");
            }
            var natFactory = new NativeComponentFactory(api, factory);
            OverwriteFactoryVtable(node.Value.Data, natFactory.Instance);
            node.Value.Data = natFactory.Instance;
        }

        private void PatchBuiltins()
        {
            PatchBuiltin("DebugWriteFile", new MyDebugWriteFileFactory());
            PatchBuiltin("DebugWriteFileCompositeFormat", new MyDebugWriteFileCompositeFormatFactory());
            PatchBuiltin("DebugWriteFileRiftTable", new MyDebugWriteFileRiftTableFactory());
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
                stringmap_internal_lookup = GetFunction<pfnStringMapInternalLookup>("?InternalLookup@StringPool@compo@@AEAAPEBDPEBD_K_N@Z"),
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
                rift_table_serialize = GetFunction<pfnRiftTableSerialize>("?Serialize@RiftTable@compo@@SAPEAVBufferObject@2@PEAVEnvironment@2@PEBVObject@2@@Z"),
                composite_format_serialize = GetFunction<pfnCompositeFormatSerialize>("?Serialize@CompositeFormat@compo@@SAPEAVBufferObject@2@PEAVEnvironment@2@PEBVObject@2@@Z"),
                gp_environment = GetSymbolAddress("?g_environment@PullcapiContext@compo@@0PEAVEnvironment@2@EA"),
                gp_applyPatchFactory = GetSymbolAddress("?g_applyPatchFactory@PullcapiContext@compo@@0PEAVComponentFactory@2@EA"),
                gp_bufferObject = GetSymbolAddress("??_7BufferObject@compo@@6B@")
            };
            PInvoke.SymCleanup(PInvoke.GetCurrentProcess_SafeHandle());
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            //TestNativeComponent();
            PatchBuiltins();
        }
    }
}
