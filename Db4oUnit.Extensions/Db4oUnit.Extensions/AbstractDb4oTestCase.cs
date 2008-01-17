/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Concurrency;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
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

		private const int DefaultConcurrencyThreadCount = 10;

		[System.NonSerialized]
		private int _threadCount = DefaultConcurrencyThreadCount;

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

		protected virtual bool IsEmbeddedClientServer()
		{
			return IsClientServer() && ((IDb4oClientServerFixture)Fixture()).EmbeddedClients(
				);
		}

		protected virtual bool IsMTOC()
		{
			// TODO: The following code is only a temporary addition until MTOC
			//       is part of the core. When it is, all occurences of this 
			//       method should be replaced with    isEmbeddedClientServer() 
			return Fixture().Db() is EmbeddedClientObjectContainer;
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Reopen()
		{
			_fixture.Reopen(GetType());
		}

		/// <exception cref="Exception"></exception>
		public void SetUp()
		{
			_fixture.Clean();
			Configure(_fixture.Config());
			_fixture.Open(GetType());
			Db4oSetupBeforeStore();
			Store();
			_fixture.Db().Commit();
			_fixture.Close();
			_fixture.Open(GetType());
			Db4oSetupAfterStore();
		}

		/// <exception cref="Exception"></exception>
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

		/// <exception cref="Exception"></exception>
		protected virtual void Db4oSetupBeforeStore()
		{
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Db4oSetupAfterStore()
		{
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Db4oTearDownBeforeClean()
		{
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Db4oTearDownAfterClean()
		{
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Configure(IConfiguration config)
		{
		}

		/// <exception cref="Exception"></exception>
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

		public virtual int RunConcurrencyAll()
		{
			return RunConcurrencyAll(true);
		}

		protected virtual int RunEmbeddedClientServer(bool independentConfig)
		{
			return new TestRunner(EmbeddedClientServerSuite(independentConfig)).Run();
		}

		public virtual int RunClientServer(bool independentConfig)
		{
			return new TestRunner(ClientServerSuite(independentConfig)).Run();
		}

		private int RunConcurrency(bool independentConfig)
		{
			return new TestRunner(ConcurrenyClientServerSuite(independentConfig, false, "CONC"
				)).Run();
		}

		private int RunEmbeddedConcurrency(bool independentConfig)
		{
			return new TestRunner(ConcurrenyClientServerSuite(independentConfig, true, "CONC EMBEDDED"
				)).Run();
		}

		private int RunConcurrencyAll(bool independentConfig)
		{
			return new TestRunner(new TestSuite(new ITest[] { ConcurrenyClientServerSuite(independentConfig
				, false, "CONC").Build(), ConcurrenyClientServerSuite(independentConfig, true, "CONC EMBEDDED"
				).Build() })).Run();
		}

		protected virtual Db4oTestSuiteBuilder SoloSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oSolo(ConfigSource(independentConfig)), TestCases
				());
		}

		protected virtual Db4oTestSuiteBuilder ClientServerSuite(bool independentConfig)
		{
			return new Db4oTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), false, "C/S"), TestCases());
		}

		protected virtual Db4oTestSuiteBuilder EmbeddedClientServerSuite(bool independentConfig
			)
		{
			return new Db4oTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), true, "C/S EMBEDDED"), TestCases());
		}

		protected virtual Db4oTestSuiteBuilder ConcurrenyClientServerSuite(bool independentConfig
			, bool embedded, string label)
		{
			return new Db4oConcurrencyTestSuiteBuilder(new Db4oClientServer(ConfigSource(independentConfig
				), embedded, label), TestCases());
		}

		protected virtual IConfigurationSource ConfigSource(bool independentConfig)
		{
			return (independentConfig ? (IConfigurationSource)new IndependentConfigurationSource
				() : new GlobalConfigurationSource());
		}

		protected virtual ObjectContainerBase Stream()
		{
			return ((IInternalObjectContainer)Db()).Container();
		}

		public virtual LocalObjectContainer FileSession()
		{
			return Fixture().FileSession();
		}

		public virtual Transaction Trans()
		{
			return ((IInternalObjectContainer)Db()).Transaction();
		}

		protected virtual Transaction SystemTrans()
		{
			return Trans().SystemTransaction();
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

		protected static IQuery NewQuery(IExtObjectContainer oc)
		{
			return oc.Query();
		}

		protected virtual IQuery NewQuery(Type clazz)
		{
			return NewQuery(Db(), clazz);
		}

		protected static IQuery NewQuery(IExtObjectContainer oc, Type clazz)
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
			return Stream().NewUserTransaction();
		}

		public virtual object RetrieveOnlyInstance(Type clazz)
		{
			return RetrieveOnlyInstance(Db(), clazz);
		}

		public static object RetrieveOnlyInstance(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			Assert.AreEqual(1, result.Size());
			return result.Next();
		}

		protected virtual int CountOccurences(Type clazz)
		{
			return CountOccurences(Db(), clazz);
		}

		protected virtual int CountOccurences(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			return result.Size();
		}

		protected virtual void AssertOccurrences(Type clazz, int expected)
		{
			AssertOccurrences(Db(), clazz, expected);
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
			Foreach(clazz, new _IVisitor4_302(this, oc));
		}

		private sealed class _IVisitor4_302 : IVisitor4
		{
			public _IVisitor4_302(AbstractDb4oTestCase _enclosing, IExtObjectContainer oc)
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

		public void Store(object obj)
		{
			Db().Store(obj);
		}

		protected virtual ClassMetadata ClassMetadataFor(Type clazz)
		{
			return Stream().ClassMetadataForReflectClass(ReflectClass(clazz));
		}

		protected virtual IReflectClass ReflectClass(Type clazz)
		{
			return Reflector().ForClass(clazz);
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Defragment()
		{
			Fixture().Close();
			Fixture().Defragment();
			Fixture().Open(GetType());
		}

		public int ThreadCount()
		{
			return _threadCount;
		}

		public void ConfigureThreadCount(int count)
		{
			_threadCount = count;
		}

		protected virtual IEventRegistry EventRegistry()
		{
			return EventRegistryFor(Db());
		}

		protected virtual IEventRegistry EventRegistryFor(IExtObjectContainer container)
		{
			return EventRegistryFactory.ForObjectContainer(container);
		}

		protected virtual IEventRegistry ServerEventRegistry()
		{
			return EventRegistryFor(FileSession());
		}
	}
}
