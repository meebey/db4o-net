namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentTestCase : Db4oUnit.ITestLifeCycle
	{
		private static readonly string PRIMITIVE_FIELDNAME = "_id";

		private static readonly string WRAPPER_FIELDNAME = "_wrapper";

		private static readonly string TYPEDOBJECT_FIELDNAME = "_next";

		public class Data
		{
			public int _id;

			public int _wrapper;

			public Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data _next;

			public Data(int id, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data
				 next)
			{
				_id = id;
				_wrapper = id;
				_next = next;
			}
		}

		private const int VALUE = 42;

		public virtual void TestPrimitiveIndex()
		{
			AssertIndex(PRIMITIVE_FIELDNAME);
		}

		public virtual void TestWrapperIndex()
		{
			AssertIndex(WRAPPER_FIELDNAME);
		}

		public virtual void TestTypedObjectIndex()
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
			query.Descend(TYPEDOBJECT_FIELDNAME).Descend(PRIMITIVE_FIELDNAME).Constrain(VALUE
				);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public virtual void TestNoForceDelete()
		{
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.BACKUPFILENAME
				);
			Db4oUnit.Assert.Expect(typeof(System.IO.IOException), new _AnonymousInnerClass82(
				this));
		}

		private sealed class _AnonymousInnerClass82 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass82(SlotDefragmentTestCase _enclosing)
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
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data data = null;
			for (int value = VALUE - 1; value <= VALUE + 1; value++)
			{
				data = new Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data(value
					, data);
				db.Set(data);
			}
			db.Close();
		}

		private void ForceIndex()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).ObjectField(PRIMITIVE_FIELDNAME).Indexed(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).ObjectField(WRAPPER_FIELDNAME).Indexed(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).ObjectField(TYPEDOBJECT_FIELDNAME).Indexed(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(config
				, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentConstants.FILENAME);
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).StoredField(PRIMITIVE_FIELDNAME, typeof(int)).HasIndex());
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).StoredField(WRAPPER_FIELDNAME, typeof(int)).HasIndex());
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).StoredField(TYPEDOBJECT_FIELDNAME, typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestCase.Data)
				).HasIndex());
			db.Close();
		}

		private void AssertIndex(string fieldName)
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
			query.Descend(fieldName).Constrain(VALUE);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
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
