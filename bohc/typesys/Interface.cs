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
	public class Interface : typesys.Type
	{
		public List<Interface> implements = new List<Interface>();
		public readonly List<Function> functions = new List<Function>();

		private static readonly List<Interface> instances = new List<Interface>();

		public Interface(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
		}

		public IEnumerable<Function> getAllFuncs()
		{
			return implements.SelectMany(x => x.functions).Concat(functions);
		}

		public override parsing.Expression defaultVal()
		{
			return new parsing.Literal(this, "NULL");
		}

		public static Interface get(Package package, Modifiers modifiers, string name)
		{
			lock (instances)
			{
				Interface i = instances.SingleOrDefault(x => (x.package == package && x.name == name));
				if (i == default(Interface))
				{
					Interface newi = new Interface(package, modifiers, name);
					instances.Add(newi);
					return newi;
				}

				return i;
			}
		}

		public IEnumerable<Function> getFunctions(string id)
		{
			IEnumerable<Function> inThis = functions.Where(x => x.identifier == id).ToList();
			return inThis.Concat(implements.SelectMany(x => x.getFunctions(id)));
		}

		public override int extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			foreach (Interface impl in implements)
			{
				int extds = impl.extends(other);
				if (extds > 0)
				{
					return extds + 1;
				}
			}

			return 0;
		}
	}
}
