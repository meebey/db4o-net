/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Concurrency;
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

		private const int DERFAULT_CONCURRENCY_THREAD_COUNT = 10;

		[System.NonSerialized]
		private int _threadCount = DERFAULT_CONCURRENCY_THREAD_COUNT;

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
			return Fixture() is IDb4oClientServerFixture;
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
			try
			{
				Db4oTearDownBeforeClean();
			}
			finally
			{
				_fixture.Close();
				_fixture.Clean();
			}
			Db4oTearDownAfterClean();
		}

		protected virtual void Db4oSetupBeforeStore()
		{
		}

		protected virtual void Db4oSetupAfterStore()
		{
		}

		protected virtual void Db4oTearDownBeforeClean()
		{
		}

		protected virtual void Db4oTearDownAfterClean()
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

		public virtual int RunConcurrency()
		{
			return RunConcurrency(true);
		}

		public virtual int RunEmbeddedConcurrency()
		{
			return RunEmbeddedConcurrency(true);
		}

		private int RunEmbeddedClientServer(bool independentConfig)
		{
			return new TestRunner(EmbeddedClientServerSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer(bool independentConfig)
		{
			return new TestRunner(ClientServerSuite(independentConfig)).Run();
		}

		private int RunConcurrency(bool independentConfig)
		{
			return new TestRunner(ConcurrenyClientServerSuite(independentConfig, false)).Run(
				);
		}

		private int RunEmbeddedConcurrency(bool independentConfig)
		{
			return new TestRunner(ConcurrenyClientServerSuite(independentConfig, true)).Run();
		}

		private Db4oTestSuiteBuilder SoloSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oSolo(ConfigSource(independentConfig)), TestCases
				());
		}

		private Db4oTestSuiteBuilder ClientServerSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), false), TestCases());
		}

		private Db4oTestSuiteBuilder EmbeddedClientServerSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), true), TestCases());
		}

		private Db4oTestSuiteBuilder ConcurrenyClientServerSuite(bool independentConfig, 
			bool embedded)
		{
			return new Db4oConcurrencyTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), embedded), TestCases());
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
			return NewQuery(Db());
		}

		protected virtual IQuery NewQuery(IExtObjectContainer oc)
		{
			return oc.Query();
		}

		protected virtual IQuery NewQuery(Type clazz)
		{
			return NewQuery(Db(), clazz);
		}

		protected virtual IQuery NewQuery(IExtObjectContainer oc, Type clazz)
		{
			IQuery query = NewQuery(oc);
			query.Constrain(clazz);
			return query;
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

		protected virtual object RetrieveOnlyInstance(Type clazz)
		{
			return RetrieveOnlyInstance(Db(), clazz);
		}

		protected virtual object RetrieveOnlyInstance(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			Assert.AreEqual(1, result.Size());
			return result.Next();
		}

		protected virtual int CountOccurences(Type clazz)
		{
			IObjectSet result = NewQuery(clazz).Execute();
			return result.Size();
		}

		protected virtual int CountOccurences(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			return result.Size();
		}

		protected virtual void AssertOccurrences(IExtObjectContainer oc, Type clazz, int 
			expected)
		{
			Assert.AreEqual(expected, CountOccurences(oc, clazz));
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

		protected void DeleteAll(Type clazz)
		{
			DeleteAll(Db(), clazz);
		}

		protected void DeleteAll(IExtObjectContainer oc, Type clazz)
		{
			Foreach(clazz, new _IVisitor4_275(this, oc));
		}

		private sealed class _IVisitor4_275 : IVisitor4
		{
			public _IVisitor4_275(AbstractDb4oTestCase _enclosing, IExtObjectContainer oc)
			{
				this._enclosing = _enclosing;
				this.oc = oc;
			}

			public void Visit(object obj)
			{
				oc.Delete(obj);
			}

			private readonly AbstractDb4oTestCase _enclosing;

			private readonly IExtObjectContainer oc;
		}

		protected void DeleteObjectSet(IObjectSet os)
		{
			DeleteObjectSet(Db(), os);
		}

		protected void DeleteObjectSet(IObjectContainer oc, IObjectSet os)
		{
			while (os.HasNext())
			{
				oc.Delete(os.Next());
			}
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

		public int ThreadCount()
		{
			return _threadCount;
		}

		public void ConfigureThreadCount(int count)
		{
			_threadCount = count;
		}
	}
}
