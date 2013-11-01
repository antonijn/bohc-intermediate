#include "boh_lang_array_boh_lang_string.h"



const struct vtable_c_boh_p_lang_p_Array_boh_lang_String instance_vtable_c_boh_p_lang_p_Array_boh_lang_String = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Array_boh_lang_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Array_boh_lang_String * new_c_boh_p_lang_p_Array_boh_lang_String(int32_t p_length)
{
	struct c_boh_p_lang_p_Array_boh_lang_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Array_boh_lang_String));
	result->f_length = 0;
	result->f_items = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Array_boh_lang_String;
	c_boh_p_lang_p_Array_boh_lang_String_m_this_2607005255(result, p_length);
	return result;
}

void c_boh_p_lang_p_Array_boh_lang_String_m_this_2607005255(struct c_boh_p_lang_p_Array_boh_lang_String * const self, int32_t p_length)
{
	struct c_boh_p_lang_p_String * l_dummy;
	(self->f_items = (String*)(GC_malloc((p_length * sizeof(l_dummy)))));
	if ((c_boh_p_lang_p_Object_m_valEquals_2338730496((struct c_boh_p_lang_p_Object *)(self->f_items), (struct c_boh_p_lang_p_Object *)(NULL))))
	{
	}
	(self->f_length = p_length);
}
int32_t c_boh_p_lang_p_Array_boh_lang_String_m_size_3526476(struct c_boh_p_lang_p_Array_boh_lang_String * const self)
{
	return self->f_length;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Array_boh_lang_String_m_get_2607005255(struct c_boh_p_lang_p_Array_boh_lang_String * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void c_boh_p_lang_p_Array_boh_lang_String_m_set_3778044987(struct c_boh_p_lang_p_Array_boh_lang_String * const self, int32_t p_i, struct c_boh_p_lang_p_String * p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
struct c_boh_p_lang_p_IIterator_boh_lang_String * c_boh_p_lang_p_Array_boh_lang_String_m_getIterator_3526476(struct c_boh_p_lang_p_Array_boh_lang_String * const self)
{
	struct c_boh_p_lang_p_Array_boh_lang_String *temp14;
	return new_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String(new_c_boh_p_lang_p_IIndexedCollection_boh_lang_String(temp14 = (self), &c_boh_p_lang_p_Array_boh_lang_String_m_size_3526476, &c_boh_p_lang_p_Array_boh_lang_String_m_get_2607005255, &c_boh_p_lang_p_Array_boh_lang_String_m_set_3778044987));
}
