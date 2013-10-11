using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class VarDeclaration : Statement
	{
		public readonly typesys.Local refersto;
		public readonly Expression initial;

		public VarDeclaration(typesys.Local refersto, Expression initial)
		{
			this.refersto = refersto;
			this.initial = initial;
		}
	}
}
