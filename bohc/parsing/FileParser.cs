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

using Bohc.Boh;
using Bohc.Exceptions;
using Bohc.TypeSystem;
using Bohc.Parsing;
using Bohc.Parsing.Statements;
using Bohc.General;

namespace Bohc.Parsing
{
	public class FileParser : IFileParser
	{
		public readonly IStatementParser Statements;
		public readonly Project input;

		public FileParser(IStatementParser Statements, Project input)
		{
			this.Statements = Statements;
			this.Statements.init(this);
			this.input = input;
		}

		public Project proj()
		{
			return input;
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

			f.state = ParserState.TS;

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
			Package package = Package.Global;

			headerParts = headerParts.Select(x => ParserTools.remDupW(x));
			foreach (string headerPart in headerParts)
			{
				string[] parts = headerPart.Split(' ');
				Boh.Exception.require<ParserException>(parts.Length == 2, "Invalid file header");

				string kw = parts[0];
				string pack = parts[1];
				switch (kw)
				{
					case "package":
						Boh.Exception.require<ParserException>(package == Package.Global, "Cannot declare multiple package directives");
						package = Package.GetFromString(pack);
						break;
					case "import":
						imports.Add(Package.GetFromString(pack));
						break;
					default:
						Boh.Exception._throw<ParserException>(kw + " invalid directive");
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

			Modifiers mod = Modifiers.None;

			if (idxExtImpl > 2)
			{
				IEnumerable<string> mods = parts.Take(idxExtImpl - 2);
				mod = ModifierHelper.GetModifiersFromStrings(mods);
				Boh.Exception.require<ParserException>(ModifierHelper.AreModifiersLegalForType(mod), "Illegal modifier for type");
			}

			// TODO: better generics
			// consider the name string "GenType< T,  U>"

			if (name.Contains("<"))
			{
				// generic
				int f = name.IndexOf('<');
				//int l = name.LastIndexOf('>');
				string genname = name.Substring(0, f);
				string[] types = ParserTools.split(name, f, '>', ',').ToArray();
				file.type = new GenericType(types, genname);
			}
			else
			{
				switch (type)
				{
					case "class":
						file.type = Class.Get<Class>(file.package, mod, name);
						break;
					case "struct":
						file.type = Struct.Get<Struct>(file.package, mod, name);
						break;
					case "enum":
						file.type = Bohc.TypeSystem.Enum.Get(file.package, mod, name);
						break;
					case "interface":
						file.type = Interface.Get(file.package, mod, name);
						break;
					default:
						Boh.Exception._throw<ParserException>("Invalid start of type detected");
						break;
				}
			}

			file.type.SetFile(file);
		}

		#endregion
		
		#region Type Parsing (TP)

		public void parseFileTP(File f)
		{
			string file = f.content;

			if (f.state >= Bohc.ParserState.TP)
			{
				return;
			}

			f.state = ParserState.TP;

			int openCurly = file.IndexOf('{');
			string beforeTypeBody = file.Substring(0, openCurly - 1);
			int lastSemicol = beforeTypeBody.LastIndexOf(';');
			string typedec = ParserTools.remDupW(beforeTypeBody.Substring(lastSemicol + 1)).Trim();

			if (f.type is Class)
			{
				Class c = f.type as Class;
				c.Super = parseExt(f, typedec);
				Boh.Exception.require<ParserException>(c.Super != c, "Type cannot inherit itself");

				if (c.Super == null)
				{
					Class objClass = StdType.Obj;
					if (c != objClass)
					{
						c.Super = objClass;
					}
				}

				foreach (Interface i in parseImpl(f, typedec))
				{
					c.Implement(i);
				}
			}
			else if (f.type is Interface)
			{
				Interface i = f.type as Interface;
				i.Implements = parseImpl(f, typedec).ToList();
			}
		}

		private Class parseExt(File file, string typedec)
		{
			string[] parts = typedec.Split(' ');
			int idxExt = Array.IndexOf(parts, "extends");

			if (idxExt != -1)
			{
				string super = parts[idxExt + 1];
				Class superClass = Bohc.TypeSystem.Type.GetExisting(file.getContext(), super, this) as Class;
				Boh.Exception.require<ParserException>(superClass != null, "Type was not a class: " + super);
				return superClass;
			}

			// TODO: should be default super class (Boh.std.Object)
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
					Interface iface = Bohc.TypeSystem.Type.GetExisting(fileContext, ifacestr, this) as Interface;
					Boh.Exception.require<ParserException>(iface != null, "Type was not an interface: " + ifacestr);
					yield return iface;
				}
			}
		}

		#endregion

		#region Type Content Skimming (TCS)

		public void parseFileTCS(File f)
		{
			string file = f.content;

			if (f.state >= ParserState.TCS)
			{
				return;
			}

			f.state = ParserState.TCS;

			int typeStartCurly = file.IndexOf('{');
			int typeStopCurly = file.LastIndexOf('}');

			string content = file.Substring(typeStartCurly + 1, typeStopCurly - typeStartCurly - 1);

			if (f.type is Class)
			{
				Class c = (Class)f.type;
				parseClassTCS(f, content);
				if (c.Constructors.Count == 0 || c is Struct)
				{
					c.AddMember(new Constructor(Modifiers.Public, c, new List<Parameter>(), string.Empty));
				}
				if (c.StaticConstr == null)
				{
					c.AddMember(new StaticConstructor(c, string.Empty));
				}
			}
			else if (f.type is Interface)
			{
				parseInterfaceTCS(f, content);
			}
			else if (f.type is Bohc.TypeSystem.Enum)
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

				((Class)f.type).AddMember(parseFunctionTCS(f, content.Substring(0, idxCurly), body, true));

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
						Boh.Exception.require<ParserException>(((Class)f.type).Modifiers.HasFlag(Modifiers.Abstract), "Abstract functions require the surrounding class to be abstract too");
					}

					((Class)f.type).AddMember(parseFunctionTCS(f, content.Substring(0, idxSemicol), null, true));

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
				Boh.Exception.require<ParserException>(funcAct is Function, "Interfaces may not contain generic functions");
				((Interface)f.type).Functions.Add((Function)funcAct);
				content = content.Substring(semicol + 1);
			}
		}

		private IFunction parseFunctionTCS(File f, string fDec, string body, bool requiresAccess)
		{
			// make sure the static constructors are called
			Bohc.Parsing.BinaryOperation.ADD.GetType();
			Bohc.Parsing.UnaryOperation.DECREMENT_POST.GetType();

			fDec = fDec.Trim();

			Modifiers mods;
			Bohc.TypeSystem.Type type;
			string identifier;

			parsePreFunctionParamsTCS(f, fDec, requiresAccess, out mods, out type, out identifier);

			if (identifier == "this")
			{
				// constructor
				Boh.Exception.require<ParserException>(
					!(mods.HasFlag(Modifiers.Abstract) || mods.HasFlag(Modifiers.Final) ||
					mods.HasFlag(Modifiers.Override) || mods.HasFlag(Modifiers.Static) ||
					mods.HasFlag(Modifiers.Virtual) || mods.HasFlag(Modifiers.Native)), "Invalid modifier for constructor");

				List<Parameter> parameters = new List<Parameter>();
				Constructor func = new Constructor(mods, (Class)f.type, parameters, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}
			else if (identifier == "indexer")
			{
				// indexer
				List<Parameter> indices = new List<Parameter>();
				Indexer idxer = new Indexer((Bohc.TypeSystem.Type)f.type, mods, type, indices, body);
				parseFunctionParamsTCS(f, fDec, idxer);
				return idxer;
			}
			else if (identifier == "static")
			{
				StaticConstructor func = new StaticConstructor((Class)f.type, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}
			else if (Bohc.Parsing.Operator.isOperator(identifier))
			{
				Boh.Exception.require<ParserException>(
					mods.HasFlag(Modifiers.Public) && mods.HasFlag(Modifiers.Static), "Invalid modifier for constructor");

				List<Parameter> parameters = new List<Parameter>();
				OverloadedOperator func = new OverloadedOperator(
					(Bohc.TypeSystem.Class)type,
					Bohc.Parsing.Operator.getExisting(identifier, Bohc.Parsing.OperationType.DOESNT_MATTER),
					type, parameters, body);

				parseFunctionParamsTCS(f, fDec, func);

				Boh.Exception.require<ParserException>(func.Parameters.Any(x => x.Type == f.type), "An overloaded operator must apply to the type on which it is overloaded");
				Boh.Exception.require<ParserException>(func.OpType == Bohc.Parsing.OperationType.BINARY ? parameters.Count == 2 : parameters.Count == 1,
					"Invalid parameter count for operator " + identifier);

				return func;
			}
			else
			{
				Boh.Exception.require<ParserException>(Bohc.TypeSystem.Type.IsValidIdentifier(identifier), identifier + " is not a valid identifier");

				List<Parameter> parameters = new List<Parameter>();
				Function func = new Function((Bohc.TypeSystem.Type)f.type, mods, type, identifier, parameters, body);
				parseFunctionParamsTCS(f, fDec, func);
				return func;
			}

		}

		private void parsePreFunctionParamsTCS(File f, string fDec, bool requiresAccess, out Modifiers mods, out Bohc.TypeSystem.Type type, out string identifier)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = ParserTools.getMatchingBraceCharBackwards(fDec, idxClose, '(');
			fDec = ParserTools.remDupW(fDec.Substring(0, idxParent).Trim());

			// check if indexer
			bool indexer = false;
			if (fDec.EndsWith("]"))
			{
				indexer = true;
				idxClose = fDec.Length - 1;
				idxParent = ParserTools.getMatchingBraceCharBackwards(fDec, idxClose, '[');
				fDec = ParserTools.remDupW(fDec.Substring(0, idxParent));
			}

			string[] parts = fDec.Split(' ');
			bool isStaticConstr = parts.Last() == "static";
			bool isConstr = !indexer && parts.Last() == "this" || isStaticConstr;

			Boh.Exception.require<ParserException>(
				isStaticConstr || (isConstr && parts.Length >= 2) || parts.Length >= 3 || (!requiresAccess && parts.Length >= 2),
				fDec + ": function access modifier and/or type expected");

			identifier = parts[parts.Length - 1];
			string typeName = isConstr ? null : parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - (isConstr ? 1 : 2));

			type = isConstr ? null : Bohc.TypeSystem.Type.GetExisting(f.getContext(), typeName, this);
			mods = isStaticConstr ? Modifiers.None : ModifierHelper.GetModifiersFromStrings(modifiers);
		}

		private void parseFunctionParamsTCS(File f, string fDec, Function func)
		{
			int idxClose = fDec.LastIndexOf(')');
			int idxParent = ParserTools.getMatchingBraceCharBackwards(fDec, idxClose, '(');

			Indexer indexer = func as Indexer;
			if (indexer != null)
			{
				string assignment = ParserTools.remDupW(fDec.Substring(idxParent + 1, idxClose - idxParent - 1).Trim());
				if (!string.IsNullOrEmpty(assignment))
				{
					string[] assnmnts = ParserTools.split(assignment, new[] { '<', '(' }, new[] { '>', ')' }, ',').ToArray();
					for (int i = 0; i < assnmnts.Length; ++i)
					{
						assnmnts[i] = assnmnts[i].Trim();
					}
					Boh.Exception.require<ParserException>(
						assnmnts.Length == 1, "Indexers may only have one assignment parameter");

					string identifier;
					Modifiers mods;
					Bohc.TypeSystem.Type type;
					parseParam(f, assnmnts.Single(), out identifier, out mods, out type);

					Parameter param = new Parameter(func, mods, identifier, type);
					indexer.Assignment = param;
				}

				string b4 = fDec.Substring(0, idxParent).TrimEnd();
				idxClose = b4.Length - 1;
				idxParent = ParserTools.getMatchingBraceCharBackwards(b4, idxClose, '[');
			}

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
				Bohc.TypeSystem.Type type;
				parseParam(f, part, out identifier, out mods, out type);

				Parameter param = new Parameter(func, mods, identifier, type);
				func.Parameters.Add(param);
			}
		}

		public void parseParam(File f, string part, out string identifier, out Modifiers mods, out Bohc.TypeSystem.Type type)
		{
			string[] paramParts = part.Split(' ');
			Boh.Exception.require<ParserException>(paramParts.Length >= 2, "Parameter type expected");
			Boh.Exception.require<ParserException>(paramParts.Length <= 3, "Parameters may only have one modifier");

			identifier = paramParts[paramParts.Length - 1];
			string typeName = paramParts[paramParts.Length - 2];

			mods = Modifiers.None;
			if (paramParts.Length > 2)
			{
				Boh.Exception.require<ParserException>(
					paramParts.First() == "final" || paramParts.First() == "ref", "'final' and 'ref' are the only legal modifiers for parameters");
				mods = ModifierHelper.GetModifierFromString(paramParts.First());
			}

			type = Bohc.TypeSystem.Type.GetExisting(f.getContext(), typeName, this);
			Boh.Exception.require<ParserException>(type != null, "type doesn't exist");
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
			Boh.Exception.require<ParserException>(parts.Length >= 3, fDec + ": field access modifier and/or type expected");

			string identifier = parts[parts.Length - 1];
			string type = parts[parts.Length - 2];
			IEnumerable<string> modifiers = parts.Take(parts.Length - 2);

			Bohc.TypeSystem.Type actualType = Bohc.TypeSystem.Type.GetExisting(f.getContext(), type, this);
			Modifiers mods = ModifierHelper.GetModifiersFromStrings(modifiers);

			Boh.Exception.require<ParserException>(Bohc.TypeSystem.Type.IsValidIdentifier(identifier), identifier + " is not a valid identifier");
			Boh.Exception.require<ParserException>(ModifierHelper.AreModifiersLegal(mods, true, proj()), mods.ToString() + ": invalid modifiers");

			Field field = new Field(mods, identifier, actualType, (Class)f.type, initvalstr);
			((Class)f.type).AddMember(field);
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

				Boh.Exception.require<ParserException>(Bohc.TypeSystem.Type.IsValidIdentifier(enumerator), enumerator + " is not a valid enumerator");
				Boh.Exception.warnIf(enumerator.ToUpperInvariant() != enumerator, "Enumerator names should be uppercase");

				((Bohc.TypeSystem.Enum)f.type).Enumerators.Add(new Enumerator(enumerator, (Bohc.TypeSystem.Enum)f.type));
			}
		}

		#endregion

		#region Type Content Parsing

		public void parseFileTCP(File f)
		{
			string file = f.content;

			if (f.state >= ParserState.TCP)
			{
				return;
			}
			f.state = ParserState.TCP;

			Class c = f.type as Class;
			if (c != null)
			{
				parseClassTCP(c);
			}
		}

		private void parseClassTCP(Class c)
		{
			foreach (Field f in c.Fields)
			{
				if (f.InitValStr != null)
				{
					f.Initial = Statements.getExpressions().analyze(f.InitValStr, new List<Variable>(), c.File);
				}
			}
		}

		#endregion

		#region Code Parsing

		public void parseFileCP(File f)
		{
			string file = f.content;

			if (f.state >= ParserState.CP)
			{
				return;
			}
			f.state = ParserState.CP;

			Class c = f.type as Class;
			if (c == null)
			{
				return;
			}

			foreach (Function func in c.Functions)
			{
				if (!func.Modifiers.HasFlag(Modifiers.Abstract) && !func.Modifiers.HasFlag(Modifiers.Native))
				{
					/*try
					{*/
						func.Body = Statements.parseBody(func.BodyStr, func, f);
					/*}
					catch (System.Exception e)
					{
						throw new System.Exception(func.Identifier, e);
					}*/

					if (func.ReturnType != Primitive.Void && func.ReturnType != null)
					{
						Boh.Exception.require<ParserException>(func.Body.hasReturned(), "Function must return a value");
					}
					else if (func is Constructor)
					{
						Boh.Exception.require<ParserException>(
							c.Super == null ||
							c.Super.Constructors.Any(x => !(x.Modifiers.HasFlag(Modifiers.Private) || x.Modifiers.HasFlag(Modifiers.CVisible)) && x.Parameters.Count == 0) ||
							func.Body.hasSuperBeenCalled(),
							"Constructor must call super constructor");
					}
				}
			}
		}

		#endregion
	}
}
