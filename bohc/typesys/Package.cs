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
	public class Package
	{
		private static readonly List<Package> Instances = new List<Package>();

		public static readonly Package Global = new Package(null, string.Empty);

		public readonly Package Parent;
		public readonly string Name;

		private Package(Package parent, string name)
		{
			Boh.Exception.require<Exceptions.ParserException>(parent == null || Bohc.TypeSystem.Type.isValidName(name, false), name + " invalid package name");

			this.Parent = parent;
			this.Name = name;
		}

		public override string ToString()
		{
			if (Parent != Global && Parent != null)
			{
				return Parent.ToString() + "." + Name;
			}

			return Name;
		}

		public static Package Get(Package parent, string name)
		{
			lock (Instances)
			{
				Package p = Instances.SingleOrDefault(x => (x.Parent == parent && x.Name == name));
				if (p == default(Package))
				{
					Package newp = new Package(parent, name);
					Instances.Add(newp);
					return newp;
				}

				return p;
			}
		}

		public static Package GetFromStringExisting(string str)
		{
			if (str == "")
			{
				return Global;
			}
			string[] parts = str.Split('.');
			Package last = Global;

			foreach (string part in parts)
			{
				last = Instances.SingleOrDefault(x => (x.Parent == last && x.Name == part));
			}

			return last;
		}

		public static Package GetFromString(string str)
		{
			if (str == "")
			{
				return Global;
			}
			string[] parts = str.Split('.');
			Package last = Global;

			foreach (string part in parts)
			{
				last = Get(last, part);
			}

			return last;
		}
	}
}
