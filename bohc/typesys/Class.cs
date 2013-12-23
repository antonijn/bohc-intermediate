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

using bohc.exceptions;

namespace bohc.typesys
{
	public class Class : typesys.Type
	{
		public readonly Variable THIS;
		public readonly parsing.ThisVar THISVAR;

		public List<Interface> implements = new List<Interface>();
		public Class super;

		public List<Function> functions = new List<Function>();
		public List<Constructor> constructors = new List<Constructor>();
		public List<OverloadedOperator> operators = new List<OverloadedOperator>();
		public List<Field> fields = new List<Field>();
		public List<IMember> members = new List<IMember>();
		public List<GenericFunction> genfuncs = new List<GenericFunction>();
		public StaticConstructor staticConstr;

		private class FuncComp : IEqualityComparer<Function>
		{
			public bool Equals(Function f0, Function f1)
			{
				// can't be private
				// TODO: can't have different access modifiers
				// names must match
				// parameter types must be equal
				// neither can be override

				return !(f0.modifiers.HasFlag(Modifiers.PRIVATE) || f1.modifiers.HasFlag(Modifiers.PRIVATE)) &&
					(f0.identifier == f1.identifier) &&
					(f0.parameters.Select(x => x.type).SequenceEqual(f1.parameters.Select(x => x.type))) &&
					!(f0.modifiers.HasFlag(Modifiers.OVERRIDE) || f1.modifiers.HasFlag(Modifiers.OVERRIDE));
			}

			public int GetHashCode(Function f)
			{
				return f.GetHashCode();
			}
		}

		public IEnumerable<Function> getAllFuncs()
		{
			if (super != null)
			{
				return functions.Union(super.functions, new FuncComp());
			}

			return functions;
		}

		public override parsing.Expression defaultVal()
		{
			return new parsing.Literal(this, "NULL");
		}

		public override int extends(Type other)
		{
			if (other == this)
			{
				return 1;
			}

			foreach (Interface iface in implements)
			{
				if (iface == other)
				{
					return 2;
				}
			}

			if (super == null)
			{
				return 0;
			}

			return super.extends(other) + 1;
		}

		public void addMember(IFunction f)
		{
			Function func = f as Function;
			if (func != null)
			{
				addMember(func);
			}
			else
			{
				genfuncs.Add((GenericFunction)f);
			}
		}

		public void addMember(StaticConstructor f)
		{
			staticConstr = f;
			functions.Add(f);
			members.Add(f);
		}

		public void addMember(Constructor f)
		{
			functions.Add(f);
			constructors.Add(f);
			members.Add(f);
		}

		public void addMember(OverloadedOperator f)
		{
			functions.Add(f);
			operators.Add(f);
			members.Add(f);
		}

		public void addMember(Function f)
		{
			Constructor constr = f as Constructor;
			OverloadedOperator op = f as OverloadedOperator;
			StaticConstructor sconstr = f as StaticConstructor;

			if (constr != null)
			{
				addMember(constr);
			}
			else if (op != null)
			{
				addMember(op);
			}
			else if (sconstr != null)
			{
				addMember(sconstr);
			}
			else
			{
				functions.Add(f);
				members.Add(f);
			}
		}

		public void addMember(Field f)
		{
			fields.Add(f);
			members.Add(f);
		}

		private static readonly List<Class> instances = new List<Class>();

		protected Class(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
			this.THIS = new Variable("this", this);
			this.THISVAR = new parsing.ThisVar(this);
		}

		public static Class get<T>(Package package, Modifiers modifiers, string name)
			where T : Class
		{
			lock (instances)
			{
				T c = (T)instances.SingleOrDefault(x => (x.package == package && x.name == name));
				if (c == default(T))
				{
					if (typeof(T) == typeof(Class))
					{
						Class newc = new Class(package, modifiers, name);
						instances.Add(newc);
						return newc;
					}

					Struct news = new Struct(package, modifiers, name);
					instances.Add(news);
					return news;
				}

				return c;
			}
		}

		public void implement(Interface iface)
		{
			foreach (Interface other in iface.implements)
			{
				implement(other);
			}

			boh.Exception.require<ParserException>(!implements.Contains(iface), "Interfaces may not be implemented multiple times");
			implements.Add(iface);
		}

		public bool compatibleWith(Type other)
		{
			return extends(other) != 0;
		}

		private bool sameGenType(Class ctx)
		{
			return ctx.originalGenType == originalGenType && originalGenType != null;
		}

		public IEnumerable<Function> getFunctions(string id, Class context)
		{
			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this || sameGenType(context))
			{
				_private = true;
				_protected = true;
			}
			else if (compatibleWith(context))
			{
				_protected = true;
			}

			return getAllFuncs().Where(x => x.identifier == id &&
				((_public && x.modifiers.HasFlag(Modifiers.PUBLIC)) ||
				(_protected && x.modifiers.HasFlag(Modifiers.PROTECTED)) ||
				(_private && x.modifiers.HasFlag(Modifiers.PRIVATE))));
		}

		public Field getField(string id, Class context)
		{
			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this || sameGenType(context))
			{
				_private = true;
				_protected = true;
			}
			else if (compatibleWith(context))
			{
				_protected = true;
			}

			// TODO: pf_stuff, firstordefault isn't ideal...
			Field result = fields.FirstOrDefault(x => x.identifier == id &&
				((_public && x.modifiers.HasFlag(Modifiers.PUBLIC)) ||
				(_protected && x.modifiers.HasFlag(Modifiers.PROTECTED)) ||
				(_private && x.modifiers.HasFlag(Modifiers.PRIVATE))));

			if (result == null && super != null)
			{
				return super.getField(id, context);
			}

			return result;
		}

		public IEnumerable<Field> getAllFields()
		{
			if (super == null)
			{
				return fields;
			}

			return super.getAllFields().Concat(fields);
		}
	}
}
