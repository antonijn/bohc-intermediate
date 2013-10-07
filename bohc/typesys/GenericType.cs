using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing.ts;

namespace bohc.typesys
{
	public abstract class GenericType : IType
	{
		File IType.getFile()
		{
			return file;
		}

		void IType.setFile(File f)
		{
			file = f;
		}

		public File file;

		private Dictionary<typesys.Type[], typesys.Type> types = new Dictionary<typesys.Type[], typesys.Type>();

		public typesys.Type getTypeFor(typesys.Type[] what)
		{
			if (types.ContainsKey(what))
			{
				return types[what];
			}

			typesys.Type newType = getNewTypeFor(what);
			types[what] = newType;
			return newType;
		}

		protected abstract typesys.Type getNewTypeFor(typesys.Type[] what);
	}
}
