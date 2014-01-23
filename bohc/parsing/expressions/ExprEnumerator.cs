using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class ExprEnumerator : Expression
	{
		public readonly Bohc.TypeSystem.Enumerator enumerator;

		public ExprEnumerator(Bohc.TypeSystem.Enumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return enumerator.EnumType;
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
