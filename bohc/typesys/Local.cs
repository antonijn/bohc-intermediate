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
	public sealed class Local : Variable
	{
		public Modifiers Modifiers;

		public Stack<bool> assignedTo = new Stack<bool>(new [] { false });
		public int usageCount = 0;

		public bool wasAssignedTo()
		{
			return assignedTo.Any(x => x);
		}

		public Bohc.Parsing.TokenRange info;

		public Local(string identifier, Type type, Modifiers modifiers)
			: base(identifier, type)
		{
			this.Modifiers = modifiers;
		}
	}
}
