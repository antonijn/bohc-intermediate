using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class DoWhileStatement : BodyStatement
	{
		public readonly Expression condition;
		public readonly Body body;

		public DoWhileStatement(Expression condition, Body body)
		{
			boh.Exception.require<exceptions.ParserException>(condition.getType() == typesys.Primitive.BOOLEAN, "Condition must be boolean");

			this.condition = condition;
			this.body = body;
		}
	}
}
