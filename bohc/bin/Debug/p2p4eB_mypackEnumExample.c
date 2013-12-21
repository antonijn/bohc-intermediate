#include "p2p4eB_mypackEnumExample.h"
struct p3p3c6_bohstdString * p2p4eB_mypackEnumExample_m_toString_35cf4c(enum p2p4eB_mypackEnumExample const self)
{
	if ((p3p3c6_bohstdObject_m_valEquals_4eb476e0((struct p3p3c6_bohstdObject *)(self), (struct p3p3c6_bohstdObject *)(p2p4eB_mypackEnumExampleFIRST))))
	{
		return boh_create_string(u"FIRST", 5);
	}
	else
	{
		if ((p3p3c6_bohstdObject_m_valEquals_4eb476e0((struct p3p3c6_bohstdObject *)(self), (struct p3p3c6_bohstdObject *)(p2p4eB_mypackEnumExampleSECOND))))
		{
			return boh_create_string(u"SECOND", 6);
		}
		else
		{
			if ((p3p3c6_bohstdObject_m_valEquals_4eb476e0((struct p3p3c6_bohstdObject *)(self), (struct p3p3c6_bohstdObject *)(p2p4eB_mypackEnumExampleTHIRD))))
			{
				return boh_create_string(u"THIRD", 5);
			}
			else
			{
				return p03_int_m_toString_35cf4c((int32_t)(self));
			}
		}
	}
}
