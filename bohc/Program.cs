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

	class Program
	{
		public static readonly Dictionary<string, parsing.ts.File> genTypes = new Dictionary<string, parsing.ts.File>();

		public static ParserState pstate;

		public static void Main(string[] args)
		{
			string[] filenames = new string[]
			{
				"src/EnumExample.boh",
				"src/Main.boh",
				"src/Box.boh",
				"src/Object.boh",
				"src/String.boh",
				"src/Type.boh",
				"src/Exception.boh",
				"src/Array.boh",
				"src/ICollection.boh",
				"src/IIndexedCollection.boh",
				"src/IIterator.boh",
				"src/Vector2f.boh",
			};
			//string[] filenames = new string[] { "src/String.boh", "src/Exception.boh", "src/Object.boh", "src/Type.boh", "src/Class.boh", "src/Interface.boh" };
			string[] files = new string[filenames.Length];

			for (int i = 0; i < filenames.Length; ++i)
			{
				files[i] = System.IO.File.ReadAllText(filenames[i]);
			}

			Dictionary<string, parsing.ts.File> filesassoc = new Dictionary<string, parsing.ts.File>();

			Stopwatch sw = new Stopwatch();
			sw.Start();

			pstate = ParserState.TS;
			for (int i = 0; i < files.Length; ++i)
			{
				string file = files[i];
				parsing.ts.File f = Parser.parseFileTS(ref file);
				files[i] = file;
				filesassoc[file] = f;
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Type skimming for: {0}", ((typesys.Type)f.type).fullName());
				}
			}

			pstate = ParserState.TP;
			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Parser.parseFileTP(f, file);
					Console.WriteLine("Type parsing for: {0}", ((typesys.Type)f.type).fullName());
				}
			}

			typesys.Primitive.figureOutFunctionsForAll();

			pstate = ParserState.TCS;
			KeyValuePair<string, parsing.ts.File>[] vpairs = genTypes.ToArray();
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.ts.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)v.Value.type).fullName());
				Parser.parseFileTCS(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
					Parser.parseFileTCS(f, file);
				}
			}

			foreach (typesys.Enum e in typesys.Enum.instances)
			{
				e.sortOutFunctions();
			}

			pstate = ParserState.TCP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.ts.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				Parser.parseFileTCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)f.type).fullName());
					Parser.parseFileTCP(f, file);
				}
			}

			pstate = ParserState.CP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.ts.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Code parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				Parser.parseFileCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Code parsing for: {0}", ((typesys.Type)f.type).fullName());
					Parser.parseFileCP(f, file);
				}
			}

			//typesys.GenericType arrayt = filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).Single();
			//arrayt.getTypeFor(new[] { typesys.Primitive.BOOLEAN });

			Console.WriteLine("Generating code");

			IMangler mangler = new CMangler();
			ICodeGen codegen = new CodeGen(mangler);

			IEnumerable<typesys.Type> types = filesassoc.Values.Select(x => x.type as typesys.Type).Where(x => x != null).Concat(
				filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).SelectMany(x => x.types.Values));

			Console.WriteLine("Generating lambdas");
			codegen.generateGeneralBit(types);

			foreach (typesys.Type type in types)
			{
				Console.WriteLine("Generating code for: {0}", type.fullName());
				codegen.generateFor(type, types);
			}

			sw.Stop();

			Console.WriteLine("Done generating code");
			Console.WriteLine("Compilation of {0} files took: {1} milliseconds", files.Length, sw.Elapsed.TotalMilliseconds);

			Console.ReadKey();
		}
	}
}
