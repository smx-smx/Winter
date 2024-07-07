cmake_minimum_required(VERSION 3.13)
project(msdelta)

set(CMAKE_EXPORT_COMPILE_COMMANDS ON)
set(CMAKE_C_STANDARD 11)
set(CMAKE_C_EXTENSIONS ON)

add_definitions(-D_AMD64_)

find_library(MSDELTA_LIB msdelta
	PATHS ${WINSDK_LIBDIRS}
	REQUIRED
)

add_executable(msdelta
	msdelta.c
)

set(WITH_DLLINIT_HOOK FALSE)
if(WITH_DLLINIT_HOOK)
	target_sources(msdelta PRIVATE dll_loader.c)
	target_compile_definitions(msdelta PRIVATE -DWITH_DLLINIT_HOOK)
endif()

target_link_libraries(msdelta minhook dbghelp)
target_compile_definitions(msdelta PRIVATE -D_GNU_SOURCE)

