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

			if (constr != null)
			{
				addMember(constr);
			}
			else if (op != null)
			{
				addMember(op);
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

		public static Class get(Package package, Modifiers modifiers, string name)
		{
			lock (instances)
			{
				Class c = instances.SingleOrDefault(x => (x.package == package && x.name == name));
				if (c == default(Class))
				{
					Class newc = new Class(package, modifiers, name);
					instances.Add(newc);
					return newc;
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

		public IEnumerable<Function> getFunctions(string id, Class context)
		{
			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this)
			{
				_private = true;
				_protected = true;
			}
			else if (compatibleWith(context))
			{
				_protected = true;
			}

			return functions.Where(x => x.identifier == id &&
				((_public && x.modifiers.HasFlag(Modifiers.PUBLIC)) ||
				(_protected && x.modifiers.HasFlag(Modifiers.PROTECTED)) ||
				(_private && x.modifiers.HasFlag(Modifiers.PRIVATE))));
		}

		public Field getField(string id, Class context)
		{
			bool _private = false;
			bool _protected = false;
			bool _public = true;

			if (context == this)
			{
				_private = true;
				_protected = true;
			}
			else if (compatibleWith(context))
			{
				_protected = true;
			}

			Field result = fields.SingleOrDefault(x => x.identifier == id &&
				((_public && x.modifiers.HasFlag(Modifiers.PUBLIC)) ||
				(_protected && x.modifiers.HasFlag(Modifiers.PROTECTED)) ||
				(_private && x.modifiers.HasFlag(Modifiers.PRIVATE))));

			if (result == null && super != null)
			{
				return super.getField(id, context);
			}

			return result;
		}
	}
}
