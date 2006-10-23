namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractSoloDb4oFixture : Db4oUnit.Extensions.Fixtures.AbstractDb4oFixture
	{
		private Db4objects.Db4o.Ext.IExtObjectContainer _db;

		protected AbstractSoloDb4oFixture(Db4oUnit.Extensions.Fixtures.IConfigurationSource
			 configSource) : base(configSource)
		{
		}

		public sealed override void Open()
		{
			_db = CreateDatabase(Config()).Ext();
		}

		public override void Close()
		{
			_db.Close();
			_db = null;
		}

		public override Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			return _db;
		}

		protected abstract Db4objects.Db4o.IObjectContainer CreateDatabase(Db4objects.Db4o.Config.IConfiguration
			 config);
	}
}