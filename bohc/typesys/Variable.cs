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
	public class Variable
	{
		public readonly string identifier;
		public readonly typesys.Type type;

		// 0 if not in lambda, 1 if in lambda, 2 if in lambda in lambda, etc.
		public int lambdaLevel = 0;
		public bool enclosed = false;

		public Variable(string identifier, typesys.Type type)
		{
			this.identifier = identifier;
			this.type = type;
		}
	}
}
