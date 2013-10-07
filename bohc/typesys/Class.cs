using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Class : typesys.Type
	{
		public List<Interface> implements = new List<Interface>();
		public Class super;

		public List<Function> functions = new List<Function>();
		public List<Field> fields = new List<Field>();
		public List<IMember> members = new List<IMember>();

		public void addMember(Function f)
		{
			functions.Add(f);
			members.Add(f);
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

			implement(iface);
		}
	}
}
