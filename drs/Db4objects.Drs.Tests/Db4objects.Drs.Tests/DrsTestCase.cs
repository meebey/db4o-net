/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;
using Sharpen.Lang;

namespace Db4objects.Drs.Tests
{
	public abstract class DrsTestCase : ITestCase, ITestLifeCycle
	{
		public static readonly Type[] mappings;

		public static readonly Type[] extraMappingsForCleaning = new Type[] { typeof(IDictionary
			), typeof(IList) };

		static DrsTestCase()
		{
			mappings = new Type[] { typeof(Replicated), typeof(SPCParent), typeof(SPCChild), 
				typeof(ListHolder), typeof(ListContent), typeof(SimpleListHolder), typeof(SimpleItem
				), typeof(CollectionHolder), typeof(MapContent), typeof(SimpleArrayContent), typeof(
				SimpleArrayHolder), typeof(R0), typeof(Pilot), typeof(Car), typeof(Student), typeof(
				Person) };
		}

		private readonly DrsFixturePair _fixtures = DrsFixtureVariable.Value();

		private Db4objects.Drs.Inside.ReplicationReflector _reflector;

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			CleanBoth();
			ConfigureBoth();
			OpenBoth();
			Store();
			Reopen();
		}

		private void CleanBoth()
		{
			A().Clean();
			B().Clean();
		}

		protected virtual void Clean()
		{
			for (int i = 0; i < mappings.Length; i++)
			{
				A().Provider().DeleteAllInstances(mappings[i]);
				B().Provider().DeleteAllInstances(mappings[i]);
			}
			for (int i = 0; i < extraMappingsForCleaning.Length; i++)
			{
				A().Provider().DeleteAllInstances(extraMappingsForCleaning[i]);
				B().Provider().DeleteAllInstances(extraMappingsForCleaning[i]);
			}
			A().Provider().Commit();
			B().Provider().Commit();
		}

		protected virtual void Store()
		{
		}

		private void ConfigureBoth()
		{
			ConfigureInitial(_fixtures.a);
			ConfigureInitial(_fixtures.b);
		}

		private void ConfigureInitial(IDrsFixture fixture)
		{
			IConfiguration config = Db4oConfiguration(fixture);
			if (config == null)
			{
				return;
			}
			config.GenerateUUIDs(ConfigScope.Globally);
			config.GenerateVersionNumbers(ConfigScope.Globally);
			Configure(config);
		}

		private IConfiguration Db4oConfiguration(IDrsFixture fixture)
		{
			if (!(fixture is Db4oDrsFixture))
			{
				return null;
			}
			return ((Db4oDrsFixture)fixture).Config();
		}

		protected virtual void Configure(IConfiguration config)
		{
		}

		/// <exception cref="Exception"></exception>
		protected virtual void Reopen()
		{
			CloseBoth();
			OpenBoth();
		}

		/// <exception cref="Exception"></exception>
		private void OpenBoth()
		{
			A().Open();
			B().Open();
			_reflector = new Db4objects.Drs.Inside.ReplicationReflector(A().Provider(), B().Provider
				());
			A().Provider().ReplicationReflector(_reflector);
			B().Provider().ReplicationReflector(_reflector);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
			CloseBoth();
			CleanBoth();
		}

		/// <exception cref="Exception"></exception>
		private void CloseBoth()
		{
			A().Close();
			B().Close();
		}

		public virtual IDrsFixture A()
		{
			return _fixtures.a;
		}

		public virtual IDrsFixture B()
		{
			return _fixtures.b;
		}

		protected virtual void EnsureOneInstance(IDrsFixture fixture, Type clazz)
		{
			EnsureInstanceCount(fixture, clazz, 1);
		}

		protected virtual void EnsureInstanceCount(IDrsFixture fixture, Type clazz, int count
			)
		{
			IObjectSet objectSet = fixture.Provider().GetStoredObjects(clazz);
			Assert.AreEqual(count, objectSet.Count);
		}

		protected virtual object GetOneInstance(IDrsFixture fixture, Type clazz)
		{
			IEnumerator objectSet = fixture.Provider().GetStoredObjects(clazz).GetEnumerator(
				);
			object candidate = null;
			if (objectSet.MoveNext())
			{
				candidate = objectSet.Current;
				if (objectSet.MoveNext())
				{
					throw new Exception("Found more than one instance of + " + clazz + " in provider = "
						 + fixture);
				}
			}
			return candidate;
		}

		protected virtual void ReplicateAll(ITestableReplicationProviderInside providerFrom
			, ITestableReplicationProviderInside providerTo)
		{
			//System.out.println("from = " + providerFrom + ", to = " + providerTo);
			IReplicationSession replication = Db4objects.Drs.Replication.Begin(providerFrom, 
				providerTo);
			IObjectSet changedSet = providerFrom.ObjectsChangedSinceLastReplication();
			if (changedSet.Count == 0)
			{
				throw new Exception("Can't find any objects to replicate");
			}
			ReplicateAll(replication, changedSet.GetEnumerator());
		}

		private void ReplicateAll(IReplicationSession replication, IEnumerator allObjects
			)
		{
			while (allObjects.MoveNext())
			{
				object changed = allObjects.Current;
				//System.out.println("changed = " + changed);
				replication.Replicate(changed);
			}
			replication.Commit();
		}

		protected virtual void ReplicateAll(ITestableReplicationProviderInside from, ITestableReplicationProviderInside
			 to, IReplicationEventListener listener)
		{
			IReplicationSession replication = Db4objects.Drs.Replication.Begin(from, to, listener
				);
			ReplicateAll(replication, from.ObjectsChangedSinceLastReplication().GetEnumerator
				());
		}

		protected virtual void Delete(Type[] classes)
		{
			for (int i = 0; i < classes.Length; i++)
			{
				A().Provider().DeleteAllInstances(classes[i]);
				B().Provider().DeleteAllInstances(classes[i]);
			}
			A().Provider().Commit();
			B().Provider().Commit();
		}

		protected virtual void ReplicateClass(ITestableReplicationProviderInside providerA
			, ITestableReplicationProviderInside providerB, Type clazz)
		{
			//System.out.println("ReplicationTestcase.replicateClass");
			IReplicationSession replication = Db4objects.Drs.Replication.Begin(providerA, providerB
				);
			IEnumerator allObjects = providerA.ObjectsChangedSinceLastReplication(clazz).GetEnumerator
				();
			ReplicateAll(replication, allObjects);
		}

		protected static void Sleep(int millis)
		{
			try
			{
				Thread.Sleep(millis);
			}
			catch (Exception e)
			{
				throw new Exception(e.ToString());
			}
		}

		protected virtual Db4objects.Drs.Inside.ReplicationReflector ReplicationReflector
			()
		{
			return _reflector;
		}
	}
}
