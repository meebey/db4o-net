namespace Db4oUnit.Extensions
{
	public class AbstractDb4oTestCase : Db4oUnit.Extensions.IDb4oTestCase
	{
		[Db4objects.Db4o.Transient]
		private Db4oUnit.Extensions.IDb4oFixture _fixture;

		public virtual void Fixture(Db4oUnit.Extensions.IDb4oFixture fixture)
		{
			_fixture = fixture;
		}

		public virtual Db4oUnit.Extensions.IDb4oFixture Fixture()
		{
			return _fixture;
		}

		protected virtual void Reopen()
		{
			_fixture.Reopen();
		}

		public void SetUp()
		{
			_fixture.Clean();
			Configure(_fixture.Config());
			_fixture.Open();
			Db4oSetupBeforeStore();
			Store();
			_fixture.Db().Commit();
			_fixture.Close();
			_fixture.Open();
			Db4oSetupAfterStore();
		}

		public void TearDown()
		{
			Db4oCustomTearDown();
			_fixture.Close();
			_fixture.Clean();
		}

		protected virtual void Db4oSetupBeforeStore()
		{
		}

		protected virtual void Db4oSetupAfterStore()
		{
		}

		protected virtual void Db4oCustomTearDown()
		{
		}

		protected virtual void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
		}

		protected virtual void Store()
		{
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer Db()
		{
			return Fixture().Db();
		}

		protected virtual System.Type[] TestCases()
		{
			return new System.Type[] { GetType() };
		}

		public virtual int RunSoloAndClientServer()
		{
			return RunSoloAndClientServer(true);
		}

		private int RunSoloAndClientServer(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(new Db4oUnit.TestSuite(new Db4oUnit.ITest[] { SoloSuite
				(independentConfig).Build(), ClientServerSuite(independentConfig).Build() })).Run
				();
		}

		public virtual int RunSolo()
		{
			return RunSolo(true);
		}

		public virtual int RunSolo(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(SoloSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer()
		{
			return RunClientServer(true);
		}

		public virtual int RunClientServer(bool independentConfig)
		{
			return new Db4oUnit.TestRunner(ClientServerSuite(independentConfig)).Run();
		}

		private Db4oUnit.Extensions.Db4oTestSuiteBuilder SoloSuite(bool independentConfig
			)
		{
			return new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSolo
				(ConfigSource(independentConfig)), TestCases());
		}

		private Db4oUnit.Extensions.Db4oTestSuiteBuilder ClientServerSuite(bool independentConfig
			)
		{
			return new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4oUnit.Extensions.Fixtures.Db4oSingleClient
				(ConfigSource(independentConfig)), TestCases());
		}

		private Db4oUnit.Extensions.Fixtures.IConfigurationSource ConfigSource(bool independentConfig
			)
		{
			return (independentConfig ? (Db4oUnit.Extensions.Fixtures.IConfigurationSource)new 
				Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource() : new Db4oUnit.Extensions.Fixtures.GlobalConfigurationSource
				());
		}

		protected virtual Db4objects.Db4o.YapStream Stream()
		{
			return (Db4objects.Db4o.YapStream)Db();
		}

		protected virtual Db4objects.Db4o.Transaction Trans()
		{
			return Stream().GetTransaction();
		}

		protected virtual Db4objects.Db4o.Transaction SystemTrans()
		{
			return Stream().GetSystemTransaction();
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewQuery(Db4objects.Db4o.Transaction
			 transaction, System.Type clazz)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(transaction);
			query.Constrain(clazz);
			return query;
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewQuery(Db4objects.Db4o.Transaction
			 transaction)
		{
			return Stream().Query(transaction);
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewQuery()
		{
			return Db().Query();
		}

		protected virtual Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return Stream().Reflector();
		}

		protected virtual void IndexField(Db4objects.Db4o.Config.IConfiguration config, System.Type
			 clazz, string fieldName)
		{
			config.ObjectClass(clazz).ObjectField(fieldName).Indexed(true);
		}

		protected virtual Db4objects.Db4o.Transaction NewTransaction()
		{
			return Stream().NewTransaction();
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewQuery(System.Type clazz)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery();
			query.Constrain(clazz);
			return query;
		}

		protected virtual object RetrieveOnlyInstance(System.Type clazz)
		{
			Db4objects.Db4o.IObjectSet result = NewQuery(clazz).Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			return result.Next();
		}

		protected virtual int CountOccurences(System.Type clazz)
		{
			Db4objects.Db4o.IObjectSet result = NewQuery(clazz).Execute();
			return result.Size();
		}

		protected virtual void Foreach(System.Type clazz, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			Db4objects.Db4o.Ext.IExtObjectContainer oc = Db();
			oc.Deactivate(clazz, int.MaxValue);
			Db4objects.Db4o.IObjectSet set = NewQuery(clazz).Execute();
			while (set.HasNext())
			{
				visitor.Visit(set.Next());
			}
		}

		protected virtual void DeleteAll(System.Type clazz)
		{
			Foreach(clazz, new _AnonymousInnerClass183(this));
		}

		private sealed class _AnonymousInnerClass183 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass183(AbstractDb4oTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing.Db().Delete(obj);
			}

			private readonly AbstractDb4oTestCase _enclosing;
		}

		protected void Store(object obj)
		{
			Db().Set(obj);
		}
	}
}
