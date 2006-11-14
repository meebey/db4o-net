namespace Db4objects.Db4o.Inside.Replication
{
	/// <exclude></exclude>
	public class MigrationConnection
	{
		public readonly Db4objects.Db4o.YapStream _peerA;

		public readonly Db4objects.Db4o.YapStream _peerB;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _referenceMap;

		private readonly Db4objects.Db4o.Foundation.Hashtable4 _identityMap;

		public MigrationConnection(Db4objects.Db4o.YapStream peerA, Db4objects.Db4o.YapStream
			 peerB)
		{
			_referenceMap = new Db4objects.Db4o.Foundation.Hashtable4();
			_identityMap = new Db4objects.Db4o.Foundation.Hashtable4();
			_peerA = peerA;
			_peerB = peerB;
		}

		public virtual void MapReference(object obj, Db4objects.Db4o.YapObject @ref)
		{
			_referenceMap.Put(Sharpen.Runtime.IdentityHashCode(obj), @ref);
		}

		public virtual void MapIdentity(object obj, object otherObj)
		{
			_identityMap.Put(Sharpen.Runtime.IdentityHashCode(obj), otherObj);
		}

		public virtual Db4objects.Db4o.YapObject ReferenceFor(object obj)
		{
			int hcode = Sharpen.Runtime.IdentityHashCode(obj);
			Db4objects.Db4o.YapObject @ref = (Db4objects.Db4o.YapObject)_referenceMap.Get(hcode
				);
			_referenceMap.Remove(hcode);
			return @ref;
		}

		public virtual object IdentityFor(object obj)
		{
			int hcode = Sharpen.Runtime.IdentityHashCode(obj);
			return _identityMap.Get(hcode);
		}

		public virtual void Terminate()
		{
			_peerA.MigrateFrom(null);
			_peerB.MigrateFrom(null);
		}

		public virtual Db4objects.Db4o.YapStream Peer(Db4objects.Db4o.YapStream stream)
		{
			if (_peerA == stream)
			{
				return _peerB;
			}
			return _peerA;
		}
	}
}
