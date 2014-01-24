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
	public class NullType : Type
	{
		public static readonly NullType Null = new NullType();

		private NullType()
			: base(Package.Global, Modifiers.Public | Modifiers.Final, "\r")
		{
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.Literal(this, "NULL");
		}

		public override int Extends(Type other)
		{
			if (other is Class || other is Interface)
			{
				return 1;
			}

			return 0;
		}

		public override bool IsReferenceType()
		{
			throw new NotImplementedException();
		}
	}
}
