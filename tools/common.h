#pragma once

#define VPTR(x) ((void *)(x))
#define UPTR(x) ((uintptr_t)(x))
#define PTRADD(a, b) ( UPTR(a) + UPTR(b) )
#define PTRDIFF(a, b) ( UPTR(a) - UPTR(b) )

#define STRINGIFY(x) STRINGIFY2(x)
#define STRINGIFY2(x) #x
#define LOG_PREFIX "[" __FILE__ ":" STRINGIFY(__LINE__) "] "
#define LOG(verb, fmt, ...) do{ \
    printf(LOG_PREFIX fmt "\n", ##__VA_ARGS__); \
} while(0)

#define DBG(fmt, ...) LOG(V_DBG, "[DEBG] " fmt, ##__VA_ARGS__)
#define CHECK(x) ({\
typeof(x) _tmp = (x);\
DBG("%s = %llx", #x, (unsigned long long)_tmp);\
_tmp;})
