using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class ExprPackage : Expression
	{
		public readonly typesys.Package refersto;

		public ExprPackage(typesys.Package refersto)
		{
			this.refersto = refersto;
		}

		public override typesys.Type getType()
		{
			throw new NotImplementedException();
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			throw new NotImplementedException();
		}

		public override bool isStatement()
		{
			throw new NotImplementedException();
		}
	}
}
