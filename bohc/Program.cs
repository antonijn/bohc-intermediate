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
				"src/Main.boh",
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

			pstate = ParserState.TS;
			foreach (string file in files)
			{
				filesassoc[file] = Parser.parseFileTS(file);
			}

			pstate = ParserState.TP;
			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Parser.parseFileTP(f, file);
				}
			}

			pstate = ParserState.TCS;
			KeyValuePair<string, parsing.ts.File>[] vpairs = genTypes.ToArray();
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.ts.File> v = vpairs.ToArray()[i];
				Parser.parseFileTCS(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Parser.parseFileTCS(f, file);
				}
			}

			pstate = ParserState.TCP;
			for (int i = 0; i < genTypes.Count; ++i)
			{
				if (vpairs.Length != genTypes.Count)
				{
					vpairs = genTypes.ToArray();
				}
				KeyValuePair<string, parsing.ts.File> v = vpairs.ToArray()[i];
				Parser.parseFileTCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
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
				Parser.parseFileCP(v.Value, v.Key);
			}

			foreach (string file in files)
			{
				parsing.ts.File f = filesassoc[file];
				if (f.type is typesys.Type)
				{
					Parser.parseFileCP(f, file);
				}
			}

			//typesys.GenericType arrayt = filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).Single();
			//arrayt.getTypeFor(new[] { typesys.Primitive.BOOLEAN });

			IEnumerable<typesys.Type> types = filesassoc.Values.Select(x => x.type as typesys.Type).Where(x => x != null).Concat(
				filesassoc.Values.Select(x => x.type as typesys.GenericType).Where(x => x != null).SelectMany(x => x.types.Values));

			CodeGen.generateGeneralBit(types);

			foreach (typesys.Type type in types)
			{
				CodeGen.generateFor(type, types);
			}

			Console.ReadKey();
		}
	}
}
