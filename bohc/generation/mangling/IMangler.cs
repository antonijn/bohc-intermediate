using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bohc.TypeSystem;
using Bohc.Parsing;

namespace Bohc.Generation.Mangling
{
	public interface IMangler
	{
		string getFieldInitName(Class c);
		string getIncludeGuardName(Bohc.TypeSystem.Type type);
		string getHeaderName(Bohc.TypeSystem.Type type);
		string getCodeFileName(Bohc.TypeSystem.Type type);
		string getThisParamTypeName(Bohc.TypeSystem.Type type);
		string getCTypeName(Bohc.TypeSystem.Type type);
		string getCStructName(Bohc.TypeSystem.Type type);
		string getCName(Bohc.TypeSystem.Type type);
		string getNewName(Constructor constr);
		string getVarName(Variable variable);
		string getHeapVarDeclName(Variable variable);
		string getHeapVarAssignName(Variable variable);
		string getVarUsageName(Variable variable, int lambdaStack);
		string getParamTypeName(Parameter param);
		string getParamTypeName(LambdaParam param);
		string getVFuncName(Function func);
		string getOpName(Operator op);
		string getCFuncName(Function func);
		string getVtableName(Class c);
		string getEnumeratorName(Enumerator e);
		string getGetInterfaceName(Class c);
	}
}
