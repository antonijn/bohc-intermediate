using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public sealed class NewDummy : Expression
	{
		public override typesys.Type getType()
		{
			return null;
		}

		public override bool isLvalue()
		{
			throw new NotImplementedException();
		}

		public override bool isStatement()
		{
			throw new NotImplementedException();
		}
	}
}
