using System;
using System.Collections.Generic;

namespace Bohc.TypeSystem
{
	public class NativeFunction
	{
		public readonly int ParamCount;

		public NativeFunction(int paramCount, string name)
		{
			this.ParamCount = paramCount;

			Funcs[name] = this;
		}

		public static readonly Dictionary<string, NativeFunction> Funcs = new Dictionary<string, NativeFunction>();

		public static readonly NativeFunction NativeSizeof = new NativeFunction(1, "native_sizeof");
	}
}

