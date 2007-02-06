namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STETH4 : Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STETH2
	{
		public string foo4;

		public STETH4()
		{
		}

		public STETH4(string str1, string str2, string str3) : base(str1, str2)
		{
			foo4 = str3;
		}
	}
}
