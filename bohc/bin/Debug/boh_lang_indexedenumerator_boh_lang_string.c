#include "boh_lang_indexedenumerator_boh_lang_string.h"



const struct vtable_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String instance_vtable_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String * new_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String(struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String * p_collection)
{
	struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String));
	result->f_collection = NULL;
	result->f_current = 0;
	result->vtable = &instance_vtable_c_boh_p_lang_p_IndexedEnumerator_boh_lang_String;
	c_boh_p_lang_p_IndexedEnumerator_boh_lang_String_m_this_3530284202(result, p_collection);
	return result;
}

void c_boh_p_lang_p_IndexedEnumerator_boh_lang_String_m_this_3530284202(struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String * const self, struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String * p_collection)
{
	(self->f_collection = p_collection);
	(self->f_current = (int32_t)(0));
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_IndexedEnumerator_boh_lang_String_m_current_3526476(struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String * const self)
{
	struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String * temp16;
	return (temp16 = self->f_collection)->m_get_2607005255(temp16->object, self->f_current);
}
