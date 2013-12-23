using System;

using bohc.typesys;

namespace bohc.parsing
{
	public class NativeFunctionCall : Expression
	{
		public readonly NativeFunction function;
		public readonly Expression[] parameters;

		public NativeFunctionCall(NativeFunction function, Expression[] parameters)
		{
			this.function = function;
			this.parameters = parameters;

			boh.Exception.require<exceptions.ParserException>(function.paramCount == parameters.Length, "Native function requires " + function.paramCount + " parameters");
		}

		public override bohc.typesys.Type getType()
		{
			if (function == NativeFunction.NATIVE_DEREF)
			{
				return ((ExprType)parameters [0]).type;
			}
			else if (function == NativeFunction.NATIVE_REF)
			{
				return Primitive.INT;
			}
			else if (function == NativeFunction.NATIVE_SIZEOF)
			{
				return Primitive.INT;
			}

			throw new NotImplementedException();
		}

		public override bool isLvalue(Function ctx)
		{
			return function == NativeFunction.NATIVE_DEREF;
		}

		public override bool isStatement()
		{
			return function != NativeFunction.NATIVE_DEREF;
		}
	}
}

