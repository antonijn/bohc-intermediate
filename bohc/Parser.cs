﻿// Copyright (c) 2013 Antonie Blom
// The antonijn open-source license, draft 1, short form.
// This source file is licensed under the antonijn open-source license, a
// full version of which is included with the project.
// Please refer to the long version for a list of rights and restrictions
// pertaining to source file use and modification.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using bohc.boh;
using bohc.exceptions;
using bohc.typesys;
using bohc.parsing;
using bohc.parsing.ts;
using bohc.parsing.statements;

namespace bohc
{
	public static class Parser
	{
		public static int indexOf(string str, char[] begin, char[] end, char idxOf)
		{
			bool instring = false;

			int scope = 0;
			int i = 0;
			for (; true; ++i)
			{
				if (i >= str.Length)
				{
					return -1;
				}

				char ch = str[i];
				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

				if (begin.Contains(ch))
				{
					++scope;
				}
				else if (end.Contains(ch))
				{
					--scope;
				}

				if (ch == idxOf && scope <= 0)
				{
					break;
				}
			}

			return i;
		}

		public static int indexOf(string str, char begin, char end, char idxOf)
		{
			return indexOf(str, new[] { begin }, new[] { end }, idxOf);
		}

		private static int getMatchingBraceCh(string str, int first, char matches, int step)
		{
			bool instring = false;
			char startscope = str[first];
			int scope = 0;
			int i;
			for (i = first + step; scope != -1; i += step)
			{
				char ch = str[i];
				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

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

		public static IEnumerable<string> split(string str, char[] begin, char[] end, char seperator)
		{
			bool instring = false;
			int scope = 0;

			int temp = 0;
			for (int i = 0; scope >= 0 && i < str.Length; ++i)
			{
				char ch = str[i];

				if (ch == '"')
				{
					instring = !instring;
				}

				if (instring)
				{
					continue;
				}

				if (begin.Contains(ch))
				{
					++scope;
				}
				else if (end.Contains(ch))
				{
					--scope;
				}
				else if (scope == 0 && ch == seperator)
				{
					yield return str.Substring(temp, i - temp);
					temp = i + 1;
				}
			}

			yield return str.Substring(temp, str.Length - temp);
		}

		public static IEnumerable<string> split(string str, int first, char matches, char seperator)
		{
			char begin = str[first];
			int stop = getMatchingBraceChar(str, first, matches);
			return split(str.Substring(first + 1, stop - first - 1), new[] { begin }, new[] { matches }, seperator);
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
			string beforeTypeBody = file.Substring(0, openCurly);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string header = lastSemicol != -1 ? beforeTypeBody.Substring(0, lastSemicol) : string.Empty;

			File f = parseCommonFileHeader(header, file);

			string typedec = beforeTypeBody.Substring(lastSemicol + 1);
			parseTypeStart(f, typedec);

			return f;
		}

		private static File parseCommonFileHeader(string header, string content)
		{
			IEnumerable<string> headerParts = header.Split(';').Select(x => x.Trim());
			return parseHeaderParts(headerParts, content);
		}

		private static File parseHeaderParts(IEnumerable<string> headerParts, string content)
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

			return new File(imports, package, content);
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

			// TODO: better generics
			// consider the name string "GenType< T,  U>"

			if (name.Contains("<"))
			{
				// generic
				int f = name.IndexOf('<');
				int l = name.LastIndexOf('>');
				string genname = name.Substring(0, f);
				string[] types = split(name, f, '>', ',').ToArray();
				file.type = new GenericType(types, genname);
			}
			else
			{
				switch (type)
				{
					case "class":
						file.type = Class.get<Class>(file.package, mod, name);
						break;
					case "struct":
						file.type = Struct.get<Struct>(file.package, mod, name);
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

				if (c.super == null)
				{
					Class objClass = StdType.obj;
					if (c != objClass)
					{
						c.super = objClass;
					}
				}

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

			// TODO: should be default super class (boh.std.Object)
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
				Class c = (Class)f.type;
				parseClassTCS(f, content);
				if (c.constructors.Count == 0)
				{
					c.addMember(new Constructor(Modifiers.PUBLIC, c, new List<Parameter>(), string.Empty));
				}
				if (c.staticConstr == null)
				{
					c.addMember(new StaticConstructor(c, string.Empty));
				}
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

			if ((idxSemicol > idxCurly || idxSemicol == -1) && idxCurly != -1)
			{
				// function
				int idxClose = getMatchingBraceChar(content, idxCurly, '}');
				string body = content.Substring(idxCurly + 1, idxClose - idxCurly - 1);

				((Class)f.type).addMember(parseFunctionTCS(f, content.Substring(0, idxCurly), body, true));

				int closingCurly = getMatchingBraceChar(content, idxCurly, '}');
				string after = content.Substring(closingCurly + 1);
				parseClassTCS(f, after);
			}
			else if ((idxCurly > idxSemicol || idxCurly == -1) && idxSemicol != -1)
			{
				// field
				string b4semi = content.Substring(0, idxSemicol);

				if (b4semi.Contains(" abstract "))
				{
					boh.Exception.require<ParserException>(((Class)f.type).modifiers.HasFlag(Modifiers.ABSTRACT), "Abstract functions require the surrounding class to be abstract too");

					((Class)f.type).addMember(parseFunctionTCS(f, content.Substring(0, idxSemicol), null, true));

					string after = content.Substring(idxSemicol + 1);
					parseClassTCS(f, after);
				}
				else
				{
					parseFieldTCS(f, b4semi);

					string after = content.Substring(idxSemicol + 1);
					parseClassTCS(f, after);
				}
			}
		}

		private static void parseInterfaceTCS(File f, string content)
		{
			string func = string.Empty;
			while (true)
			{
				int semicol = content.IndexOf(';');
				if (semicol == -1)
				{
					return;
				}

				func = content.Substring(0, semicol);
				IFunction funcAct = parseFunctionTCS(f, func, null, false);
				boh.Exception.require<ParserException>(funcAct is Function, "Interfaces may not contain generic functions");
				((Interface)f.type).functions.Add((Function)funcAct);
				content = content.Substring(semicol + 1);
			}
		}

		private static IFunction parseFunctionTCS(File f, string fDec, string body, bool requiresAccess)
		{
			// make sure the static constructors are called
			parsing.BinaryOperation.ADD.GetType();
			parsing.UnaryOperation.DECREMENT_POST.GetType();

			fDec = fDec.Trim();

			Modifiers mods;
			typesys.Type type;
			string identifier;

			parsePreFunctionParamsTCS(f, fDec, requiresAccess, out mods, out type, out identifier);

			if (identifier == "this")
			{
				boh.Exception.require<ParserException>(
					!(mods.HasFlag(Modifiers.ABSTRACT) || mods.HasFlag(Modifiers.FINAL) ||
					mods.HasFlag(Modifiers.OVERRIDE) || mods.HasFlag(Modifiers.STATIC) ||
					mods.HasFlag(Modifiers.VIRTUAL)), "Invalid modifier for constructor");

				List<Parameter> parameters = new List<Parameter>();
				Constructor func = new Constructor(mods, (Class)f.type, parameters, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}
			else if (identifier == "static")
			{
				StaticConstructor func = new StaticConstructor((Class)f.type, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}
			else if (parsing.Operator.isOperator(identifier))
			{
				boh.Exception.require<ParserException>(
					mods.HasFlag(Modifiers.PUBLIC) && mods.HasFlag(Modifiers.STATIC), "Invalid modifier for constructor");

				List<Parameter> parameters = new List<Parameter>();
				OverloadedOperator func = new OverloadedOperator(
					(typesys.Class)type,
					parsing.Operator.getExisting(identifier, parsing.OperationType.DOESNT_MATTER),
					type, parameters, body);

				parseFunctionParamsTCS(f, fDec, func);

				boh.Exception.require<ParserException>(func.parameters.Any(x => x.type == f.type), "An overloaded operator must apply to the type on which it is overloaded");
				boh.Exception.require<ParserException>(func.optype == parsing.OperationType.BINARY ? parameters.Count == 2 : parameters.Count == 1,
					"Invalid parameter count for operator " + identifier);

				return func;
			}
			else
			{
				boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(identifier), identifier + " is not a valid identifier");

				List<Parameter> parameters = new List<Parameter>();
				Function func = new Function((typesys.Type)f.type, mods, type, identifier, parameters, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}

		}

		private static void parsePreFunctionParamsTCS(File f, string fDec, bool requiresAccess, out Modifiers mods, out typesys.Type type, out string identifier)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = getMatchingBraceCharBackwards(fDec, idxClose, '(');
			fDec = remDupW(fDec.Substring(0, idxParent).Trim());

			string[] parts = fDec.Split(' ');
			bool isStaticConstr = parts.Last() == "static";
			bool isConstr = parts.Last() == "this" || isStaticConstr;

			boh.Exception.require<ParserException>(
				isStaticConstr || (isConstr && parts.Length >= 2) || parts.Length >= 3 || (!requiresAccess && parts.Length >= 2),
				fDec + ": function access modifier and/or type expected");

			identifier = parts[parts.Length - 1];
			string typeName = isConstr ? null : parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - (isConstr ? 1 : 2));

			type = isConstr ? null : typesys.Type.getExisting(f.getContext(), typeName);
			mods = isStaticConstr ? Modifiers.NONE : ModifierHelper.getModifiersFromStrings(modifiers);
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

			string[] parts = split(paramString, new[] { '<', '(' }, new[] { '>', ')' }, ',').ToArray();
			for (int i = 0; i < parts.Length; ++i)
			{
				parts[i] = parts[i].Trim();
			}

			foreach (string part in parts)
			{
				string identifier;
				Modifiers mods;
				typesys.Type type;
				parseParam(f, part, out identifier, out mods, out type);

				Parameter param = new Parameter(func, mods, identifier, type);
				func.parameters.Add(param);
			}
		}

		public static void parseParam(File f, string part, out string identifier, out Modifiers mods, out typesys.Type type)
		{
			string[] paramParts = part.Split(' ');
			boh.Exception.require<ParserException>(paramParts.Length >= 2, "Parameter type expected");
			boh.Exception.require<ParserException>(paramParts.Length <= 3, "Parameters may only have one modifier");

			identifier = paramParts[paramParts.Length - 1];
			string typeName = paramParts[paramParts.Length - 2];

			mods = Modifiers.NONE;
			if (paramParts.Length > 2)
			{
				boh.Exception.require<ParserException>(paramParts.First() == "final", "'final' is the only legal modifier for parameters");
				mods = Modifiers.FINAL;
			}

			type = typesys.Type.getExisting(f.getContext(), typeName);
		}

		private static void parseFieldTCS(File f, string fDec)
		{
			fDec = fDec.Trim();

			string initvalstr = null;
			int equals = fDec.IndexOf('=');
			if (equals != -1)
			{
				initvalstr = fDec.Substring(equals + 1);
				fDec = fDec.Substring(0, equals).TrimEnd();
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

			Field field = new Field(mods, identifier, actualType, (Class)f.type, initvalstr);
			((Class)f.type).addMember(field);
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

		#region Type Content Parsing

		public static void parseFileTCP(File f, string file)
		{
			Class c = f.type as Class;
			if (c != null)
			{
				parseClassTCP(c);
			}
		}

		private static void parseClassTCP(Class c)
		{
			foreach (Field f in c.fields)
			{
				if (f.initvalstr != null)
				{
					f.initial = parsing.Expression.analyze(f.initvalstr, new List<Variable>(), c.file);
				}
			}
		}

		#endregion

		#region Code Parsing

		public static void parseFileCP(File f, string file)
		{
			Class c = f.type as Class;
			if (c == null)
			{
				return;
			}

			foreach (Function func in c.functions)
			{
				if (!func.modifiers.HasFlag(Modifiers.ABSTRACT))
				{
					func.body = parseBody(func.bodystr, func, f);
					if (func.returnType != Primitive.VOID && func.returnType != null)
					{
						boh.Exception.require<ParserException>(hasReturned(func.body), "Function must return a value");
					}
					else if (func is Constructor)
					{
						boh.Exception.require<ParserException>(
							c.super == null ||
							c.super.constructors.Any(x => !(x.modifiers.HasFlag(Modifiers.PRIVATE) || x.modifiers.HasFlag(Modifiers.CVISIBLE)) && x.parameters.Count == 0) ||
							hasSuperBeenCalled(func.body),
							"Constructor must call super constructor");
					}
				}
			}
		}

		public static bool hasSuperBeenCalled(Body body)
		{
			foreach (Statement s in body.statements)
			{
				ExpressionStatement expr = s as ExpressionStatement;
				if (expr != null)
				{
					FunctionCall f = expr.expression as FunctionCall;
					if (f != null)
					{
						if (f.refersto.identifier == "this")
						{
							return true;
						}
					}
				}

				IfStatement ifstat = s as IfStatement;
				if (ifstat != null)
				{
					if (ifstat.elsestat != null)
					{
						return hasSuperBeenCalled(ifstat.body) && hasSuperBeenCalled(ifstat.elsestat.body);
					}

					continue;
				}

				TryStatement trys = s as TryStatement;
				if (trys != null)
				{
					if (tryReturnsOrSuper(trys, hasSuperBeenCalled))
					{
						return true;
					}

					continue;
				}

				if (s is BreakStatement || s is ContinueStatement)
				{
					return false;
				}
			}

			return false;
		}

		public static bool hasReturned(Body body)
		{
			foreach (Statement s in body.statements)
			{
				if (s is ReturnStatement || s is ThrowStatement)
				{
					return true;
				}

				IfStatement ifstat = s as IfStatement;
				if (ifstat != null)
				{
					if (ifstat.elsestat != null)
					{
						return hasReturned(ifstat.body) && hasReturned(ifstat.elsestat.body);
					}

					continue;
				}

				TryStatement trys = s as TryStatement;
				if (trys != null)
				{
					if (tryReturnsOrSuper(trys, hasReturned))
					{
						return true;
					}

					continue;
				}
				
				if (s is BreakStatement || s is ContinueStatement)
				{
					return false;
				}
			}

			return false;
		}

		private static bool tryReturnsOrSuper(TryStatement trys, Func<Body, bool> bodyAct)
		{
			if (!bodyAct(trys.body))
			{
				return false;
			}

			foreach (CatchStatement cs in trys.catches)
			{
				if (!bodyAct(cs.body))
				{
					return false;
				}
			}

			return true;
		}

		public static Body parseBody(string body, Function func, File f)
		{
			Stack<List<Variable>> vars = new Stack<List<Variable>>();
			// TODO: remove this if it doesn't work?
			if (func != null)
			{
				vars.Push(func.parameters.ToList<typesys.Variable>());
			}
			return parseBody(body, func, vars, f);
		}

		public static Body parseBody(string body, Function func, Stack<List<Variable>> vars, File f)
		{
			Body result = new Body();
			vars.Push(new List<Variable>());

			while (true)
			{
				body = body.TrimStart();
				if (string.IsNullOrWhiteSpace(body))
				{
					break;
				}
				parseLine(ref body, func, vars, result, f);
			}

			vars.Pop();
			return result;
		}

		private static bool startsWithKW(string line, string word)
		{
			return line.StartsWith(word) && !char.IsLetterOrDigit(line[word.Length]);
		}

		private static void parseLine(ref string line, Function func, Stack<List<Variable>> vars, Body result, File f)
		{
			if (line.StartsWith("{"))
			{
				int closing = getMatchingBraceChar(line, 0, '}');
				string bod = line.Substring(1, closing - 1);
				result.statements.Add(new Scope(parseBody(bod, func, vars, f)));
				line = line.Substring(closing + 1).TrimStart();
			}
			else if (startsWithKW(line, "if"))
			{
				result.statements.Add(parseIf(ref line, func, vars, f));
			}
			else if (startsWithKW(line, "while"))
			{
				result.statements.Add(parseWhile(ref line, func, vars, f));
			}
			else if (startsWithKW(line, "for"))
			{
				result.statements.Add(parseFor(ref line, func, vars, f));
			}
			else if (startsWithKW(line, "do"))
			{
				result.statements.Add(parseDoWhile(ref line, func, vars, f));
			}
			else if (startsWithKW(line, "try"))
			{
				result.statements.Add(parseTry(ref line, func, vars, f));
			}
			else
			{
				// FIXME: void() f = void () -> { doSth(); };
				int semicol1 = indexOf(line, '(', ')', ';');
				int semicol2 = indexOf(line, '{', '}', ';');
				int semicol = Math.Max(semicol1, semicol2);
				string stat = line.Substring(0, semicol);
				result.statements.Add(parseStatement(stat, func, vars, f));
				line = line.Substring(semicol + 1).TrimStart();
			}
		}

		private static Statement parseStatement(string line, Function func, Stack<List<Variable>> vars, File f)
		{
			int wspace = Math.Max(indexOf(line, '<', '>', ' '), indexOf(line, '(', ')', ' '));
			if (wspace != -1)
			{
				string beforeWspace = line.Substring(0, wspace);
				if (typesys.Type.exists(f.getContext(), beforeWspace))
				{
					typesys.Type type = typesys.Type.getExisting(f.getContext(), beforeWspace);
					string id = null;

					Expression initial = null;

					int eq = indexOf(line, '(', ')', '=');
					if (eq != -1)
					{
						id = line.Substring(wspace + 1, eq - wspace - 1).Trim();
						boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(id), id + ": is not a valid identifier");
						string initstr = line.Substring(eq + 1);
						initial = Expression.analyze(initstr, vars.SelectMany(x => x), f);
					}
					else
					{
						id = line.Substring(wspace + 1).Trim();
					}
					Local variable = new Local(id, type);
					variable.lambdaLevel = Expression.lambdaStack;
					vars.Peek().Add(variable);

					return new VarDeclaration(variable, initial);
				}
			}

			if (startsWithKW(line, "return"))
			{
				return parseReturn(line, func, vars, f);
			}
			else if (startsWithKW(line, "throw"))
			{
				return parseThrow(line, func, vars, f);
			}
			else if (func is Constructor && line.Replace(" ", string.Empty).StartsWith("this("))
			{
				int i = line.IndexOf("(") + 1;
				IEnumerable<Expression> parameters;
				typesys.Function constr = Expression.getCompatibleFunction(ref i, "this", line, f, vars.SelectMany(x => x), ((Class)func.owner).constructors, out parameters, func);
				line = line.Substring(i);
				return new ExpressionStatement(new FunctionCall(constr, ((Class)func.owner).THISVAR, parameters));
			}
			else if (func is Constructor && line.StartsWith("super("))
			{
				int i = line.IndexOf("(") + 1;
				IEnumerable<Expression> parameters;
				typesys.Function constr = Expression.getCompatibleFunction(ref i, "this", line, f, vars.SelectMany(x => x), ((Class)func.owner).super.constructors, out parameters, func);
				line = line.Substring(i);
				return new ExpressionStatement(new FunctionCall(constr, ((Class)func.owner).THISVAR, parameters));
			}
			else if (startsWithKW(line, "break"))
			{
				// TODO: make sure that in loop
				return new BreakStatement();
			}
			else if (startsWithKW(line, "continue"))
			{
				// TODO: make sure that in loop
				return new ContinueStatement();
			}

			Expression expr = Expression.analyze(line, vars.SelectMany(x => x), f, func);
			if (expr != null)
			{
				return new ExpressionStatement(expr);
			}

			return new EmptyStatement();
		}

		private static ThrowStatement parseThrow(string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("throw ".Length);
			Expression thr = Expression.analyze(line, vars.SelectMany(x => x), f, func);
			boh.Exception.require<ParserException>(thr.getType().extends(typesys.Type.getExisting(StdType.boh_lang, "Exception")) != 0, "Expression after throw must be an exception");
			return new ThrowStatement(thr);
		}

		private static ReturnStatement parseReturn(string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("return".Length);
			Expression ret = Expression.analyze(line, vars.SelectMany(x => x), f, func);
			boh.Exception.require<ParserException>(ret.getType().extends(func.returnType) != 0, "Return statement incompatible with function return type");
			return new ReturnStatement(ret);
		}

		private static void parseStatBody(ref string line, Function func, Stack<List<Variable>> vars, File f, out Body body)
		{
			body = null;
			if (line.StartsWith("{"))
			{
				string curlies = getCurlies(line);
				body = parseBody(curlies, func, vars, f);
				line = line.TrimStart().Substring(curlies.Length + 2);
			}
			else
			{
				body = new Body();
				parseLine(ref line, func, vars, body, f);
			}
		}

		private static void parseIfLike(ref string line, Function func, Stack<List<Variable>> vars, File f, out Expression condition, out Body body)
		{
			boh.Exception.require<ParserException>(line.StartsWith("("), "Condition must be placed between parentheses");

			string condstr = getBrackets(line);
			condition = Expression.analyze(condstr, vars.SelectMany(x => x), f, func);
			line = line.Substring(condstr.Length + 2).TrimStart();

			parseStatBody(ref line, func, vars, f, out body);
		}

		private static TryStatement parseTry(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("try".Length).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(curlies, func, vars, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			List<CatchStatement> catches = new List<CatchStatement>();
			while (line.StartsWith("catch"))
			{
				catches.Add(parseCatch(ref line, func, vars, f));
			}

			FinallyStatement fin = null;
			if (line.StartsWith("finally"))
			{
				line = line.Substring("finally".Length).TrimStart();
				curlies = getCurlies(line);
				line = line.Substring(curlies.Length + 2).TrimStart();
				fin = new FinallyStatement(parseBody(curlies, func, f));
			}

			return new TryStatement(body, catches, fin);
		}

		private static CatchStatement parseCatch(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("catch".Length).TrimStart();

			string bracks = getBrackets(line);
			string[] parts = bracks.Split(' ');

			boh.Exception.require<ParserException>(parts.Length == 2, "Invalid parameter for catch block");

			string typestr = parts[0];
			string idstr = parts[1];

			typesys.Type type = typesys.Type.getExisting(f.getContext(), typestr);

			boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(idstr), idstr + " is not a valid identifier");

			Parameter param = new Parameter(func, Modifiers.NONE, idstr, type);
			vars.Push(new List<Variable>());
			vars.Peek().Add(param);

			line = line.Substring(bracks.Length + 2).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(curlies, func, vars, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			vars.Pop();

			return new CatchStatement(param, body);
		}

		private static IfStatement parseIf(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("if".Length).TrimStart();

			Expression condition;
			Body body;
			parseIfLike(ref line, func, vars, f, out condition, out body);

			line = line.TrimStart();
			if (line.StartsWith("else"))
			{
				return new IfStatement(condition, body, parseElse(ref line, func, vars, f));
			}

			return new IfStatement(condition, body, null);
		}

		private static ElseStatement parseElse(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("else".Length).TrimStart();
			Body body;
			parseStatBody(ref line, func, vars, f, out body);

			return new ElseStatement(body);
		}

		private static WhileStatement parseWhile(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("while".Length).TrimStart();

			Expression condition;
			Body body;
			parseIfLike(ref line, func, vars, f, out condition, out body);

			return new WhileStatement(condition, body);
		}

		private static ForStatement parseFor(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			vars.Push(new List<Variable>());

			line = line.Substring("for".Length).TrimStart();

			string betwBracks = "(" + getBrackets(line) + ")";
			IEnumerable<string> strs = split(betwBracks, 0, ')', ';');

			boh.Exception.require<ParserException>(strs.Count() == 3, "For loops require three parts between brackets");

			string[] strparts = strs.ToArray();

			Statement initial = parseStatement(strparts[0], func, vars, f);
			Expression condition = Expression.analyze(strparts[1], vars.SelectMany(x => x), f);
			Statement post = parseStatement(strparts[2], func, vars, f);

			line = line.Substring(betwBracks.Length).TrimStart();

			Body body;
			parseStatBody(ref line, func, vars, f, out body);

			vars.Pop();

			return new ForStatement(initial, condition, post, body);
		}

		private static DoWhileStatement parseDoWhile(ref string line, Function func, Stack<List<Variable>> vars, File f)
		{
			line = line.Substring("do".Length).TrimStart();

			string curlies = getCurlies(line);
			Body body = parseBody(getCurlies(line), func, f);
			line = line.Substring(curlies.Length + 2).TrimStart();

			if (line.StartsWith("while"))
			{
				line = line.Substring("while".Length).TrimStart();
				string pars = getBrackets(line);
				Expression condition = Expression.analyze(pars, vars.SelectMany(x => x), f, func);

				line = line.Substring(pars.Length + 2).TrimStart().Substring(1);

				return new DoWhileStatement(condition, body);
			}

			return new DoWhileStatement(new parsing.Literal(Primitive.BOOLEAN, "false"), body);
		}

		public static string getBrackets(string line)
		{
			int lbrack = line.IndexOf('(');
			int rbrack = getMatchingBraceChar(line, lbrack, ')');

			return line.Substring(lbrack + 1, rbrack - lbrack - 1);
		}

		private static string getCurlies(string line)
		{
			int lbrack = line.IndexOf('{');
			int rbrack = getMatchingBraceChar(line, lbrack, '}');

			return line.Substring(lbrack + 1, rbrack - lbrack - 1);
		}

		#endregion
	}
}
