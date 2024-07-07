/**
 * @file msdelta.c
 * @author Stefano Moioli <smxdev4@gmail.com>
 * @brief 
 * @version 0.1
 * @date 2024-07-06
 * 
 * @copyright Copyright (c) 2024
 * 
 */

#include "msdelta.h"
#include <errhandlingapi.h>
#include <heapapi.h>
#include <libloaderapi.h>
#include <minwindef.h>
#include <processthreadsapi.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdint.h>
#include <MinHook.h>
#include <dbghelp.h>
#include <assert.h>
#include <winnt.h>

#include "common.h"

typedef uint64_t QWORD;

typedef struct compoBufferObject compoBufferObject;
typedef struct compoObject compoObject;

typedef void (*pfnInitDelta)(void);
typedef void (*pfnInitEnv)();

typedef void *(*pfnComponentGet)(void *env, const char *name, long nameLength);
typedef void (*pfnComponentPut)(void *env, const char *name, long nameLength, void *compo);
typedef void (*pfnComponentInit)(void *compo, void *compo_factory);

typedef compoBufferObject *(*pfnInputBufferGet)(const DELTA_INPUT *input);
typedef void (*pfnInputBufferPut)(compoBufferObject *obj, DELTA_OUTPUT *output);

typedef void (*pfnComponentFactoriesPutAll)(void *env);

typedef void *(*pfnCdpNew)(size_t size);
typedef void *(*pfnFlowComponentFactory)(void *cdp);
typedef void (*pfnFlowComponentInit)(void *compo, void *env, const char *code, int code_len);
typedef void (*pfnComponentProcess)(void *factory,
	long numInputs,
	compoObject **inputArgs,
	long numOutputs,
	compoObject **results);

typedef void (*pfnComponentInitFactory)(
	void *compo, void *env,
	int numInputs, int *inputTypes,
	int numOutputs, int *outputTypes
);

typedef struct {
	pfnInitDelta init;
	pfnInitEnv init_env;
	pfnComponentGet compo_get;
	pfnComponentPut compo_put;
	pfnComponentInit compo_init;
	pfnComponentInitFactory compo_init_factory;
	pfnCdpNew cdp_new;
	pfnFlowComponentFactory flow_component_factory;
	pfnFlowComponentInit flow_component_init;
	pfnInputBufferGet input_buffer_get;
	pfnInputBufferPut output_buffer_put;
	pfnComponentProcess component_process;
	pfnComponentFactoriesPutAll component_factories_put_all;
	void ***gp_environment;
	void ***gp_applyPatchFactory;
	void *__API_END;
} MSDELTA_API;

static MSDELTA_API gMsdelta = { 0 };
#define API_GET() (&gMsdelta)

const char* symbolSearchPath = "SRV*C:\\WINDOWS\\TEMP*https://msdl.microsoft.com/download/symbols";

void LoadSymbols(HMODULE hModule){
	if(hModule == NULL){
		hModule = GetModuleHandle(TEXT("msdelta.dll"));
	}

	SymSetOptions(SYMOPT_DEFERRED_LOADS | SYMOPT_DEBUG);
	if(!SymInitialize(GetCurrentProcess(), symbolSearchPath, FALSE)){
		fprintf(stderr, "SymInitialize() failed\n");
		return;
	}
	if(!SymLoadModuleEx(
		GetCurrentProcess(),
		NULL,
		TEXT("msdelta.dll"),
		NULL, 
		(DWORD64)hModule,
		0, NULL, 0
	)){
		fprintf(stderr, "SymLoadModuleEx() failed (0x%08lX)\n", GetLastError());
		return;
	}
}

void* SymbolGet(const char *mangledName){
    DWORD64 buffer[(sizeof(SYMBOL_INFO) + MAX_SYM_NAME * sizeof(TCHAR) + sizeof(DWORD64) - 1) / sizeof(DWORD64)] = {0};
	SYMBOL_INFO *si = (SYMBOL_INFO *)buffer;
	si->SizeOfStruct = sizeof(*si);
	si->MaxNameLen = MAX_SYM_NAME;
	if(SymFromName(GetCurrentProcess(), mangledName, si)){
		return (void *)si->Address;
		//printf("%s: 0x%llx\n", si->Name, si->Address);
	} else {
		printf("FAILURE: 0x%lx\n", GetLastError());
	}
	return NULL;
}

typedef void (*pfnDtor)(void *ptr, size_t v);
typedef void *(*pfnFactoryInternalInstantiate)(void *this);
typedef void (*pfnComponentInternalProcess)(void *this);
typedef void *(*pfnObjectCreateCopy)(void *this);

struct vtb_ComponentFactory {
	pfnDtor dtor;
	pfnFactoryInternalInstantiate internal_instantiate;
};

struct vtb_Component {
	pfnDtor dtor;
	pfnComponentInternalProcess internal_process;
};

struct vtb_Object {
	pfnDtor dtor;
	pfnObjectCreateCopy create_copy;
};

struct vtb_ComponentFactory myNativeComponentFactory;
struct vtb_Component myNativeComponent;

typedef struct {
	uintptr_t dummy;
} myNativeComponent_instanceData;

void *myNativeComponentFactory_ctor(void *this){
	void **mem = (void **)this;
	mem[0] = &myNativeComponentFactory;
	return mem;
}

void myNativeComponentFactory_dtor(void *ptr, size_t v){
	puts("FACTORY DTOR");
}
void *myNativeComponentFactory_InternalInstantiate(void *this){
	puts("FACTORY INVOKED!");

	MSDELTA_API *api = API_GET();
	void **instance = api->cdp_new(32);
	memset(instance, 0x0, 32);

	// write vtable
	instance[0] = &myNativeComponent;

	api->compo_init(instance, this);
	return instance;
}

void myNativeComponent_dtor(void *ptr, size_t v){
	puts("COMPONENT DTOR");
}
void myNativeComponent_InternalProcess(void *this){
	puts("COMPONENT INVOKED!");
}


const char szMyNativeComponent[] = "ApplySmxNative";

const char szMyComponent[] = "ApplySmx";
const char szMyComponentCode[] =
	// debug = DebugWriteFile("dummy.bin", input[0])
    "debug( DebugWriteFile ) : \"dummy.bin\", input [ 0 ];"
	// helloWorld = TToAscii("Hello World from MsDelta Flow Scripting!");
	"helloWorld( TToAscii ): \"Hello World from MsDelta Flow Scripting!\";"
	// output = helloWorld[0]
	"output: helloWorld[ 0 ];"
;


#ifdef WITH_DLLINIT_HOOK
pfnComponentFactoriesPutAll o_component_factories_put_all = NULL;
#endif

void add_components(void *env){

	MSDELTA_API *api = &gMsdelta;
	void *component_flow = NULL;
	{
		void *node = api->cdp_new(0xC8);
		memset(node, 0x0, 0xC8);
		component_flow = api->flow_component_factory(node);
	}

	void *stringPool = (*api->gp_environment)[2];

	// register flow component
	api->compo_put(stringPool, 
		szMyComponent, 
		sizeof(szMyComponent) - 1,
		component_flow);
	api->flow_component_init(
		component_flow, env,
		szMyComponentCode,
		sizeof(szMyComponentCode) - 1);

	// register native component
	void *component_native = NULL;
	{
		void *node = api->cdp_new(0xC0 + sizeof(myNativeComponent_instanceData));
		memset(node, 0x00, 0xC0 + sizeof(myNativeComponent_instanceData));
		component_native = myNativeComponentFactory_ctor(node);
	}

	api->compo_put(stringPool,
		szMyNativeComponent,
		sizeof(szMyNativeComponent) - 1,
		component_native
	);

#if BROKEN
	int inputTypes[] = { 10 };
	int outputTypes[] = { 10 };

	api->compo_init_factory(
		component_native, env,
		0, NULL,
		0, NULL
	);
#endif
}

uint8_t get_initialized_flag(){
	MSDELTA_API *api = &gMsdelta;
	uintptr_t v1 = *(uintptr_t *)((*api->gp_environment) + 1);
	return *(uint8_t *)(v1 + 8248);
}

void set_initialized_flag(uint8_t value){
	MSDELTA_API *api = &gMsdelta;
	uintptr_t v1 = *(uintptr_t *)((*api->gp_environment) + 1);
	*(uint8_t *)(v1 + 8248) = value;
}

#ifdef WITH_DLLINIT_HOOK
void my_component_factories_put_all(void *env){
	puts("!INVOKED");
	puts("orig call");
	o_component_factories_put_all(env);
	puts("<--");

	add_components(env);
	puts("<--");


	printf("flag: %x\n", get_initialized_flag());
}
#endif

void* LoadDllWithoutDllMain(const char* dllPath);

BOOL api_check(){
	MSDELTA_API *api = API_GET();

	int nZeros = 0;
	int i = 0;
	for(void **pp = (void **)api; *pp != VPTR(-1); ++pp, ++i){
		if(!(*pp)){
			fprintf(stderr, "api index %d not found\n", i);
			++nZeros;
		}
	}
	if(nZeros > 1){
		fprintf(stderr, "API not found\n");
		return FALSE;
	}
	return TRUE;
} 

BOOL api_init_classes(){	
	myNativeComponentFactory = (struct vtb_ComponentFactory){
		.dtor = myNativeComponentFactory_dtor,
		.internal_instantiate = myNativeComponentFactory_InternalInstantiate
	};
	myNativeComponent = (struct vtb_Component){
		.dtor = myNativeComponent_dtor,
		.internal_process = myNativeComponent_InternalProcess
	};
	return TRUE;
}

BOOL api_init(){
	HMODULE msdelta = NULL;
#ifdef WITH_DLLINIT_HOOK
	if(MH_Initialize() != MH_OK){
		fprintf(stderr, "Minhook failure\n");
		return 1;
	}
	msdelta = (HMODULE) LoadDllWithoutDllMain("C:\\Windows\\System32\\msdelta.dll");
#else
	msdelta = LoadLibraryA("msdelta.dll");
#endif
	if(!msdelta){
		fprintf(stderr, "Loader failure\n");
		return FALSE;
	}

	LoadSymbols(msdelta);

	MSDELTA_API *api = API_GET();
	api->__API_END = VPTR(-1);
	api->init = (pfnInitDelta)CHECK(SymbolGet("?InitDelta@@YAHXZ"));
	api->init_env = (pfnInitEnv)CHECK(SymbolGet("?Init@Environment@compo@@QEAAXXZ"));
	api->compo_get = (pfnComponentGet)CHECK(SymbolGet("?Get@?$StringMap@VComponentFactory@compo@@@compo@@QEBAPEAVComponentFactory@2@PEBD_K_N@Z"));
	api->compo_put = (pfnComponentPut)CHECK(SymbolGet("?Put@?$StringMap@VComponentFactory@compo@@@compo@@QEAAPEAVComponentFactory@2@PEBD_KPEAV32@_N@Z"));
	api->compo_init = (pfnComponentInit)CHECK(SymbolGet("?Init@Component@compo@@IEAAXPEAVComponentFactory@2@@Z"));
	api->compo_init_factory = (pfnComponentInitFactory)CHECK(SymbolGet("?Init@ComponentFactory@compo@@IEAAXPEAVEnvironment@2@_KPEBUInputPortSpec@12@1PEBK@Z"));
	api->cdp_new = (pfnCdpNew)CHECK(SymbolGet("?_New@cdp@@YAPEAX_K@Z"));
	api->flow_component_factory = (pfnFlowComponentFactory)CHECK(SymbolGet("??0FlowComponentFactory@compo@@QEAA@XZ"));
	api->flow_component_init = (pfnFlowComponentInit)CHECK(SymbolGet("?Init@FlowComponentFactory@compo@@QEAAXPEAVEnvironment@2@PEBD_K@Z"));
	api->component_process = (pfnComponentProcess)(CHECK(SymbolGet("?ProcessComponent@ComponentFactory@compo@@QEAAX_KPEAPEAVObject@2@01@Z")));
	api->input_buffer_get = (pfnInputBufferGet)CHECK(SymbolGet("?GetInputBuffer@PullcapiContext@compo@@SAPEAVBufferObject@2@PEBU_DELTA_INPUT@@@Z"));
	api->output_buffer_put = (pfnInputBufferPut)CHECK(SymbolGet("?PutOutputBuffer@PullcapiContext@compo@@SAXPEAVBufferObject@2@PEAU_DELTA_OUTPUT@@@Z"));
	api->component_factories_put_all = (pfnComponentFactoriesPutAll)CHECK(SymbolGet("?PutAllComponentFactories@Environment@compo@@QEAAXXZ"));

	api->gp_environment = (void ***)CHECK(SymbolGet("?g_environment@PullcapiContext@compo@@0PEAVEnvironment@2@EA"));
	api->gp_applyPatchFactory = (void ***)CHECK(SymbolGet("?g_applyPatchFactory@PullcapiContext@compo@@0PEAVComponentFactory@2@EA"));

	BOOL mustInit = FALSE;
	if(*api->gp_environment == NULL || *api->gp_applyPatchFactory == NULL){
		mustInit = TRUE;
	}

	if(!api_check()){
		return FALSE;
	}

	api_init_classes();

#ifdef WITH_DLLINIT_HOOK
	if(MH_CreateHook(
		api->component_factories_put_all, 
		&my_component_factories_put_all, 
		(void **)&o_component_factories_put_all) != MH_OK
	|| MH_EnableHook(api->component_factories_put_all) != MH_OK) {
		fprintf(stderr, "Minhook failure\n");
		return FALSE;
	}
	api->init();
#else
	// enable pushing new components
	uint8_t prevInitState = get_initialized_flag();
	set_initialized_flag(1); //1: uninitialized
	printf("%x\n", get_initialized_flag());

	add_components(*api->gp_environment);
	
	set_initialized_flag(prevInitState);
#endif

	return TRUE;
}

int main(){
	if(!CHECK(api_init())){
		fprintf(stderr, "api_init() failed\n");
		return 1;
	}

	MSDELTA_API *api = API_GET();

	void *stringPool = (*api->gp_environment)[2];
	void *flowComponent = api->compo_get(stringPool, szMyComponent, sizeof(szMyComponent)-1);	
	void *nativeComponent = api->compo_get(stringPool, szMyNativeComponent, sizeof(szMyNativeComponent) - 1);

	const char szHelloWorld1[] = "HELLO There, i'm an input buffer!";

	DELTA_INPUT buf = {
		.lpcStart = szHelloWorld1,
		.uSize = sizeof(szHelloWorld1),
		.Editable = FALSE
	};

	compoBufferObject *buf1 = api->input_buffer_get(&buf);

	compoObject *retval = NULL;
	compoObject *args[] = { (compoObject *)buf1 };
	api->component_process(flowComponent,
		1, args,
		1, &retval);
	
	DELTA_OUTPUT out = {0};
	api->output_buffer_put((compoBufferObject *)retval, &out);

	const char *outputFilename = "output.bin";

	FILE *fhOut = fopen(outputFilename, "wb");
	if(fhOut){
		fwrite(out.lpStart, sizeof(uint8_t), out.uSize, fhOut);
		fclose(fhOut);
	}

	printf("output written to: %s\n", outputFilename);

#if BROKEN
	api->component_process(nativeComponent,
		0, NULL,
		0, NULL);
#endif

	SymCleanup(GetCurrentProcess());
	return 0;
}