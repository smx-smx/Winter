#include <libloaderapi.h>
#include <winnt.h>
#include <fileapi.h>
#include <handleapi.h>
#include <memoryapi.h>
#include <stdio.h>

#include <windef.h>
#include <winuser.h>

#include "common.h"

void* LoadDllWithoutDllMain(const char* dllPath) {
    HANDLE hFile = CreateFileA(dllPath, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile == INVALID_HANDLE_VALUE) {
        return NULL;
    }

    DWORD fileSize = GetFileSize(hFile, NULL);
    BYTE* fileBuffer = (BYTE*)malloc(fileSize);

    DWORD bytesRead = 0;
    ReadFile(hFile, fileBuffer, fileSize, &bytesRead, NULL);
    CloseHandle(hFile);

    IMAGE_DOS_HEADER* dosHeader = (IMAGE_DOS_HEADER*)fileBuffer;
    IMAGE_NT_HEADERS* ntHeaders = (IMAGE_NT_HEADERS*)(fileBuffer + dosHeader->e_lfanew);

    BYTE* dllBase = (BYTE*)VirtualAlloc(NULL, ntHeaders->OptionalHeader.SizeOfImage, MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
    if (!dllBase) {
        free(fileBuffer);
        return NULL;
    }

    memcpy(dllBase, fileBuffer, ntHeaders->OptionalHeader.SizeOfHeaders);

    IMAGE_SECTION_HEADER* sectionHeaders = (IMAGE_SECTION_HEADER*)((BYTE*)&ntHeaders->OptionalHeader + ntHeaders->FileHeader.SizeOfOptionalHeader);
    for (int i = 0; i < ntHeaders->FileHeader.NumberOfSections; ++i) {
        memcpy(dllBase + sectionHeaders[i].VirtualAddress, fileBuffer + sectionHeaders[i].PointerToRawData, sectionHeaders[i].SizeOfRawData);
    }

	typedef struct _IMAGE_RELOCATION_ENTRY {
		WORD Offset : 12;
		WORD Type : 4;
	} IMAGE_RELOCATION_ENTRY, * PIMAGE_RELOCATION_ENTRY;


    // Apply relocations
    IMAGE_DATA_DIRECTORY* relocationDirectory = &ntHeaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_BASERELOC];
    if (relocationDirectory->Size > 0) {
        PIMAGE_BASE_RELOCATION relocation = (PIMAGE_BASE_RELOCATION)(dllBase + relocationDirectory->VirtualAddress);
		//printf("pvv %p\n", relocation->VirtualAddress);
        while (relocation->VirtualAddress) {
			IMAGE_RELOCATION_ENTRY* TypeOffset = (IMAGE_RELOCATION_ENTRY *)PTRADD(relocation, sizeof(*relocation));
			DWORD numberOfEntries = (relocation->SizeOfBlock - sizeof(IMAGE_BASE_RELOCATION)) / sizeof(IMAGE_RELOCATION_ENTRY);
			BYTE *relocationBase = dllBase + relocation->VirtualAddress;
            for (int i = 0; i < numberOfEntries; ++i) {
				switch(TypeOffset[i].Type){
					case IMAGE_REL_BASED_ABSOLUTE: break;
					case IMAGE_REL_BASED_HIGHLOW: {
						DWORD* address = (DWORD*)(relocationBase + TypeOffset[i].Offset);
                    	*address += PTRDIFF(dllBase, ntHeaders->OptionalHeader.ImageBase);
						break;
					}
					case IMAGE_REL_BASED_DIR64: {
						ULONGLONG* address = (ULONGLONG*)(relocationBase + TypeOffset[i].Offset);
                    	*address += (ULONGLONG)(dllBase - ntHeaders->OptionalHeader.ImageBase);
						break;
					}
					default:
						fprintf(stderr, "Unhandled %u\n", TypeOffset[i].Type);
						return NULL;
				}
            }
            relocation = (PIMAGE_BASE_RELOCATION)((BYTE*)relocation + relocation->SizeOfBlock);
        }
    }

    // Resolve imports
    IMAGE_DATA_DIRECTORY* importDirectory = &ntHeaders->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_IMPORT];
    if (importDirectory->Size > 0) {
        PIMAGE_IMPORT_DESCRIPTOR importDescriptor = (PIMAGE_IMPORT_DESCRIPTOR)(dllBase + importDirectory->VirtualAddress);
        while (importDescriptor->Name) {
            char* moduleName = (char*)(dllBase + importDescriptor->Name);
            HMODULE importModule = LoadLibraryA(moduleName);

            PIMAGE_THUNK_DATA thunk = (PIMAGE_THUNK_DATA)(dllBase + importDescriptor->FirstThunk);
            while (thunk->u1.AddressOfData) {
				FARPROC target;
				if((*(ULONGLONG *)thunk & IMAGE_ORDINAL_FLAG64) != 0){
					ULONG ordinal = IMAGE_ORDINAL64(thunk->u1.Ordinal);
					target = GetProcAddress(importModule, MAKEINTRESOURCE(ordinal));
					if(!target){
						fprintf(stderr, "[%s] IMPORT ORDINAL '%lu' not found!\n", moduleName, ordinal);
					} else {
						//fprintf(stderr, "ok [%s] IMPORT ORDINAL '%lu'!\n", moduleName, ordinal);
					}
				} else {
                	PIMAGE_IMPORT_BY_NAME import = (PIMAGE_IMPORT_BY_NAME)(dllBase + thunk->u1.AddressOfData);
                	target = GetProcAddress(importModule, import->Name);
					if(!target){
						//printf("0x%02hhx,0x%02hhx\n", import->Name[0], import->Name[1]);
						fprintf(stderr, "[%s] IMPORT NAME '%s' not found (%u)!\n", moduleName, import->Name, import->Hint);
					} else {
						//fprintf(stderr, "ok [%s] IMPORT NAME '%s' (%u)!\n", moduleName, import->Name, import->Hint);
					}
				}
				if(target){
					thunk->u1.Function = (ULONGLONG)target;
				}
                ++thunk;
            }
            ++importDescriptor;
        }
    }

    free(fileBuffer);
    return dllBase;
}

