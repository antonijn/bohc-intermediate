using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class NativeFCall : Expression
	{
		public readonly string str;
		public readonly Expression[] parameters;

		public NativeFCall(string str, Expression[] parameters)
		{
			this.str = str;
			this.parameters = parameters;
		}

		public override typesys.Type getType()
		{
			return new typesys.CompatibleWithAllType();
		}

		public override bool isLvalue()
		{
			return false;
		}

		public override bool isStatement()
		{
			return true;
		}
	}
}
