#pragma once

struct c_boh_p_lang_p_Package;

#include "boh_internal.h"
#include <stdint.h>
#include <stddef.h>
#include <uchar.h>
#include <setjmp.h>
#include "boh_lang_string.h"
#include "boh_lang_exception.h"
#include "boh_lang_object.h"
#include "boh_lang_type.h"
#include "boh_lang_character.h"
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
#include "boh_lang_vector3_float.h"
#include "boh_lang_vector3_boh_lang_string.h"

extern struct c_boh_p_lang_p_Type * typeof_c_boh_p_lang_p_Package(void);

extern struct c_boh_p_lang_p_Package * new_c_boh_p_lang_p_Package(struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_owner);

extern void c_boh_p_lang_p_Package_m_this_3863939627(struct c_boh_p_lang_p_Package * const self, struct c_boh_p_lang_p_String * p_name, struct c_boh_p_lang_p_Package * p_owner);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Package_m_getName_3526476(struct c_boh_p_lang_p_Package * const self);
extern struct c_boh_p_lang_p_Package * c_boh_p_lang_p_Package_m_getOwner_3526476(struct c_boh_p_lang_p_Package * const self);
extern struct c_boh_p_lang_p_String * c_boh_p_lang_p_Package_m_toString_3526476(struct c_boh_p_lang_p_Package * const self);

extern struct c_boh_p_lang_p_Package * c_boh_p_lang_p_Package_sf_GLOBAL;

struct vtable_c_boh_p_lang_p_Package
{
	struct c_boh_p_lang_p_String * (*m_toString_3526476)(struct c_boh_p_lang_p_Object * const self);
	int64_t (*m_hash_3526476)(struct c_boh_p_lang_p_Object * const self);
	struct c_boh_p_lang_p_Type * (*m_getType_3526476)(struct c_boh_p_lang_p_Object * const self);
	_Bool (*m_equals_2378881924)(struct c_boh_p_lang_p_Object * const self, struct c_boh_p_lang_p_Object * p_other);
};

extern const struct vtable_c_boh_p_lang_p_Package instance_vtable_c_boh_p_lang_p_Package;

struct c_boh_p_lang_p_Package
{
	const struct vtable_c_boh_p_lang_p_Package * vtable;
	struct c_boh_p_lang_p_String * f_name;
	struct c_boh_p_lang_p_Package * f_owner;
};

