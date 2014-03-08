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
		private Project p;

		private readonly TokenizedStatementParser tsp;

		public TokenizedFileParser(Project p)
		{
			this.p = p;

			tsp = new TokenizedStatementParser(this);
		}

		private Package package(TokenStream t)
		{
			Package p = Package.Global;
			while (t.next())
			{
				Token to = t.get();
				if (!to.isType(TokenType.IDENTIFIER))
				{
					to.error("unexpected token '{0}', expected package name", to.value);
					return Package.Global;
				}
				p = Package.Get(p, to.value);
				if (!t.next())
				{
					return p;
				}
				if (t.get().value != ".")
				{
					t.get().error("invalid token: '{0}', expected '.'", t.get().value);
					return Package.Global;
				}
			}
			return p;
		}

		private File parseCommonHeader(TokenStream t)
		{
			Package pckg = null;
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

		private GenericType genericType(Modifiers mods, TokenStream namestream)
		{
			List<string> typenames = new List<string>();

			namestream.next();
			if (!namestream.get().isType(TokenType.IDENTIFIER))
			{
				namestream.get().error("invalid typename: '{0}'", namestream.get().value);
				return null;
			}

			// TODO
			throw new Exception();
		}

		public static TokenRange readTypeName(TokenStream t)
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
					t.get().error("unexpected token '{0}', expected identifier", t.get().value);
				}

				sb.Append(t.get().value);

				last = t.get();

				if (!t.next())
				{
					break;
				}

				Token to = t.get();
				if (to.value != "." && to.value != "<")
				{
					t.prior();
					break;
				}

				if (to.value == "<")
				{
					TokenStream gpars = t.until(">", new Tuple<string, string>("<", ">"));
					while (gpars.next())
					{
						sb.Append(gpars.get().value);
					}
					sb.Append(">");
					t.prior();
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
				strings.Add(readTypeName(ts));
				if (ts.next())
				{
					ts.get().error("unexpected token '{0}'", ts.get().value);
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
					extendsImplements.error("unexpected token '{0}', expected 'extends' or 'implements'", extendsImplements.value);
				}

				if (extendsImplements.value == "extends")
				{
					try
					{
						extends = readTypeName(namestream);
					}
					catch (Bohc.Exceptions.ParserException)
					{
						extendsImplements.error("a type name must follow the 'extends' keyword");
					}
					if (namestream.next())
					{
						extendsImplements = namestream.get();
						if (extendsImplements.value != "implements")
						{
							extendsImplements.error("unexpected token '{0}', expected implements'", extendsImplements.value);
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
				to.error("unexpected token '{0}', expected access modifier", to.value);
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

			if (!to.isType(TokenType.CLASS_ENUM_STRUCT))
			{
				to.error("unexpected token '{0}', expected 'class', 'enum' or 'struct'", to.value);
				// TODO: panic
			}

			ts.next();
			TokenStream namestream = ts.until("{");
			if (namestream.peek(2).value == "<")
			{
				f.type = genericType(modifiers, namestream);
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
			if (f.state >= ParserState.TP)
			{
				return;
			}
			f.state = ParserState.TP;

			TypeSystem.Type ty = (TypeSystem.Type)f.type;
			Tuple<TokenRange, List<TokenRange>, TokenStream> tuple = (Tuple<TokenRange, List<TokenRange>, TokenStream>)ty.parserinfo;
			Tuple<TypeSystem.Type, List<TypeSystem.Type>, TokenStream> pinfo = new Tuple<TypeSystem.Type, List<TypeSystem.Type>, TokenStream>(
				tuple.Item1 == null ? null : TypeSystem.Type.GetExisting(f.getContext(), tuple.Item1.str, this),
				tuple.Item2 == null ? null : tuple.Item2.Select(x => TypeSystem.Type.GetExisting(f.getContext(), x.str, this)).ToList(),
				tuple.Item3
				);
			if (pinfo.Item1 == ty)
			{
				tuple.Item1.error("cannot extend self");
			}
			if (pinfo.Item1 == null && tuple.Item1 != null)
			{
				tuple.Item1.error("type not found: '{0}'", tuple.Item1.str);
			}
			else if (pinfo.Item1 != null && pinfo.Item1.Modifiers.HasFlag(Modifiers.Final))
			{
				tuple.Item1.error("cannot extend a class marked 'final'");
			}

			if (pinfo.Item1 != null)
			{
				((Class)ty).Super = (Class)pinfo.Item1;
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
			if (f.state >= ParserState.TCS)
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
				if (to.value != "private" && to.value != "protected" && to.value != "public")
				{
					to.error("unexpected token '{0}', expected access modifier", to.value);
				}
				Modifiers mf = ModifierHelper.GetModifierFromString(to.value);
				while (t.next() && t.get().isType(TokenType.MODIFIER))
				{
					mf |= ModifierHelper.GetModifierFromString(t.get().value);
				}
				t.prior();
				TokenRange tyr = readTypeName(t);
				TypeSystem.Type ty = TypeSystem.Type.GetExisting(f.getContext(), tyr.str, this);
				t.next();
				if (t.peek(1).value == ";")
				{
					((Class)f.type).Fields.Add(new Field(mf, t.get().value, ty, (Class)f.type, null));
					t.next();
				}
				else if (t.peek(1).value == "=")
				{
					t.next();
					t.next();
					TokenStream init = t.until(";");
					t.prior();
					((Class)f.type).Fields.Add(new Field(mf, t.get().value, ty, (Class)f.type, init));
				}
				else if (t.peek(1).value == "(")
				{
					Function func = new Function((TypeSystem.Type)f.type, mf, ty, t.get().value, new List<Parameter>(), new object());
					parseMethodParamsTCS(func, t);
					t.next();
					func.BodyStr = t.until("}", new Tuple<string, string>("{", "}"));
					((Class)f.type).AddMember(func);
				}
				else
				{
					t.peek(1).error("unexpected token '{0}'", t.peek(1).value);
				}
			}
			if (((Class)f.type).Constructors.Count == 0)
			{
				((Class)f.type).Constructors.Add(new Constructor(Modifiers.Public, (Class)f.type, new List<Parameter>(), null));
			}
		}

		private void parseMethodParamsTCS(Function f, TokenStream t)
		{
			t.next();
			t.next(); // skip '('
			TokenStream pars = t.until(")", new Tuple<string, string>("(", ")"));
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
						pars.get().error("expected ','");
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
			TokenRange tyr = readTypeName(param);
			TypeSystem.Type ty = TypeSystem.Type.GetExisting(f.Owner.File.getContext(), tyr.str, this);
			param.next();
			if (!param.get().isType(TokenType.IDENTIFIER))
			{
				param.get().error("expected parameter identifier");
			}
			string id = param.get().value;
			f.Parameters.Add(new Parameter(f, mf, id, ty));
		}

		public void parseFileTCP(File f)
		{
			if (f.state >= ParserState.TCP)
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
					func.Body = tsp.parseBody(func.BodyStr, func, null, f);

					if (func.ReturnType != Primitive.Void && func.ReturnType != null && !func.Body.hasReturned())
					{
						((TokenStream)func.BodyStr).get().error("function must return a value");
					}
					else if (func is Constructor)
					{
						if (c.Super != null &&
						    !c.Super.Constructors.Any(x => !(x.Modifiers.HasFlag(Modifiers.Private) || x.Modifiers.HasFlag(Modifiers.CVisible)) && x.Parameters.Count == 0) &&
						    !func.Body.hasSuperBeenCalled())
						{
							((TokenStream)func.BodyStr).get().error("constructor must call super constructor");
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

