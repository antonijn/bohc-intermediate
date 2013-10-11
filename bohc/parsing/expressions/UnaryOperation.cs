using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class UnaryOperation : Expression
	{
		public static readonly int[] UNARY_PRECENDENCES = new int[] { 7 };

		public static readonly Operator PLUS = new Operator("+", 7);
		public static readonly Operator MINUS = new Operator("-", 7);
		public static readonly Operator INCREMENT = new Operator("++", 7);
		public static readonly Operator DECREMENT = new Operator("--", 7);
		public static readonly Operator INVERT = new Operator("!", 7);
		public static readonly Operator NOT = new Operator("~", 7);

		public readonly Expression onwhat;
		public readonly Operator operation;

		public UnaryOperation(Expression onwhat, Operator operation)
		{
			this.onwhat = onwhat;
			this.operation = operation;
		}

		public override typesys.Type getType()
		{
			return onwhat.getType();
		}
	}
}
