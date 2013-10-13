using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class UnaryOperation : Expression
	{
		public static readonly int[] UNARY_PRECENDENCES = new int[] { 7, 8 };

		public static readonly Operator PLUS = new Operator("+", 7);
		public static readonly Operator MINUS = new Operator("-", 7);
		public static readonly Operator INCREMENT = new Operator("++", 7);
		public static readonly Operator INCREMENT_POST = new Operator("++", 7);
		public static readonly Operator DECREMENT = new Operator("--", 7);
		public static readonly Operator DECREMENT_POST = new Operator("--", 7);
		public static readonly Operator INVERT = new Operator("!", 7);
		public static readonly Operator NOT = new Operator("~", 7);
		public static readonly Operator TYPEOF = new Operator("typeof", 8);

		public readonly Expression onwhat;
		public readonly Operator operation;

		public UnaryOperation(Expression onwhat, Operator operation)
		{
			this.onwhat = onwhat;
			this.operation = operation;
		}

		public override typesys.Type getType()
		{
			if (operation == TYPEOF)
			{
				return typesys.Type.getExisting(typesys.Package.getFromString("boh.lang"), "Type");
			}
			return onwhat.getType();
		}

		public override bool isLvalue()
		{
			return false;
		}

		public override bool isStatement()
		{
			return operation == INCREMENT_POST ||
			       operation == INCREMENT ||
				   operation == DECREMENT_POST ||
				   operation == DECREMENT;
		}
	}
}
