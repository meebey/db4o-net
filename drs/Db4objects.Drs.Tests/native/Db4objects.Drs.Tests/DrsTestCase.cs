/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Tests
{
	public abstract class DrsTestCase : Db4oUnit.ITestCase, Db4oUnit.ITestLifeCycle
	{
		public static readonly System.Type[] mappings;

		public static readonly System.Type[] extraMappingsForCleaning = new System.Type[] { typeof(System.Collections.IDictionary), typeof(System.Collections.IList) };

		static DrsTestCase()
		{
			mappings = new System.Type[]
			{
				typeof(Db4objects.Drs.Tests.Replicated),
				typeof(Db4objects.Drs.Tests.SPCParent),
				typeof(Db4objects.Drs.Tests.SPCChild),
				typeof(Db4objects.Drs.Tests.ListHolder),
				typeof(Db4objects.Drs.Tests.ListContent),
				typeof(Db4objects.Drs.Tests.MapContent), 
				typeof(Db4objects.Drs.Tests.SimpleArrayContent),
				typeof(Db4objects.Drs.Tests.SimpleArrayHolder), 
				typeof(Db4objects.Drs.Tests.R0),
				typeof(Db4objects.Drs.Tests.Pilot),
				typeof(Db4objects.Drs.Tests.Car),
				typeof(Db4objects.Drs.Tests.Student),
				typeof(Db4objects.Drs.Tests.Person)
			};
		}

		private Db4objects.Drs.Tests.IDrsFixture _a;

		private Db4objects.Drs.Tests.IDrsFixture _b;

		public virtual void SetUp()
		{
			CleanBoth();
			Configure();
			OpenBoth();
			Store();
			Reopen();
		}

		private void CleanBoth()
		{
			_a.Clean();
			_b.Clean();
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

		protected virtual void Configure()
		{
			Db4objects.Db4o.Db4oFactory.Configure().GenerateUUIDs(int.MaxValue);
			Db4objects.Db4o.Db4oFactory.Configure().GenerateVersionNumbers(int.MaxValue);
		}

		protected virtual void Reopen()
		{
			CloseBoth();
			OpenBoth();
		}

		private void OpenBoth()
		{
			_a.Open();
			_b.Open();
		}

		public virtual void TearDown()
		{
			CloseBoth();
			CleanBoth();
		}

		private void CloseBoth()
		{
			_a.Close();
			_b.Close();
		}

		public virtual void A(Db4objects.Drs.Tests.IDrsFixture fixture)
		{
			_a = fixture;
		}

		public virtual void B(Db4objects.Drs.Tests.IDrsFixture fixture)
		{
			_b = fixture;
		}

		public virtual Db4objects.Drs.Tests.IDrsFixture A()
		{
			return _a;
		}

		public virtual Db4objects.Drs.Tests.IDrsFixture B()
		{
			return _b;
		}

		protected virtual void EnsureOneInstance(Db4objects.Drs.Tests.IDrsFixture fixture,
			System.Type clazz)
		{
			EnsureInstanceCount(fixture, clazz, 1);
		}

		protected virtual void EnsureInstanceCount(Db4objects.Drs.Tests.IDrsFixture fixture
			, System.Type clazz, int count)
		{
			Db4objects.Db4o.IObjectSet objectSet = fixture.Provider().GetStoredObjects(clazz);
			Db4oUnit.Assert.AreEqual(count, objectSet.Count);
		}

		protected virtual object GetOneInstance(Db4objects.Drs.Tests.IDrsFixture fixture,
			System.Type clazz)
		{
			System.Collections.IEnumerator objectSet = fixture.Provider().GetStoredObjects(clazz
				).GetEnumerator();
			object candidate = null;
			if (objectSet.MoveNext())
			{
				candidate = objectSet.Current;
				if (objectSet.MoveNext())
				{
					throw new System.Exception("Found more than one instance of + " + clazz + " in provider = "
						 + fixture);
				}
			}
			return candidate;
		}

		protected virtual void ReplicateAll(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 providerFrom, Db4objects.Drs.Inside.ITestableReplicationProviderInside providerTo
			)
		{
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(providerFrom, providerTo);
			Db4objects.Db4o.IObjectSet allObjects = providerFrom.ObjectsChangedSinceLastReplication
				();
			if (!allObjects.HasNext())
			{
				throw new System.Exception("Can't find any objects to replicate");
			}
			while (allObjects.HasNext())
			{
				object changed = allObjects.Next();
				replication.Replicate(changed);
			}
			replication.Commit();
		}

		protected virtual void ReplicateAll(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 from, Db4objects.Drs.Inside.ITestableReplicationProviderInside to, Db4objects.Drs.IReplicationEventListener
			 listener)
		{
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(from, to, listener);
			Db4objects.Db4o.IObjectSet allObjects = from.ObjectsChangedSinceLastReplication();
			while (allObjects.HasNext())
			{
				object changed = allObjects.Next();
				replication.Replicate(changed);
			}
			replication.Commit();
		}

		protected virtual void Delete(System.Type[] classes)
		{
			for (int i = 0; i < classes.Length; i++)
			{
				A().Provider().DeleteAllInstances(classes[i]);
				B().Provider().DeleteAllInstances(classes[i]);
			}
			A().Provider().Commit();
			B().Provider().Commit();
		}

		protected virtual void ReplicateClass(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 providerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside providerB,
			System.Type clazz)
		{
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(providerA, providerB);
			System.Collections.IEnumerator allObjects = providerA.ObjectsChangedSinceLastReplication
				(clazz).GetEnumerator();
			while (allObjects.MoveNext())
			{
				object obj = allObjects.Current;
				replication.Replicate(obj);
			}
			replication.Commit();
		}

		protected static void Sleep(int millis)
		{
			try
			{
				Sharpen.Lang.Thread.Sleep(millis);
			}
			catch (System.Exception e)
			{
				throw new System.Exception(e.ToString());
			}
		}
	}
}
