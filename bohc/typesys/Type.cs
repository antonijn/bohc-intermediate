using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing.ts;

namespace bohc.typesys
{
	public class Type : IType
	{
		File IType.getFile()
		{
			return file;
		}

		void IType.setFile(File f)
		{
			file = f;
		}

		public static bool isValidIdentifier(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}

			char first = name.First();
			if (!char.IsLetter(first) && first != '_')
			{
				return false;
			}

			foreach (char ch in name)
			{
				if (!char.IsLetterOrDigit(ch) && ch != '_')
				{
					return false;
				}
			}

			return true;
		}

		public static bool isValidName(string name, bool canbeprimitive)
		{
			if (!canbeprimitive && Primitive.isPrimitiveTypeName(name))
			{
				return false;
			}

			return isValidIdentifier(name);
		}

		public readonly Package package;
		public readonly Modifiers modifiers;
		public readonly string name;

		public parsing.ts.File file;

		private static readonly List<Type> types = new List<Type>();

		public Type(Package package, Modifiers modifiers, string name)
		{
			boh.Exception.require<exceptions.ParserException>(isValidName(name, this is Primitive), name + " is not a valid typename");

			this.package = package;
			this.modifiers = modifiers;
			this.name = name;

			lock (types)
			{
				boh.Exception.require<exceptions.ParserException>(!types.Any(x => x.package == package && x.name == name), "multiple definitions for " + package.ToString() + name);
				types.Add(this);
			}
		}

		/// <returns>null on failure</returns>
		public static Type getExisting(Package package, string name)
		{
			lock (types)
			{
				return types.SingleOrDefault(x => x.package == package && x.name == name);
			}
		}

		public static Type getExisting(IEnumerable<Package> packages, string name)
		{
			lock (types)
			{
				foreach (Package p in packages.Concat(new[] { Package.GLOBAL }))
				{
					Type t = getExisting(p, name);
					if (t != null)
					{
						return t;
					}
				}
			}

			boh.Exception._throw<exceptions.ParserException>("type not found: " + name);
			return null;
		}
	}
}
