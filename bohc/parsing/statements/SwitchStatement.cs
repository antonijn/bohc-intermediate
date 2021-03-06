using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class SwitchStatement : Statement
	{
		public Expression expression;
		public List<SwitchLabel> labels;

		public SwitchStatement(Expression expression, List<SwitchLabel> labels)
		{
			this.expression = expression;
			this.labels = labels;
		}
	}
}
