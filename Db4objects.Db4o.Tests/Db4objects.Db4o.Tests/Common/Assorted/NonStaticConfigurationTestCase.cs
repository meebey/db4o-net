namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class NonStaticConfigurationTestCase : Db4oUnit.ITestCase
	{
		public class Data
		{
			public int id;

			public Data(int id)
			{
				this.id = id;
			}
		}

		private static readonly string FILENAME = "nonstaticcfg.yap";

		public virtual void TestOpenWithNonStaticConfiguration()
		{
			new Sharpen.IO.File(FILENAME).Delete();
			Db4objects.Db4o.Config.IConfiguration cfg = Db4objects.Db4o.Db4o.NewConfiguration
				();
			cfg.ReadOnly(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4o.OpenFile(cfg, FILENAME
				);
			try
			{
				db.Set(new Db4objects.Db4o.Tests.Common.Assorted.NonStaticConfigurationTestCase.Data
					(1));
			}
			finally
			{
				db.Close();
			}
			cfg = Db4objects.Db4o.Db4o.NewConfiguration();
			db = Db4objects.Db4o.Db4o.OpenFile(cfg, FILENAME);
			try
			{
				db.Set(new Db4objects.Db4o.Tests.Common.Assorted.NonStaticConfigurationTestCase.Data
					(2));
				Db4oUnit.Assert.AreEqual(1, db.Query(typeof(Db4objects.Db4o.Tests.Common.Assorted.NonStaticConfigurationTestCase.Data)
					).Size());
			}
			finally
			{
				db.Close();
			}
		}
	}
}
