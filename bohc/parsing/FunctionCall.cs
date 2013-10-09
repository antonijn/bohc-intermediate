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

		public FunctionCall(typesys.Function refersto, Expression belongsto)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
		}

		public override typesys.Type getType()
		{
			return refersto.returnType;
		}
	}
}
