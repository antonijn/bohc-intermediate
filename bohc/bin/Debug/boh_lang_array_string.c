#include "boh_lang_array_string.h"



const struct vtable_c_boh_p_lang_p_Array_String instance_vtable_c_boh_p_lang_p_Array_String = { &c_boh_p_lang_p_Object_m_toString_3584862187, &c_boh_p_lang_p_Object_m_hash_3584862187, &c_boh_p_lang_p_Object_m_getType_3584862187, &c_boh_p_lang_p_Object_m_equals_4071216051 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Array_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_Array_String * new_c_boh_p_lang_p_Array_String(int32_t p_length)
{
	struct c_boh_p_lang_p_Array_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_Array_String));
	result->f_length = 0;
	result->f_items = NULL;
	result->vtable = &instance_vtable_c_boh_p_lang_p_Array_String;
	c_boh_p_lang_p_Array_String_m_this_2325378186(result, p_length);
	return result;
}

void c_boh_p_lang_p_Array_String_m_this_2325378186(struct c_boh_p_lang_p_Array_String * const self, int32_t p_length)
{
	struct c_boh_p_lang_p_String * l_dummy;
	(self->f_items = (String*)(GC_malloc((p_length * sizeof(l_dummy)))));
	if ((c_boh_p_lang_p_Object_m_valEquals_3207794263((struct c_boh_p_lang_p_Object *)(self->f_items), (struct c_boh_p_lang_p_Object *)(NULL))))
	{
	}
	(self->f_length = p_length);
}
int32_t c_boh_p_lang_p_Array_String_m_size_3584862187(struct c_boh_p_lang_p_Array_String * const self)
{
	return self->f_length;
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_Array_String_m_get_2325378186(struct c_boh_p_lang_p_Array_String * const self, int32_t p_i)
{
	return boh_deref_ptr(self->f_items, p_i);
}
void c_boh_p_lang_p_Array_String_m_set_1796709708(struct c_boh_p_lang_p_Array_String * const self, int32_t p_i, struct c_boh_p_lang_p_String * p_value)
{
	boh_set_deref(self->f_items, p_i, p_value);
}
struct c_boh_p_lang_p_IIterator_String * c_boh_p_lang_p_Array_String_m_getIterator_3584862187(struct c_boh_p_lang_p_Array_String * const self)
{
	struct c_boh_p_lang_p_Array_String *temp15;
	return new_c_boh_p_lang_p_IndexedEnumerator_String(new_c_boh_p_lang_p_IIndexedCollection_String(temp15 = (self), &c_boh_p_lang_p_Array_String_m_size_3584862187, &c_boh_p_lang_p_Array_String_m_get_2325378186, &c_boh_p_lang_p_Array_String_m_set_1796709708));
}
