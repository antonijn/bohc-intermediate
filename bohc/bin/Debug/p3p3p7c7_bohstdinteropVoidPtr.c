#include "p3p3p7c7_bohstdinteropVoidPtr.h"



const struct vtable_p3p3p7c7_bohstdinteropVoidPtr instance_vtable_p3p3p7c7_bohstdinteropVoidPtr = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3p7c7_bohstdinteropVoidPtr(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3p7c7_bohstdinteropVoidPtr new_p3p3p7c7_bohstdinteropVoidPtr_112000b3(int64_t p_ptr)
{
	struct p3p3p7c7_bohstdinteropVoidPtr result;
	p3p3p7c7_bohstdinteropVoidPtr_m_static_0();
	p3p3p7c7_bohstdinteropVoidPtr_fi(&result);
	p3p3p7c7_bohstdinteropVoidPtr_m_this_112000b3(&result, p_ptr);
	return result;
}
struct p3p3p7c7_bohstdinteropVoidPtr new_p3p3p7c7_bohstdinteropVoidPtr_adeaa357(int32_t p_ptr)
{
	struct p3p3p7c7_bohstdinteropVoidPtr result;
	p3p3p7c7_bohstdinteropVoidPtr_m_static_0();
	p3p3p7c7_bohstdinteropVoidPtr_fi(&result);
	p3p3p7c7_bohstdinteropVoidPtr_m_this_adeaa357(&result, p_ptr);
	return result;
}
struct p3p3p7c7_bohstdinteropVoidPtr new_p3p3p7c7_bohstdinteropVoidPtr_35cf4c(void)
{
	struct p3p3p7c7_bohstdinteropVoidPtr result;
	p3p3p7c7_bohstdinteropVoidPtr_m_static_0();
	p3p3p7c7_bohstdinteropVoidPtr_fi(&result);
	p3p3p7c7_bohstdinteropVoidPtr_m_this_35cf4c(&result);
	return result;
}

void p3p3p7c7_bohstdinteropVoidPtr_fi(struct p3p3p7c7_bohstdinteropVoidPtr * const self)
{
	self->f_ptr = 0;
	self->f_ptr = 0;
}

#if defined(PF_DESKTOP64)
void p3p3p7c7_bohstdinteropVoidPtr_m_this_112000b3(struct p3p3p7c7_bohstdinteropVoidPtr * const self, int64_t p_ptr)
{
	((*self).f_ptr = p_ptr);
}
#endif
#if defined(PF_DESKTOP32)
void p3p3p7c7_bohstdinteropVoidPtr_m_this_adeaa357(struct p3p3p7c7_bohstdinteropVoidPtr * const self, int32_t p_ptr)
{
	((*self).f_ptr = (int64_t)(p_ptr));
}
#endif
#if defined(PF_DESKTOP64)
int64_t p3p3p7c7_bohstdinteropVoidPtr_m_toInt_35cf4c(struct p3p3p7c7_bohstdinteropVoidPtr * const self)
{
	return (*self).f_ptr;
}
#endif
#if defined(PF_DESKTOP32)
int32_t p3p3p7c7_bohstdinteropVoidPtr_m_toInt_35cf4c(struct p3p3p7c7_bohstdinteropVoidPtr * const self)
{
	return (*self).f_ptr;
}
#endif
void p3p3p7c7_bohstdinteropVoidPtr_m_this_35cf4c(struct p3p3p7c7_bohstdinteropVoidPtr * const self)
{
}
void p3p3p7c7_bohstdinteropVoidPtr_m_static_0(void)
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
