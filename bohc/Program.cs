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

using bohc.generation;
using bohc.generation.c;
using bohc.generation.mangling;

using bohc.parsing;
using bohc.parsing.statements;
using bohc.parsing.expressions;

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
		public static readonly Dictionary<string, parsing.File> genTypes = new Dictionary<string, parsing.File>();

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
				"src/List.boh",
				"src/StringBuilder.boh",
				"src/Ptr.boh",
				"src/VoidPtr.boh",
				"src/Interop.boh",
			};
			string[] files = new string[filenames.Length];

			for (int i = 0; i < filenames.Length; ++i)
			{
				files[i] = System.IO.File.ReadAllText(filenames[i]);
			}

			Dictionary<string, parsing.File> filesassoc = new Dictionary<string, parsing.File>();

			Stopwatch sw = new Stopwatch();
			sw.Start();

			Parser parser = new Parser(new DefaultStatementParser(new DefaultExpressionParser()));

			pstate = ParserState.TS;
			for (int i = 0; i < files.Length; ++i)
			{
				string file = files[i];
				parsing.File f = parser.parseFileTS(ref file);
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
				parsing.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					parser.parseFileTP(f, file);
					Console.WriteLine("Type parsing for: {0}", ((typesys.Type)f.type).fullName());
				}
			}

			typesys.Primitive.figureOutFunctionsForAll();

			pstate = ParserState.TCS;
			KeyValuePair<string, parsing.File>[] vpairs = genTypes.ToArray();
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileTCS(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
					parser.parseFileTCS(f, file);
				}
			}

			foreach (typesys.Enum e in typesys.Enum.instances)
			{
				e.sortOutFunctions(parser);
			}

			pstate = ParserState.TCP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileTCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)f.type).fullName());
					parser.parseFileTCP(f, file);
				}
			}

			pstate = ParserState.CP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.File> v = vpairs.ToArray()[i];
				Console.WriteLine("Code parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Console.WriteLine("Code parsing for: {0}", ((typesys.Type)f.type).fullName());
					parser.parseFileCP(f, file);
				}
			}

			//typesys.GenericType arrayt = filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).Single();
			//arrayt.getTypeFor(new[] { typesys.Primitive.BOOLEAN });

			Console.WriteLine("Generating code");

			IMangler mangler = new CMangler();
			ICodeGen codegen = new CCodeGen(mangler);

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
