/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Db4o
{
	internal class FileReplicationProvider : Db4objects.Drs.Db4o.IDb4oReplicationProvider
	{
		private Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature _mySignature;

		protected readonly Db4objects.Db4o.Internal.ExternalObjectContainer _stream;

		private readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		private Db4objects.Db4o.ReplicationRecord _replicationRecord;

		internal Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl _referencesByObject;

		private Db4objects.Drs.Db4o.Db4oSignatureMap _signatureMap;

		private Db4objects.Db4o.Foundation.Tree _idsReplicatedInThisSession;

		private readonly string _name;

		public FileReplicationProvider(Db4objects.Db4o.IObjectContainer objectContainer) : 
			this(objectContainer, "null")
		{
		}

		public FileReplicationProvider(Db4objects.Db4o.IObjectContainer objectContainer, 
			string name)
		{
			// TODO: Add additional query methods (whereModified )
			Db4objects.Db4o.Config.IConfiguration cfg = objectContainer.Ext().Configure();
			cfg.ObjectClass(typeof(object)).CascadeOnDelete(false);
			cfg.Callbacks(false);
			_name = name;
			_stream = (Db4objects.Db4o.Internal.ExternalObjectContainer)objectContainer;
			_reflector = _stream.Reflector();
			_signatureMap = new Db4objects.Drs.Db4o.Db4oSignatureMap(_stream);
		}

		public virtual Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature GetSignature
			()
		{
			if (_mySignature == null)
			{
				_mySignature = new Db4objects.Drs.Db4o.Db4oReplicationProviderSignature(_stream.Identity
					());
			}
			return _mySignature;
		}

		public virtual object GetMonitor()
		{
			return _stream.Lock();
		}

		public virtual void StartReplicationTransaction(Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature
			 peerSignature)
		{
			ClearAllReferences();
			lock (GetMonitor())
			{
				Db4objects.Db4o.Internal.Transaction trans = _stream.Transaction();
				Db4objects.Db4o.Ext.Db4oDatabase myIdentity = _stream.Identity();
				_signatureMap.Put(myIdentity);
				Db4objects.Db4o.Ext.Db4oDatabase otherIdentity = _signatureMap.Produce(peerSignature
					.GetSignature(), peerSignature.GetCreated());
				Db4objects.Db4o.Ext.Db4oDatabase younger = null;
				Db4objects.Db4o.Ext.Db4oDatabase older = null;
				if (myIdentity.IsOlderThan(otherIdentity))
				{
					younger = otherIdentity;
					older = myIdentity;
				}
				else
				{
					younger = myIdentity;
					older = otherIdentity;
				}
				_replicationRecord = Db4objects.Db4o.ReplicationRecord.QueryForReplicationRecord(
					_stream, trans, younger, older);
				if (_replicationRecord == null)
				{
					_replicationRecord = new Db4objects.Db4o.ReplicationRecord(younger, older);
					_replicationRecord.Store(_stream);
				}
				long localInitialVersion = _stream.Version();
			}
		}

		public virtual void SyncVersionWithPeer(long version)
		{
			long versionTest = GetCurrentVersion();
			_replicationRecord._version = version;
			_replicationRecord.Store(_stream);
		}

		public virtual void CommitReplicationTransaction(long raisedDatabaseVersion)
		{
			long versionTest = GetCurrentVersion();
			_stream.RaiseVersion(raisedDatabaseVersion);
			_stream.Commit();
			_idsReplicatedInThisSession = null;
		}

		public virtual void RollbackReplication()
		{
			_stream.Rollback();
			_referencesByObject = null;
			_idsReplicatedInThisSession = null;
		}

		public virtual long GetCurrentVersion()
		{
			return _stream.Version();
		}

		public virtual long GetLastReplicationVersion()
		{
			return _replicationRecord._version;
		}

		public virtual void StoreReplica(object obj)
		{
			lock (GetMonitor())
			{
				_stream.StoreByNewReplication(this, obj);
				// the ID is an int internally, it can be casted to int.
				Db4objects.Db4o.Internal.TreeInt node = new Db4objects.Db4o.Internal.TreeInt((int
					)_stream.GetID(obj));
				if (_idsReplicatedInThisSession == null)
				{
					_idsReplicatedInThisSession = node;
				}
				else
				{
					_idsReplicatedInThisSession = _idsReplicatedInThisSession.Add(node);
				}
			}
		}

		public virtual void Activate(object obj)
		{
			if (obj == null)
			{
				return;
			}
			Db4objects.Db4o.Reflect.IReflectClass claxx = _reflector.ForObject(obj);
			int level = claxx.IsCollection() ? 3 : 1;
			_stream.Activate(obj, level);
		}

		public virtual Db4objects.Db4o.Internal.Replication.IDb4oReplicationReference ReferenceFor
			(object obj)
		{
			if (_referencesByObject == null)
			{
				return null;
			}
			return _referencesByObject.Find(obj);
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ProduceReference(object
			 obj, object unused, string unused2)
		{
			if (obj == null)
			{
				return null;
			}
			if (_referencesByObject != null)
			{
				Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl existingNode = _referencesByObject
					.Find(obj);
				if (existingNode != null)
				{
					return existingNode;
				}
			}
			Refresh(obj);
			Db4objects.Db4o.Ext.IObjectInfo objectInfo = _stream.GetObjectInfo(obj);
			if (objectInfo == null)
			{
				return null;
			}
			Db4objects.Db4o.Ext.Db4oUUID uuid = objectInfo.GetUUID();
			if (uuid == null)
			{
				throw new System.ArgumentNullException();
			}
			Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl newNode = new Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl
				(objectInfo);
			AddReference(newNode);
			return newNode;
		}

		protected virtual void Refresh(object obj)
		{
		}

		//empty in File Provider
		private void AddReference(Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl newNode
			)
		{
			if (_referencesByObject == null)
			{
				_referencesByObject = newNode;
			}
			else
			{
				_referencesByObject = _referencesByObject.Add(newNode);
			}
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ReferenceNewObject(object
			 obj, Db4objects.Drs.Inside.IReplicationReference counterpartReference, Db4objects.Drs.Inside.IReplicationReference
			 referencingObjCounterPartRef, string fieldName)
		{
			Db4objects.Db4o.Ext.Db4oUUID uuid = counterpartReference.Uuid();
			if (uuid == null)
			{
				return null;
			}
			byte[] signature = uuid.GetSignaturePart();
			long longPart = uuid.GetLongPart();
			long version = counterpartReference.Version();
			Db4objects.Db4o.Ext.Db4oDatabase db = _signatureMap.Produce(signature, 0);
			Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl @ref = new Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl
				(obj, db, longPart, version);
			AddReference(@ref);
			return @ref;
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ProduceReferenceByUUID
			(Db4objects.Db4o.Ext.Db4oUUID uuid, System.Type hint)
		{
			if (uuid == null)
			{
				return null;
			}
			object obj = _stream.GetByUUID(uuid);
			if (obj == null)
			{
				return null;
			}
			if (!_stream.IsActive(obj))
			{
				_stream.Activate(obj, 1);
			}
			return ProduceReference(obj, null, null);
		}

		public virtual void VisitCachedReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			if (_referencesByObject != null)
			{
				_referencesByObject.Traverse(new _IVisitor4_265(visitor));
			}
		}

		private sealed class _IVisitor4_265 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _IVisitor4_265(Db4objects.Db4o.Foundation.IVisitor4 visitor)
			{
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl node = (Db4objects.Drs.Db4o.Db4oReplicationReferenceImpl
					)obj;
				visitor.Visit(node);
			}

			private readonly Db4objects.Db4o.Foundation.IVisitor4 visitor;
		}

		public virtual void ClearAllReferences()
		{
			_referencesByObject = null;
		}

		public virtual Db4objects.Db4o.IObjectSet ObjectsChangedSinceLastReplication()
		{
			Db4objects.Db4o.Query.IQuery q = _stream.Query();
			WhereModified(q);
			return q.Execute();
		}

		public virtual Db4objects.Db4o.IObjectSet ObjectsChangedSinceLastReplication(System.Type
			 clazz)
		{
			Db4objects.Db4o.Query.IQuery q = _stream.Query();
			q.Constrain(clazz);
			WhereModified(q);
			return q.Execute();
		}

		/// <summary>
		/// adds a constraint to the passed Query to query only for objects that were
		/// modified since the last replication process between this and the other
		/// ObjectContainer involved in the current replication process.
		/// </summary>
		/// <remarks>
		/// adds a constraint to the passed Query to query only for objects that were
		/// modified since the last replication process between this and the other
		/// ObjectContainer involved in the current replication process.
		/// </remarks>
		/// <param name="query">the Query to be constrained</param>
		public virtual void WhereModified(Db4objects.Db4o.Query.IQuery query)
		{
			query.Descend(Db4objects.Db4o.Ext.VirtualField.Version).Constrain(GetLastReplicationVersion
				()).Greater();
		}

		public virtual Db4objects.Db4o.IObjectSet GetStoredObjects(System.Type type)
		{
			Db4objects.Db4o.Query.IQuery query = _stream.Query();
			query.Constrain(type);
			return query.Execute();
		}

		public virtual void StoreNew(object o)
		{
			_stream.Store(o);
		}

		public virtual void Update(object o)
		{
			_stream.Store(o);
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual void Destroy()
		{
		}

		// do nothing
		public virtual void Commit()
		{
			_stream.Commit();
		}

		public virtual void DeleteAllInstances(System.Type clazz)
		{
			Db4objects.Db4o.Query.IQuery q = _stream.Query();
			q.Constrain(clazz);
			System.Collections.IEnumerator objectSet = q.Execute().GetEnumerator();
			while (objectSet.MoveNext())
			{
				Delete(objectSet.Current);
			}
		}

		public virtual void Delete(object obj)
		{
			_stream.Delete(obj);
		}

		public virtual bool WasModifiedSinceLastReplication(Db4objects.Drs.Inside.IReplicationReference
			 reference)
		{
			if (_idsReplicatedInThisSession != null)
			{
				int id = (int)_stream.GetID(reference.Object());
				if (_idsReplicatedInThisSession.Find(new Db4objects.Db4o.Internal.TreeInt(id)) !=
					 null)
				{
					return false;
				}
			}
			return reference.Version() > GetLastReplicationVersion();
		}

		public virtual bool SupportsMultiDimensionalArrays()
		{
			return true;
		}

		public virtual bool SupportsHybridCollection()
		{
			return true;
		}

		public virtual bool SupportsRollback()
		{
			return false;
		}

		public virtual bool SupportsCascadeDelete()
		{
			return true;
		}

		public override string ToString()
		{
			return GetName();
		}

		public virtual void ReplicateDeletion(Db4objects.Db4o.Ext.Db4oUUID uuid)
		{
			object obj = _stream.GetByUUID(uuid);
			if (obj == null)
			{
				return;
			}
			_stream.Delete(obj);
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer GetObjectContainer()
		{
			return _stream;
		}
	}
}
