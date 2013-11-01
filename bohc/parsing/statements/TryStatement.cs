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

namespace bohc.parsing.statements
{
	public class TryStatement : BodyStatement
	{
		public readonly List<CatchStatement> catches;
		public readonly FinallyStatement fin;

		public TryStatement(Body body, List<CatchStatement> catches, FinallyStatement fin)
			: base(body)
		{
			this.catches = catches;
			this.fin = fin;
		}
	}
}
