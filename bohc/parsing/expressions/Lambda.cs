using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.Parsing
{
	public class Lambda : Expression
	{
		public Bohc.TypeSystem.FunctionRefType type;
		public Bohc.Parsing.Statements.Body body;
		public Expression expression;
		public List<Bohc.TypeSystem.Variable> enclosed;
		public IEnumerable<Bohc.TypeSystem.LambdaParam> lambdaParams;

		public int lambdaLevel = 0;

		public static readonly List<Lambda> lambdas = new List<Lambda>();

		public Lambda(Bohc.TypeSystem.FunctionRefType type, Bohc.Parsing.Statements.Body body, Expression expression, IEnumerable<Bohc.TypeSystem.LambdaParam> lambdaParams)
		{
			this.type = type;
			this.body = body;
			this.expression = expression;
			this.enclosed = new List<Bohc.TypeSystem.Variable>();
			this.lambdaParams = lambdaParams;

			lambdas.Add(this);
		}

		public override Bohc.TypeSystem.Type getType()
		{
			return type;
		}

		public override bool isLvalue(Bohc.TypeSystem.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
