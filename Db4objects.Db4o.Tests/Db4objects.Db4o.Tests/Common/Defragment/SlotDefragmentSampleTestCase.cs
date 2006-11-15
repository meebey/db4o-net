namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class SlotDefragmentSampleTestCase : Db4oUnit.ITestLifeCycle
	{
		private static readonly string SOURCEFILE = "original.yap";

		private static readonly string TARGETFILE = "copied.yap";

		private const int NUM_ENTRIES = 5;

		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(new Db4oUnit.ReflectionTestSuiteBuilder(typeof(Db4objects.Db4o.Tests.Common.Defragment.SlotDefragmentSampleTestCase)
				)).Run();
		}

		public virtual void SetUp()
		{
			CreateSource(SOURCEFILE);
			DeleteFile(TARGETFILE);
		}

		public virtual void TearDown()
		{
		}

		public virtual void TestDefrag()
		{
			long start = Sharpen.Runtime.CurrentTimeMillis();
			Db4objects.Db4o.Defragment.Defragment.Defrag(new Db4objects.Db4o.Defragment.DefragmentConfig
				(SOURCEFILE, TARGETFILE));
			Sharpen.Runtime.Out.WriteLine("TIME " + (Sharpen.Runtime.CurrentTimeMillis() - start
				) + " ms");
			CheckCopied();
		}

		private static Db4objects.Db4o.Config.IConfiguration Config()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			Db4objects.Db4o.Config.IObjectClass clazz = config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Defragment.Data)
				);
			clazz.ObjectField("_id").Indexed(true);
			clazz.ObjectField("_name").Indexed(true);
			return config;
		}

		private static void CreateSource(string fileName)
		{
			DeleteFile(fileName);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Config
				(), fileName);
			Db4objects.Db4o.Tests.Common.Defragment.Data data = null;
			for (int i = 0; i < NUM_ENTRIES; i++)
			{
				string name = "X" + i;
				data = new Db4objects.Db4o.Tests.Common.Defragment.Data(i, name, data, new Db4objects.Db4o.Tests.Common.Defragment.Data
					[] { data, data });
				db.Set(data);
				if (i % 100000 == 0)
				{
					db.Commit();
				}
			}
			CheckStoredClasses(db);
			db.Close();
		}

		private static void DeleteFile(string path)
		{
			new Sharpen.IO.File(path).Delete();
		}

		private static void CheckCopied()
		{
			try
			{
				Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(Config
					(), TARGETFILE);
				Sharpen.Runtime.Out.WriteLine("IDENTITY: " + db.Ext().Identity());
				try
				{
					Sharpen.Runtime.Out.WriteLine(db);
					CheckStoredClasses(db);
					CheckQuery(db);
				}
				finally
				{
					db.Close();
				}
			}
			catch (System.Exception exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
		}

		private static void CheckQuery(Db4objects.Db4o.IObjectContainer db)
		{
			Db4objects.Db4o.IObjectSet all = db.Query((System.Type)null);
			Sharpen.Runtime.Out.WriteLine(all.Size());
			while (all.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Defragment.Data data = (Db4objects.Db4o.Tests.Common.Defragment.Data
					)all.Next();
				Sharpen.Runtime.Out.WriteLine(data + " <- " + data._previous);
			}
		}

		private static void CheckStoredClasses(Db4objects.Db4o.IObjectContainer db)
		{
			Sharpen.Runtime.Out.WriteLine("CLASS COLLECTION: " + ((Db4objects.Db4o.YapFile)db
				).ClassCollection().GetID());
			Db4objects.Db4o.Ext.IStoredClass[] storedClasses = db.Ext().StoredClasses();
			Sharpen.Runtime.Out.WriteLine("STORED CLASSES: " + storedClasses.Length);
			for (int classIdx = 0; classIdx < storedClasses.Length; classIdx++)
			{
				Db4objects.Db4o.Ext.IStoredClass curClass = storedClasses[classIdx];
				long[] ids = curClass.GetIDs();
				Sharpen.Runtime.Out.WriteLine(curClass.GetName() + " : (" + ((Db4objects.Db4o.YapClass
					)curClass).GetID() + ") " + ids.Length);
				System.Text.StringBuilder fieldList = new System.Text.StringBuilder();
				Db4objects.Db4o.Ext.IStoredField[] fields = curClass.GetStoredFields();
				for (int fieldIdx = 0; fieldIdx < fields.Length; fieldIdx++)
				{
					if (fieldList.Length > 0)
					{
						fieldList.Append(',');
					}
					Db4objects.Db4o.Ext.IStoredField curField = fields[fieldIdx];
					Db4objects.Db4o.Reflect.IReflectClass curType = curField.GetStoredType();
					fieldList.Append(curField.GetName() + ":" + (curType == null ? "?" : curType.GetName
						()));
					fieldList.Append(":" + ((Db4objects.Db4o.YapField)curField).HasIndex());
				}
				Sharpen.Runtime.Out.WriteLine(fieldList);
				for (int idIdx = 0; idIdx < ids.Length; idIdx++)
				{
					object obj = db.Ext().GetByID(ids[idIdx]);
					db.Ext().Activate(obj, int.MaxValue);
					Sharpen.Runtime.Out.WriteLine(ids[idIdx] + ": " + obj + (obj == null ? string.Empty
						 : " / " + obj.GetType()));
				}
			}
		}
	}
}
