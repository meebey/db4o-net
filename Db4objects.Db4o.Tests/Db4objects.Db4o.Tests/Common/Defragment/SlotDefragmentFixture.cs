namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentFixture
	{
		public static readonly string PRIMITIVE_FIELDNAME = "_id";

		public static readonly string WRAPPER_FIELDNAME = "_wrapper";

		public static readonly string TYPEDOBJECT_FIELDNAME = "_next";

		public class Data
		{
			public int _id;

			public int _wrapper;

			public Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data _next;

			public Data(int id, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data
				 next)
			{
				_id = id;
				_wrapper = id;
				_next = next;
			}
		}

		public const int VALUE = 42;

		public static Db4objects.Db4o.Defragment.DefragmentConfig DefragConfig(bool forceBackupDelete
			)
		{
			Db4objects.Db4o.Defragment.DefragmentConfig defragConfig = new Db4objects.Db4o.Defragment.DefragmentConfig
				(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.BACKUPFILENAME);
			defragConfig.ForceBackupDelete(forceBackupDelete);
			return defragConfig;
		}

		public static void CreateFile(string fileName)
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(config
				, fileName);
			Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data data = null;
			for (int value = VALUE - 1; value <= VALUE + 1; value++)
			{
				data = new Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data(value
					, data);
				db.Set(data);
			}
			db.Close();
		}

		public static void ForceIndex()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).ObjectField(PRIMITIVE_FIELDNAME).Indexed(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).ObjectField(WRAPPER_FIELDNAME).Indexed(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).ObjectField(TYPEDOBJECT_FIELDNAME).Indexed(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(config
				, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.FILENAME);
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).StoredField(PRIMITIVE_FIELDNAME, typeof(int)).HasIndex());
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).StoredField(WRAPPER_FIELDNAME, typeof(int)).HasIndex());
			Db4oUnit.Assert.IsTrue(db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).StoredField(TYPEDOBJECT_FIELDNAME, typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				).HasIndex());
			db.Close();
		}

		public static void AssertIndex(string fieldName)
		{
			ForceIndex();
			Db4objects.Db4o.Defragment.Defragment.Defrag(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME, Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants.BACKUPFILENAME
				);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Db4oFactory
				.NewConfiguration(), Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME);
			Db4objects.Db4o.Query.IQuery query = db.Query();
			query.Constrain(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
				);
			query.Descend(fieldName).Constrain(VALUE);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public static void AssertDataClassKnown(bool expected)
		{
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Db4objects.Db4o.Db4oFactory
				.NewConfiguration(), Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentTestConstants
				.FILENAME);
			try
			{
				Db4objects.Db4o.Ext.IStoredClass storedClass = db.Ext().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentFixture.Data)
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
