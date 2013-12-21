#include "p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String.h"

struct p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String * new_p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String(struct p3p3c6_bohstdObject * object, struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String * (*m_iterator_35cf4c)(struct p3p3c6_bohstdObject * const self), int32_t (*m_size_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c14_bohstdArray_boh_std_String * (*m_get_adeaa357)(struct p3p3c6_bohstdObject * const self, int32_t p_i))
{
	struct p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String * result = GC_malloc(sizeof(struct p3p3i2F_bohstdIIndexedCollection_boh_std_Array_boh_std_String));
	result->object = object;
	result->m_iterator_35cf4c = m_iterator_35cf4c;
	result->m_size_35cf4c = m_size_35cf4c;
	result->m_get_adeaa357 = m_get_adeaa357;
	return result;
}
