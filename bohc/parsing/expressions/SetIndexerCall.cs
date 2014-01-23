using System;
using System.Collections.Generic;

namespace Bohc.Parsing
{
	public class SetIndexerCall : Expression
	{
		public readonly IndexerCall origin;
		public readonly TypeSystem.Function setter;
		public readonly Expression towhat;
		public readonly Expression belongsto;
		public readonly IEnumerable<Expression> parameters;

		public SetIndexerCall(IndexerCall origin, Bohc.TypeSystem.Function setter, Expression towhat, Expression belongsto, IEnumerable<Expression> parameters)
		{
			this.origin = origin;
			this.setter = setter;
			this.towhat = towhat;
			this.belongsto = belongsto;
			this.parameters = parameters;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return setter.ReturnType;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return true;
		}
	}
}

