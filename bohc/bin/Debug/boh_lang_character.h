#pragma once

struct c_boh_p_lang_p_String;
struct c_boh_p_lang_p_Exception;
struct c_boh_p_lang_p_Object;
struct c_boh_p_lang_p_Type;
struct c_boh_p_lang_p_Package;
struct c_boh_p_lang_p_Array_int;
struct c_boh_p_lang_p_Array_boh_lang_String;
struct c_boh_p_lang_p_ICollection_int;
struct c_boh_p_lang_p_ICollection_boh_lang_String;
struct c_boh_p_lang_p_IIterator_int;
struct c_boh_p_lang_p_IIterator_boh_lang_String;
struct c_boh_p_lang_p_IIndexedCollection_int;
struct c_boh_p_lang_p_IIndexedCollection_boh_lang_String;
struct c_boh_p_lang_p_IndexedEnumerator_int;
struct c_boh_p_lang_p_IndexedEnumerator_boh_lang_String;
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

struct c_boh_p_lang_p_Character
{
	char16_t f_ch0;
	char16_t f_ch1;
};

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
#include "boh_lang_array_int.h"
#include "boh_lang_array_boh_lang_string.h"
#include "boh_lang_icollection_int.h"
#include "boh_lang_icollection_boh_lang_string.h"
#include "boh_lang_iiterator_int.h"
#include "boh_lang_iiterator_boh_lang_string.h"
#include "boh_lang_iindexedcollection_int.h"
#include "boh_lang_iindexedcollection_boh_lang_string.h"
#include "boh_lang_indexedenumerator_int.h"
#include "boh_lang_indexedenumerator_boh_lang_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Character(void);

extern struct c_boh_p_lang_p_Character new_c_boh_p_lang_p_Character(void);

extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Character_m_toString_3526476(struct c_boh_p_lang_p_Character * const self);
extern void c_boh_p_lang_p_Character_m_this_3526476(struct c_boh_p_lang_p_Character * const self);


