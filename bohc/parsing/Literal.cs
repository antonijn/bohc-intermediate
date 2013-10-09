using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class Literal : Expression
	{
		public readonly typesys.Type type;
		public readonly string representation;

		public Literal(typesys.Type type, string representation)
		{
			this.type = type;
			this.representation = representation;
		}

		public override typesys.Type getType()
		{
			return type;
		}
	}
}
