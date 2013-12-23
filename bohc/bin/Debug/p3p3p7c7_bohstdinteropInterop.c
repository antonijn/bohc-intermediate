#include "p3p3p7c7_bohstdinteropInterop.h"


#if defined(PF_DESKTOP64)
static int64_t GC_malloc(int32_t p_size);
#endif
#if defined(PF_DESKTOP32)
static int32_t GC_malloc(int32_t p_size);
#endif
#if defined(PF_DESKTOP64)
static int64_t GC_realloc(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size);
#endif
#if defined(PF_DESKTOP32)
static int32_t GC_realloc(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size);
#endif
#if defined(PF_DESKTOP64)
static int64_t malloc(int32_t p_size);
#endif
#if defined(PF_DESKTOP32)
static int32_t malloc(int32_t p_size);
#endif
#if defined(PF_DESKTOP64)
static int64_t realloc(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size);
#endif
#if defined(PF_DESKTOP32)
static int32_t realloc(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size);
#endif
#if defined(PF_DESKTOP64)
static void free(struct p3p3p7c7_bohstdinteropVoidPtr p_size);
#endif
#if defined(PF_DESKTOP32)
static void free(struct p3p3p7c7_bohstdinteropVoidPtr p_size);
#endif

const struct vtable_p3p3p7c7_bohstdinteropInterop instance_vtable_p3p3p7c7_bohstdinteropInterop = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3p7c7_bohstdinteropInterop(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3p7c7_bohstdinteropInterop * new_p3p3p7c7_bohstdinteropInterop_35cf4c(void)
{
	struct p3p3p7c7_bohstdinteropInterop * result = GC_malloc(sizeof(struct p3p3p7c7_bohstdinteropInterop));
	result->vtable = &instance_vtable_p3p3p7c7_bohstdinteropInterop;
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	p3p3p7c7_bohstdinteropInterop_fi(result);
	p3p3p7c7_bohstdinteropInterop_m_this_35cf4c(result);
	return result;
}

void p3p3p7c7_bohstdinteropInterop_fi(struct p3p3p7c7_bohstdinteropInterop * const self)
{
}

struct p3p3p7c7_bohstdinteropVoidPtr p3p3p7c7_bohstdinteropInterop_m_gcAlloc_799e0023(int32_t p_size)
{
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	return new_p3p3p7c7_bohstdinteropVoidPtr_112000b3(GC_malloc(p_size));
}
struct p3p3p7c7_bohstdinteropVoidPtr p3p3p7c7_bohstdinteropInterop_m_gcRealloc_e6ee3e80(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size)
{
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	return new_p3p3p7c7_bohstdinteropVoidPtr_112000b3(GC_realloc(p_ptr, p_size));
}
struct p3p3p7c7_bohstdinteropVoidPtr p3p3p7c7_bohstdinteropInterop_m_alloc_799e0023(int32_t p_size)
{
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	return new_p3p3p7c7_bohstdinteropVoidPtr_112000b3(malloc(p_size));
}
struct p3p3p7c7_bohstdinteropVoidPtr p3p3p7c7_bohstdinteropInterop_m_realloc_e6ee3e80(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr, int32_t p_size)
{
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	return new_p3p3p7c7_bohstdinteropVoidPtr_112000b3(realloc(p_ptr, p_size));
}
void p3p3p7c7_bohstdinteropInterop_m_free_654e8043(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr)
{
	p3p3p7c7_bohstdinteropInterop_m_static_0();
	free(p_ptr);
}
void p3p3p7c7_bohstdinteropInterop_m_this_35cf4c(struct p3p3p7c7_bohstdinteropInterop * const self)
{
}
void p3p3p7c7_bohstdinteropInterop_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	{
	}
}
