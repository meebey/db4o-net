namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	public class STETH3 : Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STETH2
	{
		public string foo3;

		public STETH3()
		{
		}

		public STETH3(string str1, string str2, string str3) : base(str1, str2)
		{
			foo3 = str3;
		}
	}
}
