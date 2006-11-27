namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : Db4oUnit.ITestLifeCycle
	{
		private static readonly string FIELDNAME = "_id";

		public class Data
		{
			public int _id;

			public Data(int id)
			{
				_id = id;
			}
		}

		private const int VALUE = 42;

		public virtual void _testPrimitiveIndex()
		{
			ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.BACKUPFILENAME
				);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Db4oFactory
				.NewConfiguration(), Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME);
			Db4objects.Db4o.Query.IQuery query = db.Query();
			query.Constrain(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				);
			query.Descend(FIELDNAME).Constrain(VALUE);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.BACKUPFILENAME
				);
			Db4oUnit.Assert.Expect(typeof(System.IO.IOException), new _AnonymousInnerClass68(
				this));
		}

		private sealed class _AnonymousInnerClass68 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass68(SlotDefragmentTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
					.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.BACKUPFILENAME
					);
			}

			private readonly SlotDefragmentTestCase _enclosing;
		}

		public virtual void SetUp()
		{
			new Sharpen.IO.File(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME).Delete();
			new Sharpen.IO.File(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.BACKUPFILENAME).Delete();
			CreateFile(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.FILENAME
				);
		}

		public virtual void TearDown()
		{
		}

		private Db4objects.Db4o.Defragment.DefragmentConfig DefragConfig(bool forceBackupDelete
			)
		{
			Db4objects.Db4o.Defragment.DefragmentConfig defragConfig = new Db4objects.Db4o.Defragment.DefragmentConfig
				(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.BACKUPFILENAME);
			defragConfig.ForceBackupDelete(forceBackupDelete);
			return defragConfig;
		}

		private void CreateFile(string fileName)
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(config
				, fileName);
			db.Set(new Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data(VALUE
				));
			db.Close();
		}

		private void ForceIndex()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).ObjectField(FIELDNAME).Indexed(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(config
				, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.FILENAME);
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).StoredField("_id", typeof(int)).HasIndex());
			db.Close();
		}

		private void AssertDataClassKnown(bool expected)
		{
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Db4oFactory
				.NewConfiguration(), Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME);
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
