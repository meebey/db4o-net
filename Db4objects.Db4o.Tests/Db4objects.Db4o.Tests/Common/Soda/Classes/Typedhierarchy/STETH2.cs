namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STETH2 : Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STETH1TestCase
	{
		public string foo2;

		public STETH2()
		{
		}

		public STETH2(string str1, string str2) : base(str1)
		{
			foo2 = str2;
		}
	}
}
