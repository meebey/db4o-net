/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Sharpen;

namespace Db4objects.Db4o.Internal.Replication
{
	/// <exclude></exclude>
	public class MigrationConnection
	{
		public readonly ObjectContainerBase _peerA;

		public readonly ObjectContainerBase _peerB;

		private readonly Hashtable4 _referenceMap;

		private readonly Hashtable4 _identityMap;

		public MigrationConnection(ObjectContainerBase peerA, ObjectContainerBase peerB)
		{
			_referenceMap = new Hashtable4();
			_identityMap = new Hashtable4();
			_peerA = peerA;
			_peerB = peerB;
		}

		public virtual void MapReference(object obj, ObjectReference @ref)
		{
			// FIXME: Identityhashcode is not unique
			// ignored for now, since it is on most VMs.
			// This should be fixed by adding 
			// putIdentity and getIdentity methods to Hashtable4,
			// using the actual object as the parameter and 
			// checking for object identity in addition to the
			// hashcode
			_referenceMap.Put(Runtime.IdentityHashCode(obj), @ref);
		}

		public virtual void MapIdentity(object obj, object otherObj)
		{
			_identityMap.Put(Runtime.IdentityHashCode(obj), otherObj);
		}

		public virtual ObjectReference ReferenceFor(object obj)
		{
			int hcode = Runtime.IdentityHashCode(obj);
			ObjectReference @ref = (ObjectReference)_referenceMap.Get(hcode);
			_referenceMap.Remove(hcode);
			return @ref;
		}

		public virtual object IdentityFor(object obj)
		{
			int hcode = Runtime.IdentityHashCode(obj);
			return _identityMap.Get(hcode);
		}

		public virtual void Terminate()
		{
			_peerA.MigrateFrom(null);
			_peerB.MigrateFrom(null);
		}

		public virtual ObjectContainerBase Peer(ObjectContainerBase stream)
		{
			if (_peerA == stream)
			{
				return _peerB;
			}
			return _peerA;
		}
	}
}
