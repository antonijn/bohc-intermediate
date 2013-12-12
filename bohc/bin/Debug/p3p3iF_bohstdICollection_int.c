#include "p3p3iF_bohstdICollection_int.h"

struct p3p3iF_bohstdICollection_int * new_p3p3iF_bohstdICollection_int(struct p3p3c6_bohstdObject * object, struct p3p3iD_bohstdIIterator_int * (*m_iterator_d5aca7eb)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3iF_bohstdICollection_int * result = GC_malloc(sizeof(struct p3p3iF_bohstdICollection_int));
	result->object = object;
	result->m_iterator_d5aca7eb = m_iterator_d5aca7eb;
	return result;
}
