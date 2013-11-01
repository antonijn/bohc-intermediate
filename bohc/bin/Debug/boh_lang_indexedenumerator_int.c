#include "boh_lang_indexedenumerator_int.h"



const struct vtable_c_boh_p_lang_p_IndexedEnumerator_int instance_vtable_c_boh_p_lang_p_IndexedEnumerator_int = { &c_boh_p_lang_p_Object_m_toString_3526476, &c_boh_p_lang_p_Object_m_hash_3526476, &c_boh_p_lang_p_Object_m_getType_3526476, &c_boh_p_lang_p_Object_m_equals_2378881924 };

struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_IndexedEnumerator_int(void)
{
	static struct c_boh_p_lang_p_Type * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct c_boh_p_lang_p_IndexedEnumerator_int * new_c_boh_p_lang_p_IndexedEnumerator_int(struct c_boh_p_lang_p_IIndexedCollection_int * p_collection)
{
	struct c_boh_p_lang_p_IndexedEnumerator_int * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IndexedEnumerator_int));
	result->f_collection = NULL;
	result->f_current = 0;
	result->vtable = &instance_vtable_c_boh_p_lang_p_IndexedEnumerator_int;
	c_boh_p_lang_p_IndexedEnumerator_int_m_this_3422026619(result, p_collection);
	return result;
}

void c_boh_p_lang_p_IndexedEnumerator_int_m_this_3422026619(struct c_boh_p_lang_p_IndexedEnumerator_int * const self, struct c_boh_p_lang_p_IIndexedCollection_int * p_collection)
{
	(self->f_collection = p_collection);
	(self->f_current = (int32_t)(0));
}
int32_t c_boh_p_lang_p_IndexedEnumerator_int_m_current_3526476(struct c_boh_p_lang_p_IndexedEnumerator_int * const self)
{
	struct c_boh_p_lang_p_IIndexedCollection_int * temp15;
	return (temp15 = self->f_collection)->m_get_2607005255(temp15->object, self->f_current);
}
