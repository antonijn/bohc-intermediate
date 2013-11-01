#include "boh_lang_icollection_boh_lang_string.h"

struct c_boh_p_lang_p_ICollection_boh_lang_String * new_c_boh_p_lang_p_ICollection_boh_lang_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_boh_lang_String * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self))
{
	struct c_boh_p_lang_p_ICollection_boh_lang_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_ICollection_boh_lang_String));
	result->object = object;
	result->m_iterator_3526476 = m_iterator_3526476;
	return result;
}
