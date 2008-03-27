/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class TheSimplest : Db4objects.Drs.Tests.DrsTestCase
	{
		public virtual void Test()
		{
			StoreInA();
			Replicate();
			ModifyInB();
			Replicate2();
			ModifyInA();
			Replicate3();
		}

		private void Replicate3()
		{
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Tests.SPCChild
				));
			EnsureNames(A(), "c3");
			EnsureNames(B(), "c3");
		}

		private void ModifyInA()
		{
			Db4objects.Drs.Tests.SPCChild child = GetTheObject(A());
			child.SetName("c3");
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), "c3");
		}

		private void Replicate2()
		{
			ReplicateAll(B().Provider(), A().Provider());
			EnsureNames(A(), "c2");
			EnsureNames(B(), "c2");
		}

		private void StoreInA()
		{
			Db4objects.Drs.Tests.SPCChild child = new Db4objects.Drs.Tests.SPCChild("c1");
			A().Provider().StoreNew(child);
			A().Provider().Commit();
			EnsureNames(A(), "c1");
		}

		private void Replicate()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), "c1");
			EnsureNames(B(), "c1");
		}

		private void ModifyInB()
		{
			Db4objects.Drs.Tests.SPCChild child = GetTheObject(B());
			child.SetName("c2");
			B().Provider().Update(child);
			B().Provider().Commit();
			EnsureNames(B(), "c2");
		}

		private void EnsureNames(Db4objects.Drs.Tests.IDrsFixture fixture, string childName
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCChild));
			Db4objects.Drs.Tests.SPCChild child = GetTheObject(fixture);
			Db4oUnit.Assert.AreEqual(childName, child.GetName());
		}

		private Db4objects.Drs.Tests.SPCChild GetTheObject(Db4objects.Drs.Tests.IDrsFixture
			 fixture)
		{
			return (Db4objects.Drs.Tests.SPCChild)GetOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCChild
				));
		}

		protected override void ReplicateClass(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 providerA, Db4objects.Drs.Inside.ITestableReplicationProviderInside providerB, 
			System.Type clazz)
		{
			//System.out.println("ReplicationTestcase.replicateClass");
			Db4objects.Drs.IReplicationSession replication = Db4objects.Drs.Replication.Begin
				(providerA, providerB);
			System.Collections.IEnumerator allObjects = providerA.ObjectsChangedSinceLastReplication
				(clazz).GetEnumerator();
			while (allObjects.MoveNext())
			{
				object obj = allObjects.Current;
				//System.out.println("obj = " + obj);
				replication.Replicate(obj);
			}
			replication.Commit();
		}
	}
}
