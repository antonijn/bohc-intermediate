#include "boh_lang_iindexedcollection_boh_lang_string.h"

struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String * new_c_boh_p_lang_p_IIndexedCollection_boh_lang_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_boh_lang_String * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_size_3526476)(struct c_boh_p_lang_p_Object * const self), struct c_boh_p_lang_p_String * (*m_get_2607005255)(struct c_boh_p_lang_p_Object * const self, int32_t p_i), void (*m_set_3778044987)(struct c_boh_p_lang_p_Object * const self, int32_t p_i, struct c_boh_p_lang_p_String * p_value))
{
	struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String));
	result->object = object;
	result->m_iterator_3526476 = m_iterator_3526476;
	result->m_size_3526476 = m_size_3526476;
	result->m_get_2607005255 = m_get_2607005255;
	result->m_set_3778044987 = m_set_3778044987;
	return result;
}
