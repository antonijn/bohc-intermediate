using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class ForeachStatement : BodyStatement
	{
		public VarDeclaration vardecl;
		public Expression expr;

		public ForeachStatement(VarDeclaration vardecl, Expression expr, Body body)
			: this(vardecl, expr, new Scope(body)) { }
		public ForeachStatement(VarDeclaration vardecl, Expression expr, Statement body)
			: base(body)
		{
			this.vardecl = vardecl;
			++vardecl.refersto.usageCount;
			this.expr = expr;
		}
	}
}
