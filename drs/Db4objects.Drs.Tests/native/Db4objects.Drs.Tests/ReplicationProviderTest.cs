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
	public class ReplicationProviderTest : Db4objects.Drs.Tests.DrsTestCase
	{
		protected byte[] B_SIGNATURE_BYTES;

		protected Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature B_SIGNATURE;

		private Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature A_SIGNATURE;

		public virtual void Test()
		{
			B_SIGNATURE_BYTES = B().Provider().GetSignature().GetSignature();
			A_SIGNATURE = A().Provider().GetSignature();
			B_SIGNATURE = B().Provider().GetSignature();
			TstObjectUpdate();
			TstSignature();
			TstObjectsChangedSinceLastReplication();
			TstReferences();
			TstStore();
			TstRollback();
			TstDeletion();
		}

		protected virtual void TstDeletion()
		{
			A().Provider().StoreNew(new Db4objects.Drs.Tests.Pilot("Pilot1", 42));
			A().Provider().StoreNew(new Db4objects.Drs.Tests.Pilot("Pilot2", 43));
			A().Provider().Commit();
			A().Provider().StoreNew(new Db4objects.Drs.Tests.Pilot("Pilot3", 44));
			A().Provider().Delete(FindPilot("Pilot1"));
			Db4objects.Drs.Tests.Car car = new Db4objects.Drs.Tests.Car("Car1");
			car._pilot = FindPilot("Pilot2");
			A().Provider().StoreNew(car);
			A().Provider().Commit();
			StartReplication();
			Db4objects.Db4o.Ext.Db4oUUID uuidCar1 = Uuid(FindCar("Car1"));
			Db4oUnit.Assert.IsNotNull(uuidCar1);
			A().Provider().ReplicateDeletion(uuidCar1);
			CommitReplication();
			Db4oUnit.Assert.IsNull(FindCar("Car1"));
			StartReplication();
			Db4objects.Db4o.Ext.Db4oUUID uuidPilot2 = Uuid(FindPilot("Pilot2"));
			Db4oUnit.Assert.IsNotNull(uuidPilot2);
			A().Provider().ReplicateDeletion(uuidPilot2);
			CommitReplication();
			Db4oUnit.Assert.IsNull(FindPilot("Pilot2"));
		}

		private void CommitReplication()
		{
			long maxVersion = A().Provider().GetCurrentVersion() > B().Provider().GetCurrentVersion
				() ? A().Provider().GetCurrentVersion() : B().Provider().GetCurrentVersion();
			A().Provider().SyncVersionWithPeer(maxVersion);
			B().Provider().SyncVersionWithPeer(maxVersion);
			maxVersion++;
			A().Provider().CommitReplicationTransaction(maxVersion);
			B().Provider().CommitReplicationTransaction(maxVersion);
		}

		private object FindCar(string model)
		{
			System.Collections.IEnumerator cars = A().Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.Car)
				).GetEnumerator();
			while (cars.MoveNext())
			{
				Db4objects.Drs.Tests.Car candidate = (Db4objects.Drs.Tests.Car)cars.Current;
				if (candidate.GetModel().Equals(model))
				{
					return candidate;
				}
			}
			return null;
		}

		private Db4objects.Drs.Tests.Pilot FindPilot(string name)
		{
			System.Collections.IEnumerator pilots = A().Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.Pilot)
				).GetEnumerator();
			while (pilots.MoveNext())
			{
				Db4objects.Drs.Tests.Pilot candidate = (Db4objects.Drs.Tests.Pilot)pilots.Current;
				if (candidate._name.Equals(name))
				{
					return candidate;
				}
			}
			return null;
		}

		private Db4objects.Drs.Tests.SPCChild GetOneChildFromA()
		{
			Db4objects.Db4o.IObjectSet storedObjects = A().Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.SPCChild)
				);
			Db4oUnit.Assert.AreEqual(1, storedObjects.Count);
			System.Collections.IEnumerator iterator = storedObjects.GetEnumerator();
			Db4oUnit.Assert.IsTrue(iterator.MoveNext());
			return (Db4objects.Drs.Tests.SPCChild)iterator.Current;
		}

		private void StartReplication()
		{
			A().Provider().StartReplicationTransaction(B_SIGNATURE);
			B().Provider().StartReplicationTransaction(A_SIGNATURE);
		}

		private void TstObjectUpdate()
		{
			Db4objects.Drs.Tests.SPCChild child = new Db4objects.Drs.Tests.SPCChild("c1");
			A().Provider().StoreNew(child);
			A().Provider().Commit();
			StartReplication();
			Db4objects.Drs.Tests.SPCChild reloaded = GetOneChildFromA();
			long oldVer = A().Provider().ProduceReference(reloaded, null, null).Version();
			CommitReplication();
			Db4objects.Drs.Tests.SPCChild reloaded2 = GetOneChildFromA();
			reloaded2.SetName("c3");
			A().Provider().Update(reloaded2);
			A().Provider().Commit();
			StartReplication();
			Db4objects.Drs.Tests.SPCChild reloaded3 = GetOneChildFromA();
			long newVer = A().Provider().ProduceReference(reloaded3, null, null).Version();
			CommitReplication();
			Db4oUnit.Assert.IsTrue(newVer > oldVer);
		}

		private void TstObjectsChangedSinceLastReplication()
		{
			Db4objects.Drs.Tests.Pilot object1 = new Db4objects.Drs.Tests.Pilot("John Cleese", 
				42);
			Db4objects.Drs.Tests.Pilot object2 = new Db4objects.Drs.Tests.Pilot("Terry Gilliam"
				, 53);
			Db4objects.Drs.Tests.Car object3 = new Db4objects.Drs.Tests.Car("Volvo");
			A().Provider().StoreNew(object1);
			A().Provider().StoreNew(object2);
			A().Provider().StoreNew(object3);
			A().Provider().Commit();
			StartReplication();
			int i = A().Provider().ObjectsChangedSinceLastReplication().Count;
			Db4oUnit.Assert.AreEqual(i, 3);
			Db4objects.Db4o.IObjectSet os = A().Provider().ObjectsChangedSinceLastReplication
				(typeof(Db4objects.Drs.Tests.Pilot));
			Db4oUnit.Assert.AreEqual(os.Count, 2);

			//System.Collections.IEnumerator pilots = os.GetEnumerator();
            Db4objects.Db4o.IObjectSet cars = A().Provider().ObjectsChangedSinceLastReplication(typeof(Db4objects.Drs.Tests.Car));
			Db4oUnit.Assert.IsTrue(cars.HasNext());
			Db4oUnit.Assert.AreEqual(((Db4objects.Drs.Tests.Car)cars.Next()).GetModel(), "Volvo"
				);
			Db4oUnit.Assert.IsFalse(cars.HasNext());
			CommitReplication();
			StartReplication();
			Db4oUnit.Assert.IsFalse(A().Provider().ObjectsChangedSinceLastReplication().HasNext());
			CommitReplication();
			Db4objects.Drs.Tests.Pilot pilot = (Db4objects.Drs.Tests.Pilot)A().Provider().GetStoredObjects
				(typeof(Db4objects.Drs.Tests.Pilot)).Next();
			pilot._name = "Terry Jones";
			Db4objects.Drs.Tests.Car car = (Db4objects.Drs.Tests.Car)A().Provider().GetStoredObjects
				(typeof(Db4objects.Drs.Tests.Car)).Next();
			car.SetModel("McLaren");
			A().Provider().Update(pilot);
			A().Provider().Update(car);
			A().Provider().Commit();
			StartReplication();
			Db4oUnit.Assert.AreEqual(A().Provider().ObjectsChangedSinceLastReplication().Count
				, 2);
            Db4objects.Db4o.IObjectSet pilots = A().Provider().ObjectsChangedSinceLastReplication(typeof(Db4objects.Drs.Tests.Pilot)
				);
			Db4oUnit.Assert.AreEqual(((Db4objects.Drs.Tests.Pilot)pilots.Next())._name, "Terry Jones"
				);
			Db4oUnit.Assert.IsFalse(pilots.HasNext());
			cars = A().Provider().ObjectsChangedSinceLastReplication(typeof(Db4objects.Drs.Tests.Car)
				);
			Db4oUnit.Assert.AreEqual(((Db4objects.Drs.Tests.Car)cars.Next()).GetModel(), "McLaren"
				);
			Db4oUnit.Assert.IsFalse(cars.HasNext());
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.Pilot));
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.Car));
			A().Provider().Commit();
		}

		private void TstReferences()
		{
			A().Provider().StoreNew(new Db4objects.Drs.Tests.Pilot("tst References", 42));
			A().Provider().Commit();
			StartReplication();
			Db4objects.Drs.Tests.Pilot object1 = (Db4objects.Drs.Tests.Pilot)A().Provider().GetStoredObjects
				(typeof(Db4objects.Drs.Tests.Pilot)).Next();
			Db4objects.Drs.Inside.IReplicationReference reference = A().Provider().ProduceReference
				(object1, null, null);
			Db4oUnit.Assert.AreEqual(reference.Object(), object1);
			Db4objects.Db4o.Ext.Db4oUUID uuid = reference.Uuid();
			Db4objects.Drs.Inside.IReplicationReference ref2 = A().Provider().ProduceReferenceByUUID
				(uuid, typeof(Db4objects.Drs.Tests.Pilot));
			Db4oUnit.Assert.AreEqual(ref2, reference);
			A().Provider().ClearAllReferences();
			Db4objects.Db4o.Ext.Db4oUUID db4oUUID = A().Provider().ProduceReference(object1, 
				null, null).Uuid();
			Db4oUnit.Assert.IsTrue(db4oUUID.Equals(uuid));
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.Pilot));
			A().Provider().Commit();
		}

		private void TstRollback()
		{
			if (!A().Provider().SupportsRollback())
			{
				return;
			}
			if (!B().Provider().SupportsRollback())
			{
				return;
			}
			StartReplication();
			Db4objects.Drs.Tests.Pilot object1 = new Db4objects.Drs.Tests.Pilot("Albert Kwan", 
				25);
			Db4objects.Db4o.Ext.Db4oUUID uuid = new Db4objects.Db4o.Ext.Db4oUUID(5678, B_SIGNATURE_BYTES
				);
			Db4objects.Drs.Inside.IReplicationReference @ref = new Db4objects.Drs.Inside.ReplicationReferenceImpl
				(object1, uuid, 1);
			A().Provider().ReferenceNewObject(object1, @ref, null, null);
			A().Provider().StoreReplica(object1);
			Db4oUnit.Assert.IsFalse(A().Provider().WasModifiedSinceLastReplication(@ref));
			A().Provider().RollbackReplication();
			A().Provider().StartReplicationTransaction(B_SIGNATURE);
			Db4oUnit.Assert.IsNull(A().Provider().ProduceReference(object1, null, null));
			Db4objects.Drs.Inside.IReplicationReference byUUID = A().Provider().ProduceReferenceByUUID
				(uuid, object1.GetType());
			Db4oUnit.Assert.IsNull(byUUID);
			A().Provider().RollbackReplication();
			B().Provider().RollbackReplication();
		}

		private void TstSignature()
		{
			Db4oUnit.Assert.IsNotNull(A().Provider().GetSignature());
		}

		private void TstStore()
		{
			StartReplication();
			Db4objects.Drs.Tests.Pilot object1 = new Db4objects.Drs.Tests.Pilot("John Cleese", 
				42);
			Db4objects.Db4o.Ext.Db4oUUID uuid = new Db4objects.Db4o.Ext.Db4oUUID(1234, B_SIGNATURE_BYTES
				);
			Db4objects.Drs.Inside.IReplicationReference @ref = new Db4objects.Drs.Inside.ReplicationReferenceImpl
				("ignoredSinceInOtherProvider", uuid, 1);
			A().Provider().ReferenceNewObject(object1, @ref, null, null);
			A().Provider().StoreReplica(object1);
			Db4objects.Drs.Inside.IReplicationReference reference = A().Provider().ProduceReferenceByUUID
				(uuid, object1.GetType());
			Db4oUnit.Assert.AreEqual(A().Provider().ProduceReference(object1, null, null), reference
				);
			Db4oUnit.Assert.AreEqual(reference.Object(), object1);
			CommitReplication();
			StartReplication();
			System.Collections.IEnumerator storedObjects = A().Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.Pilot)
				).GetEnumerator();
			Db4oUnit.Assert.IsTrue(storedObjects.MoveNext());
			Db4objects.Drs.Tests.Pilot reloaded = (Db4objects.Drs.Tests.Pilot)storedObjects.Current;
			Db4oUnit.Assert.IsFalse(storedObjects.MoveNext());
			reference = A().Provider().ProduceReferenceByUUID(uuid, object1.GetType());
			Db4oUnit.Assert.AreEqual(A().Provider().ProduceReference(reloaded, null, null), reference
				);
			reloaded._name = "i am updated";
			A().Provider().StoreReplica(reloaded);
			A().Provider().ClearAllReferences();
			CommitReplication();
			StartReplication();
			reference = A().Provider().ProduceReferenceByUUID(uuid, reloaded.GetType());
			Db4oUnit.Assert.AreEqual(((Db4objects.Drs.Tests.Pilot)reference.Object())._name, "i am updated"
				);
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Db4objects.Drs.Tests.Pilot));
			A().Provider().Commit();
		}

		private Db4objects.Db4o.Ext.Db4oUUID Uuid(object obj)
		{
			return A().Provider().ProduceReference(obj, null, null).Uuid();
		}
	}
}
