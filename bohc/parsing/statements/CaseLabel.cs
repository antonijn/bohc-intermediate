using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class CaseLabel : SwitchLabel
	{
		public Expression expression;

		public CaseLabel(Expression expression, Body body)
			: base(body)
		{
			this.expression = expression;
		}
	}
}
