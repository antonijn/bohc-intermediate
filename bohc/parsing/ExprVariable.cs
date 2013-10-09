using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ExprVariable : Expression
	{
		public readonly typesys.Variable refersto;

		// NOTE:
		// belongsto is null for static variables
		// the class to which it belongs should be sought through "refersto"
		// refersto will be a field in that case, so the container class can be found out
		public readonly Expression belongsto;

		public ExprVariable(typesys.Variable refersto, Expression belongsto)
		{
			this.refersto = refersto;
			this.belongsto = belongsto;
		}

		public override typesys.Type getType()
		{
			return refersto.type;
		}
	}
}
