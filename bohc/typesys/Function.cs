// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using bohc.parsing;

namespace bohc.typesys
{
	public class Function : IFunction
	{
		public readonly typesys.Type owner;
		public readonly Modifiers modifiers;
		public readonly Type returnType;
		public readonly string identifier;
		public readonly List<Parameter> parameters;

		public readonly string bodystr;
		public parsing.statements.Body body;

		Modifiers IMember.getModifiers()
		{
			return modifiers;
		}

		string IMember.getName()
		{
			return identifier;
		}

		public Function(typesys.Type owner, Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters, string bodystr)
		{
			this.owner = owner;
			this.modifiers = modifiers;
			this.returnType = returnType;
			this.identifier = identifier;
			this.parameters = parameters;
			this.bodystr = bodystr;
		}

		public Function(typesys.Type owner, Modifiers modifiers, typesys.Type returnType, string identifier, List<Parameter> parameters, parsing.statements.Body body)
		{
			this.owner = owner;
			this.modifiers = modifiers;
			this.returnType = returnType;
			this.identifier = identifier;
			this.parameters = parameters;
			this.body = body;
		}

		/// <summary>
		/// Selects the function compatible with the given expressions.
		/// </summary>
		public static typesys.Function getCompatibleFunction(ref int i, string next, string str, parsing.File file, IEnumerable<typesys.Variable> locals, IEnumerable<typesys.Function> functions, out IEnumerable<Expression> parameters, typesys.Function ctx, IExpressionParser ep)
		{
			/*typesys.Function compatible = functions
				.Where(x => x.parameters.Count == parameters.Count())
				.SingleOrDefault(x => areParamsCompatible(parameters, x));*/

			parameters = getStringParams(str, i, locals, file, ctx, ep);
			IEnumerable<Expression> _parameters = parameters;
			typesys.Function compatible = functions
				.Where(x => x.parameters.Count == _parameters.Count())
				.Select(x => new Tuple<typesys.Function, int[]>(x, getArrayParams(_parameters, x)))
				.Where(x => !x.Item2.Contains(0))
				.Select(x => new Tuple<typesys.Function, int>(x.Item1, x.Item2.Sum()))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.FirstOrDefault();

			boh.Exception.require<exceptions.ParserException>(compatible != default(typesys.Function), "Method not found: " + next);

			int closeParent = ParserTools.getMatchingBraceChar(str, i - 1, ')');
			i = closeParent + 1;
			return compatible;
		}

		public static IEnumerable<Expression> getStringParams(string str, int i, IEnumerable<typesys.Variable> locals, parsing.File file, typesys.Function ctx, IExpressionParser ep)
		{
			int close = ParserTools.getMatchingBraceChar(str, i - 1, ')');
			string paramstring = str.Substring(i - 1, close - i + 2);
			Expression[] result = ParserTools.split(paramstring, 0, ')', ',')
							.Select(x => x.Trim())
							.Where(x => !string.IsNullOrEmpty(x))
							.Select(x => ep.analyze(x, locals, file, ctx))
							.Where(x => x != null)
							.ToArray();
			return result;
		}

		public static int[] getArrayParams(IEnumerable<Expression> parameters, typesys.Function func)
		{
			return getArrayParams(parameters.Select(x => x.getType()), func);
		}
		public static int[] getArrayParams(IEnumerable<typesys.Type> parameters, typesys.Function func)
		{
			int i = 0;
			int[] result = new int[parameters.Count()];
			using (IEnumerator<typesys.Type> exprsns = parameters.GetEnumerator())
			{
				using (IEnumerator<typesys.Parameter> paramsf = func.parameters.GetEnumerator())
				{
					while (exprsns.MoveNext() && paramsf.MoveNext())
					{
						result[i] = exprsns.Current.extends(paramsf.Current.type);
						++i;
					}
				}
			}
			return result;
		}
	}
}
