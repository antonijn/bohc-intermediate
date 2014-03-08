using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing.Statements
{
	public class Scope : Statement
	{
		public Body body;

		public Scope(Body body)
		{
			this.body = body;
		}

		public override bool hasReturned()
		{
			return body.hasReturned();
		}

		public override bool hasSuperBeenCalled()
		{
			return body.hasSuperBeenCalled();
		}
	}
}
