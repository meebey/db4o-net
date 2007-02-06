namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oSolo : Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture
	{
		public Db4oSolo() : this(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
			())
		{
		}

		public Db4oSolo(Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource) : 
			base(configSource, "db4oSoloTest.yap")
		{
		}

		protected override Db4objects.Db4o.IObjectContainer CreateDatabase(Db4objects.Db4o.Config.IConfiguration
			 config)
		{
			return Db4objects.Db4o.Db4oFactory.OpenFile(config, GetAbsolutePath());
		}

		public override string GetLabel()
		{
			return "SOLO";
		}
	}
}
