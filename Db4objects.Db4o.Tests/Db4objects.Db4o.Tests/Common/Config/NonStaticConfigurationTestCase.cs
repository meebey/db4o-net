namespace Db4objects.Db4o.Tests.Common.Config
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
			Db4objects.Db4o.Config.IConfiguration cfg = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			cfg.ReadOnly(true);
			Db4objects.Db4o.IObjectContainer db = Db4objects.Db4o.Db4oFactory.OpenFile(cfg, FILENAME
				);
			try
			{
				db.Set(new Db4objects.Db4o.Tests.Common.Config.NonStaticConfigurationTestCase.Data
					(1));
			}
			finally
			{
				db.Close();
			}
			cfg = Db4objects.Db4o.Db4oFactory.NewConfiguration();
			db = Db4objects.Db4o.Db4oFactory.OpenFile(cfg, FILENAME);
			try
			{
				db.Set(new Db4objects.Db4o.Tests.Common.Config.NonStaticConfigurationTestCase.Data
					(2));
				Db4oUnit.Assert.AreEqual(1, db.Query(typeof(Db4objects.Db4o.Tests.Common.Config.NonStaticConfigurationTestCase.Data)
					).Size());
			}
			finally
			{
				db.Close();
			}
		}

		public virtual void TestIndependentObjectConfigs()
		{
			Db4objects.Db4o.Config.IConfiguration config = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			Db4objects.Db4o.Config.IObjectClass objectConfig = config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Config.NonStaticConfigurationTestCase.Data)
				);
			objectConfig.Translate(new Db4objects.Db4o.Config.TSerializable());
			Db4objects.Db4o.Config.IConfiguration otherConfig = Db4objects.Db4o.Db4oFactory.NewConfiguration
				();
			Db4oUnit.Assert.AreNotSame(config, otherConfig);
			Db4objects.Db4o.Config4Class otherObjectConfig = (Db4objects.Db4o.Config4Class)otherConfig
				.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Config.NonStaticConfigurationTestCase.Data)
				);
			Db4oUnit.Assert.AreNotSame(objectConfig, otherObjectConfig);
			Db4oUnit.Assert.IsNull(otherObjectConfig.GetTranslator());
		}
	}
}
