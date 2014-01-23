using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class RefExpression : Expression
	{
		public readonly Expression onwhat;

		public RefExpression(Expression onwhat, Bohc.TypeSystem.Function ctx)
		{
			this.onwhat = onwhat;

			Boh.Exception.require<Exceptions.ParserException>(onwhat.isLvalue(ctx), "ref expression must be performed on lvalue");
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return onwhat.getType();
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
