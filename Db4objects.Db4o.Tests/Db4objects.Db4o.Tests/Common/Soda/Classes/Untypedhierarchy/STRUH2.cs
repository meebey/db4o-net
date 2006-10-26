namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy
{
	public class STRUH2
	{
		public object parent;

		public object h3;

		public string foo2;

		public STRUH2()
		{
		}

		public STRUH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STRUH3 a3
			)
		{
			h3 = a3;
			a3.parent = this;
		}

		public STRUH2(string str)
		{
			foo2 = str;
		}

		public STRUH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STRUH3 a3
			, string str)
		{
			h3 = a3;
			a3.parent = this;
			foo2 = str;
		}
	}
}
