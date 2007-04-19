using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractDb4oFixture : IDb4oFixture
	{
		private readonly IConfigurationSource _configSource;

		private IConfiguration _config;

		protected AbstractDb4oFixture(IConfigurationSource configSource)
		{
			_configSource = configSource;
		}

		public virtual void Reopen()
		{
			Close();
			Open();
		}

		public virtual IConfiguration Config()
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

		public abstract bool Accept(Type clazz);

		protected abstract void DoClean();

		protected virtual void ResetConfig()
		{
			_config = null;
		}

		protected virtual void Defragment(string fileName)
		{
			string targetFile = fileName + ".defrag.backup";
			DefragmentConfig defragConfig = new DefragmentConfig(fileName, targetFile);
			defragConfig.ForceBackupDelete(true);
			defragConfig.Db4oConfig(Config());
			Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
		}

		public abstract void Close();

		public abstract IExtObjectContainer Db();

		public abstract void Defragment();

		public abstract LocalObjectContainer FileSession();

		public abstract string GetLabel();

		public abstract void Open();
	}
}
