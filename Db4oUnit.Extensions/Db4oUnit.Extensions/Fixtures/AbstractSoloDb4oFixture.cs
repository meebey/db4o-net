/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractSoloDb4oFixture : AbstractDb4oFixture
	{
		private IExtObjectContainer _db;

		protected AbstractSoloDb4oFixture(IConfigurationSource configSource) : base(configSource
			)
		{
		}

		public sealed override void Open(Type testCaseClass)
		{
			Assert.IsNull(_db);
			IConfiguration config = Config();
			ApplyFixtureConfiguration(testCaseClass, config);
			_db = CreateDatabase(config).Ext();
		}

		/// <exception cref="Exception"></exception>
		public override void Close()
		{
			if (null != _db)
			{
				Assert.IsTrue(Db().Close());
				_db = null;
			}
		}

		public override bool Accept(Type clazz)
		{
			return !typeof(IOptOutSolo).IsAssignableFrom(clazz);
		}

		public override IExtObjectContainer Db()
		{
			return _db;
		}

		protected abstract IObjectContainer CreateDatabase(IConfiguration config);

		public override LocalObjectContainer FileSession()
		{
			return (LocalObjectContainer)_db;
		}

		public override void ConfigureAtRuntime(IRuntimeConfigureAction action)
		{
			action.Apply(Config());
		}
	}
}
