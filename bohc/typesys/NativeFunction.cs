using System;
using System.Collections.Generic;

namespace bohc.typesys
{
	public class NativeFunction
	{
		public readonly int paramCount;

		public NativeFunction(int paramCount, string name)
		{
			this.paramCount = paramCount;

			funcs[name] = this;
		}

		public static readonly Dictionary<string, NativeFunction> funcs = new Dictionary<string, NativeFunction>();

		public static readonly NativeFunction NATIVE_REF = new NativeFunction(1, "native_ref");
		public static readonly NativeFunction NATIVE_DEREF = new NativeFunction(2, "native_deref");
		public static readonly NativeFunction NATIVE_SIZEOF = new NativeFunction(1, "native_sizeof");
	}
}

