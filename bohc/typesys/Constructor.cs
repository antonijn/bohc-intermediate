using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Constructor : Function
	{
		public readonly Class _class;

		public Constructor(Modifiers mods, Class c, List<Parameter> parameters)
			: base(mods, Primitive.VOID, "this", parameters)
		{
			this._class = c;
		}
	}
}
