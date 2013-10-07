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
	}
}
