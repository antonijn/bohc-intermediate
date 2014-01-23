using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class TypeCast : Expression
	{
		public readonly Bohc.TypeSystem.Type towhat;
		public readonly Expression onwhat;

		public TypeCast(Bohc.TypeSystem.Type towhat, Expression onwhat)
		{
			this.towhat = towhat;
			this.onwhat = onwhat;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return towhat;
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
