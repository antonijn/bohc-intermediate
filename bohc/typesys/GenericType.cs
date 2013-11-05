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

using bohc.parsing.ts;

namespace bohc.typesys
{
	public class GenericType : IType
	{
		public static readonly List<GenericType> allGenTypes = new List<GenericType>();

		public readonly string[] genTypeNames;
		public readonly string name;

		public GenericType(string[] genTypeNames, string name)
		{
			this.genTypeNames = genTypeNames;
			this.name = name;

			lock (allGenTypes)
			{
				allGenTypes.Add(this);
			}
		}

		File IType.getFile()
		{
			return file;
		}

		void IType.setFile(File f)
		{
			file = f;
		}

		public File file;

		public readonly Dictionary<int, typesys.Type> types = new Dictionary<int, typesys.Type>();

		public static int getArrHash<T>(T[] arr)
		{
			int result = 666;
			foreach (T item in arr)
			{
				result ^= item.GetHashCode() * result;
			}
			return result;
		}

		public typesys.Type getTypeFor(typesys.Type[] what)
		{
			lock (types)
			{
				int hash = getArrHash(what);
				if (types.ContainsKey(hash))
				{
					return types[hash];
				}

				typesys.Type newType = getNewTypeFor(what);
				types[hash] = newType;
				return newType;
			}
		}

		protected Type getNewTypeFor(Type[] what)
		{
			// TODO: PROPER REPLACING FFS!!!

			string code = Parser.remDupW(file.content).Replace(" ,", ",").Replace(", ", ",");
			for (int i = 0; i < what.Length; ++i)
			{
				string gtname = genTypeNames[i];
				Type w = what[i];

				code = code.Replace(gtname, w.name);
			}

			StringBuilder replaceWhat = new StringBuilder();
			replaceWhat.Append(name);
			replaceWhat.Append("<");
			foreach (Type t in what)
			{
				replaceWhat.Append(t.name);
				replaceWhat.Append(",");
			}
			replaceWhat.Remove(replaceWhat.Length - 1, 1);
			replaceWhat.Append(">");

			StringBuilder byWhat = new StringBuilder();
			byWhat.Append(name);
			byWhat.Append("_");
			foreach (Type t in what)
			{
				byWhat.Append(t.fullName().Replace(".", "_"));
				byWhat.Append("_");
			}
			byWhat.Remove(byWhat.Length - 1, 1);

			code = code.Replace(replaceWhat.ToString(), byWhat.ToString());

			//code = System.Text.RegularExpressions.Regex.Replace(code, ">[\\ \n\r]*{", "{");

			parsing.ts.File newf = Parser.parseFileTS(code);
			Parser.parseFileTP(newf, code);
			Parser.parseFileTCS(newf, code);
			Parser.parseFileTCP(newf, code);
			Parser.parseFileCP(newf, code);

			newf.type.setFile(newf);

			try
			{
				return (Type)newf.type;
			}
			catch
			{
				StringBuilder typestrrep = new StringBuilder();
				foreach (typesys.Type t in what)
				{
					typestrrep.Append(t.fullName());
					typestrrep.Append(", ");
				}
				typestrrep.Remove(typestrrep.Length - 2, 2);
				boh.Exception._throw<exceptions.CodeGenException>(typestrrep.ToString() + " are not valid types for " + name);
				return null;
			}
		}
	}
}
