/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;

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
	public class ReplicationRecord : IInternal4
	{
		public Db4oDatabase _youngerPeer;

		public Db4oDatabase _olderPeer;

		public long _version;

		public ReplicationRecord()
		{
		}

		public ReplicationRecord(Db4oDatabase younger, Db4oDatabase older)
		{
			_youngerPeer = younger;
			_olderPeer = older;
		}

		public virtual void SetVersion(long version)
		{
			_version = version;
		}

		public virtual void Store(ObjectContainerBase container)
		{
			container.ShowInternalClasses(true);
			try
			{
				Transaction trans = container.CheckTransaction();
				container.SetAfterReplication(trans, this, 1, false);
				container.Commit(trans);
			}
			finally
			{
				container.ShowInternalClasses(false);
			}
		}

		public static Db4objects.Db4o.ReplicationRecord BeginReplication(Transaction transA
			, Transaction transB)
		{
			ObjectContainerBase peerA = transA.Container();
			ObjectContainerBase peerB = transB.Container();
			Db4oDatabase dbA = ((IInternalObjectContainer)peerA).Identity();
			Db4oDatabase dbB = ((IInternalObjectContainer)peerB).Identity();
			dbB.Bind(transA);
			dbA.Bind(transB);
			Db4oDatabase younger = null;
			Db4oDatabase older = null;
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
			Db4objects.Db4o.ReplicationRecord rrA = QueryForReplicationRecord(peerA, transA, 
				younger, older);
			Db4objects.Db4o.ReplicationRecord rrB = QueryForReplicationRecord(peerB, transB, 
				younger, older);
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
				try
				{
					int id = peerB.GetID(transB, rrB);
					peerB.Bind(transB, rrA, id);
				}
				finally
				{
					peerB.ShowInternalClasses(false);
				}
			}
			return rrA;
		}

		public static Db4objects.Db4o.ReplicationRecord QueryForReplicationRecord(ObjectContainerBase
			 container, Transaction trans, Db4oDatabase younger, Db4oDatabase older)
		{
			container.ShowInternalClasses(true);
			try
			{
				IQuery q = container.Query(trans);
				q.Constrain(Const4.ClassReplicationrecord);
				q.Descend("_youngerPeer").Constrain(younger).Identity();
				q.Descend("_olderPeer").Constrain(older).Identity();
				IObjectSet objectSet = q.Execute();
				return objectSet.HasNext() ? (Db4objects.Db4o.ReplicationRecord)objectSet.Next() : 
					null;
			}
			finally
			{
				container.ShowInternalClasses(false);
			}
		}
	}
}
