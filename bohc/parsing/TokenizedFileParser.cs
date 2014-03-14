using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bohc.General;

using Bohc.TypeSystem;
using Bohc.Parsing.Statements;

namespace Bohc.Parsing
{
	public class TokenizedFileParser : IFileParser
	{
		private readonly Project p;
		private readonly Platform pf;
		private readonly ErrorManager emanager;

		private readonly TokenizedStatementParser tsp;

		public TokenizedFileParser(Project p, Platform pf)
		{
			this.p = p;
			this.pf = pf;
			this.emanager = new ErrorManager(p);

			tsp = new TokenizedStatementParser(this);
		}

		private IEnumerable<Token> tokensForPackageReverse(Package p, int c, int l, string line, string fname)
		{
			while (p != Package.Global)
			{
				yield return new Token(TokenType.OPERATOR, ".", c, l, line, fname);
				yield return new Token(TokenType.IDENTIFIER, p.Name, c, l, line, fname);
				p = p.Parent;
			}
		}

		private IEnumerable<Token> tokensForType(TypeSystem.Type t, int c, int l, string line, string fname)
		{
			foreach (Token to in tokensForPackageReverse(t.Package, c, l, line, fname).Reverse())
			{
				yield return to;
			}
			yield return new Token(TokenType.IDENTIFIER, t.Name, c, l, line, fname);
		}

		public TypeSystem.Type getNewType(GenericType gt, TypeSystem.Type[] what, Action<TypeSystem.Type> reg, Action<TypeSystem.Type> regdone)
		{
			string codestr = ParserTools.remDupW((string)gt.File.parserinfo).Replace(" ,", ",").Replace(", ", ",");

			StringBuilder replaceWhat = new StringBuilder();
			replaceWhat.Append(gt.Name);
			replaceWhat.Append("<");
			foreach (string str in gt.GenTypeNames)
			{
				replaceWhat.Append(str).Append(",");
			}
			replaceWhat.Remove(replaceWhat.Length - 1, 1);
			replaceWhat.Append(">");

			StringBuilder byWhat = new StringBuilder();
			byWhat.Append(gt.Name);
			byWhat.Append("`0");
			foreach (TypeSystem.Type t in what)
			{
				byWhat.Append("`1");
				byWhat.Append(t.FullName().Replace('.', '`'));
			}
			byWhat.Append("`2");

			codestr = codestr.Replace(replaceWhat.ToString(), byWhat.ToString());

			IEnumerable<Token> code;
			using (StringReader sr = new StringReader(codestr))
			{
				Tokenizer tn = new Tokenizer(sr, gt.File.filename);
				code = tn.lex().ToArray();
			}

			for (int i = 0; i < what.Length; ++i)
			{
				string gtname = gt.GenTypeNames[i];
				TypeSystem.Type w = what[i];

				code = code.SelectMany(x => x.value == gtname ? 
					tokensForType(w, x.column, x.linenum, x.line, x.filename).ToArray()
					: new[] { x });
			}
			Token[] tokens = code.ToArray();
			StringBuilder sb = new StringBuilder();
			foreach (Token t in tokens)
			{
				sb.Append(t.value).Append(" ");
			}
			codestr = sb.ToString();

			Parsing.File newf = parseFileTS(ref codestr, gt.File.filename);
			reg((TypeSystem.Type)newf.type);
			//parser.proj().pstrat.registerRtType(newf.type as typesys.Type);
			parseFileTP(newf);
			if (getStrat().getpstate() >= ParserState.TCS)
			{
				parseFileTCS(newf);
			}
			if (getStrat().getpstate() >= ParserState.TCP)
			{
				parseFileTCP(newf);
			}
			if (getStrat().getpstate() >= ParserState.CP)
			{
				parseFileCP(newf);
			}

			((TypeSystem.Type)newf.type).OriginalGenType = gt;
			newf.type.SetFile(newf);
			regdone((TypeSystem.Type)newf.type);

			return (TypeSystem.Type)newf.type;
		}

		private IParserStrategy pstrat;
		public void regStrat(IParserStrategy pstrat)
		{
			this.pstrat = pstrat;
		}

		public IParserStrategy getStrat()
		{
			return pstrat;
		}

		public ErrorManager getEM()
		{
			return emanager;
		}

		private Package package(TokenStream t)
		{
			Package p = Package.Global;
			while (t.next())
			{
				Token to = t.get();
				if (!to.isType(TokenType.IDENTIFIER))
				{
					to.error(emanager, "unexpected token '{0}', expected package name", to.value);
					return Package.Global;
				}
				p = Package.Get(p, to.value);
				if (!t.next())
				{
					return p;
				}
				if (t.get().value != ".")
				{
					t.get().error(emanager, "invalid token: '{0}', expected '.'", t.get().value);
					return Package.Global;
				}
			}
			return p;
		}

		private File parseCommonHeader(TokenStream t)
		{
			Package pckg = Package.Global;
			if (t.get().value == "package")
			{
				t.next();
				TokenStream pack = t.until(";");
				pckg = package(pack);
			}

			List<Package> imports = new List<Package>();
			while (true)
			{
				Token nxt = t.get();
				if (nxt.value == "import")
				{
					t.next();
					TokenStream pack = t.until(";");
					imports.Add(package(pack));
				}
				else
				{
					break;
				}
			}

			return new File(imports, pckg, t);
		}

		private GenericType genericType(Modifiers mods, TokenStream namestream, File f)
		{
			List<string> typenames = new List<string>();
			List<string> inits = new List<string>();

			namestream.next();
			if (!namestream.get().isType(TokenType.IDENTIFIER))
			{
				namestream.get().error(emanager, "invalid typename: '{0}'", namestream.get().value);
				return null;
			}
			string name = namestream.get().value;
			namestream.next();
			while (namestream.next())
			{
				if (!namestream.get().isType(TokenType.IDENTIFIER))
				{
					namestream.get().error(emanager, "invalid generic parameter name: '{0}'", namestream.get().value);
					return null;
				}
				typenames.Add(namestream.get().value);
				namestream.next();
				if (namestream.get().value == "=")
				{
					TokenRange tr = readTypeName(emanager, namestream);
					inits.Add(tr.str);

					namestream.next();
				}
				if (namestream.get().value == ">")
				{
					break;
				}
				if (namestream.get().value == ",")
				{
					continue;
				}
				namestream.get().error(emanager, "unexpected token '{0}'", namestream.get());
			}

			GenericType gt = new GenericType(typenames.ToArray(), name);
			gt.File = f;

			if (inits.Count == typenames.Count)
			{
				gt.InitialTypeNames = inits.ToArray();
			}

			return gt;
		}

		public static TokenRange readTypeName(ErrorManager e, TokenStream t)
		{
			StringBuilder sb = new StringBuilder();

			Token first = default(Token);
			Token last = default(Token);
			while (t.next())
			{
				if (first.value == null)
				{
					first = t.get();
				}
				if (!t.get().isType(TokenType.IDENTIFIER) && !t.get().isType(TokenType.PRIMITIVE))
				{
					t.get().error(e, "unexpected token '{0}', expected identifier", t.get().value);
				}

				sb.Append(t.get().value);

				last = t.get();

				if (!t.next())
				{
					break;
				}

				Token to = t.get();
				if (to.value != "." && to.value != "<" && to.value != "[")
				{
					t.prior();
					break;
				}

				if (to.value == "[")
				{
					t.next();
					if (t.get().value != "]")
					{
						t.prior();
						t.prior();
						break;
					}
					sb.Append("[]");
					last = t.get();
					break;
				}

				if (to.value == "<")
				{
					sb.Append("<");
					t.next();
					TokenStream gpars = t.until(">", new Tuple<string, string>("<", ">"));
					t.prior();
					while (gpars.next())
					{
						sb.Append(gpars.get().value);
					}
					sb.Append(">");
					last = t.get();
					break;
				}

				sb.Append(to.value);
			}
			if (sb.Length == 0)
			{
				throw new Bohc.Exceptions.ParserException();
			}
			return new TokenRange(first, last, sb.ToString());
		}

		private List<TokenRange> readImplements(TokenStream t)
		{
			List<TokenRange> strings = new List<TokenRange>();
			foreach (TokenStream ts in t.split(",", new Tuple<string, string>("<", ">"), new Tuple<string, string>("(", ")")))
			{
				strings.Add(readTypeName(emanager, ts));
				if (ts.next())
				{
					ts.get().error(emanager, "unexpected token '{0}'", ts.get().value);
				}
			}
			return strings;
		}

		private TypeSystem.Type parseClassStructEnum(File f, Modifiers modifiers, string tty, TokenStream namestream, TokenStream ts, TokenStream body)
		{
			namestream.next();

			Token name = namestream.get();

			TokenRange extends = null;
			List<TokenRange> implements = null;

			if (namestream.next())
			{
				Token extendsImplements = namestream.get();
				if (extendsImplements.value != "extends" && extendsImplements.value != "implements")
				{
					extendsImplements.error(emanager, "unexpected token '{0}', expected 'extends' or 'implements'", extendsImplements.value);
				}

				if (extendsImplements.value == "extends")
				{
					try
					{
						extends = readTypeName(emanager, namestream);
					}
					catch (Bohc.Exceptions.ParserException)
					{
						extendsImplements.error(emanager, "a type name must follow the 'extends' keyword");
					}
					if (namestream.next())
					{
						extendsImplements = namestream.get();
						if (extendsImplements.value != "implements")
						{
							extendsImplements.error(emanager, "unexpected token '{0}', expected implements'", extendsImplements.value);
						}
						else
						{
							namestream.next();
							implements = readImplements(namestream);
						}
					}
				}
				else
				{
					implements = readImplements(namestream);
				}
			}
			switch (tty)
			{
				case "class":
					{
						TypeSystem.Type ty = Class.Get<Class>(f.package, modifiers, name.value);
						ty.parserinfo = new Tuple<TokenRange, List<TokenRange>, TokenStream>(extends, implements, body);
						return ty;
					}
				case "struct":
					{
						TypeSystem.Type ty = Struct.Get<Struct>(f.package, modifiers, name.value);
						ty.parserinfo = new Tuple<TokenRange, List<TokenRange>, TokenStream>(extends, implements, body);
						return ty;
					}
				case "enum":
					{
						TypeSystem.Type ty = TypeSystem.Enum.Get(f.package, modifiers, name.value);
						ty.parserinfo = new Tuple<TokenRange, List<TokenRange>, TokenStream>(extends, implements, body);
						return ty;
					}
			}

			throw new Exception();
		}

		public File parseFileTS(ref string file, string filename)
		{
			file = ParserTools.removeComments(file);

			StringReader sr = new StringReader(file);
			Token[] tokens = new Tokenizer(sr, filename).lex().ToArray();
			TokenStream ts = new TokenStream(tokens, 0, tokens.Length);
			ts.next();

			File f = parseCommonHeader(ts);
			f.state = ParserState.TS;

			// parse type
			Modifiers modifiers = Modifiers.Public;

			Token to = ts.get();
			if (to.value != "private" && to.value != "public")
			{
				to.error(emanager, "unexpected token '{0}', expected access modifier", to.value);
			}
			else
			{
				modifiers = ModifierHelper.GetModifierFromString(to.value);
			}

			while (ts.next())
			{
				to = ts.get();
				if (!to.isType(TokenType.MODIFIER))
				{
					break;
				}

				modifiers |= ModifierHelper.GetModifierFromString(to.value);
			}

			if (!Platform.IsPlatform(modifiers, pf))
			{
				f.ignore = true;
				return f;
			}

			if (!to.isType(TokenType.CLASS_ENUM_STRUCT))
			{
				to.error(emanager, "unexpected token '{0}', expected 'class', 'enum' or 'struct'", to.value);
				// TODO: panic
			}

			ts.next();
			TokenStream namestream = ts.until("{");
			if (namestream.peek(2).value == "<")
			{
				f.type = genericType(modifiers, namestream, f);
				ts.prior();
				f.parserinfo = file;
				return f;
			}
			ts.prior();
			TokenStream intype = ts.until("}", new Tuple<string, string>("{", "}"));
			intype.next();
			ts.next();

			TypeSystem.Type ty = parseClassStructEnum(f, modifiers, to.value, namestream, ts, intype);

			f.type = ty;
			ty.File = f;
			return f;
		}

		public void parseFileTP(File f)
		{
			if (f.state >= ParserState.TP || f.ignore)
			{
				return;
			}
			f.state = ParserState.TP;

			if (f.type is GenericType)
			{
				((GenericType)f.type).InitialTypes = ((GenericType)f.type).InitialTypeNames.Select(x => TypeSystem.Type.GetExisting(f.getContext(), x, this)).ToArray();
				return;
			}

			TypeSystem.Type ty = (TypeSystem.Type)f.type;
			Tuple<TokenRange, List<TokenRange>, TokenStream> tuple = (Tuple<TokenRange, List<TokenRange>, TokenStream>)ty.parserinfo;
			Tuple<TypeSystem.Type, List<TypeSystem.Type>, TokenStream> pinfo = new Tuple<TypeSystem.Type, List<TypeSystem.Type>, TokenStream>(
				tuple.Item1 == null ? null : TypeSystem.Type.GetExisting(f.getContext(), tuple.Item1.str, this),
				tuple.Item2 == null ? null : tuple.Item2.Select(x => TypeSystem.Type.GetExisting(f.getContext(), x.str, this)).ToList(),
				tuple.Item3
				);
			if (pinfo.Item1 == ty)
			{
				tuple.Item1.error(emanager, "cannot extend self");
			}
			if (pinfo.Item1 == null && tuple.Item1 != null)
			{
				tuple.Item1.error(emanager, "type not found: '{0}'", tuple.Item1.str);
			}
			else if (pinfo.Item1 != null && pinfo.Item1.Modifiers.HasFlag(Modifiers.Final))
			{
				tuple.Item1.error(emanager, "cannot extend a class marked 'final'");
			}

			if (pinfo.Item1 != null)
			{
				((Class)ty).Super = (Class)pinfo.Item1;
			}
			else
			{
				((Class)ty).Super = StdType.Obj;
			}
			if (pinfo.Item2 != null)
			{
				foreach (TypeSystem.Type t in pinfo.Item2)
				{
					((Class)ty).Implement((Interface)t);
				}
			}

			ty.parserinfo = pinfo.Item3;
		}

		public void parseFileTCS(File f)
		{
			if (f.state >= ParserState.TCS || f.ignore)
			{
				return;
			}
			f.state = ParserState.TCS;

			TokenStream t = (TokenStream)((TypeSystem.Type)f.type).parserinfo;

			parseClassTCS(f, t);
		}

		void parseClassTCS(File f, TokenStream t)
		{
			while (t.next())
			{
				Token to = t.get();
				if (to.value == "}")
				{
					break;
				}
				if (to.value != "private" && to.value != "protected" && to.value != "public")
				{
					to.error(emanager, "unexpected token '{0}', expected access modifier", to.value);
				}

				Modifiers mf = ModifierHelper.GetModifierFromString(to.value);
				while (t.next() && t.get().isType(TokenType.MODIFIER))
				{
					mf |= ModifierHelper.GetModifierFromString(t.get().value);
				}

				TypeSystem.Type ty = Primitive.Void;
				bool isConstr = false;

				if (!(isConstr = t.get().value == "this"))
				{
					t.prior();

					TokenRange tyr = readTypeName(emanager, t);
					ty = TypeSystem.Type.GetExisting(f.getContext(), tyr.str, this);
					t.next();
				}

				if (t.peek(1).value == ";")
				{
					if (Platform.IsPlatform(mf, pf))
					{
						((Class)f.type).Fields.Add(new Field(mf, t.get().value, ty, (Class)f.type, null));
					}
					t.next();
				}
				else if (t.peek(1).value == "=")
				{
					string id = t.get().value;
					t.next();
					t.next();
					TokenStream init = t.until(";");
					t.prior();

					if (Platform.IsPlatform(mf, pf))
					{
						((Class)f.type).Fields.Add(new Field(mf, id, ty, (Class)f.type, init));
					}
				}
				else if (t.peek(1).value == "(")
				{
					Function func = isConstr ? 
					                new Constructor(mf, (TypeSystem.Class)f.type, new List<Parameter>(), null)
					                : new Function((TypeSystem.Type)f.type, mf, ty, t.get().value, new List<Parameter>(), new object());
					parseMethodParamsTCS(func, t, '(', ')');
					t.next();
					func.BodyStr = t.until("}", new Tuple<string, string>("{", "}"));
					if (Platform.IsPlatform(mf, pf))
					{
						((Class)f.type).AddMember(func);
					}
					t.prior();
				}
				else if (t.peek(1).value == "[")
				{
					if (t.get().value != "this")
					{
						t.get().error(emanager, "indexers must be called 'this'");
					}
					Indexer func = new Indexer((TypeSystem.Type)f.type, mf, ty, new List<Parameter>(), null);
					parseMethodParamsTCS(func, t, '[', ']');
					if (t.get().value != "(")
					{
						t.get().error(emanager, "expected '('");
					}

					Parameter assign = null;

					t.next();
					if (t.get().value != ")")
					{
						Modifiers amf = Modifiers.None;

						t.prior();
						while (t.next() && t.get().isType(TokenType.MODIFIER))
						{
							amf |= ModifierHelper.GetModifierFromString(t.get().value);
						}

						t.prior();
						TokenRange atyr = readTypeName(emanager, t);
						TypeSystem.Type aty = TypeSystem.Type.GetExisting(f.getContext(), atyr.str, this);
						t.next();
						assign = new Parameter(func, amf, t.get().value, aty);
						t.next();
					}
					t.next();
					t.next();
					func.BodyStr = t.until("}", new Tuple<string, string>("{", "}"));
					func.Assignment = assign;
					if (Platform.IsPlatform(mf, pf))
					{
						((Class)f.type).AddMember(func);
					}
					t.prior();
				}
				else
				{
					t.peek(1).error(emanager, "unexpected token '{0}'", t.peek(1).value);
				}
			}
			if (((Class)f.type).Constructors.Count == 0)
			{
				((Class)f.type).Constructors.Add(new Constructor(Modifiers.Public, (Class)f.type, new List<Parameter>(), null));
			}
			if (((Class)f.type).StaticConstr == null)
			{
				((Class)f.type).StaticConstr = new StaticConstructor((Class)f.type, null);
				((Class)f.type).StaticConstr.Body = new Body(null);
			}
		}

		private void parseMethodParamsTCS(Function f, TokenStream t, char open, char close)
		{
			t.next();
			t.next(); // skip '('
			TokenStream pars = t.until(close.ToString(), new Tuple<string, string>(open.ToString(), close.ToString()));
			while (pars.next())
			{
				parseParamTCS(f, pars);
				if (pars.next())
				{
					if (pars.get().value == ",")
					{
						continue;
					}
					else
					{
						pars.get().error(emanager, "expected ','");
					}
				}
			}
		}

		private void parseParamTCS(Function f, TokenStream param)
		{
			Modifiers mf = Modifiers.None;
			while (param.get().isType(TokenType.MODIFIER))
			{
				mf |= ModifierHelper.GetModifierFromString(param.get().value);
				param.next();
			}
			param.prior();
			TokenRange tyr = readTypeName(emanager, param);
			TypeSystem.Type ty = TypeSystem.Type.GetExisting(f.Owner.File.getContext(), tyr.str, this);
			param.next();
			if (!param.get().isType(TokenType.IDENTIFIER))
			{
				param.get().error(emanager, "expected parameter identifier");
			}
			string id = param.get().value;
			f.Parameters.Add(new Parameter(f, mf, id, ty));
		}

		public void parseFileTCP(File f)
		{
			if (f.state >= ParserState.TCP || f.ignore)
			{
				return;
			}
			f.state = ParserState.TCP;

			Class c = f.type as Class;
			if (c != null)
			{
				foreach (Field field in c.Fields.Where(x => x.InitValStr != null))
				{
					field.Initial = tsp.getEp().analyze((TokenStream)field.InitValStr, c.Fields);
				}
			}
		}

		public void parseFileCP(File f)
		{
			if (f.state >= ParserState.CP || f.ignore)
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
					func.Body = tsp.parseBody(func.BodyStr, func, null, f);

					if (func.ReturnType != Primitive.Void && func.ReturnType != null && !func.Body.hasReturned())
					{
						((TokenStream)func.BodyStr).get().error(emanager, "function must return a value");
					}
					else if (func is Constructor)
					{
						if (c.Super != null &&
						    !c.Super.Constructors.Any(x => !(x.Modifiers.HasFlag(Modifiers.Private) || x.Modifiers.HasFlag(Modifiers.CVisible)) && x.Parameters.Count == 0) &&
						    !func.Body.hasSuperBeenCalled())
						{
							((TokenStream)func.BodyStr).get().error(emanager, "constructor must call super constructor");
						}
					}
				}
			}
		}

		public Bohc.General.Project proj()
		{
			throw new NotImplementedException();
		}
	}
}

