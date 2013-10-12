﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace bohc
{
	class Program
	{
		public static void Main(string[] args)
		{
			string file = @"
package hey.hi;

import boh.lang;
import boh.lang.lala;

public class Class
{
	public int ah;

	public this()
	{
	}

	private virtual Class get(float f)
	{
	}

	public static void main()
	{
		if (4 == 4)
		{
			new Class().get(1f).get(2).ah;
		}
	}
}";
			Stopwatch total = new Stopwatch();
			total.Start();
			Stopwatch sw = new Stopwatch();
			sw.Start();
			parsing.ts.File f0 = Parser.parseFileTS(file);
			sw.Stop();
			Console.WriteLine("Type Skimming step took:             {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			sw.Reset();
			sw.Start();
			Parser.parseFileTP(f0, file);
			sw.Stop();
			Console.WriteLine("Type Parsing step took:              {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			sw.Reset();
			sw.Start();
			Parser.parseFileTCS(f0, file);
			Parser.parseFileTCP(f0, file);
			Parser.parseFileCP(f0, file);
			sw.Stop();
			Console.WriteLine("Type Content Skimming step took:     {0} milliseconds", sw.Elapsed.TotalMilliseconds);
			total.Stop();
			Console.WriteLine("Parsing took:                        {0} milliseconds", total.Elapsed.TotalMilliseconds);

			CodeGen.generateFor((typesys.Type)f0.type, new typesys.Type[0]);

			Console.ReadKey();
		}
	}
}
