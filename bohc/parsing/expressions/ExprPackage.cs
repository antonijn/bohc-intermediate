using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class ExprPackage : Expression
	{
		public readonly Bohc.TypeSystem.Package refersto;

		public ExprPackage(Bohc.TypeSystem.Package refersto)
		{
			this.refersto = refersto;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			throw new NotImplementedException();
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			throw new NotImplementedException();
		}

		public override bool isStatement()
		{
			throw new NotImplementedException();
		}
	}
}
