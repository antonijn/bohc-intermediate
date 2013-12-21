#include "p3p3c9_bohstdException.h"


#if defined(PF_WINDOWS)
static void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self);
#endif
#if defined(PF_LINUX) || defined(PF_OSX) || defined(PF_ANDROID)
static void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self);
#endif

const struct vtable_p3p3c9_bohstdException instance_vtable_p3p3c9_bohstdException = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c, &p3p3c9_bohstdException_m_what_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdException(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_f13b0af3(struct p3p3c6_bohstdString * p_description)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_0();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_f13b0af3(result, p_description);
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_35cf4c(void)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_0();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_35cf4c(result);
	return result;
}

void p3p3c9_bohstdException_fi(struct p3p3c9_bohstdException * const self)
{
	self->f_description = NULL;
}

struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_what_35cf4c(struct p3p3c9_bohstdException * const self)
{
	return boh_create_string(u"Something went wrong in the application", 39);
}
struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_getDescription_35cf4c(struct p3p3c9_bohstdException * const self)
{
	return self->f_description;
}
#if defined(PF_WINDOWS)
void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self)
{
	HANDLE l_process = (HANDLE)(GetCurrentProcess());
	SymInitialize(l_process, NULL, 1);
	void* l_stack = (void*)(boh_arr_to_ptr(new_p3p3cA_bohstdArray_long_adeaa357((int32_t)(100))));
	uint16_t l_frames = (uint16_t)(CaptureBackTrace(0, 62, l_stack, NULL));
	SYMBOL_INFO l_dummy;
	SYMBOL_INFO* l_symbol = (SYMBOL_INFO*)(GC_malloc((sizeof(l_dummy) + (256 * 2))));
	(boh_deref_ptr(l_symbol, 0).MaxNameLen = 255);
	(boh_deref_ptr(l_symbol, 0).SizeOfStruct = sizeof(l_dummy));
	for (int32_t l_i = (int32_t)(0); (l_i < l_frames); (++l_i))
	{
		SymFromAddr(l_process, boh_deref_ptr(l_stack, l_i), 0, l_symbol);
	}
}
#endif
#if defined(PF_LINUX) || defined(PF_OSX) || defined(PF_ANDROID)
void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self)
{
	void* l_callstack = (void*)(boh_arr_to_ptr(new_p3p3cA_bohstdArray_long_adeaa357((int32_t)(128))));
	int32_t l_frames = (int32_t)(backtrace(l_callstack, 128));
	unsigned char** l_strings = (unsigned char**)(backtrace_symbols(l_callstack, l_frames));
	for (int32_t l_i = (int32_t)(0); (l_i < l_frames); (++l_i))
	{
	}
	free(l_strings);
}
#endif
void p3p3c9_bohstdException_m_this_f13b0af3(struct p3p3c9_bohstdException * const self, struct p3p3c6_bohstdString * p_description)
{
	(self->f_description = p_description);
	p3p3c9_bohstdException_m_genStackTrace_35cf4c(self);
}
void p3p3c9_bohstdException_m_this_35cf4c(struct p3p3c9_bohstdException * const self)
{
	p3p3c9_bohstdException_m_this_f13b0af3(self, boh_create_string(u"", 0));
}
void p3p3c9_bohstdException_m_static_0(void)
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
