#include "boh_lang_indexedenumerator_string.h"



const struct vtable_c_boh_p_lang_p_IndexedEnumerator_String instance_vtable_c_boh_p_lang_p_IndexedEnumerator_String = { &c_boh_p_lang_p_Object_m_toString_3584862187, &c_boh_p_lang_p_Object_m_hash_3584862187, &c_boh_p_lang_p_Object_m_getType_3584862187, &c_boh_p_lang_p_Object_m_equals_4071216051 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_IndexedEnumerator_String(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_IndexedEnumerator_String * new_c_boh_p_lang_p_IndexedEnumerator_String(struct c_boh_p_lang_p_IIndexedCollection_String * p_collection)
{
	struct c_boh_p_lang_p_IndexedEnumerator_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IndexedEnumerator_String));
	result->f_collection = NULL;
	result->f_current = 0;
	result->vtable = &instance_vtable_c_boh_p_lang_p_IndexedEnumerator_String;
	c_boh_p_lang_p_IndexedEnumerator_String_m_this_4126921818(result, p_collection);
	return result;
}

void c_boh_p_lang_p_IndexedEnumerator_String_m_this_4126921818(struct c_boh_p_lang_p_IndexedEnumerator_String * const self, struct c_boh_p_lang_p_IIndexedCollection_String * p_collection)
{
	(self->f_collection = p_collection);
	(self->f_current = (int32_t)(0));
}
struct c_boh_p_lang_p_String * c_boh_p_lang_p_IndexedEnumerator_String_m_current_3584862187(struct c_boh_p_lang_p_IndexedEnumerator_String * const self)
{
	struct c_boh_p_lang_p_IIndexedCollection_String * temp17;
	return (temp17 = self->f_collection)->m_get_2325378186(temp17->object, self->f_current);
}
