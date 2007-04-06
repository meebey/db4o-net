using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CloseUnlocksFileTestCase : ITestCase
	{
		private static readonly string FILE = "unlocked.db4o";

		public virtual void Test()
		{
			File4.Delete(FILE);
			Assert.IsFalse(System.IO.File.Exists(FILE));
			IObjectContainer oc = Db4oFactory.OpenFile(FILE);
			oc.Close();
			File4.Delete(FILE);
			Assert.IsFalse(System.IO.File.Exists(FILE));
		}
	}
}
