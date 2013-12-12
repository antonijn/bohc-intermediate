using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.typesys;
using bohc.parsing;

namespace bohc
{
	public interface IMangler
	{
		string getFieldInitName(Class c);
		string getIncludeGuardName(typesys.Type type);
		string getHeaderName(typesys.Type type);
		string getCodeFileName(typesys.Type type);
		string getThisParamTypeName(typesys.Type type);
		string getCTypeName(typesys.Type type);
		string getCStructName(typesys.Type type);
		string getCName(typesys.Type type);
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
	}
}
