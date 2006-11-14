namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CloseUnlocksFileTestCase : Db4oUnit.ITestCase
	{
		private static readonly string FILE = "unlocked.db4o";

		public virtual void Test()
		{
			Db4objects.Db4o.Foundation.IO.File4.Delete(FILE);
			Db4oUnit.Assert.IsFalse(System.IO.File.Exists(FILE));
			Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4o.OpenFile(FILE);
			oc.Close();
			Db4objects.Db4o.Foundation.IO.File4.Delete(FILE);
			Db4oUnit.Assert.IsFalse(System.IO.File.Exists(FILE));
		}
	}
}
