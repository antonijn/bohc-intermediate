using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class Operator
	{
		public static readonly List<Operator> operators = new List<Operator>();

		public readonly string representation;
		public readonly int precedence;

		public Operator(string representation, int precedence)
		{
			this.representation = representation;
			this.precedence = precedence;

			operators.Add(this);
		}

		public override string ToString()
		{
			return representation;
		}

		public static Operator getExisting(string op, OperationType otype)
		{
			IEnumerable<Operator> result = operators.Where(x => x.representation == op &&
				(otype == OperationType.BINARY ? BinaryOperation.BINARY_PRECEDENCES.Contains(x.precedence) :
				(otype == OperationType.UNARY ? UnaryOperation.UNARY_PRECENDENCES.Contains(x.precedence) : true)));
			boh.Exception.require<exceptions.ParserException>(result.Count() != 0, "No operator found for '" + op + "'");
			if (result.Count() == 2)
			{
				return result.Single(x => !parsing.UnaryOperation.UNARY_PRECENDENCES.Contains(x.precedence));
			}

			return result.Single();
		}

		public static bool isOperator(string op)
		{
			return operators.Any(x => x.representation == op);
		}
	}
}
