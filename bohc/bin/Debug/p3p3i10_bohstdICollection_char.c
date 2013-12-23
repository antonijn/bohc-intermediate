#include "p3p3i10_bohstdICollection_char.h"

struct p3p3i10_bohstdICollection_char * new_p3p3i10_bohstdICollection_char(struct p3p3c6_bohstdObject * object, struct p3p3iE_bohstdIIterator_char * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3cA_bohstdQuery_char * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3i10_bohstdICollection_char * result = GC_malloc(sizeof(struct p3p3i10_bohstdICollection_char));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	result->m_query_35cf4c = m_query_35cf4c;
	return result;
}
