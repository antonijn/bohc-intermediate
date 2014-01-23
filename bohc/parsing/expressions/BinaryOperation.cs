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
	public class BinaryOperation : Expression
	{
		private static readonly Dictionary<string, Operator> operators = new Dictionary<string, Operator>();

		public static bool isOperator(string op)
		{
			return operators.ContainsKey(op);
		}

		public static Operator get(string str)
		{
			return operators[str];
		}

		private static Operator opnew(Operator op)
		{
			operators[op.representation] = op;
			return op;
		}

		public static readonly int[] BINARY_PRECEDENCES = new int[] { 0, 1, 2, 3, 4, 5, 6 };

		public static readonly Operator ASSIGN = opnew(new Operator("=", 0, null));
		public static readonly Operator ASSIGN_ADD = opnew(new Operator("+=", 0, null));
		public static readonly Operator ASSIGN_SUB = opnew(new Operator("-=", 0, null));
		public static readonly Operator ASSIGN_MUL = opnew(new Operator("*=", 0, null));
		public static readonly Operator ASSIGN_DIV = opnew(new Operator("/=", 0, null));
		public static readonly Operator ASSIGN_REM = opnew(new Operator("%=", 0, null));
		public static readonly Operator ASSIGN_SHL = opnew(new Operator("<<=", 0, null));
		public static readonly Operator ASSIGN_SHR = opnew(new Operator(">>=", 0, null));
		public static readonly Operator ASSIGN_AND = opnew(new Operator("&=", 0, null));
		public static readonly Operator ASSIGN_OR = opnew(new Operator("|=", 0, null));
		public static readonly Operator ASSIGN_XOR = opnew(new Operator("^=", 0, null));

		public static readonly Operator CONDIT_AND = opnew(new Operator("&&", 1, null));
		public static readonly Operator CONDIT_OR = opnew(new Operator("||", 1, null));

		public static readonly Operator LOGIC_AND = opnew(new Operator("&", 2, "and"));
		public static readonly Operator LOGIC_OR = opnew(new Operator("|", 2, "or"));
		public static readonly Operator LOGIC_XOR = opnew(new Operator("^", 2, "xor"));

		public static readonly Operator EQUAL = opnew(new Operator("==", 3, "equals"));
		public static readonly Operator R_EQ = opnew(new Operator("r_eq", 3, null));
		public static readonly Operator NOT_EQUAL = opnew(new Operator("!=", 3, null));
		public static readonly Operator RELAT_L = opnew(new Operator("<", 3, "less"));
		public static readonly Operator RELAT_G = opnew(new Operator(">", 3, "greater"));
		public static readonly Operator RELAT_GE = opnew(new Operator(">=", 3, "greater_equal"));
		public static readonly Operator RELAT_LE = opnew(new Operator("<=", 3, "less_equal"));

		public static readonly Operator SHL = opnew(new Operator("<<", 4, "shl"));
		public static readonly Operator SHR = opnew(new Operator(">>", 4, "shr"));

		public static readonly Operator ADD = opnew(new Operator("+", 5, "add"));
		public static readonly Operator SUB = opnew(new Operator("-", 5, "sub"));

		public static readonly Operator DIV = opnew(new Operator("/", 6, "div"));
		public static readonly Operator MUL = opnew(new Operator("*", 6, "mul"));
		public static readonly Operator REM = opnew(new Operator("%", 6, "rem"));

		public readonly Expression left;
		public readonly Expression right;
		public readonly Operator operation;

		public readonly Bohc.TypeSystem.OverloadedOperator overloaded;

		public BinaryOperation(Expression left, Expression right, string rep)
			: this(left, right, rep, null)
		{
		}

		public BinaryOperation(Expression left, Expression right, string rep, Bohc.TypeSystem.Function ctx)
			: this(left, right, Operator.getExisting(rep, OperationType.BINARY), ctx)
		{
		}

		public BinaryOperation(Expression left, Expression right, Operator operation)
			: this(left, right, operation, null)
		{
		}

		public BinaryOperation(Expression left, Expression right, Operator operation, Bohc.TypeSystem.Function ctx)
		{
			this.left = left;
			this.right = right;
			this.operation = operation;

			Boh.Exception.require<Exceptions.ParserException>(!isAssignment() || left.isLvalue(ctx), left.ToString() + ": not a modifiable lvalue");

			if (operation == ASSIGN_ADD)
			{
				this.operation = ASSIGN;
				this.right = new BinaryOperation(left, right, ADD);
			}
			else if (operation == ASSIGN_DIV)
			{
				this.operation = ASSIGN;
				this.right = new BinaryOperation(left, right, DIV);
			}
			else if (operation == ASSIGN_MUL)
			{
				this.operation = ASSIGN;
				this.right = new BinaryOperation(left, right, MUL);
			}
			else if (operation == ASSIGN_REM)
			{
				this.operation = ASSIGN;
				this.right = new BinaryOperation(left, right, REM);
			}
			else if (operation == ASSIGN_SUB)
			{
				this.operation = ASSIGN;
				this.right = new BinaryOperation(left, right, SUB);
			}

			if (operation != ASSIGN)
			{
				overloaded = figureOutOverload();
			}
		}

		private Bohc.TypeSystem.OverloadedOperator figureOutOverload()
		{
			Bohc.TypeSystem.Class cleft = left.getType() as Bohc.TypeSystem.Class;
			Bohc.TypeSystem.Class cright = right.getType() as Bohc.TypeSystem.Class;

			if (cleft != null)
			{
				try
				{
					Bohc.TypeSystem.OverloadedOperator type = getOtherTypeOlF(operation.representation, cleft, right.getType(), false);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}
			if (cright != null)
			{
				try
				{
					Bohc.TypeSystem.OverloadedOperator type = getOtherTypeOlF(operation.representation, left.getType(), cright, false);
					if (type != null)
					{
						return type;
					}
				}
				catch
				{
				}
			}

			return null;
		}

		private Bohc.TypeSystem.Type getTypeArith()
		{
			Bohc.TypeSystem.Primitive priml = left.getType() as Bohc.TypeSystem.Primitive;
			Bohc.TypeSystem.Primitive primr = right.getType() as Bohc.TypeSystem.Primitive;

			if (priml != null && primr != null)
			{
				if ((priml == Bohc.TypeSystem.Primitive.Double &&
					primr != Bohc.TypeSystem.Primitive.Double) ||
					(primr == Bohc.TypeSystem.Primitive.Double &&
					priml != Bohc.TypeSystem.Primitive.Double))
				{
					return Bohc.TypeSystem.Primitive.Double;
				}

				if ((priml == Bohc.TypeSystem.Primitive.Float) ||
					(primr == Bohc.TypeSystem.Primitive.Float))
				{
					return Bohc.TypeSystem.Primitive.Float;
				}

				return priml.Size > primr.Size ? priml : primr;
			}

			Bohc.TypeSystem.Class cleft = left.getType() as Bohc.TypeSystem.Class;
			Bohc.TypeSystem.Class cright = right.getType() as Bohc.TypeSystem.Class;

			if (cleft != null)
			{
				Bohc.TypeSystem.Type type = getOtherTypeArith(operation.representation, cleft, right.getType(), false);
				if (type != null)
				{
					return type;
				}
			}
			if (cright != null)
			{
				Bohc.TypeSystem.Type type = getOtherTypeArith(operation.representation, left.getType(), cright, false);
				if (type != null)
				{
					return type;
				}
			}

			Boh.Exception._throw<Exceptions.ParserException>(operation.representation + " isn't defined for " + left.getType().Name + " and " + right.getType().Name);
			return null;
		}

		private static Bohc.TypeSystem.OverloadedOperator getOtherTypeOlF(string op, Bohc.TypeSystem.Class c, Bohc.TypeSystem.Type other, bool shouldThrow)
		{
			Tuple<Bohc.TypeSystem.Function, int>[] funcs = c.GetFunctions(op, c)
					.Where(x => c.Extends(x.Parameters.First().Type) != 0)
					.Select(x => new Tuple<Bohc.TypeSystem.Function, int>(x, other.Extends(x.Parameters.Last().Type)))
					.OrderBy(x => x.Item2)
					.Where(x => x.Item1.Parameters.Count == 0 || x.Item2 != 0).ToArray();
			//int i = other.extends(typesys.Primitive.CHAR);
			return funcs.Select(x => x.Item1).First() as Bohc.TypeSystem.OverloadedOperator;
		}

		private static Bohc.TypeSystem.OverloadedOperator getOtherTypeOlF(string op, Bohc.TypeSystem.Type other, Bohc.TypeSystem.Class c, bool shouldThrow)
		{
			return c.GetFunctions(op, c)
				.Where(x => c.Extends(x.Parameters.Last().Type) != 0)
				.Select(x => new Tuple<Bohc.TypeSystem.Function, int>(x, other.Extends(x.Parameters.First().Type)))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.Parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.First() as Bohc.TypeSystem.OverloadedOperator;
		}

		private static Bohc.TypeSystem.Type getOtherTypeArith(string op, Bohc.TypeSystem.Class c, Bohc.TypeSystem.Type other, bool shouldThrow)
		{
			return c.GetFunctions(op, c)
				.Where(x => c.Extends(x.Parameters.First().Type) != 0)
				.Select(x => new Tuple<Bohc.TypeSystem.Function, int>(x, other.Extends(x.Parameters.Last().Type)))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.Parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.First().ReturnType;
		}

		private static Bohc.TypeSystem.Type getOtherTypeArith(string op, Bohc.TypeSystem.Type other, Bohc.TypeSystem.Class c, bool shouldThrow)
		{
			return c.GetFunctions(op, c)
				.Where(x => c.Extends(x.Parameters.Last().Type) != 0)
				.Select(x => new Tuple<Bohc.TypeSystem.Function, int>(x, other.Extends(x.Parameters.First().Type)))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.Parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.First().ReturnType;
		}

		public override Bohc.TypeSystem.Type getType()
		{
			if (operation == ASSIGN || operation == ASSIGN_ADD ||
				operation == ASSIGN_AND || operation == ASSIGN_DIV ||
				operation == ASSIGN_MUL || operation == ASSIGN_OR ||
				operation == ASSIGN_REM || operation == ASSIGN_SHL ||
				operation == ASSIGN_SHR || operation == ASSIGN_SUB ||
				operation == ASSIGN_XOR)
			{
				return left.getType();
			}

			if (operation == SHL || operation == SHR ||
				operation == LOGIC_AND || operation == LOGIC_OR ||
				operation == LOGIC_XOR)
			{
				return left.getType();
			}

			if (operation == RELAT_GE || operation == RELAT_L || operation == RELAT_G || operation == R_EQ ||
				operation == RELAT_LE || operation == EQUAL || operation == NOT_EQUAL)
			{
				return Bohc.TypeSystem.Primitive.Boolean;
			}

			return getTypeArith();
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public bool isAssignment()
		{
			if (operation == ASSIGN || operation == ASSIGN_ADD ||
				operation == ASSIGN_AND || operation == ASSIGN_DIV ||
				operation == ASSIGN_MUL || operation == ASSIGN_OR ||
				operation == ASSIGN_REM || operation == ASSIGN_SHL ||
				operation == ASSIGN_SHR || operation == ASSIGN_SUB ||
				operation == ASSIGN_XOR)
			{
				return true;
			}

			return false;
		}

		public override bool isStatement()
		{
			return isAssignment();
		}
	}
}
