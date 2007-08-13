/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Replication;

namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	[System.ObsoleteAttribute]
	public class ReplicationImpl : IReplicationProcess
	{
		internal readonly ObjectContainerBase _peerA;

		internal readonly Transaction _transA;

		internal readonly ObjectContainerBase _peerB;

		internal readonly Transaction _transB;

		internal readonly IReplicationConflictHandler _conflictHandler;

		internal readonly ReplicationRecord _record;

		private int _direction;

		private const int IGNORE = 0;

		private const int TO_B = -1;

		private const int TO_A = 1;

		private const int CHECK_CONFLICT = -99;

		public ReplicationImpl(ObjectContainerBase peerA, ObjectContainerBase peerB, IReplicationConflictHandler
			 conflictHandler)
		{
			if (conflictHandler == null)
			{
				throw new ArgumentNullException();
			}
			lock (peerA.Lock())
			{
				lock (peerB.Lock())
				{
					_peerA = peerA;
					_transA = peerA.CheckTransaction();
					_peerB = peerB;
					_transB = _peerB.CheckTransaction(null);
					MigrationConnection mgc = new MigrationConnection(_peerA, _peerB);
					_peerA._handlers.MigrationConnection(mgc);
					_peerA._handlers.Replication(this);
					_peerA.ReplicationCallState(Const4.OLD);
					_peerB._handlers.MigrationConnection(mgc);
					_peerB._handlers.Replication(this);
					_peerB.ReplicationCallState(Const4.OLD);
					_conflictHandler = conflictHandler;
					_record = ReplicationRecord.BeginReplication(_transA, _transB);
				}
			}
		}

		private int BindAndSet(Transaction trans, ObjectContainerBase peer, ObjectReference
			 @ref, object sourceObject)
		{
			if (sourceObject is IDb4oTypeImpl)
			{
				IDb4oTypeImpl db4oType = (IDb4oTypeImpl)sourceObject;
				if (!db4oType.CanBind())
				{
					IDb4oTypeImpl targetObject = (IDb4oTypeImpl)@ref.GetObject();
					targetObject.ReplicateFrom(sourceObject);
					return @ref.GetID();
				}
			}
			peer.Bind2(trans, @ref, sourceObject);
			return peer.SetAfterReplication(trans, sourceObject, 1, true);
		}

		public virtual void CheckConflict(object obj)
		{
			int temp = _direction;
			_direction = CHECK_CONFLICT;
			Replicate(obj);
			_direction = temp;
		}

		public virtual void Commit()
		{
			lock (_peerA.Lock())
			{
				lock (_peerB.Lock())
				{
					_peerA.Commit(_transA);
					_peerB.Commit(_transB);
					EndReplication();
					long versionA = _peerA.CurrentVersion();
					long versionB = _peerB.CurrentVersion();
					_record._version = (versionA > versionB) ? versionA : versionB;
					_peerA.RaiseVersion(_record._version + 1);
					_peerB.RaiseVersion(_record._version + 1);
					_record.Store(_peerA);
					_record.Store(_peerB);
				}
			}
		}

		private void EndReplication()
		{
			_peerA.ReplicationCallState(Const4.NONE);
			_peerA._handlers.MigrationConnection(null);
			_peerA._handlers.Replication(null);
			_peerA.ReplicationCallState(Const4.NONE);
			_peerB._handlers.MigrationConnection(null);
			_peerB._handlers.Replication(null);
		}

		private int IdInCaller(ObjectContainerBase caller, ObjectReference referenceA, ObjectReference
			 referenceB)
		{
			return (caller == _peerA) ? referenceA.GetID() : referenceB.GetID();
		}

		private int IgnoreOrCheckConflict()
		{
			if (_direction == CHECK_CONFLICT)
			{
				return CHECK_CONFLICT;
			}
			return IGNORE;
		}

		private bool IsInConflict(long versionA, long versionB)
		{
			if (versionA > _record._version && versionB > _record._version)
			{
				return true;
			}
			if (versionB > _record._version && _direction == TO_B)
			{
				return true;
			}
			if (versionA > _record._version && _direction == TO_A)
			{
				return true;
			}
			return false;
		}

		private long LastSynchronization()
		{
			return _record._version;
		}

		public virtual IObjectContainer PeerA()
		{
			return (IObjectContainer)_peerA;
		}

		public virtual IObjectContainer PeerB()
		{
			return (IObjectContainer)_peerB;
		}

		public virtual void Replicate(object obj)
		{
			ObjectContainerBase container = _peerB;
			Transaction trans = _transB;
			if (_peerB.IsStored(_transB, obj))
			{
				if (!_peerA.IsStored(_transA, obj))
				{
					container = _peerA;
					trans = _transA;
				}
			}
			container.Set(trans, obj);
		}

		public virtual void Rollback()
		{
			_peerA.Rollback(_transA);
			_peerB.Rollback(_transB);
			EndReplication();
		}

		public virtual void SetDirection(IObjectContainer replicateFrom, IObjectContainer
			 replicateTo)
		{
			if (replicateFrom == _peerA && replicateTo == _peerB)
			{
				_direction = TO_B;
			}
			if (replicateFrom == _peerB && replicateTo == _peerA)
			{
				_direction = TO_A;
			}
		}

		private void ShareBinding(ObjectReference sourceReference, ObjectReference referenceA
			, object objectA, ObjectReference referenceB, object objectB)
		{
			if (sourceReference == null)
			{
				return;
			}
			if (objectA is IDb4oTypeImpl)
			{
				if (!((IDb4oTypeImpl)objectA).CanBind())
				{
					return;
				}
			}
			if (sourceReference == referenceA)
			{
				_peerB.Bind2(_transB, referenceB, objectA);
			}
			else
			{
				_peerA.Bind2(_transA, referenceA, objectB);
			}
		}

		private int ToA()
		{
			if (_direction == CHECK_CONFLICT)
			{
				return CHECK_CONFLICT;
			}
			if (_direction != TO_B)
			{
				return TO_A;
			}
			return IGNORE;
		}

		private int ToB()
		{
			if (_direction == CHECK_CONFLICT)
			{
				return CHECK_CONFLICT;
			}
			if (_direction != TO_A)
			{
				return TO_B;
			}
			return IGNORE;
		}

		/// <summary>called by YapStream.set()</summary>
		/// <returns>
		/// id of reference in caller or 0 if not handled or -1
		/// if #set() should stop processing because of a direction
		/// setting.
		/// </returns>
		public virtual int TryToHandle(ObjectContainerBase caller, object obj)
		{
			int notProcessed = 0;
			ObjectContainerBase other = null;
			ObjectReference sourceReference = null;
			if (caller == _peerA)
			{
				other = _peerB;
				if (_direction == TO_B)
				{
					notProcessed = -1;
				}
			}
			else
			{
				other = _peerA;
				if (_direction == TO_A)
				{
					notProcessed = -1;
				}
			}
			lock (other._lock)
			{
				object objectA = obj;
				object objectB = obj;
				ObjectReference referenceA = _transA.ReferenceForObject(obj);
				ObjectReference referenceB = _transB.ReferenceForObject(obj);
				VirtualAttributes attA = null;
				VirtualAttributes attB = null;
				if (referenceA == null)
				{
					if (referenceB == null)
					{
						return notProcessed;
					}
					sourceReference = referenceB;
					attB = referenceB.VirtualAttributes(_transB);
					if (attB == null)
					{
						return notProcessed;
					}
					HardObjectReference hardRef = _transA.GetHardReferenceBySignature(attB.i_uuid, attB
						.i_database.i_signature);
					if (hardRef._object == null)
					{
						return notProcessed;
					}
					referenceA = hardRef._reference;
					objectA = hardRef._object;
					attA = referenceA.VirtualAttributes(_transA);
				}
				else
				{
					attA = referenceA.VirtualAttributes(_transA);
					if (attA == null)
					{
						return notProcessed;
					}
					if (referenceB == null)
					{
						sourceReference = referenceA;
						HardObjectReference hardRef = _transB.GetHardReferenceBySignature(attA.i_uuid, attA
							.i_database.i_signature);
						if (hardRef._object == null)
						{
							return notProcessed;
						}
						referenceB = hardRef._reference;
						objectB = hardRef._object;
					}
					attB = referenceB.VirtualAttributes(_transB);
				}
				if (attA == null || attB == null)
				{
					return notProcessed;
				}
				if (objectA == objectB)
				{
					if (caller == _peerA && _direction == TO_B)
					{
						return -1;
					}
					if (caller == _peerB && _direction == TO_A)
					{
						return -1;
					}
					return IdInCaller(caller, referenceA, referenceB);
				}
				_peerA.Refresh(_transA, objectA, 1);
				_peerB.Refresh(_transB, objectB, 1);
				if (attA.i_version <= _record._version && attB.i_version <= _record._version)
				{
					if (_direction != CHECK_CONFLICT)
					{
						ShareBinding(sourceReference, referenceA, objectA, referenceB, objectB);
					}
					return IdInCaller(caller, referenceA, referenceB);
				}
				int direction = IgnoreOrCheckConflict();
				if (IsInConflict(attA.i_version, attB.i_version))
				{
					object prevailing = _conflictHandler.ResolveConflict(this, objectA, objectB);
					if (prevailing == objectA)
					{
						direction = (_direction == TO_A) ? IGNORE : ToB();
					}
					if (prevailing == objectB)
					{
						direction = (_direction == TO_B) ? IGNORE : ToA();
					}
					if (direction == IGNORE)
					{
						return -1;
					}
				}
				else
				{
					direction = attB.i_version > _record._version ? ToA() : ToB();
				}
				if (direction == TO_A)
				{
					if (!referenceB.IsActive())
					{
						referenceB.Activate(_transB, objectB, 1, false);
					}
					int idA = BindAndSet(_transA, _peerA, referenceA, objectB);
					if (caller == _peerA)
					{
						return idA;
					}
				}
				if (direction == TO_B)
				{
					if (!referenceA.IsActive())
					{
						referenceA.Activate(_transA, objectA, 1, false);
					}
					int idB = BindAndSet(_transB, _peerB, referenceB, objectA);
					if (caller == _peerB)
					{
						return idB;
					}
				}
				return IdInCaller(caller, referenceA, referenceB);
			}
		}

		public virtual void WhereModified(IQuery query)
		{
			query.Descend(VirtualField.VERSION).Constrain(LastSynchronization()).Greater();
		}
	}
}
