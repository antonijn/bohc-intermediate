using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class ThrowStatement : Statement
	{
		public readonly Expression exception;

		public ThrowStatement(Expression exception)
		{
			this.exception = exception;
		}
	}
}
