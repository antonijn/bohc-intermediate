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
	public class Enum : Type
	{
		public readonly Variable THIS;

		public static readonly List<Enum> instances = new List<Enum>();

		public List<Enumerator> enumerators = new List<Enumerator>();

		public Function toString;
		public Function getType;
		public Function equals;
		public Function hash;

		public Function parse;
		public Function tryParse;
		public Function getEnumerators;

		public Enum(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
			THIS = new Variable("this", this);
		}

		private void buildToString()
		{
			parsing.statements.Body body = new parsing.statements.Body();
			toString = new Function(this, Modifiers.PUBLIC, StdType.str, "toString", new List<Parameter>(), body);

			parsing.statements.IfStatement ifs = null;
			foreach (Enumerator enumerator in enumerators.Reverse<Enumerator>()) // the reverse is just for aesthetic
			{
				parsing.statements.Body newb = new parsing.statements.Body();
				newb.statements.Add(new parsing.statements.ReturnStatement(new parsing.Literal(StdType.str, "\"" + enumerator.name + "\"")));

				parsing.statements.Body elsebod = new parsing.statements.Body();
				parsing.statements.ElseStatement elses = new parsing.statements.ElseStatement(elsebod);
				if (ifs != null)
				{
					elsebod.statements.Add(ifs);
				}
				else
				{
					elsebod.statements.Add(
						new parsing.statements.ReturnStatement(
							new parsing.FunctionCall(
								typesys.Primitive.INT.toString,
								new parsing.TypeCast(
									Primitive.INT,
									new parsing.ThisVar(this)),
								Enumerable.Empty<parsing.Expression>()
							)
						)
					);
				}

				ifs = new parsing.statements.IfStatement(
					new parsing.BinaryOperation(new parsing.ThisVar(this), new parsing.ExprEnumerator(enumerator), parsing.BinaryOperation.EQUAL),
					newb, elses);
			}

			body.statements.Add(ifs);
		}

		public void sortOutFunctions()
		{
			StdType.box.getTypeFor(new [] { this });

			buildToString();
			getType = new Function(this, Modifiers.PUBLIC, StdType.type, "getType", new List<Parameter>(), default(parsing.statements.Body));
			equals = new Function(this, Modifiers.PUBLIC, Primitive.BOOLEAN, "equals", new List<Parameter>(), default(parsing.statements.Body));
			hash = new Function(this, Modifiers.PUBLIC, Primitive.LONG, "hash", new List<Parameter>(), default(parsing.statements.Body));

			parse = new Function(this, Modifiers.PUBLIC | Modifiers.STATIC, this, "parse", new List<Parameter>(), default(parsing.statements.Body));
			parse.parameters.Add(new Parameter(parse, Modifiers.FINAL, "str", StdType.str));
			tryParse = new Function(this, Modifiers.PUBLIC | Modifiers.STATIC, Primitive.BOOLEAN, "tryParse", new List<Parameter>(), default(parsing.statements.Body));
			tryParse.parameters.Add(new Parameter(parse, Modifiers.FINAL, "str", StdType.str));
			tryParse.parameters.Add(new Parameter(parse, Modifiers.REF, "output", this));
			getEnumerators = new Function(this, Modifiers.PUBLIC | Modifiers.STATIC, StdType.array.getTypeFor(new[] { Primitive.INT }), "getEnumerators", new List<Parameter>(), default(parsing.statements.Body));			
		
		}

		public override parsing.Expression defaultVal()
		{
			return new parsing.Literal(Primitive.INT, "0");
		}

		public static Enum get(Package package, Modifiers modifiers, string name)
		{
			lock (instances)
			{
				Enum e = instances.SingleOrDefault(x => (x.package == package && x.name == name));
				if (e == default(Enum))
				{
					Enum newe = new Enum(package, modifiers, name);
					instances.Add(newe);
					return newe;
				}

				return e;
			}
		}

		public override int extends(Type other)
		{
			Primitive prim = other as Primitive;
			if (prim != null && prim.isInt())
			{
				return 2;
			}
			return base.extends(other);
		}
	}
}
