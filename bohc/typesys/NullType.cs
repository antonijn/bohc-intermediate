using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class NullType : Type
	{
		public static readonly NullType NULL = new NullType();

		private NullType()
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, "\r")
		{
		}

		public override parsing.Expression defaultVal()
		{
			return new parsing.Literal(this, "NULL");
		}

		public override int extends(Type other)
		{
			if (other is Class || other is Interface)
			{
				return 1;
			}

			return 0;
		}
	}
}
