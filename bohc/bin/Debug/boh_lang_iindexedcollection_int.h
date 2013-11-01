#pragma once

struct c_boh_p_lang_p_IIndexedCollection_int;

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
#include "boh_lang_array_boh_lang_string.h"
#include "boh_lang_icollection_int.h"
#include "boh_lang_icollection_boh_lang_string.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_boh_lang_string.h"
#include "boh_lang_iindexedcollection_boh_lang_string.h"
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_boh_lang_string.h"
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

extern struct c_boh_p_lang_p_IIndexedCollection_int * new_c_boh_p_lang_p_IIndexedCollection_int(struct c_boh_p_lang_p_Object * object, struct c_boh_p_lang_p_IIterator_int * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_size_3526476)(struct c_boh_p_lang_p_Object * const self), int32_t (*m_get_2607005255)(struct c_boh_p_lang_p_Object * const self, int32_t p_i), void (*m_set_3255497772)(struct c_boh_p_lang_p_Object * const self, int32_t p_i, int32_t p_value));

struct c_boh_p_lang_p_IIndexedCollection_int
{
	struct c_boh_p_lang_p_Object * object;
	struct c_boh_p_lang_p_IIterator_int * (*m_iterator_3526476)(struct c_boh_p_lang_p_Object * const self);
	int32_t (*m_size_3526476)(struct c_boh_p_lang_p_Object * const self);
	int32_t (*m_get_2607005255)(struct c_boh_p_lang_p_Object * const self, int32_t p_i);
	void (*m_set_3255497772)(struct c_boh_p_lang_p_Object * const self, int32_t p_i, int32_t p_value);
};

#endif
