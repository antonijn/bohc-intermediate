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
			for (i = first + step; scope != -1; i += step)
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

			return i - step;
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

			file.type.setFile(file);
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

		#region Type Content Skimming (TCS)

		public static void parseFileTCS(File f, string file)
		{
			int typeStartCurly = file.IndexOf('{');
			int typeStopCurly = file.LastIndexOf('}');

			string content = file.Substring(typeStartCurly + 1, typeStopCurly - typeStartCurly - 1);

			if (f.type is Class)
			{
				parseClassTCS(f, content);
			}
			else if (f.type is Interface)
			{
				parseInterfaceTCS(f, content);
			}
			else if (f.type is typesys.Enum)
			{
				parseEnumTCS(f, content);
			}
		}

		private static void parseClassTCS(File f, string content)
		{
			int idxSemicol = content.IndexOf(';');
			int idxCurly = content.IndexOf('{');

			// TODO: take the following special cases into account:
			// public void(int, double) field = (x, y) => { };
			// public void function(int x = 1337) {
			// public void function(void() f = () => { doSomething(); }) {

			if ((idxSemicol > idxCurly || idxSemicol == -1) && idxCurly != -1)
			{
				// function
				((Class)f.type).functions.Add(parseFunctionTCS(f, content.Substring(0, idxCurly), true));

				int closingCurly = getMatchingBraceChar(content, idxCurly, '}');
				string after = content.Substring(closingCurly + 1);
				parseClassTCS(f, after);
			}
			else if ((idxCurly > idxSemicol || idxCurly == -1) && idxSemicol != -1)
			{
				// field
				parseFieldTCS(f, content.Substring(0, idxSemicol));		

				string after = content.Substring(idxSemicol + 1);
				parseClassTCS(f, after);
			}
		}

		private static void parseInterfaceTCS(File f, string content)
		{
			int semicol = content.IndexOf(';');
			if (semicol == -1)
			{
				return;
			}

			string func = content.Substring(0, semicol);
			((Interface)f.type).functions.Add(parseFunctionTCS(f, func, false));
		}

		private static Function parseFunctionTCS(File f, string fDec, bool requiresAccess)
		{
			fDec = fDec.Trim();

			Modifiers mods;
			typesys.Type type;
			string identifier;

			parsePreFunctionParamsTCS(f, fDec, requiresAccess, out mods, out type, out identifier);

			List<Parameter> parameters = new List<Parameter>();
			Function func = new Function(mods, type, identifier, parameters);

			parseFunctionParamsTCS(f, fDec, func);

			return func;
		}

		private static void parsePreFunctionParamsTCS(File f, string fDec, bool requiresAccess, out Modifiers mods, out typesys.Type type, out string identifier)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = getMatchingBraceCharBackwards(fDec, idxClose, '(');
			fDec = remDupW(fDec.Substring(0, idxParent).Trim());

			string[] parts = fDec.Split(' ');
			boh.Exception.require<ParserException>(parts.Length >= 3 || (!requiresAccess && parts.Length >= 2), fDec + ": function access modifier and/or type expected");

			identifier = parts[parts.Length - 1];
			string typeName = parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - 2);

			boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(identifier), identifier + " is not a valid identifier");

			type = typesys.Type.getExisting(f.getContext(), typeName);
			mods = ModifierHelper.getModifiersFromStrings(modifiers);
		}

		private static void parseFunctionParamsTCS(File f, string fDec, Function func)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = getMatchingBraceCharBackwards(fDec, idxClose, '(');

			string paramString = remDupW(fDec.Substring(idxParent + 1, idxClose - idxParent - 1).Trim());

			if (string.IsNullOrEmpty(paramString))
			{
				return;
			}

			// TODO: consider special cases:
			// public void function(void() f = () => otherFunc(1, 2))

			string[] parts = paramString.Split(',');
			for (int i = 0; i < parts.Length; ++i)
			{
				parts[i] = parts[i].Trim();
			}

			foreach (string part in parts)
			{
				string[] paramParts = part.Split(' ');
				boh.Exception.require<ParserException>(paramParts.Length >= 2, "Parameter type expected");
				boh.Exception.require<ParserException>(paramParts.Length <= 3, "Parameters may only have one modifier");

				string identifier = paramParts[paramParts.Length - 1];
				string typeName = paramParts[paramParts.Length - 2];

				Modifiers mods = Modifiers.NONE;
				if (paramParts.Length > 2)
				{
					boh.Exception.require<ParserException>(paramParts.First() == "final", "'final' is the only legal modifier for parameters");
					mods = Modifiers.FINAL;
				}

				typesys.Type type = typesys.Type.getExisting(f.getContext(), typeName);

				Parameter param = new Parameter(func, mods, identifier, type);
				func.parameters.Add(param);
			}
		}

		private static void parseFieldTCS(File f, string fDec)
		{
			fDec = fDec.Trim();

			int equals = fDec.IndexOf('=');
			if (equals != -1)
			{
				fDec = fDec.Substring(0, equals - 1);
			}

			fDec = remDupW(fDec);

			string[] parts = fDec.Split(' ');
			boh.Exception.require<ParserException>(parts.Length >= 3, fDec + ": field access modifier and/or type expected");

			string identifier = parts[parts.Length - 1];
			string type = parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - 2);

			typesys.Type actualType = typesys.Type.getExisting(f.getContext(), type);
			Modifiers mods = ModifierHelper.getModifiersFromStrings(modifiers);

			boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(identifier), identifier + " is not a valid identifier");
			boh.Exception.require<ParserException>(ModifierHelper.areModifiersLegal(mods, true), mods.ToString() + ": invalid modifiers");

			Field field = new Field(mods, identifier, actualType);
			((Class)f.type).fields.Add(field);
		}

		private static void parseEnumTCS(File f, string content)
		{
			string[] enumerators = content.Split(',');
			for (int i = 0; i < enumerators.Length; ++i)
			{
				string enumerator = enumerators[i];
				int idxEq = enumerator.IndexOf('=');
				if (idxEq != -1)
				{
					enumerator = enumerator.Substring(0, idxEq);
				}
				enumerator = enumerator.Trim();

				if (i == enumerators.Length - 1 && string.IsNullOrEmpty(enumerator))
				{
					break;
				}

				boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(enumerator), enumerator + " is not a valid enumerator");
				boh.Exception.warnIf(enumerator.ToUpperInvariant() != enumerator, "Enumerator names should be uppercase");

				((typesys.Enum)f.type).enumerators.Add(new Enumerator(enumerator));
			}
		}

		#endregion
	}
}
