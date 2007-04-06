using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;

namespace Db4oUnit.Extensions
{
	public class AbstractDb4oTestCase : IDb4oTestCase
	{
		[System.NonSerialized]
		private IDb4oFixture _fixture;

		public virtual void Fixture(IDb4oFixture fixture)
		{
			_fixture = fixture;
		}

		public virtual IDb4oFixture Fixture()
		{
			return _fixture;
		}

		public virtual bool IsClientServer()
		{
			return Fixture() is AbstractClientServerDb4oFixture;
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

		protected virtual void Configure(IConfiguration config)
		{
		}

		protected virtual void Store()
		{
		}

		public virtual IExtObjectContainer Db()
		{
			return Fixture().Db();
		}

		protected virtual Type[] TestCases()
		{
			return new Type[] { GetType() };
		}

		public virtual int RunAll()
		{
			return RunAll(true);
		}

		private int RunAll(bool independentConfig)
		{
			return new TestRunner(new TestSuite(new ITest[] { SoloSuite(independentConfig).Build
				(), ClientServerSuite(independentConfig).Build(), EmbeddedClientServerSuite(independentConfig
				).Build() })).Run();
		}

		public virtual int RunSoloAndClientServer()
		{
			return RunSoloAndClientServer(true);
		}

		private int RunSoloAndClientServer(bool independentConfig)
		{
			return new TestRunner(new TestSuite(new ITest[] { SoloSuite(independentConfig).Build
				(), ClientServerSuite(independentConfig).Build() })).Run();
		}

		public virtual int RunSolo()
		{
			return RunSolo(true);
		}

		public virtual int RunSolo(bool independentConfig)
		{
			return new TestRunner(SoloSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer()
		{
			return RunClientServer(true);
		}

		public virtual int RunEmbeddedClientServer()
		{
			return RunEmbeddedClientServer(true);
		}

		private int RunEmbeddedClientServer(bool independentConfig)
		{
			return new TestRunner(EmbeddedClientServerSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer(bool independentConfig)
		{
			return new TestRunner(ClientServerSuite(independentConfig)).Run();
		}

		private Db4oTestSuiteBuilder SoloSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oSolo(ConfigSource(independentConfig)), TestCases
				());
		}

		private Db4oTestSuiteBuilder ClientServerSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oSingleClient(ConfigSource(independentConfig
				)), TestCases());
		}

		private Db4oTestSuiteBuilder EmbeddedClientServerSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oSingleClient(ConfigSource(independentConfig
				), 0), TestCases());
		}

		private IConfigurationSource ConfigSource(bool independentConfig)
		{
			return (independentConfig ? (IConfigurationSource)new IndependentConfigurationSource
				() : new GlobalConfigurationSource());
		}

		protected virtual ObjectContainerBase Stream()
		{
			return (ObjectContainerBase)Db();
		}

		public virtual LocalObjectContainer FileSession()
		{
			return Fixture().FileSession();
		}

		protected virtual Transaction Trans()
		{
			return Stream().GetTransaction();
		}

		protected virtual Transaction SystemTrans()
		{
			return Stream().SystemTransaction();
		}

		protected virtual IQuery NewQuery(Transaction transaction, Type clazz)
		{
			IQuery query = NewQuery(transaction);
			query.Constrain(clazz);
			return query;
		}

		protected virtual IQuery NewQuery(Transaction transaction)
		{
			return Stream().Query(transaction);
		}

		protected virtual IQuery NewQuery()
		{
			return Db().Query();
		}

		protected virtual IReflector Reflector()
		{
			return Stream().Reflector();
		}

		protected virtual void IndexField(IConfiguration config, Type clazz, string fieldName
			)
		{
			config.ObjectClass(clazz).ObjectField(fieldName).Indexed(true);
		}

		protected virtual Transaction NewTransaction()
		{
			return Stream().NewTransaction();
		}

		protected virtual IQuery NewQuery(Type clazz)
		{
			IQuery query = NewQuery();
			query.Constrain(clazz);
			return query;
		}

		protected virtual object RetrieveOnlyInstance(Type clazz)
		{
			IObjectSet result = NewQuery(clazz).Execute();
			Assert.AreEqual(1, result.Size());
			return result.Next();
		}

		protected virtual int CountOccurences(Type clazz)
		{
			IObjectSet result = NewQuery(clazz).Execute();
			return result.Size();
		}

		protected virtual void Foreach(Type clazz, IVisitor4 visitor)
		{
			IExtObjectContainer oc = Db();
			oc.Deactivate(clazz, int.MaxValue);
			IObjectSet set = NewQuery(clazz).Execute();
			while (set.HasNext())
			{
				visitor.Visit(set.Next());
			}
		}

		protected virtual void DeleteAll(Type clazz)
		{
			Foreach(clazz, new _AnonymousInnerClass218(this));
		}

		private sealed class _AnonymousInnerClass218 : IVisitor4
		{
			public _AnonymousInnerClass218(AbstractDb4oTestCase _enclosing)
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

		protected virtual IReflectClass ReflectClass(Type clazz)
		{
			return Reflector().ForClass(clazz);
		}

		protected virtual void Defragment()
		{
			Fixture().Close();
			Fixture().Defragment();
			Fixture().Open();
		}
	}
}
