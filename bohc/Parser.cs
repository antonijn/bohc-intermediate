using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using bohc.boh;
using bohc.exceptions;
using bohc.typesys;
using bohc.parsing.ts;

namespace bohc
{
	public static class Parser
	{
		private static int getMatchingBraceCh(string str, int first, char matches, int step)
		{
			char startscope = str[first];
			int scope = 0;
			int i;
			for (i = first + 1; scope != -1; i += step)
			{
				char ch = str[i];
				if (ch == startscope)
				{
					++scope;
				}
				else if (ch == matches)
				{
					--scope;
				}
			}

			return i;
		}

		public static int getMatchingBraceChar(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, 1);
		}

		public static int getMatchingBraceCharBackwards(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, -1);
		}

		/// <summary>
		/// Removes duplicate whitespace in string
		/// </summary>
		public static string remDupW(string str)
		{
			return Regex.Replace(str, "\\s+", " ");
		}


		#region Type Skimming

		public static File parseFile(string file)
		{
			int openCurly = file.IndexOf('{');
			string beforeTypeBody = file.Substring(0, openCurly - 1);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string header = lastSemicol != -1 ? beforeTypeBody.Substring(0, lastSemicol) : string.Empty;

			File f = parseCommonFileHeader(header);

			string typedec = beforeTypeBody.Substring(lastSemicol + 1);
			parseTypeStart(f, typedec);

			return f;
		}

		private static File parseCommonFileHeader(string header)
		{
			IEnumerable<string> headerParts = header.Split(';').Select(x => x.Trim());
			return parseHeaderParts(headerParts);
		}

		private static File parseHeaderParts(IEnumerable<string> headerParts)
		{
			// the file header contains the package directive
			// and the import directives
			// either of those are optional

			List<Package> imports = new List<Package>();
			Package package = Package.GLOBAL;

			headerParts = headerParts.Select(x => remDupW(x));
			foreach (string headerPart in headerParts)
			{
				string[] parts = headerPart.Split(' ');
				boh.Exception.require<exceptions.ParserException>(parts.Length == 2, "Invalid file header");

				string kw = parts[0];
				string pack = parts[1];
				switch (kw)
				{
					case "package":
						boh.Exception.require<exceptions.ParserException>(package == Package.GLOBAL, "Cannot declare multiple package directives");
						package = Package.getFromString(pack);
						break;
					case "import":
						imports.Add(Package.getFromString(pack));
						break;
					default:
						boh.Exception._throw<exceptions.ParserException>(kw + " invalid directive");
						break;
				}
			}

			return new File(imports, package);
		}

		private static void parseTypeStart(File file, string typedec)
		{
			typedec = typedec.Trim();
			typedec = typedec.Substring(0, typedec.Length).TrimEnd();
			typedec = remDupW(typedec);

			string[] parts = typedec.Split(' ');
			string name = parts[parts.Length - 1];
			string type = parts[parts.Length - 2];

			Modifiers mod = Modifiers.NONE;

			if (parts.Length > 2)
			{
				IEnumerable<string> mods = parts.Take(parts.Length - 2);
				mod = ModifierHelper.getModifiersFromStrings(mods);
			}

			switch (type)
			{
				case "class":
					file.type = Class.get(file.package, mod, name);
					break;
				case "enum":
					throw new NotImplementedException();
				case "interface":
					file.type = Interface.get(file.package, mod, name);
					break;
				default:
					boh.Exception._throw<exceptions.ParserException>("Invalid start of type detected");
					break;
			}
		}

		#endregion
	}
}
