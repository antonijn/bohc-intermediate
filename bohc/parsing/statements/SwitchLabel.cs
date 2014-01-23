using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class SwitchLabel
	{
		public Body body;

		public SwitchLabel(Body body)
		{
			this.body = body;
		}
	}
}
