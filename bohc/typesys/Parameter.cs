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
	public class Parameter : Variable
	{
		public readonly Function Function;
		public readonly Modifiers Modifiers;

		public Parameter(Function function, Modifiers modifiers, string identifier, Bohc.TypeSystem.Type type)
			: base(identifier, type)
		{
			this.Function = function;
			this.Modifiers = modifiers;
		}
	}
}
