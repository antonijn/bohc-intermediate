#include "boh_lang_array_int.h"



const struct vtable_c_boh_p_lang_p_Array_int instance_vtable_c_boh_p_lang_p_Array_int = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Array_int(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Array_int * new_c_boh_p_lang_p_Array_int(int32_t p_length)
{
	struct c_boh_p_lang_p_Array_int * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Array_int));
	result->f_length = 0;
	result->f_items = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Array_int;
	c_boh_p_lang_p_Array_int_m_this_2607005255(result, p_length);
	return result;
}

void c_boh_p_lang_p_Array_int_m_this_2607005255(struct c_boh_p_lang_p_Array_int * const self, int32_t p_length)
{
	int32_t l_dummy;
	(self->f_items = (int*)(GC_malloc((p_length * sizeof(l_dummy)))));
	if ((c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(self->f_items), (struct c_boh_p_lang_p_Object *)(NULL))))
	{
	}
	(self->f_length = p_length);
}
int32_t c_boh_p_lang_p_Array_int_m_size_3526476(struct c_boh_p_lang_p_Array_int * const self)
{
	return self->f_length;
}
int32_t c_boh_p_lang_p_Array_int_m_get_2607005255(struct c_boh_p_lang_p_Array_int * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void c_boh_p_lang_p_Array_int_m_set_3255497772(struct c_boh_p_lang_p_Array_int * const self, int32_t p_i, int32_t p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
struct c_boh_p_lang_p_IIterator_int * c_boh_p_lang_p_Array_int_m_getIterator_3526476(struct c_boh_p_lang_p_Array_int * const self)
{
	struct c_boh_p_lang_p_Array_int *temp13;
	return new_c_boh_p_lang_p_IndexedEnumerator_int(new_c_boh_p_lang_p_IIndexedCollection_int(temp13 = (self), &c_boh_p_lang_p_Array_int_m_size_3526476, &c_boh_p_lang_p_Array_int_m_get_2607005255, &c_boh_p_lang_p_Array_int_m_set_3255497772));
}
