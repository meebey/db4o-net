/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
		private readonly CachingConfigurationSource _configSource;

		private IFixtureConfiguration _fixtureConfiguration;

		protected AbstractDb4oFixture(IConfigurationSource configSource)
		{
			_configSource = new CachingConfigurationSource(configSource);
		}

		public virtual void FixtureConfiguration(IFixtureConfiguration fc)
		{
			_fixtureConfiguration = fc;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Reopen(Type testCaseClass)
		{
			Close();
			Open(testCaseClass);
		}

		public virtual IConfiguration Config()
		{
			return _configSource.Config();
		}

		public virtual void Clean()
		{
			DoClean();
			ResetConfig();
		}

		public abstract bool Accept(Type clazz);

		protected abstract void DoClean();

		public virtual void ResetConfig()
		{
			_configSource.Reset();
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Defragment(string fileName)
		{
			string targetFile = fileName + ".defrag.backup";
			DefragmentConfig defragConfig = new DefragmentConfig(fileName, targetFile);
			defragConfig.ForceBackupDelete(true);
			defragConfig.Db4oConfig(CloneConfiguration());
			Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
		}

		protected virtual string BuildLabel(string label)
		{
			if (null == _fixtureConfiguration)
			{
				return label;
			}
			return label + " - " + _fixtureConfiguration.GetLabel();
		}

		protected virtual void ApplyFixtureConfiguration(Type testCaseClass, IConfiguration
			 config)
		{
			if (null == _fixtureConfiguration)
			{
				return;
			}
			_fixtureConfiguration.Configure(testCaseClass, config);
		}

		public override string ToString()
		{
			return Label();
		}

		protected virtual Config4Impl CloneConfiguration()
		{
			return CloneDb4oConfiguration((Config4Impl)Config());
		}

		protected virtual Config4Impl CloneDb4oConfiguration(IConfiguration config)
		{
			return (Config4Impl)((Config4Impl)config).DeepClone(this);
		}

		public abstract string Label();

		public abstract void Close();

		public abstract void ConfigureAtRuntime(IRuntimeConfigureAction arg1);

		public abstract IExtObjectContainer Db();

		public abstract void Defragment();

		public abstract LocalObjectContainer FileSession();

		public abstract void Open(Type arg1);
	}
}
