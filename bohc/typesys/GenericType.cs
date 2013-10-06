using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public abstract class GenericType
	{
		private Dictionary<typesys.Type, typesys.Type> types = new Dictionary<typesys.Type, typesys.Type>();

		public typesys.Type getTypeFor(typesys.Type what)
		{
			if (types.ContainsKey(what))
			{
				return types[what];
			}

			typesys.Type newType = getNewTypeFor(what);
			types[newType] = newType;
			return newType;
		}

		protected abstract typesys.Type getNewTypeFor(typesys.Type what);
	}
}
