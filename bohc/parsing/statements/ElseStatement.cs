using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class ElseStatement : BodyStatement
	{
		public readonly Body body;

		public ElseStatement(Body body)
		{
			this.body = body;
		}
	}
}
