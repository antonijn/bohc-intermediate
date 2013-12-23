using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class StaticConstructor : Function
	{
		public StaticConstructor(Class c, string body)
			: base(c, Modifiers.PUBLIC | Modifiers.STATIC, Primitive.VOID, "static", new List<Parameter>(), body)
		{
		}
	}
}
