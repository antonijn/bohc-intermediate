using System;
using Bohc.General;

namespace Bohc.Parsing
{
	public class TokenException : Exception
	{
		public Token token;
		public string msg;
		private ErrorManager e;

		public TokenException(ErrorManager e, Token token, string frmt, params object[] args)
		{
			this.token = token;
			this.msg = string.Format(frmt, args);
			this.e = e;
		}

		public void display()
		{
			token.error(e, msg);
		}
	}
}

