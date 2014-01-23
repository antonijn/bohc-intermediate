using System;
using System.Collections.Generic;
using System.Linq;

namespace Bohc.Parsing
{
	public class IndexerCall : Expression
	{
		public readonly Bohc.TypeSystem.Indexer getter;
		public readonly Bohc.TypeSystem.Indexer setter;
		public readonly Expression belongsto;
		public readonly Expression[] parameters;

		public IndexerCall(Bohc.TypeSystem.Indexer getter, Bohc.TypeSystem.Indexer setter, Expression belongsto, IEnumerable<Expression> parameters)
		{
			// check whether the refs are alright
			int i = 0;
			foreach (Expression expr in parameters)
			{
				Bohc.TypeSystem.Parameter corresponding = (getter != null ? getter : setter).Parameters[i++];
				Boh.Exception.require<Exceptions.ParserException>(
					(!corresponding.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Ref) ||
				 (expr is RefExpression)),
					"ref parameter requires ref expression");
				Boh.Exception.require<Exceptions.ParserException>(
					(corresponding.Modifiers.HasFlag(Bohc.TypeSystem.Modifiers.Ref) ||
				 !(expr is RefExpression)),
					"ref expression requires ref parameter");
			}

			this.getter = getter;
			this.setter = setter;
			this.belongsto = belongsto;
			this.parameters = parameters.ToArray();
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return getter.ReturnType;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return setter != null;
		}

		public override Expression useAsLvalue(BinaryOperation binop)
		{
			return new SetIndexerCall(this, setter, binop.right, belongsto, parameters);
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}

