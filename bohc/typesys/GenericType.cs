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

		private static int getArrHash<T>(T[] arr)
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

			string code = file.content;			
			for (int i = 0; i < what.Length; ++i)
			{
				string gtname = genTypeNames[i];
				Type w = what[i];

				code = code.Replace(gtname, w.name);
			}

			code = code.Replace(name + "<", name + "_");
			//code = System.Text.RegularExpressions.Regex.Replace(code, ">[\\ \n\r]*{", "{");
			string b4type = code.Substring(0, code.IndexOf('{'));
			string repl = b4type.Replace("> implements", " implements");
			if (b4type == repl)
			{
				repl = b4type.Replace("> extends", " extends");
				if (b4type == repl)
				{
					repl = b4type.Replace(">", string.Empty);
				}
			}
			code = code.Replace(b4type, repl);

			parsing.ts.File newf = Parser.parseFileTS(code);
			Parser.parseFileTP(newf, code);
			Parser.parseFileTCS(newf, code);
			Parser.parseFileTCP(newf, code);
			Parser.parseFileCP(newf, code);

			newf.type.setFile(newf);

			return (Type)newf.type;
		}
	}
}
