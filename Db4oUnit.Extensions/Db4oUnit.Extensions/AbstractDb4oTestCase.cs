/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections;
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
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;

namespace Db4oUnit.Extensions
{
	public partial class AbstractDb4oTestCase : IDb4oTestCase, ITestLifeCycle
	{
		private const int DefaultConcurrencyThreadCount = 10;

		[System.NonSerialized]
		private int _threadCount = DefaultConcurrencyThreadCount;

		public static IDb4oFixture Fixture()
		{
			return Db4oFixtureVariable.Fixture();
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

		// TODO: The following code is only a temporary addition until MTOC
		//       is part of the core. When it is, all occurences of this 
		//       method should be replaced with    isEmbeddedClientServer() 
		protected virtual bool IsMTOC()
		{
			return Fixture().Db() is ObjectContainerSession;
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Reopen()
		{
			Fixture().Reopen(this);
		}

		/// <exception cref="System.Exception"></exception>
		public void SetUp()
		{
			IDb4oFixture _fixture = Fixture();
			_fixture.Clean();
			Configure(_fixture);
			_fixture.Open(this);
			Db4oSetupBeforeStore();
			Store();
			_fixture.Db().Commit();
			_fixture.Close();
			_fixture.Open(this);
			Db4oSetupAfterStore();
		}

		/// <exception cref="System.Exception"></exception>
		public void TearDown()
		{
			try
			{
				Db4oTearDownBeforeClean();
			}
			finally
			{
				IDb4oFixture fixture = Fixture();
				fixture.Close();
				fixture.Clean();
			}
			Db4oTearDownAfterClean();
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Db4oSetupBeforeStore()
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Db4oSetupAfterStore()
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Db4oTearDownBeforeClean()
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Db4oTearDownAfterClean()
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected virtual void Configure(IConfiguration config)
		{
		}

		/// <exception cref="System.Exception"></exception>
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
			return new ConsoleTestRunner(Iterators.Concat(new IEnumerable[] { SoloSuite(), ClientServerSuite
				(), EmbeddedClientServerSuite() })).Run();
		}

		public virtual int RunSolo(string testLabelSubstring)
		{
			return new ConsoleTestRunner(Iterators.Filter(SoloSuite(), new _IPredicate4_116(testLabelSubstring
				))).Run();
		}

		private sealed class _IPredicate4_116 : IPredicate4
		{
			public _IPredicate4_116(string testLabelSubstring)
			{
				this.testLabelSubstring = testLabelSubstring;
			}

			public bool Match(object candidate)
			{
				return (((ITest)candidate).Label().IndexOf(testLabelSubstring) >= 0);
			}

			private readonly string testLabelSubstring;
		}

		public virtual int RunSoloAndClientServer()
		{
			return new ConsoleTestRunner(Iterators.Concat(new IEnumerable[] { SoloSuite(), ClientServerSuite
				() })).Run();
		}

		public virtual int RunSoloAndEmbeddedClientServer()
		{
			return new ConsoleTestRunner(Iterators.Concat(new IEnumerable[] { SoloSuite(), EmbeddedClientServerSuite
				() })).Run();
		}

		public virtual int RunSolo()
		{
			return new ConsoleTestRunner(SoloSuite()).Run();
		}

		public virtual int RunClientServer()
		{
			return new ConsoleTestRunner(ClientServerSuite()).Run();
		}

		public virtual int RunEmbeddedClientServer()
		{
			return new ConsoleTestRunner(EmbeddedClientServerSuite()).Run();
		}

		public virtual int RunConcurrency()
		{
			return new ConsoleTestRunner(ConcurrenyClientServerSuite(false, "CONC")).Run();
		}

		public virtual int RunEmbeddedConcurrency()
		{
			return new ConsoleTestRunner(ConcurrenyClientServerSuite(true, "CONC EMBEDDED")).
				Run();
		}

		public virtual int RunConcurrencyAll()
		{
			return new ConsoleTestRunner(Iterators.Concat(new IEnumerable[] { ConcurrenyClientServerSuite
				(false, "CONC"), ConcurrenyClientServerSuite(true, "CONC EMBEDDED") })).Run();
		}

		protected virtual Db4oTestSuiteBuilder SoloSuite()
		{
			return new Db4oTestSuiteBuilder(Db4oFixtures.NewSolo(), TestCases());
		}

		protected virtual Db4oTestSuiteBuilder ClientServerSuite()
		{
			return new Db4oTestSuiteBuilder(Db4oFixtures.NewNetworkingCS(), TestCases());
		}

		protected virtual Db4oTestSuiteBuilder EmbeddedClientServerSuite()
		{
			return new Db4oTestSuiteBuilder(Db4oFixtures.NewEmbeddedCS(), TestCases());
		}

		protected virtual Db4oTestSuiteBuilder ConcurrenyClientServerSuite(bool embedded, 
			string label)
		{
			return new Db4oConcurrencyTestSuiteBuilder(new Db4oClientServer(embedded, label), 
				TestCases());
		}

		protected virtual IInternalObjectContainer Stream()
		{
			return (IInternalObjectContainer)Db();
		}

		protected virtual ObjectContainerBase Container()
		{
			return Stream().Container();
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
			return Container().Query(transaction);
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
			return Container().NewUserTransaction();
		}

		public virtual object RetrieveOnlyInstance(Type clazz)
		{
			return RetrieveOnlyInstance(Db(), clazz);
		}

		public static object RetrieveOnlyInstance(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			Assert.AreEqual(1, result.Count);
			return result.Next();
		}

		protected virtual int CountOccurences(Type clazz)
		{
			return CountOccurences(Db(), clazz);
		}

		protected virtual int CountOccurences(IExtObjectContainer oc, Type clazz)
		{
			IObjectSet result = NewQuery(oc, clazz).Execute();
			return result.Count;
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
			Foreach(Db(), clazz, visitor);
		}

		protected virtual void Foreach(IExtObjectContainer container, Type clazz, IVisitor4
			 visitor)
		{
			IObjectSet set = NewQuery(container, clazz).Execute();
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
			Foreach(oc, clazz, new _IVisitor4_290(oc));
		}

		private sealed class _IVisitor4_290 : IVisitor4
		{
			public _IVisitor4_290(IExtObjectContainer oc)
			{
				this.oc = oc;
			}

			public void Visit(object obj)
			{
				oc.Delete(obj);
			}

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

		/// <exception cref="System.Exception"></exception>
		protected virtual void Defragment()
		{
			Fixture().Close();
			Fixture().Defragment();
			Fixture().Open(this);
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

		protected virtual IContext Context()
		{
			return Trans().Context();
		}
	}
}
