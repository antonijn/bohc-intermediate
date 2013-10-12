#ifndef HEY_HI_CLASS_H
#define HEY_HI_CLASS_H
#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>
struct c_hey_p_hi_p_Class;
extern struct c_hey_p_hi_p_Class * new_c_hey_p_hi_p_Class(void);
extern void c_hey_p_hi_p_Class_m_this(struct c_hey_p_hi_p_Class * self);
extern void c_hey_p_hi_p_Class_m_main(void);
struct vtable_c_hey_p_hi_p_Class
{
	struct c_hey_p_hi_p_Class * (*c_hey_p_hi_p_Class_m_get)(struct c_hey_p_hi_p_Class * self, float p_f);
}
extern const instance_vtable_c_hey_p_hi_p_Class;
struct c_hey_p_hi_p_Class
{
	struct vtable_c_hey_p_hi_p_Class * vtable;
	int32_t f_ah;
};
#endif
