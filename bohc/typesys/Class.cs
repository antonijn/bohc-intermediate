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

using Bohc.Exceptions;

namespace Bohc.TypeSystem
{
	public class Class : Bohc.TypeSystem.Type
	{
		public readonly Variable This;
		public readonly Parsing.ExprVariable ThisVarExpr;
		public readonly Variable SuperVar;
		public readonly Parsing.ExprVariable SuperVarExpr;

		public List<Interface> Implements = new List<Interface>();
		public Class Super;

		public List<Function> Functions = new List<Function>();
		public List<Constructor> Constructors = new List<Constructor>();
		public List<OverloadedOperator> Operators = new List<OverloadedOperator>();
		public List<Field> Fields = new List<Field>();
		public List<IMember> Members = new List<IMember>();
		public List<GenericFunction> GenFuncs = new List<GenericFunction>();
		public StaticConstructor StaticConstr;

		private class FuncComp : IEqualityComparer<Function>
		{
			public bool Equals(Function f0, Function f1)
			{
				// can't be private
				// TODO: can't have different access modifiers
				// names must match
				// parameter types must be equal
				// neither can be override

				return (f0.Identifier == f1.Identifier) &&
					(f0.Parameters.Select(x => x.Type).SequenceEqual(f1.Parameters.Select(x => x.Type))) &&
					!(f0.Modifiers.HasFlag(Modifiers.Override) || f1.Modifiers.HasFlag(Modifiers.Override));
			}

			public int GetHashCode(Function f)
			{
				return f.GetHashCode();
			}
		}

		public IEnumerable<Function> GetAllFuncs()
		{
			if (Super != null)
			{
				return Functions.Union(Super.GetAllFuncs()
				                        .Where(x => !x.Modifiers.HasFlag(TypeSystem.Modifiers.Private))
				                        .Where(x => !(x is TypeSystem.Constructor))
				                        .Where(x => !(x is TypeSystem.StaticConstructor)), new FuncComp());
			}

			return Functions;
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.Literal(this, "NULL");
		}

		public override int Extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			foreach (Interface iface in Implements)
			{
				if (iface == other)
				{
					return 2;
				}
			}

			if (Super == null)
			{
				return 0;
			}

			int tmp = Super.Extends(other);
			return tmp == 0 ? 0 : tmp + 1;
		}

		public void AddMember(IFunction f)
		{
			Function func = f as Function;
			if (func != null)
			{
				AddMember(func);
			}
			else
			{
				GenFuncs.Add((GenericFunction)f);
			}
		}

		public void AddMember(StaticConstructor f)
		{
			StaticConstr = f;
			Functions.Add(f);
			Members.Add(f);
		}

		public void AddMember(Constructor f)
		{
			Functions.Add(f);
			Constructors.Add(f);
			Members.Add(f);
		}

		public void AddMember(OverloadedOperator f)
		{
			Functions.Add(f);
			Operators.Add(f);
			Members.Add(f);
		}

		public void AddMember(Function f)
		{
			Constructor constr = f as Constructor;
			OverloadedOperator op = f as OverloadedOperator;
			StaticConstructor sconstr = f as StaticConstructor;

			if (constr != null)
			{
				AddMember(constr);
			}
			else if (op != null)
			{
				AddMember(op);
			}
			else if (sconstr != null)
			{
				AddMember(sconstr);
			}
			else
			{
				Functions.Add(f);
				Members.Add(f);
			}
		}

		public void AddMember(Field f)
		{
			Fields.Add(f);
			Members.Add(f);
		}

		private static readonly List<Class> Instances = new List<Class>();

		protected Class(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
			this.This = new Variable("this", this);
			this.ThisVarExpr = new Parsing.ExprVariable(This, null);
			this.SuperVar = new Variable("super", this);
			this.SuperVarExpr = new Parsing.ExprVariable(SuperVar, null);
		}

		public static Class Get<T>(Package package, Modifiers modifiers, string name)
			where T : Class
		{
			lock (Instances)
			{
				T c = (T)Instances.SingleOrDefault(x => (x.Package == package && x.Name == name));
				if (c == default(T))
				{
					if (typeof(T) == typeof(Class))
					{
						Class newc = new Class(package, modifiers, name);
						Instances.Add(newc);
						return newc;
					}

					Struct news = new Struct(package, modifiers, name);
					Instances.Add(news);
					return news;
				}

				return c;
			}
		}

		public void Implement(Interface iface)
		{
			foreach (Interface other in iface.Implements)
			{
				Implement(other);
			}

			Boh.Exception.require<ParserException>(!Implements.Contains(iface), "Interfaces may not be implemented multiple times");
			Implements.Add(iface);
		}

		public bool CompatibleWith(Type other)
		{
			return Extends(other) != 0;
		}

		private bool SameGenType(Class ctx)
		{
			return ctx.OriginalGenType == OriginalGenType && OriginalGenType != null;
		}

		public override IEnumerable<Function> GetFunctions(string id, TypeSystem.Type ctx)
		{
			Class context = ctx as Class;

			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this || SameGenType(context))
			{
				_private = true;
				_protected = true;
			}
			else if (CompatibleWith(context))
			{
				_protected = true;
			}

			IEnumerable<Function> allfuncs = GetAllFuncs();

			return allfuncs.Where(x => x.Identifier == id &&
				((_public && x.Modifiers.HasFlag(Modifiers.Public)) ||
				(_protected && x.Modifiers.HasFlag(Modifiers.Protected)) ||
				(_private && x.Modifiers.HasFlag(Modifiers.Private)))).ToArray();
		}

		public override Field GetField(string id, TypeSystem.Type ctx)
		{
			Class context = ctx as Class;

			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this || SameGenType(context))
			{
				_private = true;
				_protected = true;
			}
			else if (CompatibleWith(context))
			{
				_protected = true;
			}

			// TODO: pf_stuff, firstordefault isn't ideal...
			Field result = Fields.FirstOrDefault(x => x.Identifier == id &&
				((_public && x.Modifiers.HasFlag(Modifiers.Public)) ||
				(_protected && x.Modifiers.HasFlag(Modifiers.Protected)) ||
				(_private && x.Modifiers.HasFlag(Modifiers.Private))));

			if (result == null && Super != null)
			{
				return Super.GetField(id, context);
			}

			return result;
		}

		public IEnumerable<Field> GetAllFields()
		{
			if (Super == null)
			{
				return Fields;
			}

			return Super.GetAllFields().Concat(Fields);
		}

		public override bool IsReferenceType()
		{
			return true;
		}
	}
}
