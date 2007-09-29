/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Test
{
	public class TheSimplest : Db4objects.Drs.Test.DrsTestCase
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
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Test.SPCChild)
				);
			EnsureNames(A(), "c3");
			EnsureNames(B(), "c3");
		}

		private void ModifyInA()
		{
			Db4objects.Drs.Test.SPCChild child = GetTheObject(A());
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
			Db4objects.Drs.Test.SPCChild child = new Db4objects.Drs.Test.SPCChild("c1");
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
			Db4objects.Drs.Test.SPCChild child = GetTheObject(B());
			child.SetName("c2");
			B().Provider().Update(child);
			B().Provider().Commit();
			EnsureNames(B(), "c2");
		}

		private void EnsureNames(Db4objects.Drs.Test.IDrsFixture fixture, string childName
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCChild));
			Db4objects.Drs.Test.SPCChild child = GetTheObject(fixture);
			Db4oUnit.Assert.AreEqual(childName, child.GetName());
		}

		private Db4objects.Drs.Test.SPCChild GetTheObject(Db4objects.Drs.Test.IDrsFixture
			 fixture)
		{
			return (Db4objects.Drs.Test.SPCChild)GetOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCChild)
				);
		}

		protected override void ReplicateClass(Db4objects.Drs.Inside.ITestableReplicationProviderInside
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
	}
}
