#include "boh_lang_icollection_int.h"

struct c_boh_p_lang_p_ICollection_int * new_c_boh_p_lang_p_ICollection_int(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_int * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self))
{
	struct c_boh_p_lang_p_ICollection_int * result = GC_malloc(sizeof(struct c_boh_p_lang_p_ICollection_int));
	result->object = object;
	result->m_iterator_3526476 = m_iterator_3526476;
	return result;
}
