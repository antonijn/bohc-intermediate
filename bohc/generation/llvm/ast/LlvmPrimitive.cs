using System;

namespace Bohc.Generation.Llvm
{
	public class LlvmPrimitive : LlvmType
	{
		private string name;

		public LlvmPrimitive(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}

		public override int size(General.Platform p)
		{
			switch (name)
			{
				case "i1":
					return 1;
				case "i8":
					return 1;
				case "i16":
					return 2;
				case "i32":
					return 4;
				case "i64":
					return 8;
				case "float":
					return 4;
				case "double":
					return 8;
			}

			throw new NotImplementedException();
		}
		public override int padding(Bohc.General.Platform p)
		{
			if (name == "double" && p == General.Platform.Pf_Linux32)
			{
				return 4;
			}
			return size(p);
		}
	}
}

