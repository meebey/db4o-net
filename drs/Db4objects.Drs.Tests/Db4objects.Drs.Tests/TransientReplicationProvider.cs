/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class TransientReplicationProvider : Db4objects.Drs.Inside.ITestableReplicationProvider
		, Db4objects.Drs.Inside.ITestableReplicationProviderInside
	{
		private Db4objects.Db4o.Foundation.TimeStampIdGenerator _timeStampIdGenerator = new 
			Db4objects.Db4o.Foundation.TimeStampIdGenerator();

		private readonly string _name;

		private readonly Db4objects.Drs.Inside.Traversal.ITraverser _traverser;

		private readonly System.Collections.IDictionary _storedObjects = new Sharpen.Util.IdentityHashMap
			();

		private readonly System.Collections.IDictionary _activatedObjects = new Sharpen.Util.IdentityHashMap
			();

		private readonly System.Collections.IDictionary _referencesByObject = new Sharpen.Util.IdentityHashMap
			();

		private readonly Db4objects.Drs.Tests.TransientReplicationProvider.MySignature _signature;

		private Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature _peerSignature;

		private long _lastReplicationVersion = 0;

		private Db4objects.Db4o.Foundation.Collection4 _uuidsDeletedSinceLastReplication = 
			new Db4objects.Db4o.Foundation.Collection4();

		public TransientReplicationProvider(byte[] signature) : this(signature, null)
		{
		}

		public TransientReplicationProvider(byte[] signature, string name)
		{
			_signature = new Db4objects.Drs.Tests.TransientReplicationProvider.MySignature(this
				, signature);
			_name = name;
			Db4objects.Drs.Inside.ReplicationReflector reflector = Db4objects.Drs.Inside.ReplicationReflector
				.GetInstance();
			Db4objects.Drs.Inside.ICollectionHandler _collectionHandler = new Db4objects.Drs.Inside.CollectionHandlerImpl
				(reflector.Reflector());
			_traverser = new Db4objects.Drs.Tests.TransientReplicationProvider.MyTraverser(this
				, reflector.Reflector(), _collectionHandler);
		}

		public override string ToString()
		{
			return _name;
		}

		// --------------------- Interface ReplicationProvider ---------------------
		public virtual Db4objects.Db4o.IObjectSet ObjectsChangedSinceLastReplication()
		{
			return ObjectsChangedSinceLastReplication(typeof(object));
		}

		public virtual Db4objects.Db4o.IObjectSet ObjectsChangedSinceLastReplication(System.Type
			 clazz)
		{
			Db4objects.Db4o.Foundation.Collection4 result = new Db4objects.Db4o.Foundation.Collection4
				();
			for (System.Collections.IEnumerator iterator = StoredObjectsCollection(clazz).GetEnumerator
				(); iterator.MoveNext(); )
			{
				object candidate = iterator.Current;
				if (WasChangedSinceLastReplication(candidate))
				{
					result.Add(candidate);
				}
			}
			return new Db4objects.Drs.Foundation.ObjectSetCollection4Facade(result);
		}

		// --------------------- Interface ReplicationProviderInside ---------------------
		public virtual void Activate(object @object)
		{
			_activatedObjects.Add(@object, @object);
		}

		public virtual void ClearAllReferences()
		{
			_referencesByObject.Clear();
		}

		public virtual void CommitReplicationTransaction(long raisedDatabaseVersion)
		{
			_uuidsDeletedSinceLastReplication.Clear();
			_timeStampIdGenerator.SetMinimumNext(raisedDatabaseVersion);
		}

		public virtual void Destroy()
		{
		}

		// do nothing
		public virtual long GetCurrentVersion()
		{
			return _timeStampIdGenerator.Generate();
		}

		public virtual long GetLastReplicationVersion()
		{
			return _lastReplicationVersion;
		}

		public virtual object GetMonitor()
		{
			return this;
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature GetSignature
			()
		{
			return _signature;
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ProduceReference(object
			 obj, object unused, string unused2)
		{
			Db4objects.Drs.Inside.IReplicationReference cached = GetCachedReference(obj);
			if (cached != null)
			{
				return cached;
			}
			if (!IsStored(obj))
			{
				return null;
			}
			return CreateReferenceFor(obj);
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ProduceReferenceByUUID
			(Db4objects.Db4o.Ext.Db4oUUID uuid, System.Type hintIgnored)
		{
			if (uuid == null)
			{
				return null;
			}
			object @object = GetObject(uuid);
			if (@object == null)
			{
				return null;
			}
			return ProduceReference(@object, null, null);
		}

		public virtual Db4objects.Drs.Inside.IReplicationReference ReferenceNewObject(object
			 obj, Db4objects.Drs.Inside.IReplicationReference counterpartReference, Db4objects.Drs.Inside.IReplicationReference
			 unused, string unused2)
		{
			//System.out.println("referenceNewObject: " + obj + "  UUID: " + counterpartReference.uuid());
			Db4objects.Db4o.Ext.Db4oUUID uuid = counterpartReference.Uuid();
			long version = counterpartReference.Version();
			if (GetObject(uuid) != null)
			{
				throw new System.Exception("Object exists already.");
			}
			Db4objects.Drs.Inside.IReplicationReference result = CreateReferenceFor(obj);
			Store(obj, uuid, version);
			return result;
		}

		public virtual void RollbackReplication()
		{
			throw new System.NotSupportedException();
		}

		public virtual void StartReplicationTransaction(Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature
			 peerSignature)
		{
			if (_peerSignature != null)
			{
				if (!_peerSignature.Equals(peerSignature))
				{
					throw new System.ArgumentException("This provider can only replicate with a single peer."
						);
				}
			}
			_peerSignature = peerSignature;
			_timeStampIdGenerator.SetMinimumNext(_lastReplicationVersion);
		}

		public virtual void StoreReplica(object obj)
		{
			Db4objects.Drs.Inside.IReplicationReference @ref = GetCachedReference(obj);
			if (@ref == null)
			{
				throw new System.Exception();
			}
			Store(obj, @ref.Uuid(), @ref.Version());
		}

		public virtual void SyncVersionWithPeer(long version)
		{
			_lastReplicationVersion = version;
		}

		public virtual void UpdateCounterpart(object obj)
		{
			StoreReplica(obj);
		}

		public virtual void VisitCachedReferences(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
		{
			System.Collections.IEnumerator i = _referencesByObject.Values.GetEnumerator();
			while (i.MoveNext())
			{
				visitor.Visit(i.Current);
			}
		}

		public virtual bool WasModifiedSinceLastReplication(Db4objects.Drs.Inside.IReplicationReference
			 reference)
		{
			return reference.Version() > _lastReplicationVersion;
		}

		// --------------------- Interface SimpleObjectContainer ---------------------
		public virtual void Commit()
		{
		}

		// do nothing
		public virtual void Delete(object obj)
		{
			Db4objects.Db4o.Ext.Db4oUUID uuid = ProduceReference(obj, null, null).Uuid();
			_uuidsDeletedSinceLastReplication.Add(uuid);
			_storedObjects.Remove(obj);
		}

		public virtual void DeleteAllInstances(System.Type clazz)
		{
			System.Collections.IEnumerator iterator = StoredObjectsCollection(clazz).GetEnumerator
				();
			while (iterator.MoveNext())
			{
				Delete(iterator.Current);
			}
		}

		public virtual Db4objects.Db4o.IObjectSet GetStoredObjects(System.Type clazz)
		{
			return new Db4objects.Drs.Foundation.ObjectSetCollection4Facade(StoredObjectsCollection
				(clazz));
		}

		private Db4objects.Db4o.Foundation.Collection4 StoredObjectsCollection(System.Type
			 clazz)
		{
			Db4objects.Db4o.Foundation.Collection4 result = new Db4objects.Db4o.Foundation.Collection4
				();
			for (System.Collections.IEnumerator iterator = _storedObjects.Keys.GetEnumerator(
				); iterator.MoveNext(); )
			{
				object candidate = iterator.Current;
				if (clazz.IsAssignableFrom(candidate.GetType()))
				{
					result.Add(candidate);
				}
			}
			return result;
		}

		public virtual void StoreNew(object o)
		{
			_traverser.TraverseGraph(o, new _IVisitor_210(this));
		}

		private sealed class _IVisitor_210 : Db4objects.Drs.Inside.Traversal.IVisitor
		{
			public _IVisitor_210(TransientReplicationProvider _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Visit(object obj)
			{
				if (this._enclosing.IsStored(obj))
				{
					return false;
				}
				this._enclosing.TransientProviderSpecificStore(obj);
				return true;
			}

			private readonly TransientReplicationProvider _enclosing;
		}

		public virtual void Update(object o)
		{
			TransientProviderSpecificStore(o);
		}

		// --------------------- Interface TestableReplicationProviderInside ---------------------
		public virtual bool SupportsCascadeDelete()
		{
			return false;
		}

		public virtual bool SupportsHybridCollection()
		{
			return true;
		}

		public virtual bool SupportsMultiDimensionalArrays()
		{
			return true;
		}

		public virtual bool SupportsRollback()
		{
			return false;
		}

		public virtual System.Collections.IDictionary ActivatedObjects()
		{
			return _activatedObjects;
		}

		private Db4objects.Drs.Inside.IReplicationReference CreateReferenceFor(object obj
			)
		{
			Db4objects.Drs.Tests.TransientReplicationProvider.MyReplicationReference result = 
				new Db4objects.Drs.Tests.TransientReplicationProvider.MyReplicationReference(this
				, obj);
			_referencesByObject.Add(obj, result);
			return result;
		}

		private Db4objects.Drs.Inside.IReplicationReference GetCachedReference(object obj
			)
		{
			return (Db4objects.Drs.Inside.IReplicationReference)_referencesByObject[obj];
		}

		private Db4objects.Drs.Tests.TransientReplicationProvider.ObjectInfo GetInfo(object
			 candidate)
		{
			return (Db4objects.Drs.Tests.TransientReplicationProvider.ObjectInfo)_storedObjects
				[candidate];
		}

		public virtual object GetObject(Db4objects.Db4o.Ext.Db4oUUID uuid)
		{
			System.Collections.IEnumerator iter = StoredObjectsCollection(typeof(object)).GetEnumerator
				();
			while (iter.MoveNext())
			{
				object candidate = iter.Current;
				if (GetInfo(candidate)._uuid.Equals(uuid))
				{
					return candidate;
				}
			}
			return null;
		}

		public virtual Db4objects.Db4o.IObjectSet GetStoredObjects()
		{
			return GetStoredObjects(typeof(object));
		}

		private bool IsStored(object obj)
		{
			return GetInfo(obj) != null;
		}

		public virtual void ReplicateDeletion(Db4objects.Drs.Inside.IReplicationReference
			 reference)
		{
			_storedObjects.Remove(reference.Object());
		}

		private void Store(object obj, Db4objects.Db4o.Ext.Db4oUUID uuid, long version)
		{
			if (obj == null)
			{
				throw new System.Exception();
			}
			_storedObjects.Add(obj, new Db4objects.Drs.Tests.TransientReplicationProvider.ObjectInfo
				(uuid, version));
		}

		public virtual void TransientProviderSpecificStore(object obj)
		{
			//TODO ak: this implementation of vvv is copied from Hibernate, which works.
			// However, vvv should be supposed to be replaced by getCurrentVersion(), but that wouldn't work. Find out
			long vvv = new Db4objects.Db4o.Foundation.TimeStampIdGenerator(_lastReplicationVersion
				).Generate();
			Db4objects.Drs.Tests.TransientReplicationProvider.ObjectInfo info = GetInfo(obj);
			if (info == null)
			{
				Store(obj, new Db4objects.Db4o.Ext.Db4oUUID(_timeStampIdGenerator.Generate(), _signature
					.GetSignature()), vvv);
			}
			else
			{
				info._version = vvv;
			}
		}

		public virtual Db4objects.Db4o.IObjectSet UuidsDeletedSinceLastReplication()
		{
			return new Db4objects.Drs.Foundation.ObjectSetCollection4Facade(_uuidsDeletedSinceLastReplication
				);
		}

		private bool WasChangedSinceLastReplication(object candidate)
		{
			return GetInfo(candidate)._version > _lastReplicationVersion;
		}

		public virtual bool WasDeletedSinceLastReplication(Db4objects.Db4o.Ext.Db4oUUID uuid
			)
		{
			return _uuidsDeletedSinceLastReplication.Contains(uuid);
		}

		public class MySignature : Db4objects.Drs.Inside.IReadonlyReplicationProviderSignature
		{
			private readonly byte[] _bytes;

			private long creatimeTime;

			public MySignature(TransientReplicationProvider _enclosing, byte[] signature)
			{
				this._enclosing = _enclosing;
				this._bytes = signature;
				this.creatimeTime = Sharpen.Runtime.CurrentTimeMillis();
			}

			public virtual long GetId()
			{
				throw new System.Exception("Never used?");
			}

			public virtual byte[] GetSignature()
			{
				return this._bytes;
			}

			public virtual long GetCreated()
			{
				return this.creatimeTime;
			}

			private readonly TransientReplicationProvider _enclosing;
		}

		private class MyReplicationReference : Db4objects.Drs.Inside.IReplicationReference
		{
			private readonly object _object;

			private object _counterpart;

			private bool _isMarkedForReplicating;

			private bool _isMarkedForDeleting;

			internal MyReplicationReference(TransientReplicationProvider _enclosing, object @object
				)
			{
				this._enclosing = _enclosing;
				if (@object == null)
				{
					throw new System.ArgumentException();
				}
				this._object = @object;
			}

			public virtual object Object()
			{
				return this._object;
			}

			public virtual object Counterpart()
			{
				return this._counterpart;
			}

			public virtual long Version()
			{
				return this._enclosing.GetInfo(this._object)._version;
			}

			public virtual Db4objects.Db4o.Ext.Db4oUUID Uuid()
			{
				return this._enclosing.GetInfo(this._object)._uuid;
			}

			public virtual void SetCounterpart(object obj)
			{
				this._counterpart = obj;
			}

			public virtual void MarkForReplicating()
			{
				this._isMarkedForReplicating = true;
			}

			public virtual bool IsMarkedForReplicating()
			{
				return this._isMarkedForReplicating;
			}

			public virtual void MarkForDeleting()
			{
				this._isMarkedForDeleting = true;
			}

			public virtual bool IsMarkedForDeleting()
			{
				return this._isMarkedForDeleting;
			}

			internal bool objectIsNew;

			public virtual void MarkCounterpartAsNew()
			{
				this.objectIsNew = true;
			}

			public virtual bool IsCounterpartNew()
			{
				return this.objectIsNew;
			}

			private readonly TransientReplicationProvider _enclosing;
		}

		private class ObjectInfo
		{
			public readonly Db4objects.Db4o.Ext.Db4oUUID _uuid;

			public long _version;

			public ObjectInfo(Db4objects.Db4o.Ext.Db4oUUID uuid, long version)
			{
				_uuid = uuid;
				_version = version;
			}
		}

		public class MyTraverser : Db4objects.Drs.Inside.Traversal.ITraverser
		{
			internal Db4objects.Drs.Inside.Traversal.ITraverser _delegate;

			public MyTraverser(TransientReplicationProvider _enclosing, Db4objects.Db4o.Reflect.IReflector
				 reflector, Db4objects.Drs.Inside.ICollectionHandler collectionHandler)
			{
				this._enclosing = _enclosing;
				this._delegate = new Db4objects.Drs.Inside.Traversal.GenericTraverser(reflector, 
					collectionHandler);
			}

			public virtual void TraverseGraph(object @object, Db4objects.Drs.Inside.Traversal.IVisitor
				 visitor)
			{
				this._delegate.TraverseGraph(@object, visitor);
			}

			public virtual void ExtendTraversalTo(object disconnected)
			{
				this._delegate.ExtendTraversalTo(disconnected);
			}

			private readonly TransientReplicationProvider _enclosing;
		}

		public virtual void ReplicateDeletion(Db4objects.Db4o.Ext.Db4oUUID uuid)
		{
			_storedObjects.Remove(GetObject(uuid));
		}
	}
}
