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
	public class Primitive : Bohc.TypeSystem.Type
	{
		public Function ToStringM;
		public Function GetTypeM;
		public Function EqualsM;
		public Function HashM;

		public override IEnumerable<Function> GetFunctions(string id, TypeSystem.Type context)
		{
			return new [] { ToStringM, GetTypeM, EqualsM, HashM }.Where(x => x.Identifier == id);
		}

		public static void FigureOutFunctionsForAll()
		{
			Byte.FigureOutFunctions();
			Short.FigureOutFunctions();
			Int.FigureOutFunctions();
			Long.FigureOutFunctions();
			Char.FigureOutFunctions();
			Decimal.FigureOutFunctions();
			Float.FigureOutFunctions();
			Double.FigureOutFunctions();
			Boolean.FigureOutFunctions();
		}

		private void FigureOutFunctions()
		{
			ToStringM = new Function(this, Modifiers.Public, StdType.Type, "toString", new List<Parameter>(), default(Parsing.Statements.Body));
			GetTypeM = new Function(this, Modifiers.Public, StdType.Type, "getType", new List<Parameter>(), default(Parsing.Statements.Body));
			EqualsM = new Function(this, Modifiers.Public, Primitive.Boolean, "equals", new List<Parameter>(), default(Parsing.Statements.Body));
			EqualsM.Parameters.Add(new Parameter(EqualsM, Modifiers.Final, "other", StdType.Obj));
			HashM = new Function(this, Modifiers.Public, Primitive.Long, "hash", new List<Parameter>(), default(Parsing.Statements.Body));
		}

		public readonly int Size;
		public readonly string CName;
		public readonly string LlvmName;
		public Parsing.Literal InitVal;

		public Primitive(string name, string cname, string llvmname, int size)
			: base(Package.Global, Modifiers.Public | Modifiers.Final, name)
		{
			this.Size = size;
			this.CName = cname;
			this.LlvmName = llvmname;
		}

		public override Parsing.Expression DefaultVal()
		{
			return InitVal;
		}

		public static bool IsPrimitiveTypeName(string what)
		{
			return Get(what) != null;
		}

		public static Primitive Get(string name)
		{
			switch (name)
			{
				case "byte":
					return Byte;
				case "short":
					return Short;
				case "int":
					return Int;
				case "long":
					return Long;
				case "boolean":
					return Boolean;
				case "float":
					return Float;
				case "double":
					return Double;
				case "decimal":
					return Decimal;
				case "char":
					return Char;
				default:
					return null;
			}
		}

		public static readonly Primitive Byte = new Primitive("byte", "uint8_t", "i8", 1);
		public static readonly Primitive Short = new Primitive("short", "int16_t", "i16", 2);
		public static readonly Primitive Int = new Primitive("int", "int32_t", "i32", 4);
		public static readonly Primitive Long = new Primitive("long", "int64_t", "i64", 8);
		public static readonly Primitive Boolean = new Primitive("boolean", "uint8_t", "i1", 1);
		public static readonly Primitive Float = new Primitive("float", "float", "float", 4);
		public static readonly Primitive Double = new Primitive("double", "double", "double", 8);
		public static readonly Primitive Decimal = new Primitive("decimal", "_Decimal64", null, 8);
		public static readonly Primitive Char = new Primitive("char", "unsigned char", "i8", 2);
		public static readonly Primitive Void = new Primitive("void", "void", "void", 0);

		static Primitive()
		{
			Byte.InitVal = new Parsing.Literal(Byte, "0");
			Short.InitVal = new Parsing.Literal(Short, "0");
			Int.InitVal = new Parsing.Literal(Int, "0");
			Long.InitVal = new Parsing.Literal(Long, "0L");
			Boolean.InitVal = new Parsing.Literal(Boolean, "0");
			Float.InitVal = new Parsing.Literal(Float, "0.0f");
			Double.InitVal = new Parsing.Literal(Double, "0.0");
			Decimal.InitVal = new Parsing.Literal(Decimal, "0.0DD");
			Char.InitVal = new Parsing.Literal(Char, "'\\0'");
		}

		public bool IsInt()
		{
			return (this == Byte || this == Short || this == Int || this == Long || this == Char);
		}

		public bool IsFloat()
		{
			return (this == Float || this == Double);
		}

		public override int Extends(Type other)
		{
			if (this == Void)
			{
				if (other == Void)
				{
					return 1;
				}
				return 0;
			}

			if (this == other)
			{
				return 1;
			}

			if (IsInt() && (other is Bohc.TypeSystem.Enum))
			{
				return 3;
			}

			Primitive oPrim = other as Primitive;
			if (oPrim == null)
			{
				return 0;
			}

			if (IsInt() && oPrim.IsInt())
			{
				return 2;
			}

			if (IsFloat() && oPrim.IsFloat())
			{
				return 2;
			}

			if (IsInt() && oPrim.IsFloat())
			{
				return 3;
			}

			return 0;
		}

		public override bool IsReferenceType()
		{
			return false;
		}
	}
}
