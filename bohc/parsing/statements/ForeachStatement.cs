using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class ForeachStatement : BodyStatement
	{
		public VarDeclaration vardecl;
		public Expression expr;

		public ForeachStatement(VarDeclaration vardecl, Expression expr, Body body)
			: base(body)
		{
			this.vardecl = vardecl;
			this.expr = expr;
		}
	}
}
