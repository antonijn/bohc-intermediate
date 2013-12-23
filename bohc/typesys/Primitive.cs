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
	public class Primitive : typesys.Type
	{
		public Function toString;
		public Function getType;
		public Function equals;
		public Function hash;

		public static void figureOutFunctionsForAll()
		{
			BYTE.figureOutFunctions();
			SHORT.figureOutFunctions();
			INT.figureOutFunctions();
			LONG.figureOutFunctions();
			CHAR.figureOutFunctions();
			DECIMAL.figureOutFunctions();
			FLOAT.figureOutFunctions();
			DOUBLE.figureOutFunctions();
			BOOLEAN.figureOutFunctions();
		}

		private void figureOutFunctions()
		{
			toString = new Function(this, Modifiers.PUBLIC, StdType.type, "toString", new List<Parameter>(), default(parsing.statements.Body));
			getType = new Function(this, Modifiers.PUBLIC, StdType.type, "getType", new List<Parameter>(), default(parsing.statements.Body));
			equals = new Function(this, Modifiers.PUBLIC, Primitive.BOOLEAN, "equals", new List<Parameter>(), default(parsing.statements.Body));
			equals.parameters.Add(new Parameter(equals, Modifiers.FINAL, "other", StdType.obj));
			hash = new Function(this, Modifiers.PUBLIC, Primitive.LONG, "hash", new List<Parameter>(), default(parsing.statements.Body));
		}

		public readonly int size;
		public readonly string cname;
		public parsing.Literal initval;

		public Primitive(string name, string cname, int size)
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, name)
		{
			this.size = size;
			this.cname = cname;
		}

		public override parsing.Expression defaultVal()
		{
			return initval;
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
				case "decimal":
					return DECIMAL;
				case "char":
					return CHAR;
				default:
					return null;
			}
		}

		public static readonly Primitive BYTE = new Primitive("byte", "uint8_t", 1);
		public static readonly Primitive SHORT = new Primitive("short", "int16_t", 2);
		public static readonly Primitive INT = new Primitive("int", "int32_t", 4);
		public static readonly Primitive LONG = new Primitive("long", "int64_t", 8);
		public static readonly Primitive BOOLEAN = new Primitive("boolean", "_Bool", 4);
		public static readonly Primitive FLOAT = new Primitive("float", "float", 4);
		public static readonly Primitive DOUBLE = new Primitive("double", "double", 8);
		public static readonly Primitive DECIMAL = new Primitive("decimal", "_Decimal64", 8);
		public static readonly Primitive CHAR = new Primitive("char", "unsigned char", 2);
		public static readonly Primitive VOID = new Primitive("void", "void", 0);

		static Primitive()
		{
			BYTE.initval = new parsing.Literal(BYTE, "0");
			SHORT.initval = new parsing.Literal(SHORT, "0");
			INT.initval = new parsing.Literal(INT, "0");
			LONG.initval = new parsing.Literal(LONG, "0L");
			BOOLEAN.initval = new parsing.Literal(BOOLEAN, "0");
			FLOAT.initval = new parsing.Literal(FLOAT, "0.0f");
			DOUBLE.initval = new parsing.Literal(DOUBLE, "0.0");
			DECIMAL.initval = new parsing.Literal(DECIMAL, "0.0DD");
			CHAR.initval = new parsing.Literal(CHAR, "'\\0'");
		}

		public bool isInt()
		{
			return (this == BYTE || this == SHORT || this == INT || this == LONG || this == CHAR);
		}

		public bool isFloat()
		{
			return (this == FLOAT || this == DOUBLE);
		}

		public override int extends(Type other)
		{
			if (this == VOID)
			{
				if (other == VOID)
				{
					return 1;
				}
				return 0;
			}

			if (this == other)
			{
				return 1;
			}

			if (isInt() && (other is typesys.Enum))
			{
				return 3;
			}

			Primitive oPrim = other as Primitive;
			if (oPrim == null)
			{
				return 0;
			}

			if (isInt() && oPrim.isInt())
			{
				return 2;
			}

			if (isFloat() && oPrim.isFloat())
			{
				return 2;
			}

			if (isInt() && oPrim.isFloat())
			{
				return 3;
			}

			return 0;
		}
	}
}
