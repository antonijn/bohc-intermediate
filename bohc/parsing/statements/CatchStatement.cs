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

namespace Bohc.Parsing.Statements
{
	public class CatchStatement : BodyStatement
	{
		public readonly Bohc.TypeSystem.Parameter param;

		public CatchStatement(Bohc.TypeSystem.Parameter param, Body body)
			: base(body)
		{
			this.param = param;
		}
	}
}
