using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ExprType : Expression
	{
		public readonly typesys.Type type;

		public ExprType(typesys.Type type)
		{
			this.type = type;
		}

		public override typesys.Type getType()
		{
			throw new NotImplementedException();
		}
	}
}
