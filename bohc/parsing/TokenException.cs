using System;

namespace Bohc.Parsing
{
	public class TokenException : Exception
	{
		public Token token;
		public string msg;

		public TokenException(Token token, string frmt, params object[] args)
		{
			this.token = token;
			this.msg = string.Format(frmt, args);
		}

		public void display()
		{
			token.error(msg);
		}
	}
}

