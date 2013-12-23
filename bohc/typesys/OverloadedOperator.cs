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
