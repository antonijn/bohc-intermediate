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

using Bohc.Parsing;

namespace Bohc.TypeSystem
{
	public class Enum : Type
	{
		public readonly Variable This;

		public static readonly List<Enum> Instances = new List<Enum>();

		public List<Enumerator> Enumerators = new List<Enumerator>();

		public Function ToStringM;
		public Function GetTypeM;
		public Function EqualsM;
		public Function HashM;

		public Function Parse;
		public Function TryParse;
		public Function GetEnumerators;

		public Enum(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
			This = new Variable("this", this);
		}

		private void BuildToString()
		{
			Parsing.Statements.Body body = new Parsing.Statements.Body();
			ToStringM = new Function(this, Modifiers.Public, StdType.Str, "toString", new List<Parameter>(), body);

			Parsing.Statements.IfStatement ifs = null;
			foreach (Enumerator enumerator in Enumerators.Reverse<Enumerator>()) // the reverse is just for aesthetic
			{
				Parsing.Statements.Body newb = new Parsing.Statements.Body();
				newb.Statements.Add(new Parsing.Statements.ReturnStatement(new Parsing.Literal(StdType.Str, "\"" + enumerator.Name + "\"")));

				Parsing.Statements.Body elsebod = new Parsing.Statements.Body();
				Parsing.Statements.ElseStatement elses = new Parsing.Statements.ElseStatement(elsebod);
				if (ifs != null)
				{
					elsebod.Statements.Add(ifs);
				}
				else
				{
					elsebod.Statements.Add(
						new Parsing.Statements.ReturnStatement(
							new Parsing.FunctionCall(
								Bohc.TypeSystem.Primitive.Int.ToStringM,
								new Parsing.TypeCast(
									Primitive.Int,
									new ExprVariable(new Variable("this", this), null)),
								Enumerable.Empty<Parsing.Expression>()
							)
						)
					);
				}

				ifs = new Parsing.Statements.IfStatement(
					new Parsing.BinaryOperation(new ExprVariable(new Variable("this", this), null), new Parsing.ExprEnumerator(enumerator), Parsing.BinaryOperation.EQUAL),
					newb, elses);
			}

			body.Statements.Add(ifs);
		}

		public void SortOutFunctions(IFileParser parser)
		{
			StdType.Box.GetTypeFor(new [] { this }, parser);

			BuildToString();
			GetTypeM = new Function(this, Modifiers.Public, StdType.Type, "getType", new List<Parameter>(), default(Parsing.Statements.Body));
			EqualsM = new Function(this, Modifiers.Public, Primitive.Boolean, "equals", new List<Parameter>(), default(Parsing.Statements.Body));
			HashM = new Function(this, Modifiers.Public, Primitive.Long, "hash", new List<Parameter>(), default(Parsing.Statements.Body));

			Parse = new Function(this, Modifiers.Public | Modifiers.Static, this, "parse", new List<Parameter>(), default(Parsing.Statements.Body));
			Parse.Parameters.Add(new Parameter(Parse, Modifiers.Final, "str", StdType.Str));
			TryParse = new Function(this, Modifiers.Public | Modifiers.Static, Primitive.Boolean, "tryParse", new List<Parameter>(), default(Parsing.Statements.Body));
			TryParse.Parameters.Add(new Parameter(Parse, Modifiers.Final, "str", StdType.Str));
			TryParse.Parameters.Add(new Parameter(Parse, Modifiers.Ref, "output", this));
			GetEnumerators = new Function(this, Modifiers.Public | Modifiers.Static, StdType.Array.GetTypeFor(new[] { Primitive.Int }, parser), "getEnumerators", new List<Parameter>(), default(Parsing.Statements.Body));			
		
		}

		public override IEnumerable<Function> GetFunctions(string id, TypeSystem.Type context)
		{
			return new [] { ToStringM, HashM, EqualsM, GetTypeM, Parse, TryParse, GetEnumerators }.Where(x => x.Identifier == id);
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.Literal(Primitive.Int, "0");
		}

		public static Enum Get(Package package, Modifiers modifiers, string name)
		{
			lock (Instances)
			{
				Enum e = Instances.SingleOrDefault(x => (x.Package == package && x.Name == name));
				if (e == default(Enum))
				{
					Enum newe = new Enum(package, modifiers, name);
					Instances.Add(newe);
					return newe;
				}

				return e;
			}
		}

		public override int Extends(Type other)
		{
			Primitive prim = other as Primitive;
			if (prim != null && prim.IsInt())
			{
				return 2;
			}
			return base.Extends(other);
		}

		public override bool IsReferenceType()
		{
			return false;
		}
	}
}
