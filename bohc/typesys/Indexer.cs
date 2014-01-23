using System;
using System.Collections.Generic;
using System.Linq;

using Bohc.Parsing.Statements;

namespace Bohc.TypeSystem
{
	public class Indexer : Function
	{
		public Parameter Assignment;
		public bool IsAssignment()
		{
			return Assignment != null;
		}

		public Indexer(Type owner, Modifiers mods, Type returnType, List<Parameter> indices, string body)
			: base(owner, mods, returnType, "indexer", indices, body)
		{
		}
	}
}

