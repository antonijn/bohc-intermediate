#include "p3p3i10_bohstdICollection_long.h"

struct p3p3i10_bohstdICollection_long * new_p3p3i10_bohstdICollection_long(struct p3p3c6_bohstdObject * object, struct p3p3iE_bohstdIIterator_long * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3i10_bohstdICollection_long * result = GC_malloc(sizeof(struct p3p3i10_bohstdICollection_long));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	return result;
}
