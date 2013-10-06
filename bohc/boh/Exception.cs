
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
				T exc = new T();
				exc.whatsWrong = msg;
				throw exc;
			}
		}
	}
}
