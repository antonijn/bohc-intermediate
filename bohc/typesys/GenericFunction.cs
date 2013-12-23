using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class GenericFunction : IFunction
	{
		/*private readonly string[] genTypeNames;
		public readonly typesys.Type owner;
		public readonly Modifiers modifiers;
		public readonly typesys.Type returnType;
		public readonly string identifier;
		public readonly List<Parameter> parameters;

		public readonly string bodystr;
		public parsing.statements.Body body;*/

		Modifiers IMember.getModifiers()
		{
			throw new NotImplementedException();
			//return modifiers;
		}

		string IMember.getName()
		{
			throw new NotImplementedException();
			//return identifier;
		}

		/*public GenericFunction(typesys.Type owner, Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters, string bodystr)
		{
			this.owner = owner;
			this.modifiers = modifiers;
			this.returnType = returnType;
			this.identifier = identifier;
			this.parameters = parameters;
			this.bodystr = bodystr;
		}

		public readonly Dictionary<int, typesys.Function> types = new Dictionary<int, typesys.Function>();

		public typesys.Function getTypeFor(typesys.Type[] what)
		{
			lock (types)
			{
				int hash = GenericType.getArrHash(what);
				if (types.ContainsKey(hash))
				{
					return types[hash];
				}

				typesys.Function newType = getNewTypeFor(what);
				types[hash] = newType;
				return newType;
			}
		}

		protected Function getNewTypeFor(Type[] what)
		{
			// TODO: PROPER REPLACING FFS!!!

			StringBuilder newIdB = new StringBuilder(identifier + "_");
			foreach (Type t in what)
			{
				newIdB.Append(t.name);
				newIdB.Append("_");
			}
			newIdB.Remove(newIdB.Length - 1, 1);
			string newId = newIdB.ToString();

			string bod = bodystr;
			for (int i = 0; i < what.Length; ++i)
			{
				string gtname = genTypeNames[i];
				Type w = what[i];

				bod = bod.Replace(gtname, w.name);
			}

			
		}*/
	}
}
