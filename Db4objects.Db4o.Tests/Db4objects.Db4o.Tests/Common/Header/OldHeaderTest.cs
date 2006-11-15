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
			if (!System.IO.File.Exists(ORIGINAL_FILE))
			{
				Db4oUnit.TestPlatform.EmitWarning(ORIGINAL_FILE + " does not exist. Can not run "
					 + GetType().FullName);
				return;
			}
			Db4objects.Db4o.Foundation.IO.File4.Copy(ORIGINAL_FILE, DB_FILE);
			Db4objects.Db4o.Db4oFactory.Configure().AllowVersionUpdates(true);
			Db4objects.Db4o.IObjectContainer oc = Db4objects.Db4o.Db4oFactory.OpenFile(DB_FILE
				);
			try
			{
				Db4oUnit.Assert.IsNotNull(oc);
			}
			finally
			{
				oc.Close();
				Db4objects.Db4o.Db4oFactory.Configure().AllowVersionUpdates(false);
			}
		}
	}
}
