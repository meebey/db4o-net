namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractDb4oFixture : Db4oUnit.Extensions.IDb4oFixture
	{
		private readonly Db4oUnit.Extensions.Fixtures.IConfigurationSource _configSource;

		private Db4objects.Db4o.Config.IConfiguration _config;

		protected AbstractDb4oFixture(Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource
			)
		{
			_configSource = configSource;
		}

		public virtual void Reopen()
		{
			Close();
			Open();
		}

		public virtual Db4objects.Db4o.Config.IConfiguration Config()
		{
			if (_config == null)
			{
				_config = _configSource.Config();
			}
			return _config;
		}

		public virtual void Clean()
		{
			DoClean();
			ResetConfig();
		}

		public virtual bool Accept(System.Type clazz)
		{
			return true;
		}

		protected abstract void DoClean();

		protected virtual void ResetConfig()
		{
			_config = null;
		}

		public abstract void Close();

		public abstract Db4objects.Db4o.Ext.IExtObjectContainer Db();

		public abstract Db4objects.Db4o.Internal.LocalObjectContainer FileSession();

		public abstract string GetLabel();

		public abstract void Open();
	}
}
