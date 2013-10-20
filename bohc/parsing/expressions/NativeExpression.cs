using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class NativeExpression : Expression
	{
		public readonly string str;

		public NativeExpression(string str)
		{
			this.str = str;
		}

		public override typesys.Type getType()
		{
			throw new NotImplementedException();
		}

		public override bool isLvalue()
		{
			return true;
		}

		public override bool  isStatement()
		{
			return false;
		}
	}
}
