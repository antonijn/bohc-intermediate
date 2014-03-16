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
	public class CMangler : IMangler
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
			StringBuilder prefix = new StringBuilder();
			StringBuilder tname = new StringBuilder();
			foreach (string pkg in type.Package.ToString().Split('.'))
			{
				prefix.Append("p");
				prefix.Append(pkg.Length);
			}

			if (type is Class)
			{
				prefix.Append("c");
			}
			else if (type is Bohc.TypeSystem.Enum)
			{
				prefix.Append("e");
			}
			else if (type is Interface)
			{
				prefix.Append("i");
			}
			else if (type is Struct)
			{
				prefix.Append("s");
			}
			else if (type is FunctionRefType)
			{
				prefix.Append("f");
			}
			prefix.Append(type.Name.Length.ToString("X"));

			FunctionRefType fRefType = type as FunctionRefType;
			if (fRefType != null)
			{
				tname.Append(getCName(fRefType.RetType));
				foreach (Bohc.TypeSystem.Type t in fRefType.ParamTypes)
				{
					tname.Append(getCName(t));
				}
				prefix.Clear();
				prefix.Append("f");
				prefix.Append(tname.ToString().Length.ToString("X"));
			}
			else
			{
				tname.Append(type.FullName().Replace(".", string.Empty));
			}

			return prefix.ToString() + "_" + tname.ToString();
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
				return "l_" + variable.Identifier;
			}

			if (variable is Parameter || variable is LambdaParam)
			{
				return "p_" + variable.Identifier;
			}

			if (variable is Field)
			{
				Field f = (Field)variable;
				if (f.Modifiers.HasFlag(Modifiers.Static))
				{
					return getCName(f.Owner) + "_sf_" + variable.Identifier;
				}
				return "f_" + variable.Identifier;
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
				return "*e" + getVarName(variable);
			}

			return "*" + getVarName(variable);
		}

		public string getHeapVarAssignName(Variable variable)
		{
			if (variable is Parameter || variable is LambdaParam)
			{
				return "e" + getVarName(variable);
			}

			return getVarName(variable);
		}

		public string getVarUsageName(Variable variable, int lambdaStack)
		{
			Local loc = variable as Local;
			if (variable.EnclosedBy.Count > 0)
			{
				if (variable.LamdaLevel == lambdaStack)
				{
					if (loc == null || !loc.Modifiers.HasFlag(Modifiers.Static))
					{
						if (variable.Identifier == "this")
						{
							return getVarName(variable);
						}
						if (variable is Parameter || variable is LambdaParam)
						{
							return "(*e" + getVarName(variable) + ")";
						}
						return "(*" + getVarName(variable) + ")";
					}
				}
				else
				{
					if (variable.Identifier == "this")
					{
						return "(*ctx->self)";
					}
					if (variable is Parameter || variable is LambdaParam)
					{
						return "(*ctx->e" + getVarName(variable) + ")";
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
			StringBuilder builder = new StringBuilder();
			if (!func.Modifiers.HasFlag(Modifiers.Static))
			{
				builder.Append("self");
			}
			if (ModifierHelper.GetPfMods(func.Modifiers) == Modifiers.None)
			{
				foreach (Bohc.TypeSystem.Type type in func.Parameters.Select(x => x.Type))
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
			return "_" + hash.ToString("X").ToLowerInvariant();
		}

		public string getVFuncName(Function func)
		{
			Indexer idxer = func as Indexer;
			return (idxer == null ? "m_" : (idxer.IsAssignment() ? "is_" : "ig_")) + func.Identifier + getFuncAddition(func);
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
			Indexer idxer = func as Indexer;
			return getCName(func.Owner) + (idxer == null ? "_m_" : (idxer.IsAssignment() ? "_is_" : "_ig_")) + func.Identifier + getFuncAddition(func);
		}

		public string getVtableName(Class c)
		{
			return "vtable_" + getCName(c);
		}

		public string getEnumeratorName(Enumerator e)
		{
			return getCName(e.EnumType) + e.Name;
		}

		public string getGetInterfaceName(Class c)
		{
			return getCName(c) + "_getinterface";
		}
	}
}
