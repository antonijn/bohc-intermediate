using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Type
	{
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
		public readonly string name;

		public Type(Package package, string name)
		{
			boh.Exception.require<exceptions.ParserException>(isValidName(name, this is Primitive), name + " is not a valid typename");

			this.package = package;
			this.name = name;
		}
	}
}
