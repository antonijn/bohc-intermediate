#include "boh_lang_iiterator_boh_lang_string.h"

struct c_boh_p_lang_p_IIterator_boh_lang_String * new_c_boh_p_lang_p_IIterator_boh_lang_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_String * (*m_current_3526476)(struct c_boh_p_lang_p_Object * const self), _Bool (*m_next_3526476)(struct c_boh_p_lang_p_Object * const self), _Bool (*m_previous_3526476)(struct c_boh_p_lang_p_Object * const self), void (*m_moveLast_3526476)(struct c_boh_p_lang_p_Object * const self), void (*m_reset_3526476)(struct c_boh_p_lang_p_Object * const self))
{
	struct c_boh_p_lang_p_IIterator_boh_lang_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_IIterator_boh_lang_String));
	result->object = object;
	result->m_current_3526476 = m_current_3526476;
	result->m_next_3526476 = m_next_3526476;
	result->m_previous_3526476 = m_previous_3526476;
	result->m_moveLast_3526476 = m_moveLast_3526476;
	result->m_reset_3526476 = m_reset_3526476;
	return result;
}
