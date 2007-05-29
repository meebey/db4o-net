/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;

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

			public SlotDefragmentFixture.Data _next;

			public Data(int id, SlotDefragmentFixture.Data next)
			{
				_id = id;
				_wrapper = id;
				_next = next;
			}
		}

		public const int VALUE = 42;

		public static DefragmentConfig DefragConfig(bool forceBackupDelete)
		{
			DefragmentConfig defragConfig = new DefragmentConfig(SlotDefragmentTestConstants.
				FILENAME, SlotDefragmentTestConstants.BACKUPFILENAME);
			defragConfig.ForceBackupDelete(forceBackupDelete);
			defragConfig.Db4oConfig(Db4oConfig());
			return defragConfig;
		}

		public static IConfiguration Db4oConfig()
		{
			IConfiguration db4oConfig = Db4oFactory.NewConfiguration();
			db4oConfig.ReflectWith(Platform4.ReflectorForType(typeof(SlotDefragmentFixture.Data)
				));
			return db4oConfig;
		}

		public static void CreateFile(string fileName)
		{
			IObjectContainer db = Db4oFactory.OpenFile(Db4oConfig(), fileName);
			SlotDefragmentFixture.Data data = null;
			for (int value = VALUE - 1; value <= VALUE + 1; value++)
			{
				data = new SlotDefragmentFixture.Data(value, data);
				db.Set(data);
			}
			db.Close();
		}

		public static void ForceIndex()
		{
			IConfiguration config = Db4oConfig();
			config.ObjectClass(typeof(SlotDefragmentFixture.Data)).ObjectField(PRIMITIVE_FIELDNAME
				).Indexed(true);
			config.ObjectClass(typeof(SlotDefragmentFixture.Data)).ObjectField(WRAPPER_FIELDNAME
				).Indexed(true);
			config.ObjectClass(typeof(SlotDefragmentFixture.Data)).ObjectField(TYPEDOBJECT_FIELDNAME
				).Indexed(true);
			IObjectContainer db = Db4oFactory.OpenFile(config, SlotDefragmentTestConstants.FILENAME
				);
			Assert.IsTrue(db.Ext().StoredClass(typeof(SlotDefragmentFixture.Data)).StoredField
				(PRIMITIVE_FIELDNAME, typeof(int)).HasIndex());
			Assert.IsTrue(db.Ext().StoredClass(typeof(SlotDefragmentFixture.Data)).StoredField
				(WRAPPER_FIELDNAME, typeof(int)).HasIndex());
			Assert.IsTrue(db.Ext().StoredClass(typeof(SlotDefragmentFixture.Data)).StoredField
				(TYPEDOBJECT_FIELDNAME, typeof(SlotDefragmentFixture.Data)).HasIndex());
			db.Close();
		}

		public static void AssertIndex(string fieldName)
		{
			ForceIndex();
			DefragmentConfig defragConfig = new DefragmentConfig(SlotDefragmentTestConstants.
				FILENAME, SlotDefragmentTestConstants.BACKUPFILENAME);
			defragConfig.Db4oConfig(Db4oConfig());
			Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
			IObjectContainer db = Db4oFactory.OpenFile(Db4oConfig(), SlotDefragmentTestConstants
				.FILENAME);
			IQuery query = db.Query();
			query.Constrain(typeof(SlotDefragmentFixture.Data));
			query.Descend(fieldName).Constrain(VALUE);
			IObjectSet result = query.Execute();
			Assert.AreEqual(1, result.Size());
			db.Close();
		}

		public static void AssertDataClassKnown(bool expected)
		{
			IObjectContainer db = Db4oFactory.OpenFile(Db4oConfig(), SlotDefragmentTestConstants
				.FILENAME);
			try
			{
				IStoredClass storedClass = db.Ext().StoredClass(typeof(SlotDefragmentFixture.Data)
					);
				if (expected)
				{
					Assert.IsNotNull(storedClass);
				}
				else
				{
					Assert.IsNull(storedClass);
				}
			}
			finally
			{
				db.Close();
			}
		}
	}
}
