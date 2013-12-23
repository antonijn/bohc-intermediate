#include "p3p3i16_bohstdIIndexedCollection_int.h"

struct p3p3i16_bohstdIIndexedCollection_int * new_p3p3i16_bohstdIIndexedCollection_int(struct p3p3c6_bohstdObject * object, struct p3p3iD_bohstdIIterator_int * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c9_bohstdQuery_int * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_get_adeaa357)(struct p3p3c6_bohstdObject * const self, int32_t p_i))
{
	struct p3p3i16_bohstdIIndexedCollection_int * result = GC_malloc(sizeof(struct p3p3i16_bohstdIIndexedCollection_int));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	result->m_query_35cf4c = m_query_35cf4c;
	result->m_size_35cf4c = m_size_35cf4c;
	result->m_get_adeaa357 = m_get_adeaa357;
	return result;
}
