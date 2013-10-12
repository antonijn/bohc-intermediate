using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class ForStatement : BodyStatement
	{
		public readonly Statement initial;
		public readonly Expression condition;
		public readonly Statement final;

		public ForStatement(Statement initial, Expression condition, Statement final, Body body)
			: base(body)
		{
			boh.Exception.require<exceptions.ParserException>(condition.getType() == typesys.Primitive.BOOLEAN, "Condition must be boolean");

			this.initial = initial;
			this.condition = condition;
			this.final = final;
		}
	}
}
