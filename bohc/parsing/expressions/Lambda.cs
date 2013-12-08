using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.parsing
{
	public class Lambda : Expression
	{
		public typesys.FunctionRefType type;
		public parsing.statements.Body body;
		public Expression expression;
		public List<typesys.Variable> enclosed;
		public IEnumerable<typesys.LambdaParam> lambdaParams;

		public int lambdaLevel = 0;

		public static readonly List<Lambda> lambdas = new List<Lambda>();

		public Lambda(typesys.FunctionRefType type, parsing.statements.Body body, Expression expression, IEnumerable<typesys.LambdaParam> lambdaParams)
		{
			this.type = type;
			this.body = body;
			this.expression = expression;
			this.enclosed = new List<typesys.Variable>();
			this.lambdaParams = lambdaParams;

			lambdas.Add(this);
		}

		public override typesys.Type getType()
		{
			return type;
		}

		public override bool isLvalue(typesys.Function ctx)
		{
			return false;
		}

		public override bool isStatement()
		{
			return false;
		}
	}
}
