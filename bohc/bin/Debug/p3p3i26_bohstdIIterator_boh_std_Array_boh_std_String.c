#include "p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String.h"

struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String * new_p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String(struct p3p3c6_bohstdObject * object, _Bool (*m_next_35cf4c)(struct p3p3c6_bohstdObject * const self), _Bool (*m_previous_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_moveLast_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_reset_35cf4c)(struct p3p3c6_bohstdObject * const self), struct p3p3c14_bohstdArray_boh_std_String * (*m_current_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String * result = GC_malloc(sizeof(struct p3p3i26_bohstdIIterator_boh_std_Array_boh_std_String));
	result->object = object;
	result->m_next_35cf4c = m_next_35cf4c;
	result->m_previous_35cf4c = m_previous_35cf4c;
	result->m_moveLast_35cf4c = m_moveLast_35cf4c;
	result->m_reset_35cf4c = m_reset_35cf4c;
	result->m_current_35cf4c = m_current_35cf4c;
	return result;
}
