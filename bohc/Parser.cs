using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bohc
{
	public static class Parser
	{
		private static int getMatchingBraceCh(string str, int first, char matches, int step)
		{
			char startscope = str[first];
			int scope = 0;
			int i;
			for (i = first + 1; scope != -1; i += step)
			{
				char ch = str[i];
				if (ch == startscope)
				{
					++scope;
				}
				else if (ch == matches)
				{
					--scope;
				}
			}

			return i;
		}

		public static int getMatchingBraceChar(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, 1);
		}

		public static int getMatchingBraceCharBackwards(string str, int first, char matches)
		{
			return getMatchingBraceCh(str, first, matches, -1);
		}
	}
}
