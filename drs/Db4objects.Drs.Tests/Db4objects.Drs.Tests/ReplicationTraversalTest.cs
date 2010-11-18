/* This file is part of the db4o object database http://www.db4o.com

Copyright (C) 2004 - 2009  Versant Corporation http://www.versant.com

db4o is free software; you can redistribute it and/or modify it under
the terms of version 3 of the GNU General Public License as published
by the Free Software Foundation.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program.  If not, see http://www.gnu.org/licenses/. */
using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Data;

namespace Db4objects.Drs.Tests
{
	public class ReplicationTraversalTest : ITestLifeCycle
	{
		private TransientReplicationProvider _peerA;

		private TransientReplicationProvider _peerB;

		/// <exception cref="System.Exception"></exception>
		public virtual void SetUp()
		{
			_peerA = new TransientReplicationProvider(new byte[] { 0 }, "A");
			_peerB = new TransientReplicationProvider(new byte[] { 1 }, "B");
			ReplicationReflector reflector = new ReplicationReflector(_peerA, _peerB, null);
			_peerA.ReplicationReflector(reflector);
			_peerB.ReplicationReflector(reflector);
		}

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

		/// <exception cref="System.Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
