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
using System.Diagnostics;
using System.Xml.Linq;

using Bohc.Generation;
using Bohc.Generation.C;
using Bohc.Generation.Mangling;
using Bohc.Generation.Llvm;

using Bohc.Parsing;
using Bohc.Parsing.Statements;

using Bohc.General;

namespace Bohc
{
	public enum ParserState
	{
		TS,
		TP,
		TCS,
		TCP,
		CP,
	}

	public class Program
	{

		public static void Main(string[] args)
		{
#if DEBUG

			args = new string[]
			{
				"TestClass.boh",
				"-n",
				//"-N",
				//"-S",
			};
#endif

			Project p = new Project(args);

			Stopwatch sw = new Stopwatch();
			sw.Start();
			p.Parse();
			sw.Stop();
			p.Build();

			/*System.IO.StringReader sr = new System.IO.StringReader("8 + 3 * 12 + 2 - asdf");
			Token[] tokens = new Tokenizer(sr, "example.boh").lex().ToArray();
			TokenStream ts = new TokenStream(tokens, 0, tokens.Length);

			TokenizedExpressionParser tep = new TokenizedExpressionParser();
			Expression result = tep.analyze(ts, new TypeSystem.Variable[0], new TypeSystem.Function(TypeSystem.Class.Get<TypeSystem.Class>(TypeSystem.Package.GetFromString("bla.bla"), TypeSystem.Modifiers.None, "hi"), TypeSystem.Modifiers.None, null, null, null, ""));
			*/
			//result.getType();
		}
	}
}
