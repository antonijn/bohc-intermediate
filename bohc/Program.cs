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

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

using bohc.general;

namespace bohc
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
				"/home/antonijn/code/cs/bohp/bohp/bin/Release/hw/MainClass.boh",
				"-n",
			};
#endif

			Project p = new Project(args);
			p.parse();
			p.build();
		}
	}
}
