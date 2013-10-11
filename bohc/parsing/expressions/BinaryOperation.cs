using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
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

		public static readonly Operator ASSIGN = opnew(new Operator("=", 0));
		public static readonly Operator ASSIGN_ADD = opnew(new Operator("+=", 0));
		public static readonly Operator ASSIGN_SUB = opnew(new Operator("-=", 0));
		public static readonly Operator ASSIGN_MUL = opnew(new Operator("*=", 0));
		public static readonly Operator ASSIGN_DIV = opnew(new Operator("/=", 0));
		public static readonly Operator ASSIGN_REM = opnew(new Operator("%=", 0));
		public static readonly Operator ASSIGN_SHL = opnew(new Operator("<<=", 0));
		public static readonly Operator ASSIGN_SHR = opnew(new Operator(">>=", 0));
		public static readonly Operator ASSIGN_AND = opnew(new Operator("&=", 0));
		public static readonly Operator ASSIGN_OR = opnew(new Operator("|=", 0));
		public static readonly Operator ASSIGN_XOR = opnew(new Operator("^=", 0));

		public static readonly Operator CONDIT_AND = opnew(new Operator("&&", 1));
		public static readonly Operator CONDIT_OR = opnew(new Operator("||", 1));

		public static readonly Operator LOGIC_AND = opnew(new Operator("&", 2));
		public static readonly Operator LOGIC_OR = opnew(new Operator("|", 2));
		public static readonly Operator LOGIC_XOR = opnew(new Operator("^", 2));

		public static readonly Operator EQUAL = opnew(new Operator("==", 3));
		public static readonly Operator NOT_EQUAL = opnew(new Operator("!=", 3));
		public static readonly Operator RELAT_L = opnew(new Operator("<", 3));
		public static readonly Operator RELAT_GE = opnew(new Operator(">=", 3));
		public static readonly Operator RELAT_LE = opnew(new Operator("<=", 3));

		public static readonly Operator SHL = opnew(new Operator("<<", 4));
		public static readonly Operator SHR = opnew(new Operator(">>", 4));

		public static readonly Operator ADD = opnew(new Operator("+", 5));
		public static readonly Operator SUB = opnew(new Operator("-", 5));

		public static readonly Operator DIV = opnew(new Operator("/", 6));
		public static readonly Operator MUL = opnew(new Operator("*", 6));
		public static readonly Operator REM = opnew(new Operator("%", 6));

		public readonly Expression left;
		public readonly Expression right;
		public readonly Operator operation;

		public BinaryOperation(Expression left, Expression right, string rep)
		{
			this.left = left;
			this.right = right;
			this.operation = Operator.getExisting(rep, OperationType.BINARY);
		}

		public BinaryOperation(Expression left, Expression right, Operator operation)
		{
			this.left = left;
			this.right = right;
			this.operation = operation;
		}

		private typesys.Type getTypeArith()
		{
			typesys.Primitive priml = left.getType() as typesys.Primitive;
			typesys.Primitive primr = right.getType() as typesys.Primitive;

			if (priml != null && primr != null)
			{
				if ((priml == typesys.Primitive.DOUBLE &&
					primr != typesys.Primitive.DOUBLE) ||
					(primr == typesys.Primitive.DOUBLE &&
					priml != typesys.Primitive.DOUBLE))
				{
					return typesys.Primitive.DOUBLE;
				}

				if ((priml == typesys.Primitive.FLOAT) ||
					(primr == typesys.Primitive.FLOAT))
				{
					return typesys.Primitive.FLOAT;
				}

				return priml.size > primr.size ? priml : primr;
			}

			typesys.Class cleft = left.getType() as typesys.Class;
			typesys.Class cright = right.getType() as typesys.Class;

			if (cleft != null)
			{
				typesys.Type type = getOtherTypeArith(operation.representation, cleft, right.getType(), false);
				if (type != null)
				{
					return type;
				}
			}
			if (cright != null)
			{
				typesys.Type type = getOtherTypeArith(operation.representation, left.getType(), cright, false);
				if (type != null)
				{
					return type;
				}
			}

			boh.Exception._throw<exceptions.ParserException>(operation.representation + " isn't defined for " + left.getType().name + " and " + right.getType().name);
			return null;
		}

		private static typesys.Type getOtherTypeArith(string op, typesys.Class c, typesys.Type other, bool shouldThrow)
		{
			return c.getFunctions(op, c)
				.Select(x => new Tuple<typesys.Function, int>(x, other.extends(x.parameters.Last().type)))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.First().returnType;
		}

		private static typesys.Type getOtherTypeArith(string op, typesys.Type other, typesys.Class c, bool shouldThrow)
		{
			return c.getFunctions(op, c)
				.Select(x => new Tuple<typesys.Function, int>(x, other.extends(x.parameters.First().type)))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.First().returnType;
		}

		public override typesys.Type getType()
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

			if (operation == RELAT_GE || operation == RELAT_L ||
				operation == RELAT_LE || operation == EQUAL)
			{
				return typesys.Primitive.BOOLEAN;
			}

			return getTypeArith();
		}
	}
}
