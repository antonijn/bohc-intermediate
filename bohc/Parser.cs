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
using System.Text.RegularExpressions;

using bohc.boh;
using bohc.exceptions;
using bohc.typesys;
using bohc.parsing;
using bohc.parsing.statements;

namespace bohc
{
	public class Parser
	{
		public readonly IStatementParser statements;

		public Parser(IStatementParser statements)
		{
			this.statements = statements;
			this.statements.init(this);
		}

		#region Type Skimming (TS)

		public File parseFileTS(ref string file)
		{
			file = ParserTools.removeComments(file);

			int openCurly = file.IndexOf('{');
			string beforeTypeBody = file.Substring(0, openCurly);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string header = lastSemicol != -1 ? beforeTypeBody.Substring(0, lastSemicol) : string.Empty;

			File f = parseCommonFileHeader(header, file);

			string typedec = beforeTypeBody.Substring(lastSemicol + 1);
			parseTypeStart(f, typedec);

			return f;
		}

		private File parseCommonFileHeader(string header, string content)
		{
			IEnumerable<string> headerParts = header.Split(';').Select(x => x.Trim());
			return parseHeaderParts(headerParts, content);
		}

		private File parseHeaderParts(IEnumerable<string> headerParts, string content)
		{
			// the file header contains the package directive
			// and the import directives
			// either of those are optional

			List<Package> imports = new List<Package>();
			Package package = Package.GLOBAL;

			headerParts = headerParts.Select(x => ParserTools.remDupW(x));
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

		private void parseTypeStart(File file, string typedec)
		{
			typedec = typedec.Trim();
			typedec = ParserTools.remDupW(typedec);

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
				string[] types = ParserTools.split(name, f, '>', ',').ToArray();
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

		public void parseFileTP(File f, string file)
		{
			int openCurly = file.IndexOf('{');
			string beforeTypeBody = file.Substring(0, openCurly - 1);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string typedec = ParserTools.remDupW(beforeTypeBody.Substring(lastSemicol + 1)).Trim();

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

		private Class parseExt(File file, string typedec)
		{
			string[] parts = typedec.Split(' ');
			int idxExt = Array.IndexOf(parts, "extends");

			if (idxExt != -1)
			{
				string super = parts[idxExt + 1];
				Class superClass = typesys.Type.getExisting(file.getContext(), super, this) as Class;
				boh.Exception.require<ParserException>(superClass != null, "Type was not a class: " + super);
				return superClass;
			}

			// TODO: should be default super class (boh.std.Object)
			return null;
		}

		private IEnumerable<Interface> parseImpl(File file, string typedec)
		{
			string[] parts = typedec.Split(' ');
			int idxIpml = Array.IndexOf(parts, "implements");

			if (idxIpml != -1)
			{
				IEnumerable<Package> fileContext = file.getContext();

				for (int i = idxIpml + 1; i < parts.Length; ++i)
				{
					string ifacestr = parts[i].Replace(",", string.Empty);
					Interface iface = typesys.Type.getExisting(fileContext, ifacestr, this) as Interface;
					boh.Exception.require<ParserException>(iface != null, "Type was not an interface: " + ifacestr);
					yield return iface;
				}
			}
		}

		#endregion

		#region Type Content Skimming (TCS)

		public void parseFileTCS(File f, string file)
		{
			int typeStartCurly = file.IndexOf('{');
			int typeStopCurly = file.LastIndexOf('}');

			string content = file.Substring(typeStartCurly + 1, typeStopCurly - typeStartCurly - 1);

			if (f.type is Class)
			{
				Class c = (Class)f.type;
				parseClassTCS(f, content);
				if (c.constructors.Count == 0 || c is Struct)
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

		private void parseClassTCS(File f, string content)
		{
			int idxSemicol = content.IndexOf(';');
			int idxCurly = content.IndexOf('{');

			// TODO: take the following special cases into account:
			// public void(int, double) field = (x, y) => { };

			if ((idxSemicol > idxCurly || idxSemicol == -1) && idxCurly != -1)
			{
				// function
				int idxClose = ParserTools.getMatchingBraceChar(content, idxCurly, '}');
				string body = content.Substring(idxCurly + 1, idxClose - idxCurly - 1);

				((Class)f.type).addMember(parseFunctionTCS(f, content.Substring(0, idxCurly), body, true));

				int closingCurly = ParserTools.getMatchingBraceChar(content, idxCurly, '}');
				string after = content.Substring(closingCurly + 1);
				parseClassTCS(f, after);
			}
			else if ((idxCurly > idxSemicol || idxCurly == -1) && idxSemicol != -1)
			{
				// field
				string b4semi = content.Substring(0, idxSemicol);

				if (b4semi.Contains(" abstract ") || b4semi.Contains(" native "))
				{
					if (b4semi.Contains(" abstract "))
					{
						boh.Exception.require<ParserException>(((Class)f.type).modifiers.HasFlag(Modifiers.ABSTRACT), "Abstract functions require the surrounding class to be abstract too");
					}

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

		private void parseInterfaceTCS(File f, string content)
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

		private IFunction parseFunctionTCS(File f, string fDec, string body, bool requiresAccess)
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
					mods.HasFlag(Modifiers.VIRTUAL) || mods.HasFlag(Modifiers.NATIVE)), "Invalid modifier for constructor");

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

		private void parsePreFunctionParamsTCS(File f, string fDec, bool requiresAccess, out Modifiers mods, out typesys.Type type, out string identifier)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = ParserTools.getMatchingBraceCharBackwards(fDec, idxClose, '(');
			fDec = ParserTools.remDupW(fDec.Substring(0, idxParent).Trim());

			string[] parts = fDec.Split(' ');
			bool isStaticConstr = parts.Last() == "static";
			bool isConstr = parts.Last() == "this" || isStaticConstr;

			boh.Exception.require<ParserException>(
				isStaticConstr || (isConstr && parts.Length >= 2) || parts.Length >= 3 || (!requiresAccess && parts.Length >= 2),
				fDec + ": function access modifier and/or type expected");

			identifier = parts[parts.Length - 1];
			string typeName = isConstr ? null : parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - (isConstr ? 1 : 2));

			type = isConstr ? null : typesys.Type.getExisting(f.getContext(), typeName, this);
			mods = isStaticConstr ? Modifiers.NONE : ModifierHelper.getModifiersFromStrings(modifiers);
		}

		private void parseFunctionParamsTCS(File f, string fDec, Function func)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = ParserTools.getMatchingBraceCharBackwards(fDec, idxClose, '(');

			string paramString = ParserTools.remDupW(fDec.Substring(idxParent + 1, idxClose - idxParent - 1).Trim());

			if (string.IsNullOrEmpty(paramString))
			{
				return;
			}

			// TODO: consider special cases:
			// public void function(void() f = () => otherFunc(1, 2))

			string[] parts = ParserTools.split(paramString, new[] { '<', '(' }, new[] { '>', ')' }, ',').ToArray();
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

		public void parseParam(File f, string part, out string identifier, out Modifiers mods, out typesys.Type type)
		{
			string[] paramParts = part.Split(' ');
			boh.Exception.require<ParserException>(paramParts.Length >= 2, "Parameter type expected");
			boh.Exception.require<ParserException>(paramParts.Length <= 3, "Parameters may only have one modifier");

			identifier = paramParts[paramParts.Length - 1];
			string typeName = paramParts[paramParts.Length - 2];

			mods = Modifiers.NONE;
			if (paramParts.Length > 2)
			{
				boh.Exception.require<ParserException>(
					paramParts.First() == "final" || paramParts.First() == "ref", "'final' and 'ref' are the only legal modifiers for parameters");
				mods = ModifierHelper.getModifierFromString(paramParts.First());
			}

			type = typesys.Type.getExisting(f.getContext(), typeName, this);
		}

		private void parseFieldTCS(File f, string fDec)
		{
			fDec = fDec.Trim();

			string initvalstr = null;
			int equals = fDec.IndexOf('=');
			if (equals != -1)
			{
				initvalstr = fDec.Substring(equals + 1);
				fDec = fDec.Substring(0, equals).TrimEnd();
			}

			fDec = ParserTools.remDupW(fDec);

			string[] parts = fDec.Split(' ');
			boh.Exception.require<ParserException>(parts.Length >= 3, fDec + ": field access modifier and/or type expected");

			string identifier = parts[parts.Length - 1];
			string type = parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - 2);

			typesys.Type actualType = typesys.Type.getExisting(f.getContext(), type, this);
			Modifiers mods = ModifierHelper.getModifiersFromStrings(modifiers);

			boh.Exception.require<ParserException>(typesys.Type.isValidIdentifier(identifier), identifier + " is not a valid identifier");
			boh.Exception.require<ParserException>(ModifierHelper.areModifiersLegal(mods, true), mods.ToString() + ": invalid modifiers");

			Field field = new Field(mods, identifier, actualType, (Class)f.type, initvalstr);
			((Class)f.type).addMember(field);
		}

		private void parseEnumTCS(File f, string content)
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

				((typesys.Enum)f.type).enumerators.Add(new Enumerator(enumerator, (typesys.Enum)f.type));
			}
		}

		#endregion

		#region Type Content Parsing

		public void parseFileTCP(File f, string file)
		{
			Class c = f.type as Class;
			if (c != null)
			{
				parseClassTCP(c);
			}
		}

		private void parseClassTCP(Class c)
		{
			foreach (Field f in c.fields)
			{
				if (f.initvalstr != null)
				{
					f.initial = statements.getExpressions().analyze(f.initvalstr, new List<Variable>(), c.file);
				}
			}
		}

		#endregion

		#region Code Parsing

		public void parseFileCP(File f, string file)
		{
			Class c = f.type as Class;
			if (c == null)
			{
				return;
			}

			foreach (Function func in c.functions)
			{
				if (!func.modifiers.HasFlag(Modifiers.ABSTRACT) && !func.modifiers.HasFlag(Modifiers.NATIVE))
				{
					func.body = statements.parseBody(func.bodystr, func, f);
					if (func.returnType != Primitive.VOID && func.returnType != null)
					{
						boh.Exception.require<ParserException>(func.body.hasReturned(), "Function must return a value");
					}
					else if (func is Constructor)
					{
						boh.Exception.require<ParserException>(
							c.super == null ||
							c.super.constructors.Any(x => !(x.modifiers.HasFlag(Modifiers.PRIVATE) || x.modifiers.HasFlag(Modifiers.CVISIBLE)) && x.parameters.Count == 0) ||
							func.body.hasSuperBeenCalled(),
							"Constructor must call super constructor");
					}
				}
			}
		}

		#endregion
	}
}
