#include "boh_lang_iindexedcollection_int.h"

struct c_boh_p_lang_p_IIndexedCollection_int * new_c_boh_p_lang_p_IIndexedCollection_int(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_int * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_size_3526476)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_get_2607005255)(struct c_boh_p_lang_p_Object * const self, int32_t p_i), void (*m_set_3255497772)(struct c_boh_p_lang_p_Object * const self, int32_t p_i, int32_t p_value))
{
	struct c_boh_p_lang_p_IIndexedCollection_int * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IIndexedCollection_int));
	result->object = object;
	result->m_iterator_3526476 = m_iterator_3526476;
	result->m_size_3526476 = m_size_3526476;
	result->m_get_2607005255 = m_get_2607005255;
	result->m_set_3255497772 = m_set_3255497772;
	return result;
}
