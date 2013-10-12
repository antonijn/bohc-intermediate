using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class WhileStatement : BodyStatement
	{
		public readonly Expression condition;

		public WhileStatement(Expression condition, Body body)
			: base(body)
		{
			boh.Exception.require<exceptions.ParserException>(condition.getType() == typesys.Primitive.BOOLEAN, "Condition must be boolean");

			this.condition = condition;
		}
	}
}
