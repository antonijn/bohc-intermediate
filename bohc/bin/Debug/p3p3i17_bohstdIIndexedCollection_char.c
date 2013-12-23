#include "p3p3i17_bohstdIIndexedCollection_char.h"

struct p3p3i17_bohstdIIndexedCollection_char * new_p3p3i17_bohstdIIndexedCollection_char(struct p3p3c6_bohstdObject * object, struct p3p3iE_bohstdIIterator_char * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3cA_bohstdQuery_char * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_35cf4c)(struct p3p3c6_bohstdObject * const self), unsigned char (*m_get_adeaa357)(struct p3p3c6_bohstdObject * const self, int32_t p_i))
{
	struct p3p3i17_bohstdIIndexedCollection_char * result = GC_malloc(sizeof(struct p3p3i17_bohstdIIndexedCollection_char));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	result->m_query_35cf4c = m_query_35cf4c;
	result->m_size_35cf4c = m_size_35cf4c;
	result->m_get_adeaa357 = m_get_adeaa357;
	return result;
}
