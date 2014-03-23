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
	private static String ltoa(long l, int bas) {
		bool sign = l < 0;
		if (sign) {
			l = -l;
		}

		int digits;
		int neg = 1;
		for (digits = 0; l >= neg; ++digits) {
			neg *= bas;
		}

		char[] chars = new char[digits + (sign ? 1 : 0)];
		for (; digits > 0; --digits) {
			byte b = (byte)(l % bas);
			l /= bas;
			if (b >= bas) {
				b = (byte)('a' + b);
			} else {
				b = (byte)('0' + b);
			}
			chars[digits - (sign ? 0 : 1)] = (char)b;
		}
		if (sign) {
			chars[0] = '-';
		}

		return new String(chars);
	}

		public static void Main(string[] args)
		{
			Console.WriteLine(ltoa(-1234, 10));
			Console.WriteLine(ltoa(1000, 10));

#if DEBUG

			args = new string[]
			{
				"TestClass.boh",
				"Interface.aqua",
				"-n",
				"-t",
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
