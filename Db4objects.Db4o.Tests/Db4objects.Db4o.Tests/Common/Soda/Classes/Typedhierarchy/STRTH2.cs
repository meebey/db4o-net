namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STRTH2
	{
		public Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STRTH1TestCase parent;

		public Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STRTH3 h3;

		public string foo2;

		public STRTH2()
		{
		}

		public STRTH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STRTH3 a3)
		{
			h3 = a3;
			a3.parent = this;
		}

		public STRTH2(string str)
		{
			foo2 = str;
		}

		public STRTH2(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STRTH3 a3, 
			string str)
		{
			h3 = a3;
			a3.parent = this;
			foo2 = str;
		}
	}
}
