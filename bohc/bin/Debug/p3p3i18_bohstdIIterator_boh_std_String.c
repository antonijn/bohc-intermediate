#include "p3p3i18_bohstdIIterator_boh_std_String.h"

struct p3p3i18_bohstdIIterator_boh_std_String * new_p3p3i18_bohstdIIterator_boh_std_String(struct p3p3c6_bohstdObject * object, _Bool (*m_next_d5aca7eb)(struct p3p3c6_bohstdObject * const self), _Bool (*m_previous_d5aca7eb)(struct p3p3c6_bohstdObject * const self), void (*m_moveLast_d5aca7eb)(struct p3p3c6_bohstdObject * const self), void (*m_reset_d5aca7eb)(struct p3p3c6_bohstdObject * const self), struct p3p3c6_bohstdString * (*m_current_d5aca7eb)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3i18_bohstdIIterator_boh_std_String * result = GC_malloc(sizeof(struct p3p3i18_bohstdIIterator_boh_std_String));
	result->object = object;
	result->m_next_d5aca7eb = m_next_d5aca7eb;
	result->m_previous_d5aca7eb = m_previous_d5aca7eb;
	result->m_moveLast_d5aca7eb = m_moveLast_d5aca7eb;
	result->m_reset_d5aca7eb = m_reset_d5aca7eb;
	result->m_current_d5aca7eb = m_current_d5aca7eb;
	return result;
}