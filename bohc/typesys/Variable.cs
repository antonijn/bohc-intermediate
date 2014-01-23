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
	public class Variable
	{
		public readonly string Identifier;
		public readonly Bohc.TypeSystem.Type Type;

		// 0 if not in lambda, 1 if in lambda, 2 if in lambda in lambda, etc.
		public int LamdaLevel = 0;
		public bool Enclosed = false;

		public Variable(string identifier, Bohc.TypeSystem.Type type)
		{
			this.Identifier = identifier;
			this.Type = type;
		}
	}
}
