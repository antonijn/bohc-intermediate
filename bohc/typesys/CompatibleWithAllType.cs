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
	public class CompatibleWithAllType : Type
	{
		public CompatibleWithAllType()
			: base(Package.Global, Modifiers.Final | Modifiers.Public, "\0")
		{
		}

		public override Parsing.Expression DefaultVal()
		{
			throw new NotImplementedException();
		}

		public override int Extends(Type other)
		{
			return 1;
		}

		public override bool IsReferenceType()
		{
			throw new NotImplementedException();
		}

		public override IEnumerable<Function> GetFunctions(string id, TypeSystem.Type context)
		{
			throw new NotImplementedException();
		}

		public override int getSizeof(Bohc.General.Platform pf)
		{
			throw new NotImplementedException();
		}
	}
}
