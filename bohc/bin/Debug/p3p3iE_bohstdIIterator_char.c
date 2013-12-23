#include "p3p3iE_bohstdIIterator_char.h"

struct p3p3iE_bohstdIIterator_char * new_p3p3iE_bohstdIIterator_char(struct p3p3c6_bohstdObject * object, _Bool (*m_next_35cf4c)(struct p3p3c6_bohstdObject * const self), _Bool (*m_previous_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_moveLast_35cf4c)(struct p3p3c6_bohstdObject * const self), void (*m_reset_35cf4c)(struct p3p3c6_bohstdObject * const self), unsigned char (*m_current_35cf4c)(struct p3p3c6_bohstdObject * const self))
{
	struct p3p3iE_bohstdIIterator_char * result = GC_malloc(sizeof(struct p3p3iE_bohstdIIterator_char));
	result->object = object;
	result->m_next_35cf4c = m_next_35cf4c;
	result->m_previous_35cf4c = m_previous_35cf4c;
	result->m_moveLast_35cf4c = m_moveLast_35cf4c;
	result->m_reset_35cf4c = m_reset_35cf4c;
	result->m_current_35cf4c = m_current_35cf4c;
	return result;
}