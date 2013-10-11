using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class FunctionCall : Expression
	{
		public readonly typesys.Function refersto;
		public readonly Expression belongsto;
		public readonly Expression[] parameters;

		public FunctionCall(typesys.Function refersto, Expression belongsto, IEnumerable<Expression> parameters)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
			this.parameters = parameters.ToArray();
		}

		public override typesys.Type getType()
		{
			return refersto.returnType;
		}
	}
}
