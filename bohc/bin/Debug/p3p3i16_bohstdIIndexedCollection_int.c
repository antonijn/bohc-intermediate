#include "p3p3i16_bohstdIIndexedCollection_int.h"

struct p3p3i16_bohstdIIndexedCollection_int * new_p3p3i16_bohstdIIndexedCollection_int(struct p3p3c6_bohstdObject * object, struct p3p3iD_bohstdIIterator_int * (*m_iterator_d5aca7eb)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_d5aca7eb)(struct p3p3c6_bohstdObject * const self), int32_t (*m_get_70fcd6e5)(struct p3p3c6_bohstdObject * const self, int32_t p_i))
{
	struct p3p3i16_bohstdIIndexedCollection_int * result = GC_malloc(sizeof(struct p3p3i16_bohstdIIndexedCollection_int));
	result->object = object;
	result->m_iterator_d5aca7eb = m_iterator_d5aca7eb;
	result->m_size_d5aca7eb = m_size_d5aca7eb;
	result->m_get_70fcd6e5 = m_get_70fcd6e5;
	return result;
}
