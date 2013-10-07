using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Primitive : typesys.Type
	{
		public readonly int size;

		public Primitive(string name, int size)
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, name)
		{
			this.size = size;
		}

		public static bool isPrimitiveTypeName(string what)
		{
			return get(what) != null;
		}

		public static Primitive get(string name)
		{
			switch (name)
			{
				case "byte":
					return BYTE;
				case "short":
					return SHORT;
				case "int":
					return INT;
				case "long":
					return LONG;
				case "boolean":
					return BOOLEAN;
				case "float":
					return FLOAT;
				case "double":
					return DOUBLE;
				case "char":
					return CHAR;
				default:
					return null;
			}
		}

		public static readonly Primitive BYTE = new Primitive("byte", 1);
		public static readonly Primitive SHORT = new Primitive("short", 2);
		public static readonly Primitive INT = new Primitive("int", 4);
		public static readonly Primitive LONG = new Primitive("long", 8);
		public static readonly Primitive BOOLEAN = new Primitive("boolean", 4);
		public static readonly Primitive FLOAT = new Primitive("float", 4);
		public static readonly Primitive DOUBLE = new Primitive("double", 8);
		public static readonly Primitive CHAR = new Primitive("char", 2);
		public static readonly Primitive VOID = new Primitive("void", 0);
	}
}
