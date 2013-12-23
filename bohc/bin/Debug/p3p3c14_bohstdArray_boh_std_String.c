#include "p3p3c14_bohstdArray_boh_std_String.h"



const struct vtable_p3p3c14_bohstdArray_boh_std_String instance_vtable_p3p3c14_bohstdArray_boh_std_String = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c14_bohstdArray_boh_std_String(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c14_bohstdArray_boh_std_String * new_p3p3c14_bohstdArray_boh_std_String_adeaa357(int32_t p_length)
{
	struct p3p3c14_bohstdArray_boh_std_String * result = GC_malloc(sizeof(struct p3p3c14_bohstdArray_boh_std_String));
	result->vtable = &instance_vtable_p3p3c14_bohstdArray_boh_std_String;
	p3p3c14_bohstdArray_boh_std_String_m_static_0();
	p3p3c14_bohstdArray_boh_std_String_fi(result);
	p3p3c14_bohstdArray_boh_std_String_m_this_adeaa357(result, p_length);
	return result;
}

void p3p3c14_bohstdArray_boh_std_String_fi(struct p3p3c14_bohstdArray_boh_std_String * const self)
{
	self->f_length = 0;
	self->f_items = 0;
	self->f_items = 0;
}

void p3p3c14_bohstdArray_boh_std_String_m_this_adeaa357(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_length)
{
	(self->f_length = p_length);
	(self->f_items = GC_malloc((p_length * sizeof(struct p3p3c6_bohstdString *))));
}
int32_t p3p3c14_bohstdArray_boh_std_String_m_size_35cf4c(struct p3p3c14_bohstdArray_boh_std_String * const self)
{
	return self->f_length;
}
struct p3p3c6_bohstdString * p3p3c14_bohstdArray_boh_std_String_m_get_adeaa357(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_i)
{
	return (*(struct p3p3c6_bohstdString * *)((int8_t *)((self->f_items + (p_i * sizeof(struct p3p3c6_bohstdString *))))));
}
void p3p3c14_bohstdArray_boh_std_String_m_set_d881ed08(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_i, struct p3p3c6_bohstdString * p_value)
{
	((*(struct p3p3c6_bohstdString * *)((int8_t *)((self->f_items + (p_i * sizeof(struct p3p3c6_bohstdString *)))))) = p_value);
}
struct p3p3c6_bohstdString * p3p3c14_bohstdArray_boh_std_String_m_getFast_adeaa357(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_i)
{
	return (*(struct p3p3c6_bohstdString * *)((int8_t *)((self->f_items + (p_i * sizeof(struct p3p3c6_bohstdString *))))));
}
void p3p3c14_bohstdArray_boh_std_String_m_setFast_d881ed08(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_i, struct p3p3c6_bohstdString * p_value)
{
	((*(struct p3p3c6_bohstdString * *)((int8_t *)((self->f_items + (p_i * sizeof(struct p3p3c6_bohstdString *)))))) = p_value);
}
struct p3p3i18_bohstdIIterator_boh_std_String * p3p3c14_bohstdArray_boh_std_String_m_iterator_35cf4c(struct p3p3c14_bohstdArray_boh_std_String * const self)
{
	boh_throw_ex(new_p3p3c9_bohstdException_35cf4c());
}
struct p3p3c14_bohstdQuery_boh_std_String * p3p3c14_bohstdArray_boh_std_String_m_query_35cf4c(struct p3p3c14_bohstdArray_boh_std_String * const self)
{
	struct p3p3c14_bohstdArray_boh_std_String *temp10;
	return new_p3p3c14_bohstdQuery_boh_std_String_f67e4109(new_p3p3i1A_bohstdICollection_boh_std_String(temp10 = (self), &p3p3c14_bohstdArray_boh_std_String_m_iterator_35cf4c, &p3p3c14_bohstdArray_boh_std_String_m_query_35cf4c));
}
void p3p3c14_bohstdArray_boh_std_String_m_resize_adeaa357(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_newsize)
{
	if ((p_newsize <= self->f_length))
	{
		(self->f_length = p_newsize);
		return;
	}
	(self->f_items = GC_realloc(self->f_items, p_newsize));
	(self->f_length = p_newsize);
}
void p3p3c14_bohstdArray_boh_std_String_m_move_10aba1b7(struct p3p3c14_bohstdArray_boh_std_String * const self, int32_t p_dest, int32_t p_src, int32_t p_size)
{
	if ((p_dest < 0))
	{
	}
	if ((p_src < 0))
	{
	}
	if ((((p_dest + p_size) > self->f_length) || ((p_src + p_size) > self->f_length)))
	{
	}
	if (((p_size == 0)))
	{
		return;
	}
	if ((p_size < 0))
	{
	}
	struct p3p3c14_bohstdArray_boh_std_String * l_backup = new_p3p3c14_bohstdArray_boh_std_String_adeaa357(p_size);
	for (int32_t l_i = (int32_t)(0); (l_i < p_size); (++l_i))
	{
		p3p3c14_bohstdArray_boh_std_String_m_setFast_d881ed08(l_backup, l_i, p3p3c14_bohstdArray_boh_std_String_m_getFast_adeaa357(self, (p_src + l_i)));
	}
	for (int32_t l_i = (int32_t)(0); (l_i < p_size); (++l_i))
	{
		p3p3c14_bohstdArray_boh_std_String_m_setFast_d881ed08(self, (p_dest + l_i), p3p3c14_bohstdArray_boh_std_String_m_getFast_adeaa357(l_backup, l_i));
	}
}
void p3p3c14_bohstdArray_boh_std_String_m_static_0(void)
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
