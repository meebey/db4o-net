/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Replication;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs.Db4o;
using Db4objects.Drs.Inside;

namespace Db4objects.Drs.Db4o
{
	internal class FileReplicationProvider : IDb4oReplicationProvider
	{
		private IReadonlyReplicationProviderSignature _mySignature;

		protected readonly ExternalObjectContainer _stream;

		private readonly IReflector _reflector;

		private ReplicationRecord _replicationRecord;

		internal Db4oReplicationReferenceImpl _referencesByObject;

		private Db4oSignatureMap _signatureMap;

		private Tree _idsReplicatedInThisSession;

		private readonly string _name;

		public FileReplicationProvider(IObjectContainer objectContainer) : this(objectContainer
			, "null")
		{
		}

		public FileReplicationProvider(IObjectContainer objectContainer, string name)
		{
			// TODO: Add additional query methods (whereModified )
			IConfiguration cfg = objectContainer.Ext().Configure();
			cfg.ObjectClass(typeof(object)).CascadeOnDelete(false);
			cfg.Callbacks(false);
			_name = name;
			_stream = (ExternalObjectContainer)objectContainer;
			_reflector = _stream.Reflector();
			_signatureMap = new Db4oSignatureMap(_stream);
		}

		public virtual IReadonlyReplicationProviderSignature GetSignature()
		{
			if (_mySignature == null)
			{
				_mySignature = new Db4oReplicationProviderSignature(_stream.Identity());
			}
			return _mySignature;
		}

		public virtual object GetMonitor()
		{
			return _stream.Lock();
		}

		public virtual void StartReplicationTransaction(IReadonlyReplicationProviderSignature
			 peerSignature)
		{
			ClearAllReferences();
			lock (GetMonitor())
			{
				Transaction trans = _stream.Transaction();
				Db4oDatabase myIdentity = _stream.Identity();
				_signatureMap.Put(myIdentity);
				Db4oDatabase otherIdentity = _signatureMap.Produce(peerSignature.GetSignature(), 
					peerSignature.GetCreated());
				Db4oDatabase younger = null;
				Db4oDatabase older = null;
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
				_replicationRecord = ReplicationRecord.QueryForReplicationRecord(_stream, trans, 
					younger, older);
				if (_replicationRecord == null)
				{
					_replicationRecord = new ReplicationRecord(younger, older);
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
				TreeInt node = new TreeInt((int)_stream.GetID(obj));
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
			IReflectClass claxx = _reflector.ForObject(obj);
			int level = claxx.IsCollection() ? 3 : 1;
			_stream.Activate(obj, level);
		}

		public virtual IDb4oReplicationReference ReferenceFor(object obj)
		{
			if (_referencesByObject == null)
			{
				return null;
			}
			return _referencesByObject.Find(obj);
		}

		public virtual IReplicationReference ProduceReference(object obj, object unused, 
			string unused2)
		{
			if (obj == null)
			{
				return null;
			}
			if (_referencesByObject != null)
			{
				Db4oReplicationReferenceImpl existingNode = _referencesByObject.Find(obj);
				if (existingNode != null)
				{
					return existingNode;
				}
			}
			Refresh(obj);
			IObjectInfo objectInfo = _stream.GetObjectInfo(obj);
			if (objectInfo == null)
			{
				return null;
			}
			Db4oUUID uuid = objectInfo.GetUUID();
			if (uuid == null)
			{
				throw new ArgumentNullException();
			}
			Db4oReplicationReferenceImpl newNode = new Db4oReplicationReferenceImpl(objectInfo
				);
			AddReference(newNode);
			return newNode;
		}

		protected virtual void Refresh(object obj)
		{
		}

		//empty in File Provider
		private void AddReference(Db4oReplicationReferenceImpl newNode)
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

		public virtual IReplicationReference ReferenceNewObject(object obj, IReplicationReference
			 counterpartReference, IReplicationReference referencingObjCounterPartRef, string
			 fieldName)
		{
			Db4oUUID uuid = counterpartReference.Uuid();
			if (uuid == null)
			{
				return null;
			}
			byte[] signature = uuid.GetSignaturePart();
			long longPart = uuid.GetLongPart();
			long version = counterpartReference.Version();
			Db4oDatabase db = _signatureMap.Produce(signature, 0);
			Db4oReplicationReferenceImpl @ref = new Db4oReplicationReferenceImpl(obj, db, longPart
				, version);
			AddReference(@ref);
			return @ref;
		}

		public virtual IReplicationReference ProduceReferenceByUUID(Db4oUUID uuid, Type hint
			)
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

		public virtual void VisitCachedReferences(IVisitor4 visitor)
		{
			if (_referencesByObject != null)
			{
				_referencesByObject.Traverse(new _IVisitor4_265(visitor));
			}
		}

		private sealed class _IVisitor4_265 : IVisitor4
		{
			public _IVisitor4_265(IVisitor4 visitor)
			{
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				Db4oReplicationReferenceImpl node = (Db4oReplicationReferenceImpl)obj;
				visitor.Visit(node);
			}

			private readonly IVisitor4 visitor;
		}

		public virtual void ClearAllReferences()
		{
			_referencesByObject = null;
		}

		public virtual IObjectSet ObjectsChangedSinceLastReplication()
		{
			IQuery q = _stream.Query();
			WhereModified(q);
			return q.Execute();
		}

		public virtual IObjectSet ObjectsChangedSinceLastReplication(Type clazz)
		{
			IQuery q = _stream.Query();
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
		public virtual void WhereModified(IQuery query)
		{
			query.Descend(VirtualField.Version).Constrain(GetLastReplicationVersion()).Greater
				();
		}

		public virtual IObjectSet GetStoredObjects(Type type)
		{
			IQuery query = _stream.Query();
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

		public virtual void DeleteAllInstances(Type clazz)
		{
			IQuery q = _stream.Query();
			q.Constrain(clazz);
			IEnumerator objectSet = q.Execute().GetEnumerator();
			while (objectSet.MoveNext())
			{
				Delete(objectSet.Current);
			}
		}

		public virtual void Delete(object obj)
		{
			_stream.Delete(obj);
		}

		public virtual bool WasModifiedSinceLastReplication(IReplicationReference reference
			)
		{
			if (_idsReplicatedInThisSession != null)
			{
				int id = (int)_stream.GetID(reference.Object());
				if (_idsReplicatedInThisSession.Find(new TreeInt(id)) != null)
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

		public virtual void ReplicateDeletion(Db4oUUID uuid)
		{
			object obj = _stream.GetByUUID(uuid);
			if (obj == null)
			{
				return;
			}
			_stream.Delete(obj);
		}

		public virtual IExtObjectContainer GetObjectContainer()
		{
			return _stream;
		}
	}
}
