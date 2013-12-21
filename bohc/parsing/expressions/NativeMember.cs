using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class NativeMember : Expression
	{
		public Expression owner;
		public string representation;

		public NativeMember(Expression owner, string representation)
		{
			this.owner = owner;
			this.representation = representation;
		}

		public override typesys.Type getType()
		{
			return new typesys.CompatibleWithAllType();
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			return true;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
