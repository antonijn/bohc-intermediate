using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing.statements
{
	public sealed class VariableDeclaration
	{
		public readonly typesys.Local local;

		public VariableDeclaration(typesys.Local local)
		{
			this.local = local;
		}
	}
}
