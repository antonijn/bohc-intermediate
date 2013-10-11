using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public sealed class OverloadedOperator : Function
	{
		public readonly parsing.Operator which;
		public readonly parsing.OperationType optype;

		public OverloadedOperator(typesys.Class owner, parsing.Operator which, typesys.Type returns, List<Parameter> parameters, string body)
			: base(owner, Modifiers.PUBLIC | Modifiers.STATIC, returns, which.representation, parameters, body)
		{
			this.which = which;
			this.optype = (parsing.BinaryOperation.isOperator(which.representation) ? parsing.OperationType.BINARY : parsing.OperationType.UNARY);
		}
	}
}
