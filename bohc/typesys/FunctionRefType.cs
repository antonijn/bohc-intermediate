using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bohc.TypeSystem
{
	public class FunctionRefType : Type
	{
		public readonly Type RetType;
		public readonly Type[] ParamTypes;

		private FunctionRefType(Type retType, Type[] paramTypes)
			: base(Package.Global, Modifiers.Public | Modifiers.Final, string.Empty)
		{
			this.RetType = retType;
			this.ParamTypes = paramTypes;
		}

		public override Parsing.Expression DefaultVal()
		{
			return new Parsing.Literal(this, "BOH_FP_NULL");
		}

		public override int Extends(Type other)
		{
			return other == this ? 1 : (other == StdType.Obj ? 2 : 0);
		}

		public override int GetHashCode()
		{
			return RetType.GetHashCode() + ParamTypes.Sum(x => x.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			FunctionRefType fRefType = obj as FunctionRefType;
			if (fRefType == null)
			{
				return false;
			}

			if (fRefType.ParamTypes.Length != ParamTypes.Length)
			{
				return false;
			}

			for (int i = 0; i < ParamTypes.Length; ++i)
			{
				if (ParamTypes[i].Extends(fRefType.ParamTypes[i]) != 1)
				{
					return false;
				}
			}

			return RetType.Extends(fRefType.RetType) == 1;
		}

		public static readonly List<FunctionRefType> Instances = new List<FunctionRefType>();
		public static FunctionRefType Get(Type retType, Type[] paramTypes)
		{
			lock (Instances)
			{
				FunctionRefType c = Instances.SingleOrDefault(x => x.Equals(new FunctionRefType(retType, paramTypes)));
				if (c == null)
				{
					FunctionRefType newc = new FunctionRefType(retType, paramTypes);
					Instances.Add(newc);
					return newc;
				}

				return c;
			}
		}

		public override string ExternName()
		{
			StringBuilder b = new StringBuilder();
			b.Append(RetType.ExternName());
			b.Append("(");
			foreach (Type t in ParamTypes)
			{
				b.Append(t.ExternName());
				b.Append(", ");
			}
			if (ParamTypes.Length > 0)
			{
				b.Remove(b.Length - 2, 2);
			}
			b.Append(")");
			return b.ToString();
		}

		public override bool IsReferenceType()
		{
			return true;
		}
	}
}
