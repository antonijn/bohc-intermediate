using System;

namespace bohc.generation.mangling
{
	public class CSMangler : IMangler
	{
		public CSMangler()
		{
		}

		public string getFieldInitName (bohc.typesys.Class c)
		{
			throw new System.NotImplementedException ();
		}

		public string getIncludeGuardName (bohc.typesys.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getHeaderName (bohc.typesys.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCodeFileName (bohc.typesys.Type type)
		{
			return type.fullName().Replace('.', System.IO.Path.DirectorySeparatorChar) + ".cs";
		}

		public string getThisParamTypeName (bohc.typesys.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCTypeName (bohc.typesys.Type type)
		{
			return type.fullName();
		}

		public string getCStructName (bohc.typesys.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCName (bohc.typesys.Type type)
		{
			return type.name;
		}

		public string getNewName (bohc.typesys.Constructor constr)
		{
			return "new " + getCTypeName(constr.owner);
		}

		public string getVarName (bohc.typesys.Variable variable)
		{
			return variable.identifier;
		}

		public string getHeapVarDeclName (bohc.typesys.Variable variable)
		{
			throw new System.NotImplementedException ();
		}

		public string getHeapVarAssignName (bohc.typesys.Variable variable)
		{
			throw new System.NotImplementedException ();
		}

		public string getVarUsageName (bohc.typesys.Variable variable, int lambdaStack)
		{
			return variable.identifier;
		}

		public string getParamTypeName (bohc.typesys.Parameter param)
		{
			return param.identifier;
		}

		public string getParamTypeName (bohc.typesys.LambdaParam param)
		{
			return param.identifier;
		}

		public string getVFuncName (bohc.typesys.Function func)
		{
			return func.identifier;
		}

		public string getOpName (bohc.parsing.Operator op)
		{
			return op.representation;
		}

		public string getCFuncName (bohc.typesys.Function func)
		{
			return func.identifier;
		}

		public string getVtableName (bohc.typesys.Class c)
		{
			throw new System.NotImplementedException ();
		}

		public string getEnumeratorName (bohc.typesys.Enumerator e)
		{
			return e.name;
		}
	}
}

