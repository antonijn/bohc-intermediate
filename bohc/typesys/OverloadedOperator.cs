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

namespace Bohc.TypeSystem
{
	public sealed class OverloadedOperator : Function
	{
		public readonly Parsing.Operator Which;
		public readonly Parsing.OperationType OpType;

		public OverloadedOperator(Bohc.TypeSystem.Class owner, Parsing.Operator which, Bohc.TypeSystem.Type returns, List<Parameter> parameters, string body)
			: base(owner, Modifiers.Public | Modifiers.Static, returns, which.representation, parameters, body)
		{
			this.Which = which;
			this.OpType = (Parsing.BinaryOperation.isOperator(which.representation) ? Parsing.OperationType.BINARY : Parsing.OperationType.UNARY);
		}
	}
}
