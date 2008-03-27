/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class ReplicationTraversalTest : Db4objects.Drs.Tests.DrsTestCase
	{
		private Db4objects.Drs.Tests.TransientReplicationProvider _peerA = new Db4objects.Drs.Tests.TransientReplicationProvider
			(new byte[] { 0 });

		private Db4objects.Drs.Tests.TransientReplicationProvider _peerB = new Db4objects.Drs.Tests.TransientReplicationProvider
			(new byte[] { 1 });

		public virtual void Test()
		{
			Db4objects.Drs.Tests.Replicated obj1 = new Db4objects.Drs.Tests.Replicated("1");
			Db4objects.Drs.Tests.Replicated obj2 = new Db4objects.Drs.Tests.Replicated("2");
			Db4objects.Drs.Tests.Replicated obj3 = new Db4objects.Drs.Tests.Replicated("3");
			obj1.SetLink(obj2);
			obj2.SetLink(obj3);
			obj3.SetLink(obj1);
			_peerA.StoreNew(obj1);
			//_peerA.transientProviderSpecificStore(obj2);
			//_peerA.transientProviderSpecificStore(obj3);
			Db4objects.Drs.IReplicationSession replication = new Db4objects.Drs.Inside.GenericReplicationSession
				(_peerA, _peerB);
			replication.Replicate(obj1);
			Db4oUnit.Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj1));
			Db4oUnit.Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj2));
			Db4oUnit.Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj3));
			_peerA = null;
			_peerB = null;
		}
	}
}
