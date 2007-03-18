namespace Db4objects.Db4o.Tests.Common.Regression
{
	/// <exclude></exclude>
	public class COR234TestCase : Db4oUnit.ITestCase
	{
		public virtual void Test()
		{
			if (Db4objects.Db4o.Tests.Util.WorkspaceServices.WorkspaceRoot == null)
			{
				Sharpen.Runtime.Err.WriteLine("Build environment not available. Skipping test case..."
					);
				return;
			}
			Db4objects.Db4o.Db4oFactory.Configure().AllowVersionUpdates(false);
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Ext.OldFormatException), new _AnonymousInnerClass24
				(this));
		}

		private sealed class _AnonymousInnerClass24 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass24(COR234TestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Db4oFactory.OpenFile(this._enclosing.OldDatabaseFilePath());
			}

			private readonly COR234TestCase _enclosing;
		}

		protected virtual string OldDatabaseFilePath()
		{
			string oldFile = Db4objects.Db4o.Tests.Util.IOServices.BuildTempPath("old_db.yap"
				);
			Db4objects.Db4o.Foundation.IO.File4.Copy(Db4objects.Db4o.Tests.Util.WorkspaceServices
				.WorkspaceTestFilePath("db4oVersions/db4o_3.0.3"), oldFile);
			return oldFile;
		}
	}
}
