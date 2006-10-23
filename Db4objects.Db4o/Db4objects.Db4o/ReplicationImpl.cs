namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class ReplicationImpl : Db4objects.Db4o.Replication.IReplicationProcess
	{
		internal readonly Db4objects.Db4o.YapStream _peerA;

		internal readonly Db4objects.Db4o.Transaction _transA;

		internal readonly Db4objects.Db4o.YapStream _peerB;

		internal readonly Db4objects.Db4o.Transaction _transB;

		internal readonly Db4objects.Db4o.Replication.IReplicationConflictHandler _conflictHandler;

		internal readonly Db4objects.Db4o.ReplicationRecord _record;

		private int _direction;

		private const int IGNORE = 0;

		private const int TO_B = -1;

		private const int TO_A = 1;

		private const int CHECK_CONFLICT = -99;

		public ReplicationImpl(Db4objects.Db4o.YapStream peerA, Db4objects.Db4o.IObjectContainer
			 peerB, Db4objects.Db4o.Replication.IReplicationConflictHandler conflictHandler)
		{
			if (conflictHandler == null)
			{
				throw new System.ArgumentNullException();
			}
			lock (peerA.Ext().Lock())
			{
				lock (peerB.Ext().Lock())
				{
					_peerA = peerA;
					_transA = peerA.CheckTransaction(null);
					_peerB = (Db4objects.Db4o.YapStream)peerB;
					_transB = _peerB.CheckTransaction(null);
					Db4objects.Db4o.Inside.Replication.MigrationConnection mgc = new Db4objects.Db4o.Inside.Replication.MigrationConnection
						(_peerA, _peerB);
					_peerA.i_handlers.i_migration = mgc;
					_peerA.i_handlers.i_replication = this;
					_peerA._replicationCallState = Db4objects.Db4o.YapConst.OLD;
					_peerB.i_handlers.i_migration = mgc;
					_peerB.i_handlers.i_replication = this;
					_peerB._replicationCallState = Db4objects.Db4o.YapConst.OLD;
					_conflictHandler = conflictHandler;
					_record = Db4objects.Db4o.ReplicationRecord.BeginReplication(_transA, _transB);
				}
			}
		}

		private int BindAndSet(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapStream
			 peer, Db4objects.Db4o.YapObject @ref, object sourceObject)
		{
			if (sourceObject is Db4objects.Db4o.IDb4oTypeImpl)
			{
				Db4objects.Db4o.IDb4oTypeImpl db4oType = (Db4objects.Db4o.IDb4oTypeImpl)sourceObject;
				if (!db4oType.CanBind())
				{
					Db4objects.Db4o.IDb4oTypeImpl targetObject = (Db4objects.Db4o.IDb4oTypeImpl)@ref.
						GetObject();
					targetObject.ReplicateFrom(sourceObject);
					return @ref.GetID();
				}
			}
			peer.Bind2(@ref, sourceObject);
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
					_peerA.Commit();
					_peerB.Commit();
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
			_peerA._replicationCallState = Db4objects.Db4o.YapConst.NONE;
			_peerA.i_handlers.i_migration = null;
			_peerA.i_handlers.i_replication = null;
			_peerA._replicationCallState = Db4objects.Db4o.YapConst.NONE;
			_peerB.i_handlers.i_migration = null;
			_peerB.i_handlers.i_replication = null;
		}

		private int IdInCaller(Db4objects.Db4o.YapStream caller, Db4objects.Db4o.YapObject
			 referenceA, Db4objects.Db4o.YapObject referenceB)
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

		public virtual Db4objects.Db4o.IObjectContainer PeerA()
		{
			return _peerA;
		}

		public virtual Db4objects.Db4o.IObjectContainer PeerB()
		{
			return _peerB;
		}

		public virtual void Replicate(object obj)
		{
			Db4objects.Db4o.YapStream stream = _peerB;
			if (_peerB.IsStored(obj))
			{
				if (!_peerA.IsStored(obj))
				{
					stream = _peerA;
				}
			}
			stream.Set(obj);
		}

		public virtual void Rollback()
		{
			_peerA.Rollback();
			_peerB.Rollback();
			EndReplication();
		}

		public virtual void SetDirection(Db4objects.Db4o.IObjectContainer replicateFrom, 
			Db4objects.Db4o.IObjectContainer replicateTo)
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

		private void ShareBinding(Db4objects.Db4o.YapObject sourceReference, Db4objects.Db4o.YapObject
			 referenceA, object objectA, Db4objects.Db4o.YapObject referenceB, object objectB
			)
		{
			if (sourceReference == null)
			{
				return;
			}
			if (objectA is Db4objects.Db4o.IDb4oTypeImpl)
			{
				if (!((Db4objects.Db4o.IDb4oTypeImpl)objectA).CanBind())
				{
					return;
				}
			}
			if (sourceReference == referenceA)
			{
				_peerB.Bind2(referenceB, objectA);
			}
			else
			{
				_peerA.Bind2(referenceA, objectB);
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
		internal virtual int TryToHandle(Db4objects.Db4o.YapStream caller, object obj)
		{
			int notProcessed = 0;
			Db4objects.Db4o.YapStream other = null;
			Db4objects.Db4o.YapObject sourceReference = null;
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
			lock (other.i_lock)
			{
				object objectA = obj;
				object objectB = obj;
				Db4objects.Db4o.YapObject referenceA = _peerA.GetYapObject(obj);
				Db4objects.Db4o.YapObject referenceB = _peerB.GetYapObject(obj);
				Db4objects.Db4o.VirtualAttributes attA = null;
				Db4objects.Db4o.VirtualAttributes attB = null;
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
					object[] arr = _transA.ObjectAndYapObjectBySignature(attB.i_uuid, attB.i_database
						.i_signature);
					if (arr[0] == null)
					{
						return notProcessed;
					}
					referenceA = (Db4objects.Db4o.YapObject)arr[1];
					objectA = arr[0];
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
						object[] arr = _transB.ObjectAndYapObjectBySignature(attA.i_uuid, attA.i_database
							.i_signature);
						if (arr[0] == null)
						{
							return notProcessed;
						}
						referenceB = (Db4objects.Db4o.YapObject)arr[1];
						objectB = arr[0];
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
				_peerA.Refresh(objectA, 1);
				_peerB.Refresh(objectB, 1);
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

		public virtual void WhereModified(Db4objects.Db4o.Query.IQuery query)
		{
			query.Descend(Db4objects.Db4o.Ext.VirtualField.VERSION).Constrain(LastSynchronization
				()).Greater();
		}
	}
}