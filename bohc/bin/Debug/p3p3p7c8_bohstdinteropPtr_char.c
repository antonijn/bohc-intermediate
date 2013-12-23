#include "p3p3p7c8_bohstdinteropPtr_char.h"

struct p3p3p7c8_bohstdinteropPtr_char p3p3p7c8_bohstdinteropPtr_char_sf_NULL;


const struct vtable_p3p3p7c8_bohstdinteropPtr_char instance_vtable_p3p3p7c8_bohstdinteropPtr_char = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3p7c8_bohstdinteropPtr_char(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3p7c8_bohstdinteropPtr_char new_p3p3p7c8_bohstdinteropPtr_char_bf420477(struct p3p3p7c7_bohstdinteropVoidPtr p_ptr)
{
	struct p3p3p7c8_bohstdinteropPtr_char result;
	p3p3p7c8_bohstdinteropPtr_char_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_fi(&result);
	p3p3p7c8_bohstdinteropPtr_char_m_this_bf420477(&result, p_ptr);
	return result;
}
struct p3p3p7c8_bohstdinteropPtr_char new_p3p3p7c8_bohstdinteropPtr_char_111bcd8d(unsigned char (*p_reference))
{
	struct p3p3p7c8_bohstdinteropPtr_char result;
	p3p3p7c8_bohstdinteropPtr_char_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_fi(&result);
	p3p3p7c8_bohstdinteropPtr_char_m_this_111bcd8d(&result, (*p_reference));
	return result;
}
struct p3p3p7c8_bohstdinteropPtr_char new_p3p3p7c8_bohstdinteropPtr_char_112000b3(int64_t p_ptr)
{
	struct p3p3p7c8_bohstdinteropPtr_char result;
	p3p3p7c8_bohstdinteropPtr_char_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_fi(&result);
	p3p3p7c8_bohstdinteropPtr_char_m_this_112000b3(&result, p_ptr);
	return result;
}
struct p3p3p7c8_bohstdinteropPtr_char new_p3p3p7c8_bohstdinteropPtr_char_adeaa357(int32_t p_ptr)
{
	struct p3p3p7c8_bohstdinteropPtr_char result;
	p3p3p7c8_bohstdinteropPtr_char_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_fi(&result);
	p3p3p7c8_bohstdinteropPtr_char_m_this_adeaa357(&result, p_ptr);
	return result;
}
struct p3p3p7c8_bohstdinteropPtr_char new_p3p3p7c8_bohstdinteropPtr_char_35cf4c(void)
{
	struct p3p3p7c8_bohstdinteropPtr_char result;
	p3p3p7c8_bohstdinteropPtr_char_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_fi(&result);
	p3p3p7c8_bohstdinteropPtr_char_m_this_35cf4c(&result);
	return result;
}

void p3p3p7c8_bohstdinteropPtr_char_fi(struct p3p3p7c8_bohstdinteropPtr_char * const self)
{
	self->f_ptr = 0;
	self->f_ptr = 0;
}

void p3p3p7c8_bohstdinteropPtr_char_m_this_bf420477(struct p3p3p7c8_bohstdinteropPtr_char * const self, struct p3p3p7c7_bohstdinteropVoidPtr p_ptr)
{
	struct p3p3p7c7_bohstdinteropVoidPtr temp24;
	((*self).f_ptr = p3p3p7c7_bohstdinteropVoidPtr_m_toInt_35cf4c((temp24 = p_ptr, &temp24)));
}
void p3p3p7c8_bohstdinteropPtr_char_m_this_111bcd8d(struct p3p3p7c8_bohstdinteropPtr_char * const self, unsigned char* const p_reference)
{
	((*self).f_ptr = (int64_t)(&(*p_reference)));
}
#if defined(PF_DESKTOP64)
void p3p3p7c8_bohstdinteropPtr_char_m_this_112000b3(struct p3p3p7c8_bohstdinteropPtr_char * const self, int64_t p_ptr)
{
	((*self).f_ptr = p_ptr);
}
#endif
#if defined(PF_DESKTOP32)
void p3p3p7c8_bohstdinteropPtr_char_m_this_adeaa357(struct p3p3p7c8_bohstdinteropPtr_char * const self, int32_t p_ptr)
{
	((*self).f_ptr = (int64_t)(p_ptr));
}
#endif
unsigned char p3p3p7c8_bohstdinteropPtr_char_m_deref_35cf4c(struct p3p3p7c8_bohstdinteropPtr_char * const self)
{
	return (*(unsigned char *)((int8_t *)((*self).f_ptr)));
}
unsigned char p3p3p7c8_bohstdinteropPtr_char_m_deref_adeaa357(struct p3p3p7c8_bohstdinteropPtr_char * const self, int32_t p_idx)
{
	return (*(unsigned char *)((int8_t *)(((*self).f_ptr + (p_idx * sizeof(unsigned char))))));
}
void p3p3p7c8_bohstdinteropPtr_char_m_set_d5ad6698(struct p3p3p7c8_bohstdinteropPtr_char * const self, int32_t p_idx, unsigned char p_value)
{
	((*(unsigned char *)((int8_t *)(((*self).f_ptr + (p_idx * sizeof(unsigned char)))))) = p_value);
}
void p3p3p7c8_bohstdinteropPtr_char_m_this_35cf4c(struct p3p3p7c8_bohstdinteropPtr_char * const self)
{
}
void p3p3p7c8_bohstdinteropPtr_char_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	p3p3p7c8_bohstdinteropPtr_char_sf_NULL = (new_p3p3p7c8_bohstdinteropPtr_char_35cf4c());
	{
	}
}
