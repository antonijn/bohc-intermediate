#include "boh_lang_iindexedcollection_string.h"

struct c_boh_p_lang_p_IIndexedCollection_String * new_c_boh_p_lang_p_IIndexedCollection_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_String * (*m_iterator_3584862187)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_size_3584862187)(struct c_boh_p_lang_p_Object * const self), struct c_boh_p_lang_p_String * (*m_get_2325378186)(struct c_boh_p_lang_p_Object * const self, int32_t p_i), void (*m_set_1796709708)(struct c_boh_p_lang_p_Object * const self, int32_t p_i, struct c_boh_p_lang_p_String * p_value))
{
	struct c_boh_p_lang_p_IIndexedCollection_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IIndexedCollection_String));
	result->object = object;
	result->m_iterator_3584862187 = m_iterator_3584862187;
	result->m_size_3584862187 = m_size_3584862187;
	result->m_get_2325378186 = m_get_2325378186;
	result->m_set_1796709708 = m_set_1796709708;
	return result;
}
