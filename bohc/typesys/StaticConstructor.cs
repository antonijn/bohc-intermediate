using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.TypeSystem
{
	public class StaticConstructor : Function
	{
		public StaticConstructor(Class c, string body)
			: base(c, Modifiers.Public | Modifiers.Static, Primitive.Void, "static", new List<Parameter>(), body)
		{
		}
	}
}
