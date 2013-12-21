using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class RefExpression : Expression
	{
		public readonly Expression onwhat;

		public RefExpression(Expression onwhat, typesys.Function ctx)
		{
			this.onwhat = onwhat;

			boh.Exception.require<exceptions.ParserException>(onwhat.isLvalue(ctx), "ref expression must be performed on lvalue");
		}

		public override typesys.Type getType()
		{
			return onwhat.getType();
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
