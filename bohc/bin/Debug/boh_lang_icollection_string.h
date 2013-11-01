#pragma once

struct c_boh_p_lang_p_ICollection_String;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "boh_lang_string.h"
#include "boh_lang_exception.h"
#include "boh_lang_object.h"
#include "boh_lang_type.h"
#include "boh_lang_package.h"
#include "boh_lang_character.h"
#include "boh_lang_array_int.h"
#include "boh_lang_array_string.h"
#include "boh_lang_icollection_int.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_string.h"
#include "boh_lang_iindexedcollection_int.h"
#include "boh_lang_iindexedcollection_string.h"
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_string.h"

extern struct c_boh_p_lang_p_ICollection_String * new_c_boh_p_lang_p_ICollection_String(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_String * (*m_iterator_3584862187)(struct c_boh_p_lang_p_Object * const self));

struct c_boh_p_lang_p_ICollection_String
{
	struct c_boh_p_lang_p_Object * object;
	struct c_boh_p_lang_p_IIterator_String * (*m_iterator_3584862187)(struct c_boh_p_lang_p_Object * const self);
};

#endif
