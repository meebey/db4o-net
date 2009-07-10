/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractDb4oFixture : IDb4oFixture
	{
		private IFixtureConfiguration _fixtureConfiguration;

		private IConfiguration _configuration;

		protected AbstractDb4oFixture()
		{
			ResetConfig();
		}

		public virtual void FixtureConfiguration(IFixtureConfiguration fc)
		{
			_fixtureConfiguration = fc;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Reopen(IDb4oTestCase testInstance)
		{
			Close();
			Open(testInstance);
		}

		public virtual IConfiguration Config()
		{
			return _configuration;
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
			_configuration = NewConfiguration();
		}

		/// <summary>Method can be overridden in subclasses with special instantiation requirements (oSGI for instance).
		/// 	</summary>
		/// <remarks>Method can be overridden in subclasses with special instantiation requirements (oSGI for instance).
		/// 	</remarks>
		/// <returns></returns>
		protected virtual IConfiguration NewConfiguration()
		{
			return Db4oFactory.NewConfiguration();
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

		protected virtual void ApplyFixtureConfiguration(IDb4oTestCase testInstance, IConfiguration
			 config)
		{
			if (null == _fixtureConfiguration)
			{
				return;
			}
			_fixtureConfiguration.Configure(testInstance, config);
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

		public abstract void Open(IDb4oTestCase arg1);
	}
}
