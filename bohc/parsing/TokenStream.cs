using System;
using System.Linq;
using System.Collections.Generic;

namespace Bohc.Parsing
{
	public class TokenStream
	{
		private IEnumerable<Token> tokens;
		private int _offset;
		private int length;
		private int _size;

		public TokenStream(IEnumerable<Token> tokens, int offset, int length)
		{
			this.tokens = tokens;
			this._offset = offset - 1;
			this.length = length + 1;
			this._size = length;
		}

		public TokenStream fork()
		{
			return new TokenStream(tokens, _offset, length);
		}

		public Token get()
		{
			return tokens.Skip(_offset).First();
		}

		public Token peek(int c)
		{
			if (c >= length)
			{
				return default(Token);
			}
			return tokens.Skip(c + _offset).First();
		}

		public bool next()
		{
			++_offset;
			--length;
			return length > 0;
		}

		public bool prior()
		{
			--_offset;
			++length;
			return _offset >= 0;
		}

		public TokenStream until(string str, params Tuple<string, string>[] scopetokens)
		{
			Dictionary<string, int> scopes = new Dictionary<string, int>();
			foreach (var st in scopetokens)
			{
				scopes[st.Item1] = 0;
			}
			for (int i = 0; i < length; ++i)
			{
				Token t = peek(i);

				if (t.value == str && !scopes.Any(x => x.Value > 0))
				{
					TokenStream result = read(0, i);
					next();
					return result;
				}

				Tuple<string, string> tmp;
				if ((tmp = scopetokens.SingleOrDefault(x => x.Item1 == t.value)) != null)
				{
					++scopes[tmp.Item1];
				}
				else if ((tmp = scopetokens.SingleOrDefault(x => x.Item2 == t.value)) != null)
				{
					--scopes[tmp.Item1];
				}
			}

			if (!scopes.Any(x => x.Value > 0))
			{
				return read(0, length);
			}

			// TODO: panic
			throw new Exception();
		}

		public TokenStream read(int offs, int c)
		{
			TokenStream result = new TokenStream(tokens, _offset + offs, c);
			_offset += offs + c;
			length -= offs + c;
			return result;
		}

		public IEnumerable<TokenStream> split(string str, params Tuple<string, string>[] scopetokens)
		{
			while (true)
			{
				TokenStream tstr = null;
				try
				{
					tstr = until(str, scopetokens);
					if (tstr.size() == 0)
					{
						break;
					}
				}
				catch
				{
					break;
				}
				yield return tstr;
			}
			if (length > 0)
			{
				yield return read(0, length);
			}
		}

		public int size()
		{
			return _size;
		}

		public int offset()
		{
			return _offset;
		}

		public override string ToString()
		{
			return get().ToString();
		}
	}
}

