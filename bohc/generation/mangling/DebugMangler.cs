using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.boh;
using bohc.exceptions;
using bohc.parsing;
using bohc.parsing.statements;
using bohc.typesys;

namespace bohc.generation.mangling
{
	public class DebugMangler : IMangler
	{
		public string getFieldInitName(Class c)
		{
			return getCName(c) + "_fi";
		}

		public string getIncludeGuardName(typesys.Type type)
		{
			return getCName(type) + "_H";
		}

		public string getHeaderName(typesys.Type type)
		{
			return "all.h";
		}

		public string getCodeFileName(typesys.Type type)
		{
			return getCName(type) + ".c";
		}

		public string getThisParamTypeName(typesys.Type type)
		{
			if (type is Struct)
			{
				return getCTypeName(type) + " * const";
			}

			if (type is Interface)
			{
				return getCTypeName(StdType.obj) + " const";
			}

			return getCTypeName(type) + " const";
		}

		public string getCTypeName(typesys.Type type)
		{
			NativeType nt = type as NativeType;
			if (nt != null)
			{
				int idxPtr = nt.crep.IndexOf('*');
				if (idxPtr != -1)
				{
					string prePtr = nt.crep.Substring(0, idxPtr != -1 ? idxPtr : nt.crep.Length);

					int lidxDot = prePtr.LastIndexOf('.');
					string pkg = prePtr.Substring(0, lidxDot != -1 ? lidxDot : 0);
					string cname = prePtr.Substring(lidxDot != -1 ? lidxDot + 1 : 0);
					// TODO: fix that null
					typesys.Type t = typesys.Type.getExisting(Package.getFromStringExisting(pkg), cname, null);
					string ptrs = nt.crep.Substring(idxPtr);
					return (t != null ? getCTypeName(t) : prePtr) + ptrs;
				}

				return nt.crep;
			}

			if (type is Struct || type is FunctionRefType || type is typesys.Enum)
			{
				return getCStructName(type);
			}

			if (type is Class || type is Interface)
			{
				return getCStructName(type) + " *";
			}

			if (type is Primitive)
			{
				return ((Primitive)type).cname;
			}

			if (type is NullType)
			{
				return "NULL";
			}

			throw new NotImplementedException();
		}

		public string getCStructName(typesys.Type type)
		{
			if (type is typesys.Enum)
			{
				return "enum " + getCName(type);
			}
			return "struct " + getCName(type);
		}

		public string getCName(typesys.Type type)
		{
			FunctionRefType frType = type as FunctionRefType;
			if (frType != null)
			{
				StringBuilder sb = new StringBuilder("f_");
				sb.Append(getCName(frType.retType));
				foreach (typesys.Type t in frType.paramTypes)
				{
					sb.Append(getCName(t));
				}
				return sb.ToString();
			}
			return type.name;
		}

		public string getNewName(Constructor constr)
		{
			return "new_" + getCName(constr.owner) + getFuncAddition(constr);
		}

		public string getVarName(Variable variable)
		{
			/*if (varUsageCallback != null)
			{
				varUsageCallback(variable);
			}*/

			if (variable is Local)
			{
				// FIXME: won't work if the local was created in the lambda itself
				return variable.identifier;
			}

			if (variable is Parameter || variable is LambdaParam)
			{
				return variable.identifier;
			}

			if (variable is Field)
			{
				Field f = (Field)variable;
				if (f.modifiers.HasFlag(Modifiers.STATIC))
				{
					return getCName(f.owner) + variable.identifier;
				}
				return variable.identifier;
			}

			if (variable.identifier == "this")
			{
				if (variable.type is Struct)
				{
					return "(*self)";
				}
				return "self";
			}

			throw new NotImplementedException();
		}

		// to be used when an enclosed variable is declared (in the context struct)
		public string getHeapVarDeclName(Variable variable)
		{
			if (variable.identifier == "this")
			{
				return "self";
			}

			if (variable is Parameter || variable is LambdaParam)
			{
				return "*" + getVarName(variable);
			}

			return "*" + getVarName(variable);
		}

		public string getHeapVarAssignName(Variable variable)
		{
			if (variable is Parameter || variable is LambdaParam)
			{
				return getVarName(variable);
			}

			return getVarName(variable);
		}

		public string getVarUsageName(Variable variable, int lambdaStack)
		{
			if (variable.enclosed)
			{
				if (variable.lambdaLevel == lambdaStack)
				{
					if (variable.identifier == "this")
					{
						return getVarName(variable);
					}
					if (variable is Parameter || variable is LambdaParam)
					{
						return "(*" + getVarName(variable) + ")";
					}
					return "(*" + getVarName(variable) + ")";
				}
				else
				{
					if (variable.identifier == "this")
					{
						return "(*ctx->self)";
					}
					if (variable is Parameter || variable is LambdaParam)
					{
						return "(*ctx->" + getVarName(variable) + ")";
					}
					return "(*ctx->" + getVarName(variable) + ")";
				}
			}

			Parameter param = variable as Parameter;
			if (param != null)
			{
				if (param.modifiers.HasFlag(Modifiers.REF))
				{
					return "(*" + getVarName(variable) + ")";
				}
			}
			return getVarName(variable);
		}

		public string getParamTypeName(Parameter param)
		{
			if (param.modifiers.HasFlag(Modifiers.REF))
			{
				return getCTypeName(param.type) + "* const";
			}
			if (param.modifiers.HasFlag(Modifiers.FINAL))
			{
				return getCTypeName(param.type) + " const";
			}
			return getCTypeName(param.type);
		}

		public string getParamTypeName(LambdaParam param)
		{
			// TODO: implement this
			/*if (param.modifiers.HasFlag(Modifiers.REF))
			{
				return getCTypeName(param.type) + "*";
			}*/
			return getCTypeName(param.type);
		}

		private uint hashString(string str)
		{
			return (uint)str.GetHashCode();
		}

		public string getFuncAddition(Function func)
		{
			return string.Empty;
			/*StringBuilder builder = new StringBuilder();
			if (!func.modifiers.HasFlag(Modifiers.STATIC))
			{
				builder.Append("self");
			}
			if (ModifierHelper.getPfMods(func.modifiers) == Modifiers.NONE)
			{
				foreach (typesys.Type type in func.parameters.Select(x => x.type))
				{
					builder.Append(getCName(type));
				}
			}
			else
			{
				builder.Append("XXX");
			}
			string str = builder.ToString();
			uint hash = hashString(str);
			return "_" + hash.ToString("X").ToLowerInvariant();*/
		}

		public string getVFuncName(Function func)
		{
			return func.identifier + getFuncAddition(func);
		}

		public string getOpName(Operator op)
		{
			return op.overloadName;
		}

		public string getCFuncName(Function func)
		{
			if (func.modifiers.HasFlag(Modifiers.NATIVE))
			{
				return func.identifier;
			}

			if (func is OverloadedOperator)
			{
				return getCName(func.owner) + "_op_" + getOpName(((OverloadedOperator)func).which) + getFuncAddition(func);
			}
			return getCName(func.owner) + "_" + func.identifier + getFuncAddition(func);
		}

		public string getVtableName(Class c)
		{
			return "vtable_" + getCName(c);
		}

		public string getEnumeratorName(Enumerator e)
		{
			return getCName(e.enumType) + e.name;
		}
	}
}
