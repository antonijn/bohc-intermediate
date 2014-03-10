using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bohc.Parsing
{
	public class Tokenizer
	{
		/// <summary>
		/// Returns whether a character is valid for identifiers
		/// </summary>
		private bool isIdValidChar(char ch)
		{
			return char.IsLetterOrDigit(ch) || ch == '_' || ch == '`';
		}

		/// <summary>
		/// Returns whether a character is valid for the start of an identifier
		/// </summary>
		private bool isStartIdValidChar(char ch)
		{
			return char.IsLetter(ch) || ch == '_';
		}

		private StringReader reader;
		private string filename;

		public Tokenizer(StringReader reader, string filename)
		{
			this.reader = reader;
			this.filename = filename;
		}

		private IEnumerable<Token> lex(string str, int linenum)
		{
			int i = 0;
			while (i < str.Length)
			{
				Token res;
				i = lex(str, i, linenum, out res);
				if (res.tokentype[0] != TokenType.UNIDENTIFIED)
				{
					yield return res;
				}
			}
		}

		public IEnumerable<Token> lex()
		{
			string s;
			int linenum = 0;
			while ((s = reader.ReadLine()) != null)
			{
				foreach (Token t in lex(s, linenum))
				{
					yield return t;
				}
				++linenum;
			}
		}

		private int lex(string str, int i, int linenum, out Token res)
		{
			char ch = str[i];

			// skip spaces
			if (ch == ' ')
			{
				++i;
				res = new Token(TokenType.UNIDENTIFIED, " ", i, linenum, str, filename);
				return i;
			}

			// check if id
			if (isStartIdValidChar(ch))
			{
				return lexId(str, i, linenum, out res);
			}

			// check if statement terminator
			if (ch == ';')
			{
				++i;
				res = new Token(TokenType.SEMICOLON, ";", i, linenum, str, filename);
				return i;
			}

			// check if number
			if (char.IsDigit(ch))
			{
				return lexNumber(str, i, linenum, out res);
			}

			if (ch == '(' || ch == ')')
			{
				++i;
				res = new Token(TokenType.BRACKET, ch.ToString(), i, linenum, str, filename);
				return i;
			}
			if (ch == '{' || ch == '}')
			{
				++i;
				res = new Token(TokenType.CURLY_BRACKET, ch.ToString(), i, linenum, str, filename);
				return i;
			}
			if (ch == '[' || ch == ']')
			{
				++i;
				res = new Token(TokenType.SQUARE_BRACKET, ch.ToString(), i, linenum, str, filename);
				return i;
			}

			if (ch == '\'')
			{
				return lexChar(str, i, linenum, out res);
			}

			if (ch == '"')
			{
				return lexString(str, i, linenum, out res);
			}

			if (ch == '\t')
			{
				++i;
				res = new Token(TokenType.UNIDENTIFIED, "\t", i, linenum, str, filename);
				return i;
			}

			if (ch == ',')
			{
				++i;
				res = new Token(TokenType.COMMA, ",", i, linenum, str, filename);
				return i;
			}

			return lexOp(str, i, linenum, out res);
		}

		private bool isArithLogicOp(char ch)
		{
			return (ch == '+' || ch == '-' || ch == '*' || ch == '/' ||
			        ch == '%' || ch == '~' ||
			        ch == '!' || ch == '^');
		}

		private bool getArithLogicOp(string str, ref int i, out string res)
		{
			char ch = str[i++];
			if (isArithLogicOp(ch))
			{
				res = ch.ToString();
				return true;
			}
			if (ch == '<')
			{
				char peek = str[i++];
				if (peek == '<')
				{
					res = "<<";
					return true;
				}
				if (peek == '=')
				{
					res = "<=";
					return true;
				}
				res = "<";
				--i;
				return true;
			}
			if (ch == '>')
			{
				if (i < str.Length)
				{
					char peek = str[i++];
					if (peek == '>')
					{
						res = ">>";
						return true;
					}
					if (peek == '=')
					{
						res = ">=";
						return true;
					}
					--i;
				}
				res = ">";
				return true;
			}
			res = null;
			return false;
		}

		private int lexOp(string str, int i, int linenum, out Token res)
		{
			char ch = str[i++];

			if (ch == '.')
			{
				res = new Token(TokenType.OPERATOR, ".", i, linenum, str, filename);
				return i;
			}

			if (ch == '=')
			{
				char peek = str[i++];
				if (peek == '=')
				{
					res = new Token(TokenType.OPERATOR, "==", i, linenum, str, filename);
					return i;
				}
				--i;
				res = new Token(TokenType.OPERATOR, "=", i, linenum, str, filename);
				return i;
			}
			else if (ch == '!')
			{
				char peek = str[i++];
				if (peek == '=')
				{
					res = new Token(TokenType.OPERATOR, "!=", i, linenum, str, filename);
					return i;
				}
				--i;
				res = new Token(TokenType.OPERATOR, "!", i, linenum, str, filename);
				return i;
			}
			else if (ch == '&')
			{
				char peek = str[i++];
				if (peek == '&')
				{
					res = new Token(TokenType.OPERATOR, "&&", i, linenum, str, filename);
					return i;
				}
				--i;
			}
			else if (ch == '|')
			{
				char peek = str[i++];
				if (peek == '|')
				{
					res = new Token(TokenType.OPERATOR, "||", i, linenum, str, filename);
					return i;
				}
				--i;
			}
			else if (ch == '+')
			{
				char peek = str[i++];
				if (peek == '+')
				{
					res = new Token(TokenType.OPERATOR, "++", i, linenum, str, filename);
					return i;
				}
				--i;
			}
			else if (ch == '-')
			{
				char peek = str[i++];
				if (peek == '-')
				{
					res = new Token(TokenType.OPERATOR, "--", i, linenum, str, filename);
					return i;
				}
				--i;
			}

			--i;
			string prim;
			if (!getArithLogicOp(str, ref i, out prim))
			{
				// TODO: panic
				throw new Exception();
			}
			if (i < str.Length)
			{
				char pk = str[i++];
				if (pk == '=')
				{
					res = new Token(TokenType.OPERATOR, prim + '=', i, linenum, str, filename);
					return i;
				}
				--i;
			}
			res = new Token(TokenType.OPERATOR, prim, i, linenum, str, filename);
			return i;
		}

		private int lexString(string str, int i, int linenum, out Token res)
		{
			++i; // skip <<">>

			StringBuilder sb = new StringBuilder();
			string nxt;
			do
			{
				i = nextChar(str, i, out nxt);
				sb.Append(nxt);
			}
			while (nxt != "\"");

			res = new Token(TokenType.STRING, '"' + sb.ToString(), i, linenum, str, filename);
			return i;
		}

		private int nextChar(string str, int i, out string ch)
		{
			char cha = str[i++];
			if (cha == '\\')
			{
				ch = "\\" + str[i++].ToString();
			}
			else
			{
				ch = cha.ToString();
			}
			return i;
		}

		private int lexChar(string str, int i, int linenum, out Token res)
		{
			i++; // step over <<'>>

			string chrep;
			i = nextChar(str, i, out chrep);
			if (str[i++] != '\'')
			{
				// TODO: PANIC
				throw new Exception();
			}

			res = new Token(TokenType.CHAR, '\'' + chrep + '\'', i, linenum, str, filename);
			return i;
		}

		private Token tokenForId(string str, string id, int i, int linenum)
		{
			if (id == "package")
			{
				return new Token(new[] { TokenType.DIRECTIVE, TokenType.MODIFIER }, id, i, linenum, str, filename);
			}
			TypeSystem.Modifiers mf;
			if (Enum.TryParse<TypeSystem.Modifiers>(id, true, out mf))
			{
				return new Token(TokenType.MODIFIER, id, i, linenum, str, filename);
			}
			if (id == "class" || id == "enum" || id == "struct" || id == "interface")
			{
				return new Token(TokenType.CLASS_ENUM_STRUCT, id, i, linenum, str, filename);
			}
			if (id == "if" || id == "for" || id == "while" || id == "do" || id == "try" || id == "catch" || id == "foreach")
			{
				return new Token(TokenType.IF_OR_LOOP, id, i, linenum, str, filename);
			}
			if (id == "break" || id == "continue" || id == "return" || id == "throw")
			{
				return new Token(TokenType.CONTROL_FLOW, id, i, linenum, str, filename);
			}
			if (id == "import")
			{
				return new Token(TokenType.DIRECTIVE, id, i, linenum, str, filename);
			}
			if (id == "void" || id == "byte" || id == "short" || id == "int" || id == "long" || id == "boolean" ||
			    id == "float" || id == "double" || id == "decimal" || id == "char")
			{
				return new Token(TokenType.PRIMITIVE, id, i, linenum, str, filename);
			}
			if (id == "true" || id == "false")
			{
				return new Token(TokenType.BOOLEAN, id, i, linenum, str, filename);
			}
			if (id == "null")
			{
				return new Token(TokenType.NULL, id, i, linenum, str, filename);
			}
			if (id == "sizeof" || id == "typeof")
			{
				return new Token(TokenType.OPERATOR, id, i, linenum, str, filename);
			}
			return new Token(TokenType.IDENTIFIER, id, i, linenum, str, filename);
		}

		private int lexId(string str, int i, int linenum, out Token res)
		{
			StringBuilder sb = new StringBuilder();
			for (; i < str.Length && isIdValidChar(str[i]); ++i)
			{
				sb.Append(str[i]);
			}
			res = tokenForId(str, sb.ToString(), i, linenum);
			return i;
		}

		private bool isHexDigit(char ch)
		{
			return char.IsDigit(ch) || ch == 'a' || ch == 'A'
				|| ch == 'b' || ch == 'B'
					|| ch == 'c' || ch == 'C'
					|| ch == 'd' || ch == 'D'
					|| ch == 'e' || ch == 'E'
					|| ch == 'f' || ch == 'F';
		}

		private int lexNumber(string str, int i, int linenum, out Token res)
		{
			StringBuilder sb = new StringBuilder();
			TokenType tty = TokenType.DEC_INTEGER;

			for (; char.IsDigit(str[i]); ++i)
			{
				sb.Append(str[i]);
			}
			if (str[i] == 'x' && sb.Length == 1 && str[i - 1] == '0') // hex
			{
				sb.Append("x");
				++i;
				for (; isHexDigit(str[i]); ++i)
				{
					sb.Append(str[i]);
				}
				tty = TokenType.HEX_INTEGER;
			}
			else if (str[i] == 'b' && sb.Length == 1 && str[i - 1] == '0')
			{
				sb.Append("b");
				++i;
				for (; str[i] == '0' || str[i] == '1'; ++i)
				{
					sb.Append(str[i]);
				}
				tty = TokenType.BIN_INTEGER;
			}
			else if (str[i] == '.') // floating point number
			{
				sb.Append(".");
				++i;
				for (; char.IsDigit(str[i]); ++i)
				{
					sb.Append(str[i]);
				}

				tty = TokenType.DOUBLE;

				if (str[i] == 'f') // float
				{
					sb.Append("f");
					++i;
					tty = TokenType.FLOAT;
				}
			}

			if (sb[0] == '0' && sb.Length > 1 && tty == TokenType.DEC_INTEGER)
			{
				tty = TokenType.OCT_INTEGER;
			}

			res = new Token(tty, sb.ToString(), i, linenum, str, filename);
			return i;
		}
	}
}

