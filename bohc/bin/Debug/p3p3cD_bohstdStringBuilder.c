#include "p3p3cD_bohstdStringBuilder.h"

int32_t p3p3cD_bohstdStringBuilder_sf_INITIAL_SIZE;


const struct vtable_p3p3cD_bohstdStringBuilder instance_vtable_p3p3cD_bohstdStringBuilder = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3cD_bohstdStringBuilder_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3cD_bohstdStringBuilder(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3cD_bohstdStringBuilder * new_p3p3cD_bohstdStringBuilder_adeaa357(int32_t p_capacity)
{
	struct p3p3cD_bohstdStringBuilder * result = GC_malloc(sizeof(struct p3p3cD_bohstdStringBuilder));
	result->vtable = &instance_vtable_p3p3cD_bohstdStringBuilder;
	p3p3cD_bohstdStringBuilder_m_static_0();
	p3p3cD_bohstdStringBuilder_fi(result);
	p3p3cD_bohstdStringBuilder_m_this_adeaa357(result, p_capacity);
	return result;
}
struct p3p3cD_bohstdStringBuilder * new_p3p3cD_bohstdStringBuilder_35cf4c(void)
{
	struct p3p3cD_bohstdStringBuilder * result = GC_malloc(sizeof(struct p3p3cD_bohstdStringBuilder));
	result->vtable = &instance_vtable_p3p3cD_bohstdStringBuilder;
	p3p3cD_bohstdStringBuilder_m_static_0();
	p3p3cD_bohstdStringBuilder_fi(result);
	p3p3cD_bohstdStringBuilder_m_this_35cf4c(result);
	return result;
}

void p3p3cD_bohstdStringBuilder_fi(struct p3p3cD_bohstdStringBuilder * const self)
{
	self->f_characters = NULL;
}

void p3p3cD_bohstdStringBuilder_m_this_adeaa357(struct p3p3cD_bohstdStringBuilder * const self, int32_t p_capacity)
{
	(self->f_characters = new_p3p3c9_bohstdList_char_adeaa357(p_capacity));
}
void p3p3cD_bohstdStringBuilder_m_this_35cf4c(struct p3p3cD_bohstdStringBuilder * const self)
{
	p3p3cD_bohstdStringBuilder_m_this_adeaa357(self, p3p3cD_bohstdStringBuilder_sf_INITIAL_SIZE);
}
void p3p3cD_bohstdStringBuilder_m_append_f13b0af3(struct p3p3cD_bohstdStringBuilder * const self, struct p3p3c6_bohstdString * p_str)
{
	p3p3c9_bohstdList_char_m_add_111bcd8d(self->f_characters, (unsigned char)(p_str));
}
void p3p3cD_bohstdStringBuilder_m_append_111bcd8d(struct p3p3cD_bohstdStringBuilder * const self, unsigned char p_ch)
{
	p3p3c9_bohstdList_char_m_add_111bcd8d(self->f_characters, p_ch);
}
void p3p3cD_bohstdStringBuilder_m_toString_35cf4c(struct p3p3cD_bohstdStringBuilder * const self)
{
	return new_p3p3c6_bohstdString_adeaa357((int32_t)(self->f_characters));
}
void p3p3cD_bohstdStringBuilder_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	p3p3cD_bohstdStringBuilder_sf_INITIAL_SIZE = 1024;
	{
	}
}
