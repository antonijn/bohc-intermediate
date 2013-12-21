using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class TypeCast : Expression
	{
		public readonly typesys.Type towhat;
		public readonly Expression onwhat;

		public TypeCast(typesys.Type towhat, Expression onwhat)
		{
			this.towhat = towhat;
			this.onwhat = onwhat;
		}

		public override typesys.Type getType()
		{
			return towhat;
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
