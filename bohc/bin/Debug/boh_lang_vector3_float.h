#pragma once

struct c_boh_p_lang_p_String;
struct c_boh_p_lang_p_Exception;
struct c_boh_p_lang_p_Object;
struct c_boh_p_lang_p_Type;
struct c_boh_p_lang_p_Package;
#include "boh_lang_character.h"
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
#include "boh_lang_vector3_boh_lang_string.h"

struct c_boh_p_lang_p_Vector3_float
{
	float f_x;
	float f_y;
	float f_z;
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

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Vector3_float(void);

extern struct c_boh_p_lang_p_Vector3_float new_c_boh_p_lang_p_Vector3_float(float p_x, float p_y, float p_z);

extern void c_boh_p_lang_p_Vector3_float_m_this_2532104116(struct c_boh_p_lang_p_Vector3_float * const self, float p_x, float p_y, float p_z);
extern struct c_boh_p_lang_p_Vector3_float c_boh_p_lang_p_Vector3_float_op_add_1884407032(struct c_boh_p_lang_p_Vector3_float p_left, struct c_boh_p_lang_p_Vector3_float p_right);


