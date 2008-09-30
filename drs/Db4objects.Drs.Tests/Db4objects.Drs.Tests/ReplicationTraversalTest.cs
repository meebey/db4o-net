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
using System;
using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ReplicationTraversalTest : ITestLifeCycle
	{
		private TransientReplicationProvider _peerA;

		private TransientReplicationProvider _peerB;

		/// <exception cref="Exception"></exception>
		public virtual void SetUp()
		{
			_peerA = new TransientReplicationProvider(new byte[] { 0 }, "A");
			_peerB = new TransientReplicationProvider(new byte[] { 1 }, "B");
			ReplicationReflector reflector = new ReplicationReflector(_peerA, _peerB);
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

		/// <exception cref="Exception"></exception>
		public virtual void TearDown()
		{
		}
	}
}
