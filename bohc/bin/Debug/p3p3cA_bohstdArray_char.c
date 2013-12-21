#include "p3p3cA_bohstdArray_char.h"



const struct vtable_p3p3cA_bohstdArray_char instance_vtable_p3p3cA_bohstdArray_char = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3cA_bohstdArray_char(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3cA_bohstdArray_char * new_p3p3cA_bohstdArray_char_adeaa357(int32_t p_length)
{
	struct p3p3cA_bohstdArray_char * result = GC_malloc(sizeof(struct p3p3cA_bohstdArray_char));
	result->vtable = &instance_vtable_p3p3cA_bohstdArray_char;
	p3p3cA_bohstdArray_char_m_static_0();
	p3p3cA_bohstdArray_char_fi(result);
	p3p3cA_bohstdArray_char_m_this_adeaa357(result, p_length);
	return result;
}

void p3p3cA_bohstdArray_char_fi(struct p3p3cA_bohstdArray_char * const self)
{
	self->f_length = 0;
	self->f_items = NULL;
}

void p3p3cA_bohstdArray_char_m_this_adeaa357(struct p3p3cA_bohstdArray_char * const self, int32_t p_length)
{
	(self->f_length = p_length);
	unsigned char l_dummy = (u8'\0');
	(self->f_items = (unsigned char*)(GC_malloc((p_length * sizeof(l_dummy)))));
}
int32_t p3p3cA_bohstdArray_char_m_size_35cf4c(struct p3p3cA_bohstdArray_char * const self)
{
	return self->f_length;
}
unsigned char p3p3cA_bohstdArray_char_m_get_adeaa357(struct p3p3cA_bohstdArray_char * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void p3p3cA_bohstdArray_char_m_set_d5ad6698(struct p3p3cA_bohstdArray_char * const self, int32_t p_i, unsigned char p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
unsigned char p3p3cA_bohstdArray_char_m_getFast_adeaa357(struct p3p3cA_bohstdArray_char * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void p3p3cA_bohstdArray_char_m_setFast_d5ad6698(struct p3p3cA_bohstdArray_char * const self, int32_t p_i, unsigned char p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
struct p3p3iE_bohstdIIterator_char * p3p3cA_bohstdArray_char_m_iterator_35cf4c(struct p3p3cA_bohstdArray_char * const self)
{
	boh_throw_ex(new_p3p3c9_bohstdException_35cf4c());
}
void p3p3cA_bohstdArray_char_m_resize_adeaa357(struct p3p3cA_bohstdArray_char * const self, int32_t p_newsize)
{
	if ((p_newsize <= self->f_length))
	{
		(self->f_length = p_newsize);
		return;
	}
	(self->f_items = (unsigned char*)(GC_realloc(p_newsize)));
	(self->f_length = p_newsize);
}
void p3p3cA_bohstdArray_char_m_move_10aba1b7(struct p3p3cA_bohstdArray_char * const self, int32_t p_dest, int32_t p_src, int32_t p_size)
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
	struct p3p3cA_bohstdArray_char * l_backup = new_p3p3cA_bohstdArray_char_adeaa357(p_size);
	for (int32_t l_i = (int32_t)(0); (l_i < p_size); (++l_i))
	{
		p3p3cA_bohstdArray_char_m_setFast_d5ad6698(l_backup, l_i, p3p3cA_bohstdArray_char_m_getFast_adeaa357(self, (p_src + l_i)));
	}
	for (int32_t l_i = (int32_t)(0); (l_i < p_size); (++l_i))
	{
		p3p3cA_bohstdArray_char_m_setFast_d5ad6698(self, (p_dest + l_i), p3p3cA_bohstdArray_char_m_getFast_adeaa357(l_backup, l_i));
	}
}
void p3p3cA_bohstdArray_char_m_static_0(void)
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
