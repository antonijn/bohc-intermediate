using System;
using System.Linq;
using System.Collections.Generic;
using Bohc.General;

namespace Bohc.Parsing
{
	public struct Token
	{
		public TokenType[] tokentype;
		public string value;
		public int column;
		public int linenum;
		public string line;
		public string filename;

		public bool isType(TokenType tt)
		{
			return tokentype.Any(x => x == tt);
		}

		public Token(TokenType tt, string v, int c, int l, string ll, string fn)
		{
			tokentype = new[] { tt };
			value = v;
			column = c - v.Length + 1;
			linenum = l + 1;
			line = ll;
			filename = fn;
		}
		public Token(TokenType[] tt, string v, int c, int l, string ll, string fn)
		{
			tokentype = tt;
			value = v;
			column = c - v.Length + 1;
			linenum = l + 1;
			line = ll;
			filename = fn;
		}

		public override string ToString()
		{
			return "[ " + linenum + ":" + column + ": " + tokentype[0].ToString() + ": \"" + value + "\" ]";
		}

		public void error(ErrorManager e, string format, params object[] args)
		{
			new TokenRange(this, this, value).error(e, format, args);
		}

		public void warning(ErrorManager e, string format, params object[] args)
		{
			new TokenRange(this, this, value).warning(e, format, args);
		}

		public void hint(ErrorManager e, string format, params object[] args)
		{
			new TokenRange(this, this, value).hint(e, format, args);
		}

		public void display(string msg, string msg2, ConsoleColor cc)
		{
			new TokenRange(this, this, value).display(msg, msg2, cc);
		}
	}
}

