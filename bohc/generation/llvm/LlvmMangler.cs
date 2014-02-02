using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.Boh;
using Bohc.Exceptions;
using Bohc.Parsing;
using Bohc.Parsing.Statements;
using Bohc.TypeSystem;

using Bohc.Generation.Mangling;

namespace Bohc.Generation.Llvm
{
	public class LlvmMangler : IMangler
	{
		public string getFieldInitName(Class c)
		{
			return "@fieldinit." + getCName(c);
		}

		public string getIncludeGuardName(Bohc.TypeSystem.Type type)
		{
			throw new NotImplementedException();
		}

		public string getHeaderName(Bohc.TypeSystem.Type type)
		{
			throw new NotImplementedException();
		}

		public string getCodeFileName(Bohc.TypeSystem.Type type)
		{
			throw new NotImplementedException();
		}

		public string getThisParamTypeName(Bohc.TypeSystem.Type type)
		{
			throw new NotImplementedException();
		}

		public string getCTypeName(Bohc.TypeSystem.Type type)
		{
			if (type is Primitive)
			{
				return getCName(type);
			}
			return "%" + getCName(type);
		}

		public string getCStructName(Bohc.TypeSystem.Type type)
		{
			throw new NotImplementedException();
		}

		public string getCName(Bohc.TypeSystem.Type type)
		{
			if (type is Struct)
			{
				return "struct." + type.FullName();
			}
			else if (type is Class)
			{
				return "class." + type.FullName();
			}
			else if (type is TypeSystem.Enum)
			{
				return "enum." + type.FullName();
			}
			else if (type is Primitive)
			{
				return ((Primitive)type).LlvmName;
			}

			throw new NotImplementedException();
		}

		public string getNewName(Constructor constr)
		{
			return "@new." + getCName(constr.Owner) + getFuncAddition(constr);
		}

		public string getVarName(Variable variable)
		{
			if (variable is Field)
			{
				Field f = (Field)variable;
				return (f.Modifiers.HasFlag(Modifiers.Static) ? "@" : "") + getCName(f.Owner) + "." + f.Identifier;
			}

			if (variable.Identifier == "this")
			{
				throw new NotImplementedException();
			}

			return variable.Identifier;
		}

		// to be used when an enclosed variable is declared (in the context struct)
		public string getHeapVarDeclName(Variable variable)
		{
			throw new NotImplementedException();
		}

		public string getHeapVarAssignName(Variable variable)
		{
			/*if (variable is Parameter || variable is LambdaParam)
			{
				return "e" + getVarName(variable);
			}*/

			return getVarName(variable);
		}

		public string getVarUsageName(Variable variable, int lambdaStack)
		{
			/*Local loc = variable as Local;
			if (variable.Enclosed)
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
			}*/
			return getVarName(variable);
		}

		public string getParamTypeName(Parameter param)
		{
			throw new NotImplementedException();
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
			return "." + hash.ToString().ToLowerInvariant();
		}

		public string getOpName(Operator op)
		{
			return op.overloadName;
		}

		public string getVFuncName(Function func)
		{
			throw new NotImplementedException();
		}

		public string getCFuncName(Function func)
		{
			if (func.Modifiers.HasFlag(Modifiers.Native))
			{
				return "@" + func.Identifier;
			}

			if (func is OverloadedOperator)
			{
				return "@op." + getCName(func.Owner) + getOpName(((OverloadedOperator)func).Which) + getFuncAddition(func);
			}
			Indexer idxer = func as Indexer;
			return (idxer == null ? "@" : (idxer.IsAssignment() ? "@is." : "@ig.")) + getCName(func.Owner) + "." + func.Identifier + getFuncAddition(func);
		}

		public string getVtableName(Class c)
		{
			return "%vtable." + getCName(c);
		}

		public string getEnumeratorName(Enumerator e)
		{
			throw new NotImplementedException();
		}

		public string getGetInterfaceName(Class c)
		{
			return "@getinterface." + getCName(c);
		}
	}
}
