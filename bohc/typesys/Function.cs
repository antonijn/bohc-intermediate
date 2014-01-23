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

using Bohc.Parsing;

namespace Bohc.TypeSystem
{
	public class Function : IFunction
	{
		public readonly Bohc.TypeSystem.Type Owner;
		public readonly Modifiers Modifiers;
		public readonly Type ReturnType;
		public readonly string Identifier;
		public readonly List<Parameter> Parameters;

		public readonly string BodyStr;
		public Parsing.Statements.Body Body;

		Modifiers IMember.GetModifiers()
		{
			return Modifiers;
		}

		string IMember.GetName()
		{
			return Identifier;
		}

		public Function(Bohc.TypeSystem.Type owner, Modifiers modifiers, Bohc.TypeSystem.Type returnType, string identifier, List<Parameter> parameters, string bodystr)
		{
			this.Owner = owner;
			this.Modifiers = modifiers;
			this.ReturnType = returnType;
			this.Identifier = identifier;
			this.Parameters = parameters;
			this.BodyStr = bodystr;
		}

		public Function(Bohc.TypeSystem.Type owner, Modifiers modifiers, Bohc.TypeSystem.Type returnType, string identifier, List<Parameter> parameters, Parsing.Statements.Body body)
		{
			this.Owner = owner;
			this.Modifiers = modifiers;
			this.ReturnType = returnType;
			this.Identifier = identifier;
			this.Parameters = parameters;
			this.Body = body;
		}

		/// <summary>
		/// Selects the function compatible with the given expressions.
		/// </summary>
		public static Bohc.TypeSystem.Function GetCompatibleFunction(ref int i, string next, string str, Parsing.File file, IEnumerable<Bohc.TypeSystem.Variable> locals, IEnumerable<Bohc.TypeSystem.Function> functions, out IEnumerable<Expression> parameters, Bohc.TypeSystem.Function ctx, IExpressionParser ep, char close)
		{
			//typesys.Function[] compatiblefs = functions.ToArray();

			parameters = GetStringParams(str, i, locals, file, ctx, ep, close);
			IEnumerable<Expression> _parameters = parameters;
			Bohc.TypeSystem.Function compatible = functions
				.Where(x => x.Parameters.Count == _parameters.Count())
				.Select(x => new Tuple<Bohc.TypeSystem.Function, int[]>(x, GetArrayParams(_parameters, x)))
				.Where(x => !x.Item2.Contains(0))
				.Select(x => new Tuple<Bohc.TypeSystem.Function, int>(x.Item1, x.Item2.Sum()))
				.OrderBy(x => x.Item2)
				.Where(x => x.Item1.Parameters.Count == 0 || x.Item2 != 0)
				.Select(x => x.Item1)
				.FirstOrDefault();

			Boh.Exception.require<Exceptions.ParserException>(compatible != default(Bohc.TypeSystem.Function), "Method not found: " + next);

			int closeParent = ParserTools.getMatchingBraceChar(str, i - 1, close);
			i = closeParent + 1;
			return compatible;
		}

		public static IEnumerable<Expression> GetStringParams(string str, int i, IEnumerable<Bohc.TypeSystem.Variable> locals, Parsing.File file, Bohc.TypeSystem.Function ctx, IExpressionParser ep, char close)
		{
			int _close = ParserTools.getMatchingBraceChar(str, i - 1, close);
			string paramstring = str.Substring(i - 1, _close - i + 2);
			Expression[] result = ParserTools.split(paramstring, 0, close, ',')
							.Select(x => x.Trim())
							.Where(x => !string.IsNullOrEmpty(x))
							.Select(x => ep.analyze(x, locals, file, ctx))
							.Where(x => x != null)
							.ToArray();
			return result;
		}

		public static int[] GetArrayParams(IEnumerable<Expression> parameters, Bohc.TypeSystem.Function func)
		{
			return GetArrayParams(parameters.Select(x => x.getType()), func);
		}
		public static int[] GetArrayParams(IEnumerable<Bohc.TypeSystem.Type> parameters, Bohc.TypeSystem.Function func)
		{
			int i = 0;
			int[] result = new int[parameters.Count()];
			using (IEnumerator<Bohc.TypeSystem.Type> exprsns = parameters.GetEnumerator())
			{
				using (IEnumerator<Bohc.TypeSystem.Parameter> paramsf = func.Parameters.GetEnumerator())
				{
					while (exprsns.MoveNext() && paramsf.MoveNext())
					{
						result[i] = exprsns.Current.Extends(paramsf.Current.Type);
						++i;
					}
				}
			}
			return result;
		}
	}
}
