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

	public class Program
	{
		public static class Options
		{
			public static bool optimize;
			public static bool debugSymbols;

			public static bool desktopMode;
			public static bool webMode;

			public static bool library;

			public static bool noDelete;

			public static bool unsafeCasts;
			public static bool unsafeNullPtr;

			public static bool sizeOverSpeedHint;
			public static bool speedOverSizeHint;

			public static string output = "application";
			public static string outputDir = ".";

			public static bool noStd;

			public static bool thisMachine;
		}

		public static readonly Dictionary<string, parsing.File> genTypes = new Dictionary<string, parsing.File>();

		public static ParserState pstate;

		private static void displayHelp()
		{
			Console.WriteLine("The compiler for the Boh programming language. Intermediate version.");
			Console.WriteLine("Usage: mono bohc.exe [options...] [input files...]");
			Console.WriteLine();
			Console.WriteLine("Options:");
			Console.WriteLine("  --help");
		}

		private static IEnumerable<string> getInput(string[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				string s = args[i];
				switch (s)
				{
					case "--help":
						displayHelp();
						Environment.Exit(0);
						break;
					case "-d":
						Options.debugSymbols = true;
						break;
					case "-D":
						Options.desktopMode = true;
						break;
					case "-e":
						Console.Error.WriteLine("WARNING: External linking not implemented yet");
						break;
					case "-L":
						Options.library = true;
						break;
					case "-n":
						Options.noDelete = true;
						break;
					case "-N":
						Console.Error.WriteLine("WARNING: Disabling of context deletion not implemented yet");
						break;
					case "-o":
						Options.output = args[++i];
						break;
					case "-O":
						Options.outputDir = args[++i];
						break;
					case "-r":
						Console.Error.WriteLine("WARNING: Disabling of context deletion not implemented yet");
						break;
					case "-S":
						Options.noStd = true;
						break;
					case "-t":
						Options.thisMachine = true;
						break;
					case "-W":
						Options.webMode = true;
						break;
					default:
						yield return s;
						break;
				}
			}
		}

		public static void Main(string[] args)
		{
#if DEBUG
			string[] filenames = new string[]
			{
				"stdlib/MainClass.boh",
				"stdlib/Box.boh",
				"stdlib/Object.boh",
				"stdlib/String.boh",
				"stdlib/Type.boh",
				"stdlib/Exception.boh",
				"stdlib/Array.boh",
				"stdlib/ICollection.boh",
				"stdlib/IIndexedCollection.boh",
				"stdlib/IIterator.boh",
				"stdlib/List.boh",
				"stdlib/StringBuilder.boh",
				"stdlib/Ptr.boh",
				"stdlib/VoidPtr.boh",
				"stdlib/Interop.boh",
				"stdlib/Query.boh",
				"stdlib/WhereIterator.boh",
				"stdlib/NullPtrException.boh",
				"stdlib/System.boh",
				"stdlib/FileOutStream.boh",
			};
#else
			string[] filenames = getInput(args).ToArray();
			if (filenames.Length == 0)
			{
				Console.Error.WriteLine("ERROR: No input files specified");
				Environment.Exit(1);
			}
#endif
			string[] files = new string[filenames.Length];

			for (int i = 0; i < filenames.Length; ++i)
			{
				files [i] = System.IO.File.ReadAllText(filenames [i]);
			}

			Dictionary<string, parsing.File> filesassoc = new Dictionary<string, parsing.File>();

			Stopwatch sw = new Stopwatch();
			sw.Start();

			Parser parser = new Parser(new DefaultStatementParser(new DefaultExpressionParser()));

			pstate = ParserState.TS;
			for (int i = 0; i < files.Length; ++i)
			{
				string file = files [i];
				parsing.File f = parser.parseFileTS(ref file);
				files [i] = file;
				filesassoc [file] = f;
				if (f.type is typesys.Type)
				{
					//Console.WriteLine("Type skimming for: {0}", ((typesys.Type)f.type).fullName());
				}
			}

			pstate = ParserState.TP;
			foreach (string file in files)
			{
				parsing.File f = filesassoc [file];
				if (f.type is typesys.Type)
				{
					parser.parseFileTP(f, file);
					//Console.WriteLine("Type parsing for: {0}", ((typesys.Type)f.type).fullName());
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
				KeyValuePair<string, parsing.File> v = vpairs.ToArray() [i];
				//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileTCS(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc [file];
				if (f.type is typesys.Type)
				{
					//Console.WriteLine("Type Content skimming for: {0}", ((typesys.Type)f.type).fullName());
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
				KeyValuePair<string, parsing.File> v = vpairs.ToArray() [i];
				//Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileTCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc [file];
				if (f.type is typesys.Type)
				{
					//Console.WriteLine("Type Content parsing for: {0}", ((typesys.Type)f.type).fullName());
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
				KeyValuePair<string, parsing.File> v = vpairs.ToArray() [i];
				//Console.WriteLine("Code parsing for: {0}", ((typesys.Type)v.Value.type).fullName());
				parser.parseFileCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.File f = filesassoc [file];
				if (f.type is typesys.Type)
				{
					//Console.WriteLine("Code parsing for: {0}", ((typesys.Type)f.type).fullName());
					parser.parseFileCP(f, file);
				}
			}

			//typesys.GenericType arrayt = filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).Single();
			//arrayt.getTypeFor(new[] { typesys.Primitive.BOOLEAN });

			//Console.WriteLine("Generating code");

			//IMangler mangler = new CMangler();
			//ICodeGen codegen = new CCodeGen(mangler);

			IMangler mangler = new CMangler();
			ICodeGen codegen = new generation.c.CCodeGen(mangler);

			IEnumerable<typesys.Type> types = filesassoc.Values.Select(x => x.type as typesys.Type).Where(x => x != null).Concat(
				filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).SelectMany(x => x.types.Values));

			//Console.WriteLine("Generating lambdas");
			codegen.generateGeneralBit(types);

			foreach (typesys.Type type in types)
			{
				//Console.WriteLine("Generating code for: {0}", type.fullName());
				codegen.generateFor(type, types);
			}

			codegen.finish(types);

			sw.Stop();

			//Console.WriteLine("Done generating code");
			//Console.WriteLine("Compilation of {0} files took: {1} milliseconds", files.Length, sw.Elapsed.TotalMilliseconds);

			StringBuilder script = new StringBuilder();
			script.AppendLine("#!/bin/sh");

			string location = System.Reflection.Assembly.GetEntryAssembly().Location;
			location = location.Substring(0, location.LastIndexOf(System.IO.Path.DirectorySeparatorChar));
			script.Append("gcc -w -g -DPF_DESKTOP64 -DPF_LINUX boh_internal.c function_types.c ");
#if DEBUG
			script.Append("stdlib/file.c ");
#endif
			foreach (typesys.Type type in types)
			{
				script.Append(mangler.getCodeFileName(type));
				script.Append(" ");
			}
			script.Append("-lgc -pthread -std=c11 -o ");
			script.Append(Options.outputDir + System.IO.Path.DirectorySeparatorChar + Options.output);
			script.AppendLine();
			System.IO.File.WriteAllText(Options.outputDir + System.IO.Path.DirectorySeparatorChar + "make.sh", script.ToString());

			//Console.ReadKey();
		}
	}
}
