/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class TheSimplest : DrsTestCase
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
			ReplicateClass(A().Provider(), B().Provider(), typeof(SPCChild));
			EnsureNames(A(), "c3");
			EnsureNames(B(), "c3");
		}

		private void ModifyInA()
		{
			SPCChild child = GetTheObject(A());
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
			string name = "c1";
			SPCChild child = CreateChildObject(name);
			A().Provider().StoreNew(child);
			A().Provider().Commit();
			EnsureNames(A(), "c1");
		}

		protected virtual SPCChild CreateChildObject(string name)
		{
			return new SPCChild(name);
		}

		private void Replicate()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), "c1");
			EnsureNames(B(), "c1");
		}

		private void ModifyInB()
		{
			SPCChild child = GetTheObject(B());
			child.SetName("c2");
			B().Provider().Update(child);
			B().Provider().Commit();
			EnsureNames(B(), "c2");
		}

		private void EnsureNames(IDrsFixture fixture, string childName)
		{
			EnsureOneInstance(fixture, typeof(SPCChild));
			SPCChild child = GetTheObject(fixture);
			Assert.AreEqual(childName, child.GetName());
		}

		private SPCChild GetTheObject(IDrsFixture fixture)
		{
			return (SPCChild)GetOneInstance(fixture, typeof(SPCChild));
		}

		protected override void ReplicateClass(ITestableReplicationProviderInside providerA
			, ITestableReplicationProviderInside providerB, Type clazz)
		{
			//System.out.println("ReplicationTestcase.replicateClass");
			IReplicationSession replication = Db4objects.Drs.Replication.Begin(providerA, providerB
				);
			IEnumerator allObjects = providerA.ObjectsChangedSinceLastReplication(clazz).GetEnumerator
				();
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
