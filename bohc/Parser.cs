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


		#region Type Skimming (TS)

		public static File parseFileTS(string file)
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
				boh.Exception.require<ParserException>(parts.Length == 2, "Invalid file header");

				string kw = parts[0];
				string pack = parts[1];
				switch (kw)
				{
					case "package":
						boh.Exception.require<ParserException>(package == Package.GLOBAL, "Cannot declare multiple package directives");
						package = Package.getFromString(pack);
						break;
					case "import":
						imports.Add(Package.getFromString(pack));
						break;
					default:
						boh.Exception._throw<ParserException>(kw + " invalid directive");
						break;
				}
			}

			return new File(imports, package);
		}

		private static void parseTypeStart(File file, string typedec)
		{
			typedec = typedec.Trim();
			typedec = remDupW(typedec);

			string[] parts = typedec.Split(' ');
			int idxExtImpl = parts.Length;
			for (int i = parts.Length - 1; i >= 0; --i)
			{
				string part = parts[i];
				if (part == "extends" || part == "implements")
				{
					idxExtImpl = i;
				}
			}
			string name = parts[idxExtImpl - 1];
			string type = parts[idxExtImpl - 2];

			Modifiers mod = Modifiers.NONE;

			if (idxExtImpl > 2)
			{
				IEnumerable<string> mods = parts.Take(idxExtImpl - 2);
				mod = ModifierHelper.getModifiersFromStrings(mods);
				boh.Exception.require<ParserException>(ModifierHelper.areModifiersLegalForType(mod), "Illegal modifier for type");
			}

			switch (type)
			{
				case "class":
					file.type = Class.get(file.package, mod, name);
					break;
				case "enum":
					file.type = typesys.Enum.get(file.package, mod, name);
					break;
				case "interface":
					file.type = Interface.get(file.package, mod, name);
					break;
				default:
					boh.Exception._throw<ParserException>("Invalid start of type detected");
					break;
			}

			file.type.file = file;
		}

		#endregion
		
		#region Type Parsing (TP)

		public static void parseFileTP(File f, string file)
		{
			int openCurly = file.IndexOf('{');
			string beforeTypeBody = file.Substring(0, openCurly - 1);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string typedec = remDupW(beforeTypeBody.Substring(lastSemicol + 1)).Trim();

			if (f.type is Class)
			{
				Class c = f.type as Class;
				c.super = parseExt(f, typedec);
				boh.Exception.require<ParserException>(c.super != c, "Type cannot inherit itself");

				foreach (Interface i in parseImpl(f, typedec))
				{
					c.implement(i);
				}
			}
			else if (f.type is Interface)
			{
				Interface i = f.type as Interface;
				i.implements = parseImpl(f, typedec).ToList();
			}
		}

		private static Class parseExt(File file, string typedec)
		{
			string[] parts = typedec.Split(' ');
			int idxExt = Array.IndexOf(parts, "extends");

			if (idxExt != -1)
			{
				string super = parts[idxExt + 1];
				Class superClass = typesys.Type.getExisting(file.getContext(), super) as Class;
				boh.Exception.require<ParserException>(superClass != null, "Type was not a class: " + super);
				return superClass;
			}

			// TODO: should be default super class (boh.lang.Object)
			return null;
		}

		private static IEnumerable<Interface> parseImpl(File file, string typedec)
		{
			string[] parts = typedec.Split(' ');
			int idxIpml = Array.IndexOf(parts, "implements");

			if (idxIpml != -1)
			{
				IEnumerable<Package> fileContext = file.getContext();

				for (int i = idxIpml + 1; i < parts.Length; ++i)
				{
					string ifacestr = parts[i].Replace(",", string.Empty);
					Interface iface = typesys.Type.getExisting(fileContext, ifacestr) as Interface;
					boh.Exception.require<ParserException>(iface != null, "Type was not an interface: " + ifacestr);
					yield return iface;
				}
			}
		}

		#endregion
	}
}
