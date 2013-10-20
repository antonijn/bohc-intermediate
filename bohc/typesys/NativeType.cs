using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class NativeType : Type
	{
		public readonly string crep;

		public NativeType(string str)
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, "native." + str)
		{
			this.crep = str;
		}

		public override parsing.Expression defaultVal()
		{
			if (crep.Contains('*'))
			{
				return NullType.NULL.defaultVal();
			}

			return new parsing.Literal(this, "0");
		}
	}
}
