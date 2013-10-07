﻿
namespace bohc.boh
{
	public class Exception : System.Exception
	{
		public string whatsWrong;

		public override string Message
		{
			get { return whatsWrong; }
		}

		public static void require<T>(bool cond, string msg)
			where T : boh.Exception, new()
		{
			if (!cond)
			{
				_throw<T>(msg);
			}
		}

		public static void _throw<T>(string msg)
			where T : boh.Exception, new()
		{
			T exc = new T();
			exc.whatsWrong = msg;
			throw exc;
		}

		public static void warn(string msg)
		{
			System.Console.Error.WriteLine("[{0}] {1}", System.DateTime.Now, msg);
		}

		public static void warnIf(bool cond, string msg)
		{
			if (cond)
			{
				warn(msg);
			}
		}
	}
}
