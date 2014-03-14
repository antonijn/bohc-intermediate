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
using System.IO;

using Bohc.Parsing;

namespace Bohc.TypeSystem
{
	public class GenericType : IType
	{
		public static readonly List<GenericType> AllGenTypes = new List<GenericType>();
		public static readonly List<Type> TypeInstances = new List<Type>();

		public readonly string[] GenTypeNames;
		public string[] InitialTypeNames;
		public readonly string Name;

		public TypeSystem.Type[] InitialTypes;

		public GenericType(string[] genTypeNames, string name)
		{
			this.GenTypeNames = genTypeNames;
			this.Name = name;

			lock (AllGenTypes)
			{
				AllGenTypes.Add(this);
			}
		}

		Parsing.File IType.GetFile()
		{
			return File;
		}

		void IType.SetFile(Parsing.File f)
		{
			File = f;
		}

		public Parsing.File File;

		private readonly Dictionary<int, Bohc.TypeSystem.Type> Types = new Dictionary<int, Bohc.TypeSystem.Type>();

		public static int GetArrHash<T>(T[] arr)
		{
			int result = 666;
			foreach (T item in arr)
			{
				result ^= item.GetHashCode() * result;
			}
			return result;
		}

		public Bohc.TypeSystem.Type GetTypeFor(Bohc.TypeSystem.Type[] what, IFileParser parser)
		{
			lock (Types)
			{
				int hash = GetArrHash(what);
				if (Types.ContainsKey(hash))
				{
					return Types[hash];
				}

				Bohc.TypeSystem.Type newType = GetNewTypeFor(what, parser, hash);
				return newType;
			}
		}

		private Type GetNewTypeFor(Type[] what, IFileParser parser, int hash)
		{
			// TODO: PROPER REPLACING FFS!!!

			return parser.getNewType(this, what, x => Types[hash] = x, x => TypeInstances.Add(x));

			/*string code = ParserTools.remDupW((string)File.parserinfo).Replace(" ,", ",").Replace(", ", ",");
			for (int i = 0; i < what.Length; ++i)
			{
				string gtname = GenTypeNames[i];
				Type w = what[i];

				StringReader sr = new StringReader(code);
				Tokenizer tn = new Tokenizer(sr, File.filename);
				tn.lex().ToArray().SelectMany(x => x.value == );
				code = code.Replace();
			}

			StringBuilder replaceWhat = new StringBuilder();
			replaceWhat.Append(Name);
			replaceWhat.Append("<");
			foreach (Type t in what)
			{
				replaceWhat.Append(t.FullName());
				replaceWhat.Append(",");
			}
			replaceWhat.Remove(replaceWhat.Length - 1, 1);
			replaceWhat.Append(">");

			StringBuilder byWhat = new StringBuilder();
			byWhat.Append(Name);
			byWhat.Append("`0");
			foreach (Type t in what)
			{
				byWhat.Append("`1");
				byWhat.Append(t.FullName().Replace(".", "_"));
			}
			byWhat.Append("`2");

			//code = System.Text.RegularExpressions.Regex.Replace(code, ">[\\ \n\r]*{", "{");


			Parsing.File newf = parser.parseFileTS(ref code, File.filename);
			Types[hash] = (Type)newf.type;
			//parser.proj().pstrat.registerRtType(newf.type as typesys.Type);
			parser.parseFileTP(newf);
			if (parser.getStrat().getpstate() >= ParserState.TCS)
			{
				parser.parseFileTCS(newf);
			}
			if (parser.getStrat().getpstate() >= ParserState.TCP)
			{
				parser.parseFileTCP(newf);
			}
			if (parser.getStrat().getpstate() >= ParserState.CP)
			{
				parser.parseFileCP(newf);
			}

			newf.type.SetFile(newf);

			try
			{
				replaceWhat.Clear();
				replaceWhat.Append(Name);
				replaceWhat.Append("<");
				foreach (Type t in what)
				{
					replaceWhat.Append(t.ExternName());
					replaceWhat.Append(",");
				}
				replaceWhat.Remove(replaceWhat.Length - 1, 1);
				replaceWhat.Append(">");

				((Type)newf.type).OriginalGenType = this;
				((Type)newf.type).GenName = replaceWhat.ToString();

				TypeInstances.Add((Type)newf.type);

				return (Type)newf.type;
			}
			catch
			{
				StringBuilder typestrrep = new StringBuilder();
				foreach (Bohc.TypeSystem.Type t in what)
				{
					typestrrep.Append(t.FullName());
					typestrrep.Append(", ");
				}
				typestrrep.Remove(typestrrep.Length - 2, 2);
				Boh.Exception._throw<Exceptions.CodeGenException>(typestrrep.ToString() + " are not valid types for " + Name);
				return null;
			}*/
		}
	}
}
