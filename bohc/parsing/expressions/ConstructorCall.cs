using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ConstructorCall : Expression
	{
		public readonly typesys.Constructor function;
		public readonly Expression[] parameters;

		public ConstructorCall(typesys.Constructor function, Expression[] parameters)
		{
			this.function = function;
			this.parameters = parameters;
		}

		public override typesys.Type getType()
		{
			return function.owner;
		}

		public override bool isLvalue()
		{
			return false;
		}

		public override bool isStatement()
		{
			return true;
		}
	}
}
