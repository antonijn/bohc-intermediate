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
				"-N",
			};
#endif

			Project p = new Project(args);

			Stopwatch sw = new Stopwatch();
			sw.Start();
			p.Parse();
			sw.Stop();
			Console.WriteLine(TimeSpan.TicksPerSecond);
			p.Build();
		}
	}
}
