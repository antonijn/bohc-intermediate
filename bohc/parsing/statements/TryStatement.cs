using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public class TryStatement : BodyStatement
	{
		public readonly List<CatchStatement> catches;
		public readonly FinallyStatement fin;

		public TryStatement(Body body, List<CatchStatement> catches, FinallyStatement fin)
			: base(body)
		{
			this.catches = catches;
			this.fin = fin;
		}
	}
}
