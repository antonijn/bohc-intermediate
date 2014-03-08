using System;
using System.Linq;
using System.Collections.Generic;
using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public class FunctionCollection : Expression
	{
		public readonly Dictionary<FunctionRefType, object> functions;

		public FunctionCollection(IEnumerable<object> functions)
		{
			this.functions = functions.ToDictionary(x => {
				if (x is ExprVariable)
				{
					return (FunctionRefType)((ExprVariable)x).getType();
				}
				return FunctionRefType.Get(((Function)x).ReturnType, ((Function)x).Parameters.Select(y => y.Type).ToArray());
			}, x => x);
		}

		public override TypeSystem.Type getType()
		{
			throw new NotImplementedException();
		}

		public override bool isLvalue(Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}

		public override Expression useAsLvalue(BinaryOperation binop)
		{
			throw new NotImplementedException();
		}

		public override void useAsRvalue()
		{
			throw new NotImplementedException();
		}
	}
}

