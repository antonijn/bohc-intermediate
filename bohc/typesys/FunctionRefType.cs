using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc.typesys
{
	public class FunctionRefType : Type
	{
		public readonly Type retType;
		public readonly Type[] paramTypes;

		private FunctionRefType(Type retType, Type[] paramTypes)
			: base(Package.GLOBAL, Modifiers.PUBLIC | Modifiers.FINAL, string.Empty)
		{
			this.retType = retType;
			this.paramTypes = paramTypes;
		}

		public override parsing.Expression defaultVal()
		{
			return new parsing.Literal(this, "BOH_FP_NULL");
		}

		public override int extends(Type other)
		{
			return other == this ? 1 : (other == StdType.obj ? 2 : 0);
		}

		public override int GetHashCode()
		{
			return retType.GetHashCode() + paramTypes.Sum(x => x.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			FunctionRefType fRefType = obj as FunctionRefType;
			if (fRefType == null)
			{
				return false;
			}

			if (fRefType.paramTypes.Length != paramTypes.Length)
			{
				return false;
			}

			for (int i = 0; i < paramTypes.Length; ++i)
			{
				if (paramTypes[i].extends(fRefType.paramTypes[i]) != 1)
				{
					return false;
				}
			}

			return retType.extends(fRefType.retType) == 1;
		}

		public static readonly List<FunctionRefType> instances = new List<FunctionRefType>();
		public static FunctionRefType get(Type retType, Type[] paramTypes)
		{
			lock (instances)
			{
				FunctionRefType c = instances.SingleOrDefault(x => x.Equals(new FunctionRefType(retType, paramTypes)));
				if (c == null)
				{
					FunctionRefType newc = new FunctionRefType(retType, paramTypes);
					instances.Add(newc);
					return newc;
				}

				return c;
			}
		}
	}
}
