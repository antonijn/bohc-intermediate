using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Interface : typesys.Type
	{
		public List<Interface> implements = new List<Interface>();

		private static readonly List<Interface> instances = new List<Interface>();

		public Interface(Package package, string name)
			: base(package, name)
		{
		}

		public static Interface get(Package package, string name)
		{
			Interface i = instances.SingleOrDefault(x => (x.package == package && x.name == name));
			if (i == default(Interface))
			{
				Interface newi = new Interface(package, name);
				instances.Add(newi);
				return newi;
			}

			return i;
		}
	}
}
