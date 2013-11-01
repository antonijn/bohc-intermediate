#include "boh_lang_icollection_string.h"

struct c_boh_p_lang_p_ICollection_String * new_c_boh_p_lang_p_ICollection_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_String * (*m_iterator_3584862187)(struct c_boh_p_lang_p_Object * const self))
{
	struct c_boh_p_lang_p_ICollection_String * result = GC_malloc(sizeof(struct c_boh_p_lang_p_ICollection_String));
	result->object = object;
	result->m_iterator_3584862187 = m_iterator_3584862187;
	return result;
}
