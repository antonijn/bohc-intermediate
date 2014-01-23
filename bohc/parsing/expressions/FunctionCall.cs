// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class FunctionCall : Expression
	{
		public readonly Bohc.TypeSystem.Function refersto;
		public readonly Expression belongsto;
		public readonly Expression[] parameters;

		public FunctionCall(Bohc.TypeSystem.Function refersto, Expression belongsto, IEnumerable<Expression> parameters)
		{
			// check whether the refs are alright
			TypeSystem.Indexer idxer = refersto as TypeSystem.Indexer;

			int i = 0;
			foreach (Expression expr in idxer == null ? parameters : parameters.Take(refersto.Parameters.Count - 1))
			{
				Bohc.TypeSystem.Parameter corresponding = refersto.Parameters[i++];
				Boh.Exception.require<Exceptions.ParserException>(
					(!corresponding.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Ref) ||
					(expr is RefExpression)),
					"ref parameter requires ref expression");
				Boh.Exception.require<Exceptions.ParserException>(
					(corresponding.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Ref) ||
					!(expr is RefExpression)),
					"ref expression requires ref parameter");
			}

			this.refersto = refersto;
			this.belongsto = belongsto;
			this.parameters = parameters.ToArray();
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return refersto.ReturnType;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			TypeSystem.Indexer idx = refersto as TypeSystem.Indexer;
			return idx == null ? false : idx.IsAssignment();
		}

		public override bool isStatement()
		{
			return true;
		}
	}
}
