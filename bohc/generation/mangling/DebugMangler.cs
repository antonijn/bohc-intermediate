using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Boh;
using Bohc.Exceptions;
using Bohc.Parsing;
using Bohc.Parsing.Statements;
using Bohc.TypeSystem;

namespace Bohc.Generation.Mangling
{
	public class DebugMangler : IMangler
	{
		public string getFieldInitName(Class c)
		{
			return getCName(c) + "_fi";
		}

		public string getIncludeGuardName(Bohc.TypeSystem.Type type)
		{
			return getCName(type) + "_H";
		}

		public string getHeaderName(Bohc.TypeSystem.Type type)
		{
			return "all.h";
		}

		public string getCodeFileName(Bohc.TypeSystem.Type type)
		{
			return getCName(type) + ".c";
		}

		public string getThisParamTypeName(Bohc.TypeSystem.Type type)
		{
			if (type is Struct)
			{
				return getCTypeName(type) + " * const";
			}

			if (type is Interface)
			{
				return getCTypeName(StdType.Obj) + " const";
			}

			return getCTypeName(type) + " const";
		}

		public string getCTypeName(Bohc.TypeSystem.Type type)
		{
			if (type is Struct || type is FunctionRefType || type is Bohc.TypeSystem.Enum)
			{
				return getCStructName(type);
			}

			if (type is Class || type is Interface)
			{
				return getCStructName(type) + " *";
			}

			if (type is Primitive)
			{
				return ((Primitive)type).CName;
			}

			if (type is NullType)
			{
				return "NULL";
			}

			throw new NotImplementedException();
		}

		public string getCStructName(Bohc.TypeSystem.Type type)
		{
			if (type is Bohc.TypeSystem.Enum)
			{
				return "enum " + getCName(type);
			}
			return "struct " + getCName(type);
		}

		public string getCName(Bohc.TypeSystem.Type type)
		{
			FunctionRefType frType = type as FunctionRefType;
			if (frType != null)
			{
				StringBuilder sb = new StringBuilder("f_");
				sb.Append(getCName(frType.RetType));
				foreach (Bohc.TypeSystem.Type t in frType.ParamTypes)
				{
					sb.Append(getCName(t));
				}
				return sb.ToString();
			}
			return type.Name;
		}

		public string getNewName(Constructor constr)
		{
			return "new_" + getCName(constr.Owner) + getFuncAddition(constr);
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
				return variable.Identifier;
			}

			if (variable is Parameter || variable is LambdaParam)
			{
				return variable.Identifier;
			}

			if (variable is Field)
			{
				Field f = (Field)variable;
				if (f.Modifiers.HasFlag(Modifiers.Static))
				{
					return getCName(f.Owner) + variable.Identifier;
				}
				return variable.Identifier;
			}

			if (variable.Identifier == "this")
			{
				if (variable.Type is Struct)
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
			if (variable.Identifier == "this")
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
			if (variable.Enclosed)
			{
				if (variable.LamdaLevel == lambdaStack)
				{
					if (variable.Identifier == "this")
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
					if (variable.Identifier == "this")
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
				if (param.Modifiers.HasFlag(Modifiers.Ref))
				{
					return "(*" + getVarName(variable) + ")";
				}
			}
			return getVarName(variable);
		}

		public string getParamTypeName(Parameter param)
		{
			if (param.Modifiers.HasFlag(Modifiers.Ref))
			{
				return getCTypeName(param.Type) + "* const";
			}
			if (param.Modifiers.HasFlag(Modifiers.Final))
			{
				return getCTypeName(param.Type) + " const";
			}
			return getCTypeName(param.Type);
		}

		public string getParamTypeName(LambdaParam param)
		{
			// TODO: implement this
			/*if (param.modifiers.HasFlag(Modifiers.REF))
			{
				return getCTypeName(param.type) + "*";
			}*/
			return getCTypeName(param.Type);
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
			return func.Identifier + getFuncAddition(func);
		}

		public string getOpName(Operator op)
		{
			return op.overloadName;
		}

		public string getCFuncName(Function func)
		{
			if (func.Modifiers.HasFlag(Modifiers.Native))
			{
				return func.Identifier;
			}

			if (func is OverloadedOperator)
			{
				return getCName(func.Owner) + "_op_" + getOpName(((OverloadedOperator)func).Which) + getFuncAddition(func);
			}
			return getCName(func.Owner) + "_" + func.Identifier + getFuncAddition(func);
		}

		public string getVtableName(Class c)
		{
			return "vtable_" + getCName(c);
		}

		public string getEnumeratorName(Enumerator e)
		{
			return getCName(e.EnumType) + e.Name;
		}
	}
}
