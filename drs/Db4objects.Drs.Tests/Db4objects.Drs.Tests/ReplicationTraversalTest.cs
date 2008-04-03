/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ReplicationTraversalTest : DrsTestCase
	{
		private TransientReplicationProvider _peerA = new TransientReplicationProvider(new 
			byte[] { 0 });

		private TransientReplicationProvider _peerB = new TransientReplicationProvider(new 
			byte[] { 1 });

		public virtual void Test()
		{
			Replicated obj1 = new Replicated("1");
			Replicated obj2 = new Replicated("2");
			Replicated obj3 = new Replicated("3");
			obj1.SetLink(obj2);
			obj2.SetLink(obj3);
			obj3.SetLink(obj1);
			_peerA.StoreNew(obj1);
			//_peerA.transientProviderSpecificStore(obj2);
			//_peerA.transientProviderSpecificStore(obj3);
			IReplicationSession replication = new GenericReplicationSession(_peerA, _peerB);
			replication.Replicate(obj1);
			Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj1));
			Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj2));
			Assert.IsTrue(_peerA.ActivatedObjects().Contains(obj3));
			_peerA = null;
			_peerB = null;
		}
	}
}
