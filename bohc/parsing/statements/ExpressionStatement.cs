using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class ExpressionStatement : Statement
	{
		public readonly Expression expression;

		public ExpressionStatement(Expression expression)
		{
			boh.Exception.require<exceptions.ParserException>(expression.isStatement(), "Expression is not a statement");

			this.expression = expression;
		}
	}
}
