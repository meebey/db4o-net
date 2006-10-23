namespace Db4objects.Db4o.Tests.Common.Header
{
	public class OldHeaderTest : Db4oUnit.ITestCase
	{
		private static readonly string ORIGINAL_FILE = Db4objects.Db4o.Tests.Util.WorkspaceServices
			.WorkspacePath("db4oj.tests/test/db4oVersions/db4o_5.5.2");

		private static readonly string DB_FILE = Db4objects.Db4o.Tests.Util.WorkspaceServices
			.WorkspacePath("db4oj.tests/test/db4oVersions/db4o_5.5.2.yap");

		public virtual void Test()
		{
			System.IO.File.Delete(DB_FILE);
			System.IO.File.Copy(ORIGINAL_FILE, DB_FILE);
			Db4objects.Db4o.Db4o.Configure().AllowVersionUpdates(true);
			Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4o.OpenFile(DB_FILE);
			try
			{
				Db4oUnit.Assert.IsNotNull(oc);
			}
			finally
			{
				oc.Close();
				Db4objects.Db4o.Db4o.Configure().AllowVersionUpdates(false);
			}
		}
	}
}
