#include "p3p3iF_bohstdICollection_int.h"

struct p3p3iF_bohstdICollection_int * new_p3p3iF_bohstdICollection_int(struct p3p3c6_bohstdObject * object, struct p3p3iD_bohstdIIterator_int * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c9_bohstdQuery_int * (*m_query_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3iF_bohstdICollection_int * result = GC_malloc(sizeof(struct p3p3iF_bohstdICollection_int));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	result->m_query_35cf4c = m_query_35cf4c;
	return result;
}
