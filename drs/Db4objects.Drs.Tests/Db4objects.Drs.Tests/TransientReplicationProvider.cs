/* This file is part of the db4o object database http://www.db4o.com

Copyright (C) 2004 - 2009  Versant Corporation http://www.versant.com

db4o is free software; you can redistribute it and/or modify it under
the terms of version 3 of the GNU General Public License as published
by the Free Software Foundation.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program.  If not, see http://www.gnu.org/licenses/. */
using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Foundation;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Inside.Traversal;
using Db4objects.Drs.Tests;
using Sharpen;
using Sharpen.Util;

namespace Db4objects.Drs.Tests
{
	public class TransientReplicationProvider : ITestableReplicationProvider, ITestableReplicationProviderInside
	{
		private TimeStampIdGenerator _timeStampIdGenerator = new TimeStampIdGenerator();

		private readonly string _name;

		private ITraverser _traverser;

		private readonly IDictionary _storedObjects = new IdentityHashMap();

		private readonly IDictionary _activatedObjects = new IdentityHashMap();

		private readonly IDictionary _referencesByObject = new IdentityHashMap();

		private readonly TransientReplicationProvider.MySignature _signature;

		private IReadonlyReplicationProviderSignature _peerSignature;

		private long _lastReplicationVersion = 0;

		private Collection4 _uuidsDeletedSinceLastReplication = new Collection4();

		public TransientReplicationProvider(byte[] signature, string name)
		{
			_signature = new TransientReplicationProvider.MySignature(this, signature);
			_name = name;
		}

		public override string ToString()
		{
			return _name;
		}

		// --------------------- Interface ReplicationProvider ---------------------
		public virtual IObjectSet ObjectsChangedSinceLastReplication()
		{
			return ObjectsChangedSinceLastReplication(typeof(object));
		}

		public virtual IObjectSet ObjectsChangedSinceLastReplication(Type clazz)
		{
			Collection4 result = new Collection4();
			for (IEnumerator iterator = StoredObjectsCollection(clazz).GetEnumerator(); iterator
				.MoveNext(); )
			{
				object candidate = iterator.Current;
				if (WasChangedSinceLastReplication(candidate))
				{
					result.Add(candidate);
				}
			}
			return new ObjectSetCollection4Facade(result);
		}

		// --------------------- Interface ReplicationProviderInside ---------------------
		public virtual void Activate(object @object)
		{
			_activatedObjects[@object] = @object;
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

		public virtual string GetName()
		{
			return _name;
		}

		public virtual IReadonlyReplicationProviderSignature GetSignature()
		{
			return _signature;
		}

		public virtual IReplicationReference ProduceReference(object obj, object unused, 
			string unused2)
		{
			IReplicationReference cached = GetCachedReference(obj);
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

		public virtual IReplicationReference ProduceReferenceByUUID(IDrsUUID uuid, Type hintIgnored
			)
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

		public virtual IReplicationReference ReferenceNewObject(object obj, IReplicationReference
			 counterpartReference, IReplicationReference unused, string unused2)
		{
			//System.out.println("referenceNewObject: " + obj + "  UUID: " + counterpartReference.uuid());
			IDrsUUID uuid = counterpartReference.Uuid();
			long version = counterpartReference.Version();
			if (GetObject(uuid) != null)
			{
				throw new Exception("Object exists already.");
			}
			IReplicationReference result = CreateReferenceFor(obj);
			Store(obj, uuid, version);
			return result;
		}

		public virtual void RollbackReplication()
		{
			throw new NotSupportedException();
		}

		public virtual void StartReplicationTransaction(IReadonlyReplicationProviderSignature
			 peerSignature)
		{
			if (_peerSignature != null)
			{
				if (!_peerSignature.Equals(peerSignature))
				{
					throw new ArgumentException("This provider can only replicate with a single peer."
						);
				}
			}
			_peerSignature = peerSignature;
			_timeStampIdGenerator.SetMinimumNext(_lastReplicationVersion);
		}

		public virtual void StoreReplica(object obj)
		{
			IReplicationReference @ref = GetCachedReference(obj);
			if (@ref == null)
			{
				throw new Exception();
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

		public virtual void VisitCachedReferences(IVisitor4 visitor)
		{
			IEnumerator i = _referencesByObject.Values.GetEnumerator();
			while (i.MoveNext())
			{
				visitor.Visit(i.Current);
			}
		}

		public virtual bool WasModifiedSinceLastReplication(IReplicationReference reference
			)
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
			IDrsUUID uuid = ProduceReference(obj, null, null).Uuid();
			_uuidsDeletedSinceLastReplication.Add(uuid);
			Sharpen.Collections.Remove(_storedObjects, obj);
		}

		public virtual void DeleteAllInstances(Type clazz)
		{
			IEnumerator iterator = StoredObjectsCollection(clazz).GetEnumerator();
			while (iterator.MoveNext())
			{
				Delete(iterator.Current);
			}
		}

		public virtual IObjectSet GetStoredObjects(Type clazz)
		{
			return new ObjectSetCollection4Facade(StoredObjectsCollection(clazz));
		}

		private Collection4 StoredObjectsCollection(Type clazz)
		{
			Collection4 result = new Collection4();
			for (IEnumerator iterator = _storedObjects.Keys.GetEnumerator(); iterator.MoveNext
				(); )
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
			_traverser.TraverseGraph(o, new _IVisitor_217(this));
		}

		private sealed class _IVisitor_217 : IVisitor
		{
			public _IVisitor_217(TransientReplicationProvider _enclosing)
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

		public virtual IDictionary ActivatedObjects()
		{
			return _activatedObjects;
		}

		private IReplicationReference CreateReferenceFor(object obj)
		{
			TransientReplicationProvider.MyReplicationReference result = new TransientReplicationProvider.MyReplicationReference
				(this, obj);
			_referencesByObject[obj] = result;
			return result;
		}

		private IReplicationReference GetCachedReference(object obj)
		{
			return (IReplicationReference)_referencesByObject[obj];
		}

		private TransientReplicationProvider.ObjectInfo GetInfo(object candidate)
		{
			return (TransientReplicationProvider.ObjectInfo)_storedObjects[candidate];
		}

		public virtual object GetObject(IDrsUUID uuid)
		{
			IEnumerator iter = StoredObjectsCollection(typeof(object)).GetEnumerator();
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

		public virtual IObjectSet GetStoredObjects()
		{
			return GetStoredObjects(typeof(object));
		}

		private bool IsStored(object obj)
		{
			return GetInfo(obj) != null;
		}

		public virtual void ReplicateDeletion(IReplicationReference reference)
		{
			Sharpen.Collections.Remove(_storedObjects, reference.Object());
		}

		private void Store(object obj, IDrsUUID uuid, long version)
		{
			if (obj == null)
			{
				throw new Exception();
			}
			_storedObjects[obj] = new TransientReplicationProvider.ObjectInfo(uuid, version);
		}

		public virtual void TransientProviderSpecificStore(object obj)
		{
			//TODO ak: this implementation of vvv is copied from Hibernate, which works.
			// However, vvv should be supposed to be replaced by getCurrentVersion(), but that wouldn't work. Find out
			long vvv = new TimeStampIdGenerator(_lastReplicationVersion).Generate();
			TransientReplicationProvider.ObjectInfo info = GetInfo(obj);
			if (info == null)
			{
				Store(obj, new DrsUUIDImpl(new Db4oUUID(_timeStampIdGenerator.Generate(), _signature
					.GetSignature())), vvv);
			}
			else
			{
				info._version = vvv;
			}
		}

		public virtual IObjectSet UuidsDeletedSinceLastReplication()
		{
			return new ObjectSetCollection4Facade(_uuidsDeletedSinceLastReplication);
		}

		private bool WasChangedSinceLastReplication(object candidate)
		{
			return GetInfo(candidate)._version > _lastReplicationVersion;
		}

		public virtual bool WasDeletedSinceLastReplication(IDrsUUID uuid)
		{
			return _uuidsDeletedSinceLastReplication.Contains(uuid);
		}

		public class MySignature : IReadonlyReplicationProviderSignature
		{
			private readonly byte[] _bytes;

			private long creatimeTime;

			public MySignature(TransientReplicationProvider _enclosing, byte[] signature)
			{
				this._enclosing = _enclosing;
				this._bytes = signature;
				this.creatimeTime = Runtime.CurrentTimeMillis();
			}

			public virtual long GetId()
			{
				throw new Exception("Never used?");
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

		private class MyReplicationReference : IReplicationReference
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
					throw new ArgumentException();
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

			public virtual IDrsUUID Uuid()
			{
				return this._enclosing.GetInfo(this._object)._uuid;
			}

			public virtual void SetCounterpart(object obj)
			{
				this._counterpart = obj;
			}

			public virtual void MarkForReplicating(bool flag)
			{
				this._isMarkedForReplicating = flag;
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
			public readonly IDrsUUID _uuid;

			public long _version;

			public ObjectInfo(IDrsUUID uuid, long version)
			{
				_uuid = uuid;
				_version = version;
			}
		}

		public class MyTraverser : ITraverser
		{
			internal ITraverser _delegate;

			public MyTraverser(TransientReplicationProvider _enclosing, ReplicationReflector 
				reflector, Db4objects.Drs.Inside.ICollectionHandler collectionHandler)
			{
				this._enclosing = _enclosing;
				this._delegate = new GenericTraverser(reflector, collectionHandler);
			}

			public virtual void TraverseGraph(object @object, IVisitor visitor)
			{
				this._delegate.TraverseGraph(@object, visitor);
			}

			public virtual void ExtendTraversalTo(object disconnected)
			{
				this._delegate.ExtendTraversalTo(disconnected);
			}

			private readonly TransientReplicationProvider _enclosing;
		}

		public virtual void ReplicateDeletion(IDrsUUID uuid)
		{
			Sharpen.Collections.Remove(_storedObjects, GetObject(uuid));
		}

		public virtual bool IsProviderSpecific(object original)
		{
			return false;
		}

		public virtual void ReplicationReflector(ReplicationReflector replicationReflector
			)
		{
			Db4objects.Drs.Inside.ICollectionHandler _collectionHandler = new CollectionHandlerImpl
				(replicationReflector);
			_traverser = new TransientReplicationProvider.MyTraverser(this, replicationReflector
				, _collectionHandler);
		}

		public virtual IReplicationReference ProduceReference(object obj)
		{
			return ProduceReference(obj, null, null);
		}

		public virtual void RunIsolated(IBlock4 block)
		{
			lock (this)
			{
				block.Run();
			}
		}

		public virtual object ReplaceIfSpecific(object value)
		{
			return value;
		}

		public virtual bool IsSecondClassObject(object obj)
		{
			return false;
		}
	}
}
