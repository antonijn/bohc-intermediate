using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bohc.General;

using Bohc.TypeSystem;

namespace Bohc.Parsing
{
	public class TokenizedFileParser : IFileParser
	{
		private Project p;

		public TokenizedFileParser(Project p)
		{
			this.p = p;
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

		private TokenRange readTypeName(TokenStream t)
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
			return f;
		}

		public void parseFileTP(File f)
		{
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
				TypeSystem.Type ty = TypeSystem.Type.GetExisting(tyr.str, this);
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
				}
				else
				{
					t.peek(1).error("unexpected token '{0}'", t.peek(1).value);
				}
			}
		}

		public void parseFileTCP(File f)
		{
			//throw new NotImplementedException();
		}

		public void parseFileCP(File f)
		{
			//throw new NotImplementedException();
		}

		public Bohc.General.Project proj()
		{
			throw new NotImplementedException();
		}
	}
}

