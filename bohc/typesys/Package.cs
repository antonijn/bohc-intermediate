using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Package
	{
		public static readonly Package GLOBAL = new Package(null, string.Empty);

		private static readonly List<Package> instances = new List<Package>();

		public readonly Package parent;
		public readonly string name;

		private Package(Package parent, string name)
		{
			boh.Exception.require<exceptions.ParserException>(typesys.Type.isValidName(name, false) || parent == null, name + " invalid package name");

			this.parent = parent;
			this.name = name;
		}

		public override string ToString()
		{
			if (parent != GLOBAL && parent != null)
			{
				return parent.ToString() + "." + name;
			}

			return name;
		}

		public static Package get(Package parent, string name)
		{
			Package p = instances.SingleOrDefault(x => (x.parent == parent && x.name == name));
			if (p == default(Package))
			{
				Package newp = new Package(parent, name);
				instances.Add(newp);
				return newp;
			}

			return p;
		}

		public static Package getFromString(string str)
		{
			string[] parts = str.Split('.');
			Package last = GLOBAL;

			foreach (string part in parts)
			{
				last = get(last, part);
			}

			return last;
		}
	}
}
