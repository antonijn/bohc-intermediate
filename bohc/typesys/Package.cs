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
	public class Package
	{
		private static readonly List<Package> instances = new List<Package>();

		public static readonly Package GLOBAL = new Package(null, string.Empty);

		public readonly Package parent;
		public readonly string name;

		private Package(Package parent, string name)
		{
			boh.Exception.require<exceptions.ParserException>(parent == null || typesys.Type.isValidName(name, false), name + " invalid package name");

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
			lock (instances)
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
		}

		public static Package getFromStringExisting(string str)
		{
			if (str == "")
			{
				return GLOBAL;
			}
			string[] parts = str.Split('.');
			Package last = GLOBAL;

			foreach (string part in parts)
			{
				last = instances.SingleOrDefault(x => (x.parent == last && x.name == part));
			}

			return last;
		}

		public static Package getFromString(string str)
		{
			if (str == "")
			{
				return GLOBAL;
			}
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
