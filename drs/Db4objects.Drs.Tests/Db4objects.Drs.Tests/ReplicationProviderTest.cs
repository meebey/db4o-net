/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ReplicationProviderTest : DrsTestCase
	{
		protected byte[] BSignatureBytes;

		protected IReadonlyReplicationProviderSignature BSignature;

		private IReadonlyReplicationProviderSignature ASignature;

		//import com.db4o.test.Test;
		public virtual void Test()
		{
			BSignatureBytes = B().Provider().GetSignature().GetSignature();
			ASignature = A().Provider().GetSignature();
			BSignature = B().Provider().GetSignature();
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
			A().Provider().StoreNew(new Pilot("Pilot1", 42));
			A().Provider().StoreNew(new Pilot("Pilot2", 43));
			A().Provider().Commit();
			A().Provider().StoreNew(new Pilot("Pilot3", 44));
			A().Provider().Delete(FindPilot("Pilot1"));
			Car car = new Car("Car1");
			car._pilot = FindPilot("Pilot2");
			A().Provider().StoreNew(car);
			A().Provider().Commit();
			StartReplication();
			Db4oUUID uuidCar1 = Uuid(FindCar("Car1"));
			Assert.IsNotNull(uuidCar1);
			A().Provider().ReplicateDeletion(uuidCar1);
			CommitReplication();
			Assert.IsNull(FindCar("Car1"));
			StartReplication();
			Db4oUUID uuidPilot2 = Uuid(FindPilot("Pilot2"));
			Assert.IsNotNull(uuidPilot2);
			A().Provider().ReplicateDeletion(uuidPilot2);
			CommitReplication();
			Assert.IsNull(FindPilot("Pilot2"));
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
			IEnumerator cars = A().Provider().GetStoredObjects(typeof(Car)).GetEnumerator();
			while (cars.MoveNext())
			{
				Car candidate = (Car)cars.Current;
				if (candidate.GetModel().Equals(model))
				{
					return candidate;
				}
			}
			return null;
		}

		private Pilot FindPilot(string name)
		{
			IEnumerator pilots = A().Provider().GetStoredObjects(typeof(Pilot)).GetEnumerator
				();
			while (pilots.MoveNext())
			{
				Pilot candidate = (Pilot)pilots.Current;
				if (candidate._name.Equals(name))
				{
					return candidate;
				}
			}
			return null;
		}

		private SPCChild GetOneChildFromA()
		{
			IObjectSet storedObjects = A().Provider().GetStoredObjects(typeof(SPCChild));
			Assert.AreEqual(1, storedObjects.Count);
			IEnumerator iterator = storedObjects.GetEnumerator();
			Assert.IsTrue(iterator.MoveNext());
			return (SPCChild)iterator.Current;
		}

		private void StartReplication()
		{
			A().Provider().StartReplicationTransaction(BSignature);
			B().Provider().StartReplicationTransaction(ASignature);
		}

		private void TstObjectUpdate()
		{
			SPCChild child = new SPCChild("c1");
			A().Provider().StoreNew(child);
			A().Provider().Commit();
			StartReplication();
			SPCChild reloaded = GetOneChildFromA();
			long oldVer = A().Provider().ProduceReference(reloaded, null, null).Version();
			CommitReplication();
			SPCChild reloaded2 = GetOneChildFromA();
			reloaded2.SetName("c3");
			//System.out.println("==============BEGIN DEBUG");
			A().Provider().Update(reloaded2);
			A().Provider().Commit();
			//System.out.println("==============END DEBUG");
			StartReplication();
			SPCChild reloaded3 = GetOneChildFromA();
			long newVer = A().Provider().ProduceReference(reloaded3, null, null).Version();
			CommitReplication();
			Assert.IsTrue(newVer > oldVer);
		}

		private void TstObjectsChangedSinceLastReplication()
		{
			Pilot object1 = new Pilot("John Cleese", 42);
			Pilot object2 = new Pilot("Terry Gilliam", 53);
			Car object3 = new Car("Volvo");
			A().Provider().StoreNew(object1);
			A().Provider().StoreNew(object2);
			A().Provider().StoreNew(object3);
			A().Provider().Commit();
			StartReplication();
			int i = A().Provider().ObjectsChangedSinceLastReplication().Count;
			Assert.AreEqual(i, 3);
			IObjectSet os = A().Provider().ObjectsChangedSinceLastReplication(typeof(Pilot));
			Assert.AreEqual(os.Count, 2);
			IEnumerator pilots = os.GetEnumerator();
			//		Assert.isTrue(pilots.contains(findPilot("John Cleese")));
			//	Assert.isTrue(pilots.contains(findPilot("Terry Gilliam")));
			IEnumerator cars = A().Provider().ObjectsChangedSinceLastReplication(typeof(Car))
				.GetEnumerator();
			Assert.AreEqual(((Car)Next(cars)).GetModel(), "Volvo");
			Assert.IsFalse(cars.MoveNext());
			CommitReplication();
			StartReplication();
			Assert.IsFalse(A().Provider().ObjectsChangedSinceLastReplication().GetEnumerator(
				).MoveNext());
			CommitReplication();
			Pilot pilot = (Pilot)Next(A().Provider().GetStoredObjects(typeof(Pilot)).GetEnumerator
				());
			pilot._name = "Terry Jones";
			Car car = (Car)Next(A().Provider().GetStoredObjects(typeof(Car)).GetEnumerator());
			car.SetModel("McLaren");
			A().Provider().Update(pilot);
			A().Provider().Update(car);
			A().Provider().Commit();
			StartReplication();
			Assert.AreEqual(A().Provider().ObjectsChangedSinceLastReplication().Count, 2);
			pilots = A().Provider().ObjectsChangedSinceLastReplication(typeof(Pilot)).GetEnumerator
				();
			Assert.AreEqual(((Pilot)Next(pilots))._name, "Terry Jones");
			Assert.IsFalse(pilots.MoveNext());
			cars = A().Provider().ObjectsChangedSinceLastReplication(typeof(Car)).GetEnumerator
				();
			Assert.AreEqual(((Car)Next(cars)).GetModel(), "McLaren");
			Assert.IsFalse(cars.MoveNext());
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Pilot));
			A().Provider().DeleteAllInstances(typeof(Car));
			A().Provider().Commit();
		}

		private object Next(IEnumerator iterator)
		{
			Assert.IsTrue(iterator.MoveNext());
			return iterator.Current;
		}

		private void TstReferences()
		{
			A().Provider().StoreNew(new Pilot("tst References", 42));
			A().Provider().Commit();
			StartReplication();
			Pilot object1 = (Pilot)Next(A().Provider().GetStoredObjects(typeof(Pilot)).GetEnumerator
				());
			IReplicationReference reference = A().Provider().ProduceReference(object1, null, 
				null);
			Assert.AreEqual(reference.Object(), object1);
			Db4oUUID uuid = reference.Uuid();
			IReplicationReference ref2 = A().Provider().ProduceReferenceByUUID(uuid, typeof(Pilot
				));
			Assert.AreEqual(ref2, reference);
			A().Provider().ClearAllReferences();
			Db4oUUID db4oUUID = A().Provider().ProduceReference(object1, null, null).Uuid();
			Assert.IsTrue(db4oUUID.Equals(uuid));
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Pilot));
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
			Pilot object1 = new Pilot("Albert Kwan", 25);
			Db4oUUID uuid = new Db4oUUID(5678, BSignatureBytes);
			IReplicationReference @ref = new ReplicationReferenceImpl(object1, uuid, 1);
			A().Provider().ReferenceNewObject(object1, @ref, null, null);
			A().Provider().StoreReplica(object1);
			Assert.IsFalse(A().Provider().WasModifiedSinceLastReplication(@ref));
			A().Provider().RollbackReplication();
			A().Provider().StartReplicationTransaction(BSignature);
			Assert.IsNull(A().Provider().ProduceReference(object1, null, null));
			IReplicationReference byUUID = A().Provider().ProduceReferenceByUUID(uuid, object1
				.GetType());
			Assert.IsNull(byUUID);
			A().Provider().RollbackReplication();
			B().Provider().RollbackReplication();
		}

		private void TstSignature()
		{
			Assert.IsNotNull(A().Provider().GetSignature());
		}

		private void TstStore()
		{
			StartReplication();
			Pilot object1 = new Pilot("John Cleese", 42);
			Db4oUUID uuid = new Db4oUUID(1234, BSignatureBytes);
			IReplicationReference @ref = new ReplicationReferenceImpl("ignoredSinceInOtherProvider"
				, uuid, 1);
			A().Provider().ReferenceNewObject(object1, @ref, null, null);
			A().Provider().StoreReplica(object1);
			IReplicationReference reference = A().Provider().ProduceReferenceByUUID(uuid, object1
				.GetType());
			Assert.AreEqual(A().Provider().ProduceReference(object1, null, null), reference);
			Assert.AreEqual(reference.Object(), object1);
			CommitReplication();
			StartReplication();
			IEnumerator storedObjects = A().Provider().GetStoredObjects(typeof(Pilot)).GetEnumerator
				();
			Pilot reloaded = (Pilot)Next(storedObjects);
			Assert.IsFalse(storedObjects.MoveNext());
			reference = A().Provider().ProduceReferenceByUUID(uuid, object1.GetType());
			Assert.AreEqual(A().Provider().ProduceReference(reloaded, null, null), reference);
			reloaded._name = "i am updated";
			A().Provider().StoreReplica(reloaded);
			A().Provider().ClearAllReferences();
			CommitReplication();
			StartReplication();
			reference = A().Provider().ProduceReferenceByUUID(uuid, reloaded.GetType());
			Assert.AreEqual(((Pilot)reference.Object())._name, "i am updated");
			CommitReplication();
			A().Provider().DeleteAllInstances(typeof(Pilot));
			A().Provider().Commit();
		}

		private Db4oUUID Uuid(object obj)
		{
			return A().Provider().ProduceReference(obj, null, null).Uuid();
		}
	}
}
