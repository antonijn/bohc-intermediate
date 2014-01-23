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
	public class Interface : Bohc.TypeSystem.Type
	{
		public List<Interface> Implements = new List<Interface>();
		public readonly List<Function> Functions = new List<Function>();

		private static readonly List<Interface> Instances = new List<Interface>();

		public Interface(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
		}

		public IEnumerable<Function> GetAllFuncs()
		{
			return Implements.SelectMany(x => x.Functions).Concat(Functions);
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.Literal(this, "NULL");
		}

		public static Interface Get(Package package, Modifiers modifiers, string name)
		{
			lock (Instances)
			{
				Interface i = Instances.SingleOrDefault(x => (x.Package == package && x.Name == name));
				if (i == default(Interface))
				{
					Interface newi = new Interface(package, modifiers, name);
					Instances.Add(newi);
					return newi;
				}

				return i;
			}
		}

		public IEnumerable<Function> GetFunctions(string id)
		{
			IEnumerable<Function> inThis = Functions.Where(x => x.Identifier == id).ToList();
			return inThis.Concat(Implements.SelectMany(x => x.GetFunctions(id)));
		}

		public override int Extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			foreach (Interface impl in Implements)
			{
				int extds = impl.Extends(other);
				if (extds > 0)
				{
					return extds + 1;
				}
			}

			return 0;
		}
	}
}
