using System;

namespace Bohc.Parsing
{
	public class TokenRange
	{
		public readonly Token first;
		public readonly Token last;
		public readonly string str;

		public TokenRange(Token first, Token last, string str)
		{
			this.first = first;
			this.last = last;
			this.str = str;
		}

		public void error(string format, params object[] args)
		{
			display("ERROR", string.Format(format, args), ConsoleColor.Red);
		}

		public void warning(string format, params object[] args)
		{
			display("warning", string.Format(format, args), ConsoleColor.DarkYellow);
		}

		public void hint(string format, params object[] args)
		{
			display("hint", string.Format(format, args), ConsoleColor.DarkCyan);
		}

		public void display(string msg, string msg2, ConsoleColor cc)
		{
			Console.Write("\x1b[1m{0}:{1}:{2}: ", first.filename, first.linenum, first.column);

			ConsoleColor tmp = Console.ForegroundColor;
			Console.ForegroundColor = cc;
			Console.Write("{0}:", msg);
			Console.ForegroundColor = tmp;

			Console.WriteLine(" {0}\x1b[0m", msg2);
			//Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(first.line);
			for (int i = 0; i < first.column - 1; ++i)
			{
				if (first.line[i] == '\t')
				{
					Console.Write("\t");
				}
				else
				{
					Console.Write(" ");
				}
			}
			//Console.Write("^");
			Console.ForegroundColor = cc;
			for (int i = 0; i < last.column + last.value.Length - first.column; ++i)
			{
				Console.Write("^");
			}
			Console.ForegroundColor = tmp;
			Console.WriteLine();
		}
	}
}

