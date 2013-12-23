#include "p3p3c9_bohstdException.h"


#if defined(PF_WINDOWS)
static void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self);
#endif
#if defined(PF_LINUX) || defined(PF_OSX) || defined(PF_ANDROID)
static void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self);
#endif

const struct vtable_p3p3c9_bohstdException instance_vtable_p3p3c9_bohstdException = { &p3p3c6_bohstdObject_m_equals_e9664e21, &p3p3c6_bohstdObject_m_hash_35cf4c, &p3p3c6_bohstdObject_m_getType_35cf4c, &p3p3c6_bohstdObject_m_toString_35cf4c, &p3p3c9_bohstdException_m_what_35cf4c };

struct p3p3c4_bohstdType * typeof_p3p3c9_bohstdException(void)
{
	struct p3p3c4_bohstdType * result = NULL;
	if (result == NULL)
	{
	}
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_f13b0af3(struct p3p3c6_bohstdString * p_description)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_0();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_f13b0af3(result, p_description);
	return result;
}
struct p3p3c9_bohstdException * new_p3p3c9_bohstdException_35cf4c(void)
{
	struct p3p3c9_bohstdException * result = GC_malloc(sizeof(struct p3p3c9_bohstdException));
	result->vtable = &instance_vtable_p3p3c9_bohstdException;
	p3p3c9_bohstdException_m_static_0();
	p3p3c9_bohstdException_fi(result);
	p3p3c9_bohstdException_m_this_35cf4c(result);
	return result;
}

void p3p3c9_bohstdException_fi(struct p3p3c9_bohstdException * const self)
{
	self->f_description = NULL;
}

struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_what_35cf4c(struct p3p3c9_bohstdException * const self)
{
	return boh_create_string(u"Something went wrong in the application", 39);
}
struct p3p3c6_bohstdString * p3p3c9_bohstdException_m_getDescription_35cf4c(struct p3p3c9_bohstdException * const self)
{
	return self->f_description;
}
#if defined(PF_WINDOWS)
void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self)
{
}
#endif
#if defined(PF_LINUX) || defined(PF_OSX) || defined(PF_ANDROID)
void p3p3c9_bohstdException_m_genStackTrace_35cf4c(struct p3p3c9_bohstdException * const self)
{
}
#endif
void p3p3c9_bohstdException_m_this_f13b0af3(struct p3p3c9_bohstdException * const self, struct p3p3c6_bohstdString * p_description)
{
	(self->f_description = p_description);
	p3p3c9_bohstdException_m_genStackTrace_35cf4c(self);
}
void p3p3c9_bohstdException_m_this_35cf4c(struct p3p3c9_bohstdException * const self)
{
	p3p3c9_bohstdException_m_this_f13b0af3(self, boh_create_string(u"", 0));
}
void p3p3c9_bohstdException_m_static_0(void)
{
	_Bool hasBeenCalled = 0;
	if (hasBeenCalled)
	{
		return;
	}
	hasBeenCalled = 1;
	p3p3c6_bohstdObject_m_static_0();
	{
	}
}
