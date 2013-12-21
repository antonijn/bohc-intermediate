using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ExprEnumerator : Expression
	{
		public readonly typesys.Enumerator enumerator;

		public ExprEnumerator(typesys.Enumerator enumerator)
		{
			this.enumerator = enumerator;
		}

		public override typesys.Type getType()
		{
			return enumerator.enumType;
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
