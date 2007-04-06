using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Config;

namespace Db4objects.Db4o.Tests.Common.Config
{
	public class NonStaticConfigurationTestCase : ITestCase
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
			IConfiguration cfg = Db4oFactory.NewConfiguration();
			cfg.ReadOnly(true);
			IObjectContainer db = Db4oFactory.OpenFile(cfg, FILENAME);
			try
			{
				db.Set(new NonStaticConfigurationTestCase.Data(1));
			}
			finally
			{
				db.Close();
			}
			cfg = Db4oFactory.NewConfiguration();
			db = Db4oFactory.OpenFile(cfg, FILENAME);
			try
			{
				db.Set(new NonStaticConfigurationTestCase.Data(2));
				Assert.AreEqual(1, db.Query(typeof(NonStaticConfigurationTestCase.Data)).Size());
			}
			finally
			{
				db.Close();
			}
		}

		public virtual void TestIndependentObjectConfigs()
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			IObjectClass objectConfig = config.ObjectClass(typeof(NonStaticConfigurationTestCase.Data)
				);
			objectConfig.Translate(new TNull());
			IConfiguration otherConfig = Db4oFactory.NewConfiguration();
			Assert.AreNotSame(config, otherConfig);
			Config4Class otherObjectConfig = (Config4Class)otherConfig.ObjectClass(typeof(NonStaticConfigurationTestCase.Data)
				);
			Assert.AreNotSame(objectConfig, otherObjectConfig);
			Assert.IsNull(otherObjectConfig.GetTranslator());
		}
	}
}
