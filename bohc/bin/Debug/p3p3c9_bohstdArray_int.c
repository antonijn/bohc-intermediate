#include "p3p3c9_bohstdArray_int.h"



const struct vtable_p3p3c9_bohstdArray_int instance_vtable_p3p3c9_bohstdArray_int = { &p3p3c6_bohstdObject_m_equals_5289cddf, &p3p3c6_bohstdObject_m_hash_d5aca7eb, &p3p3c6_bohstdObject_m_getType_d5aca7eb, &p3p3c6_bohstdObject_m_toString_d5aca7eb };

struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdArray_int(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c9_bohstdArray_int * new_p3p3c9_bohstdArray_int_70fcd6e5(int32_t p_length)
{
	struct p3p3c9_bohstdArray_int * result = GC_malloc(sizeof(struct p3p3c9_bohstdArray_int));
	result->vtable = &instance_vtable_p3p3c9_bohstdArray_int;
	p3p3c9_bohstdArray_int_m_static_2d2816fe();
	p3p3c9_bohstdArray_int_fi(result);
	p3p3c9_bohstdArray_int_m_this_70fcd6e5(result, p_length);
	return result;
}

void p3p3c9_bohstdArray_int_fi(struct p3p3c9_bohstdArray_int * const self)
{
	self->f_length = 0;
	self->f_items = NULL;
}

void p3p3c9_bohstdArray_int_m_this_70fcd6e5(struct p3p3c9_bohstdArray_int * const self, int32_t p_length)
{
	(self->f_length = p_length);
	int32_t l_dummy = (0);
	(self->f_items = (int32_t*)(GC_malloc((p_length * sizeof(l_dummy)))));
}
int32_t p3p3c9_bohstdArray_int_m_size_d5aca7eb(struct p3p3c9_bohstdArray_int * const self)
{
	return self->f_length;
}
int32_t p3p3c9_bohstdArray_int_m_get_70fcd6e5(struct p3p3c9_bohstdArray_int * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void p3p3c9_bohstdArray_int_m_set_e5adf5a9(struct p3p3c9_bohstdArray_int * const self, int32_t p_i, int32_t p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
struct p3p3iD_bohstdIIterator_int * p3p3c9_bohstdArray_int_m_iterator_d5aca7eb(struct p3p3c9_bohstdArray_int * const self)
{
	boh_throw_ex(new_p3p3c9_bohstdException_d5aca7eb());
}
void p3p3c9_bohstdArray_int_m_static_2d2816fe(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_2d2816fe();
	{
	}
}
