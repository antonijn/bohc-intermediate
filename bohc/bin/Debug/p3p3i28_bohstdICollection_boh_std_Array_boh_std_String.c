#include "p3p3i28_bohstdICollection_boh_std_Array_boh_std_String.h"

struct p3p3i28_bohstdICollection_boh_std_Array_boh_std_String * new_p3p3i28_bohstdICollection_boh_std_Array_boh_std_String(struct p3p3c6_bohstdObject * object, struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3i28_bohstdICollection_boh_std_Array_boh_std_String * result = GC_malloc(sizeof(struct p3p3i28_bohstdICollection_boh_std_Array_boh_std_String));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	return result;
}
