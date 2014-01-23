using System;

using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public class NativeFunctionCall : Expression
	{
		public readonly NativeFunction function;
		public readonly Expression[] parameters;

		public NativeFunctionCall(NativeFunction function, Expression[] parameters)
		{
			this.function = function;
			this.parameters = parameters;

			Boh.Exception.require<Exceptions.ParserException>(function.ParamCount == parameters.Length, "Native function requires " + function.ParamCount + " parameters");
		}

		public override Bohc.TypeSystem.Type getType()
		{
			if (function == NativeFunction.NativeDeref)
			{
				return ((ExprType)parameters [0]).type;
			}
			else if (function == NativeFunction.NativeRef)
			{
				return Primitive.Int;
			}
			else if (function == NativeFunction.NativeSizeof)
			{
				return Primitive.Int;
			}

			throw new NotImplementedException();
		}

		public override bool isLvalue(Function ctx)
		{
			return function == NativeFunction.NativeDeref;
		}

		public override bool isStatement()
		{
			return function != NativeFunction.NativeDeref;
		}
	}
}

