using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class FinallyStatement : BodyStatement
	{
		public FinallyStatement(Body body)
			: base(body)
		{
		}
	}
}
