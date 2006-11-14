namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : Db4oUnit.ITestLifeCycle
	{
		public static readonly string FILENAME = "defrag.yap";

		public static readonly string BACKUPFILENAME = FILENAME + ".backup";

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				_id = id;
			}
		}

		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(FILENAME, BACKUPFILENAME);
			Db4oUnit.Assert.Expect(typeof(System.IO.IOException), new _AnonymousInnerClass53(
				this));
		}

		private sealed class _AnonymousInnerClass53 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass53(SlotDefragmentTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase
					.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.BACKUPFILENAME
					);
			}

			private readonly SlotDefragmentTestCase _enclosing;
		}

		public virtual void SetUp()
		{
			new Sharpen.IO.File(FILENAME).Delete();
			new Sharpen.IO.File(BACKUPFILENAME).Delete();
			CreateFile(FILENAME);
		}

		public virtual void TearDown()
		{
		}

		private Db4objects.Db4o.Defragment.DefragmentConfig DefragConfig(bool forceBackupDelete
			)
		{
			Db4objects.Db4o.Defragment.DefragmentConfig defragConfig = new Db4objects.Db4o.Defragment.DefragmentConfig
				(FILENAME, BACKUPFILENAME);
			defragConfig.ForceBackupDelete(forceBackupDelete);
			return defragConfig;
		}

		private void CreateFile(string fileName)
		{
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4o.OpenFile(Db4objects.Db4o.Db4o
				.NewConfiguration(), fileName);
			db.Set(new Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data(42
				));
			db.Close();
		}

		private void AssertDataClassKnown(bool expected)
		{
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4o.OpenFile(Db4objects.Db4o.Db4o
				.NewConfiguration(), FILENAME);
			try
			{
				Db4objects.Db4o.Ext.IStoredClass storedClass = db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
					);
				if (expected)
				{
					Db4oUnit.Assert.IsNotNull(storedClass);
				}
				else
				{
					Db4oUnit.Assert.IsNull(storedClass);
				}
			}
			finally
			{
				db.Close();
			}
		}
	}
}
