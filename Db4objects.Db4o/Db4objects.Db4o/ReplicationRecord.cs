namespace Db4objects.Db4o
{
	/// <summary>
	/// tracks the version of the last replication between
	/// two Objectcontainers.
	/// </summary>
	/// <remarks>
	/// tracks the version of the last replication between
	/// two Objectcontainers.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class ReplicationRecord : Db4objects.Db4o.IInternal4
	{
		public Db4objects.Db4o.Ext.Db4oDatabase _youngerPeer;

		public Db4objects.Db4o.Ext.Db4oDatabase _olderPeer;

		public long _version;

		public ReplicationRecord()
		{
		}

		public ReplicationRecord(Db4objects.Db4o.Ext.Db4oDatabase younger, Db4objects.Db4o.Ext.Db4oDatabase
			 older)
		{
			_youngerPeer = younger;
			_olderPeer = older;
		}

		public virtual void SetVersion(long version)
		{
			_version = version;
		}

		public virtual void Store(Db4objects.Db4o.Internal.ObjectContainerBase stream)
		{
			stream.ShowInternalClasses(true);
			Db4objects.Db4o.Internal.Transaction ta = stream.CheckTransaction(null);
			stream.SetAfterReplication(ta, this, 1, false);
			stream.Commit();
			stream.ShowInternalClasses(false);
		}

		public static Db4objects.Db4o.ReplicationRecord BeginReplication(Db4objects.Db4o.Internal.Transaction
			 transA, Db4objects.Db4o.Internal.Transaction transB)
		{
			Db4objects.Db4o.Internal.ObjectContainerBase peerA = transA.Stream();
			Db4objects.Db4o.Internal.ObjectContainerBase peerB = transB.Stream();
			Db4objects.Db4o.Ext.Db4oDatabase dbA = peerA.Identity();
			Db4objects.Db4o.Ext.Db4oDatabase dbB = peerB.Identity();
			dbB.Bind(transA);
			dbA.Bind(transB);
			Db4objects.Db4o.Ext.Db4oDatabase younger = null;
			Db4objects.Db4o.Ext.Db4oDatabase older = null;
			if (dbA.IsOlderThan(dbB))
			{
				younger = dbB;
				older = dbA;
			}
			else
			{
				younger = dbA;
				older = dbB;
			}
			Db4objects.Db4o.ReplicationRecord rrA = QueryForReplicationRecord(peerA, younger, 
				older);
			Db4objects.Db4o.ReplicationRecord rrB = QueryForReplicationRecord(peerB, younger, 
				older);
			if (rrA == null)
			{
				if (rrB == null)
				{
					return new Db4objects.Db4o.ReplicationRecord(younger, older);
				}
				rrB.Store(peerA);
				return rrB;
			}
			if (rrB == null)
			{
				rrA.Store(peerB);
				return rrA;
			}
			if (rrA != rrB)
			{
				peerB.ShowInternalClasses(true);
				int id = peerB.GetID1(rrB);
				peerB.Bind1(transB, rrA, id);
				peerB.ShowInternalClasses(false);
			}
			return rrA;
		}

		public static Db4objects.Db4o.ReplicationRecord QueryForReplicationRecord(Db4objects.Db4o.Internal.ObjectContainerBase
			 stream, Db4objects.Db4o.Ext.Db4oDatabase younger, Db4objects.Db4o.Ext.Db4oDatabase
			 older)
		{
			Db4objects.Db4o.ReplicationRecord res = null;
			stream.ShowInternalClasses(true);
			Db4objects.Db4o.Query.IQuery q = stream.Query();
			q.Constrain(Db4objects.Db4o.Internal.Const4.CLASS_REPLICATIONRECORD);
			q.Descend("_youngerPeer").Constrain(younger).Identity();
			q.Descend("_olderPeer").Constrain(older).Identity();
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			if (objectSet.HasNext())
			{
				res = (Db4objects.Db4o.ReplicationRecord)objectSet.Next();
			}
			stream.ShowInternalClasses(false);
			return res;
		}
	}
}
