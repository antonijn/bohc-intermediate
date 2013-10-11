using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class ReturnStatement : Statement
	{
		public readonly Expression returns;

		public ReturnStatement(Expression returns)
		{
			this.returns = returns;
		}
	}
}
