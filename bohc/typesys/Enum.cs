using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class Enum : Type
	{
		private static readonly List<Enum> instances = new List<Enum>();

		public List<Enumerator> enumerators = new List<Enumerator>();

		public Enum(Package package, Modifiers modifiers, string name)
			: base(package, modifiers, name)
		{
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
	}
}
