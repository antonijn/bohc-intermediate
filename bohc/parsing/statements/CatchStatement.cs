using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class CatchStatement : BodyStatement
	{
		public readonly typesys.Parameter param;

		public CatchStatement(typesys.Parameter param, Body body)
			: base(body)
		{
			this.param = param;
		}
	}
}
