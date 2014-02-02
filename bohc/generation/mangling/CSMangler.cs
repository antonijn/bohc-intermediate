using System;

namespace Bohc.Generation.Mangling
{
	public class CSMangler : IMangler
	{
		public CSMangler()
		{
		}

		public string getFieldInitName (Bohc.TypeSystem.Class c)
		{
			throw new System.NotImplementedException ();
		}

		public string getIncludeGuardName (Bohc.TypeSystem.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getHeaderName (Bohc.TypeSystem.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCodeFileName (Bohc.TypeSystem.Type type)
		{
			return type.FullName().Replace('.', System.IO.Path.DirectorySeparatorChar) + ".cs";
		}

		public string getThisParamTypeName (Bohc.TypeSystem.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCTypeName (Bohc.TypeSystem.Type type)
		{
			return type.FullName();
		}

		public string getCStructName (Bohc.TypeSystem.Type type)
		{
			throw new System.NotImplementedException ();
		}

		public string getCName (Bohc.TypeSystem.Type type)
		{
			return type.Name;
		}

		public string getNewName (Bohc.TypeSystem.Constructor constr)
		{
			return "new " + getCTypeName(constr.Owner);
		}

		public string getVarName (Bohc.TypeSystem.Variable variable)
		{
			return variable.Identifier;
		}

		public string getHeapVarDeclName (Bohc.TypeSystem.Variable variable)
		{
			throw new System.NotImplementedException ();
		}

		public string getHeapVarAssignName (Bohc.TypeSystem.Variable variable)
		{
			throw new System.NotImplementedException ();
		}

		public string getVarUsageName (Bohc.TypeSystem.Variable variable, int lambdaStack)
		{
			return variable.Identifier;
		}

		public string getParamTypeName (Bohc.TypeSystem.Parameter param)
		{
			return param.Identifier;
		}

		public string getParamTypeName (Bohc.TypeSystem.LambdaParam param)
		{
			return param.Identifier;
		}

		public string getVFuncName (Bohc.TypeSystem.Function func)
		{
			return func.Identifier;
		}

		public string getOpName (Bohc.Parsing.Operator op)
		{
			return op.representation;
		}

		public string getCFuncName (Bohc.TypeSystem.Function func)
		{
			return func.Identifier;
		}

		public string getVtableName (Bohc.TypeSystem.Class c)
		{
			throw new System.NotImplementedException ();
		}

		public string getEnumeratorName (Bohc.TypeSystem.Enumerator e)
		{
			return e.Name;
		}

		public string getGetInterfaceName(TypeSystem.Class c)
		{
			throw new NotImplementedException();
		}
	}
}

