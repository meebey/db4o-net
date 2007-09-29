/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
namespace Db4objects.Drs.Inside
{
	public sealed class GenericReplicationSession : Db4objects.Drs.IReplicationSession
	{
		private const int SIZE = 10000;

		private readonly Db4objects.Drs.Inside.ReplicationReflector _reflector;

		private readonly Db4objects.Drs.Inside.ICollectionHandler _collectionHandler;

		private Db4objects.Drs.Inside.IReplicationProviderInside _providerA;

		private Db4objects.Drs.Inside.IReplicationProviderInside _providerB;

		private Db4objects.Drs.IReplicationProvider _directionTo;

		private readonly Db4objects.Drs.IReplicationEventListener _listener;

		private readonly Db4objects.Drs.Inside.Traversal.ITraverser _traverser;

		private long _lastReplicationVersion;

		private Db4objects.Db4o.Foundation.Hashtable4 _processedUuids;

		private bool _isReplicatingOnlyDeletions;

		public GenericReplicationSession(Db4objects.Drs.Inside.IReplicationProviderInside
			 _peerA, Db4objects.Drs.Inside.IReplicationProviderInside _peerB) : this(_peerA, 
			_peerB, new Db4objects.Drs.Inside.DefaultReplicationEventListener())
		{
		}

		public GenericReplicationSession(Db4objects.Drs.IReplicationProvider providerA, Db4objects.Drs.IReplicationProvider
			 providerB, Db4objects.Drs.IReplicationEventListener listener)
		{
			_reflector = Db4objects.Drs.Inside.ReplicationReflector.GetInstance();
			_collectionHandler = new Db4objects.Drs.Inside.CollectionHandlerImpl(_reflector.Reflector
				());
			_traverser = new Db4objects.Drs.Inside.Traversal.GenericTraverser(_reflector.Reflector
				(), _collectionHandler);
			_providerA = (Db4objects.Drs.Inside.IReplicationProviderInside)providerA;
			_providerB = (Db4objects.Drs.Inside.IReplicationProviderInside)providerB;
			_listener = listener;
			lock (_providerA.GetMonitor())
			{
				lock (_providerB.GetMonitor())
				{
					_providerA.StartReplicationTransaction(_providerB.GetSignature());
					_providerB.StartReplicationTransaction(_providerA.GetSignature());
					if (_providerA.GetLastReplicationVersion() != _providerB.GetLastReplicationVersion
						())
					{
						throw new System.Exception("Version numbers must be the same");
					}
					_lastReplicationVersion = _providerA.GetLastReplicationVersion();
				}
			}
			ResetProcessedUuids();
		}

		public void CheckConflict(object root)
		{
			try
			{
				PrepareGraphToBeReplicated(root);
			}
			finally
			{
				_providerA.ClearAllReferences();
				_providerB.ClearAllReferences();
			}
		}

		public void Close()
		{
			_providerA.Destroy();
			_providerB.Destroy();
			_providerA = null;
			_providerB = null;
			_processedUuids = null;
		}

		public void Commit()
		{
			lock (_providerA.GetMonitor())
			{
				lock (_providerB.GetMonitor())
				{
					long maxVersion = _providerA.GetCurrentVersion() > _providerB.GetCurrentVersion()
						 ? _providerA.GetCurrentVersion() : _providerB.GetCurrentVersion();
					_providerA.SyncVersionWithPeer(maxVersion);
					_providerB.SyncVersionWithPeer(maxVersion);
					maxVersion++;
					_providerA.CommitReplicationTransaction(maxVersion);
					_providerB.CommitReplicationTransaction(maxVersion);
				}
			}
		}

		public Db4objects.Drs.IReplicationProvider ProviderA()
		{
			return _providerA;
		}

		public Db4objects.Drs.IReplicationProvider ProviderB()
		{
			return _providerB;
		}

		public void Replicate(object root)
		{
			try
			{
				PrepareGraphToBeReplicated(root);
				CopyStateAcross(_providerA);
				CopyStateAcross(_providerB);
				StoreChangedObjectsIn(_providerA);
				StoreChangedObjectsIn(_providerB);
			}
			finally
			{
				_providerA.ClearAllReferences();
				_providerB.ClearAllReferences();
			}
		}

		public void ReplicateDeletions(System.Type extent)
		{
			ReplicateDeletions(extent, _providerA);
			ReplicateDeletions(extent, _providerB);
		}

		private void ReplicateDeletions(System.Type extent, Db4objects.Drs.Inside.IReplicationProviderInside
			 provider)
		{
			_isReplicatingOnlyDeletions = true;
			try
			{
				System.Collections.IEnumerator instances = provider.GetStoredObjects(extent).GetEnumerator
					();
				while (instances.MoveNext())
				{
					Replicate(instances.Current);
				}
			}
			finally
			{
				_isReplicatingOnlyDeletions = false;
			}
		}

		public void Rollback()
		{
			_providerA.RollbackReplication();
			_providerB.RollbackReplication();
		}

		public void SetDirection(Db4objects.Drs.IReplicationProvider replicateFrom, Db4objects.Drs.IReplicationProvider
			 replicateTo)
		{
			if (replicateFrom == _providerA && replicateTo == _providerB)
			{
				_directionTo = _providerB;
			}
			if (replicateFrom == _providerB && replicateTo == _providerA)
			{
				_directionTo = _providerA;
			}
		}

		private void PrepareGraphToBeReplicated(object root)
		{
			_traverser.TraverseGraph(root, new Db4objects.Drs.Inside.InstanceReplicationPreparer
				(_providerA, _providerB, _directionTo, _listener, _isReplicatingOnlyDeletions, _lastReplicationVersion
				, _processedUuids, _traverser, _reflector, _collectionHandler));
		}

		private object ArrayClone(object original, Db4objects.Db4o.Reflect.IReflectClass 
			claxx, Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider)
		{
			Db4objects.Db4o.Reflect.IReflectClass componentType = _reflector.GetComponentType
				(claxx);
			int[] dimensions = _reflector.ArrayDimensions(original);
			object result = _reflector.NewArrayInstance(componentType, dimensions);
			object[] flatContents = _reflector.ArrayContents(original);
			if (!(claxx.IsSecondClass() || componentType.IsSecondClass()))
			{
				ReplaceWithCounterparts(flatContents, sourceProvider);
			}
			_reflector.ArrayShape(flatContents, 0, result, dimensions, 0);
			return result;
		}

		private void CopyFieldValuesAcross(object src, object dest, Db4objects.Db4o.Reflect.IReflectClass
			 claxx, Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider)
		{
			Db4objects.Db4o.Reflect.IReflectField[] fields;
			fields = claxx.GetDeclaredFields();
			for (int i = 0; i < fields.Length; i++)
			{
				Db4objects.Db4o.Reflect.IReflectField field = fields[i];
				if (field.IsStatic())
				{
					continue;
				}
				if (field.IsTransient())
				{
					continue;
				}
				field.SetAccessible();
				object value = field.Get(src);
				field.Set(dest, FindCounterpart(value, sourceProvider));
			}
			Db4objects.Db4o.Reflect.IReflectClass superclass = claxx.GetSuperclass();
			if (superclass == null)
			{
				return;
			}
			CopyFieldValuesAcross(src, dest, superclass, sourceProvider);
		}

		private void CopyStateAcross(Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider
			)
		{
			if (_directionTo == sourceProvider)
			{
				return;
			}
			sourceProvider.VisitCachedReferences(new _IVisitor4_212(this, sourceProvider));
		}

		private sealed class _IVisitor4_212 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _IVisitor4_212(GenericReplicationSession _enclosing, Db4objects.Drs.Inside.IReplicationProviderInside
				 sourceProvider)
			{
				this._enclosing = _enclosing;
				this.sourceProvider = sourceProvider;
			}

			public void Visit(object obj)
			{
				this._enclosing.CopyStateAcross((Db4objects.Drs.Inside.IReplicationReference)obj, 
					sourceProvider);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider;
		}

		private void CopyStateAcross(Db4objects.Drs.Inside.IReplicationReference sourceRef
			, Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider)
		{
			if (!sourceRef.IsMarkedForReplicating())
			{
				return;
			}
			CopyStateAcross(sourceRef.Object(), sourceRef.Counterpart(), sourceProvider);
		}

		private void CopyStateAcross(object source, object dest, Db4objects.Drs.Inside.IReplicationProviderInside
			 sourceProvider)
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx = _reflector.ForObject(source);
			CopyFieldValuesAcross(source, dest, claxx, sourceProvider);
		}

		private void DeleteInDestination(Db4objects.Drs.Inside.IReplicationReference reference
			, Db4objects.Drs.Inside.IReplicationProviderInside destination)
		{
			if (!reference.IsMarkedForDeleting())
			{
				return;
			}
			destination.ReplicateDeletion(reference.Uuid());
		}

		private object FindCounterpart(object value, Db4objects.Drs.Inside.IReplicationProviderInside
			 sourceProvider)
		{
			if (value == null)
			{
				return null;
			}
			if (Db4objects.Drs.Inside.ReplicationPlatform.IsValueType(value))
			{
				return value;
			}
			Db4objects.Db4o.Reflect.IReflectClass claxx = _reflector.ForObject(value);
			if (claxx.IsArray())
			{
				return ArrayClone(value, claxx, sourceProvider);
			}
			if (claxx.IsSecondClass())
			{
				return value;
			}
			if (_collectionHandler.CanHandle(value))
			{
				return CollectionClone(value, claxx, sourceProvider);
			}
			Db4objects.Drs.Inside.IReplicationReference @ref = sourceProvider.ProduceReference
				(value, null, null);
			if (@ref == null)
			{
				throw new System.ArgumentNullException("unable to find the ref of " + value + " of class "
					 + value.GetType());
			}
			object result = @ref.Counterpart();
			if (result == null)
			{
				throw new System.ArgumentNullException("unable to find the counterpart of " + value
					 + " of class " + value.GetType());
			}
			return result;
		}

		private object CollectionClone(object original, Db4objects.Db4o.Reflect.IReflectClass
			 claxx, Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider)
		{
			return _collectionHandler.CloneWithCounterparts(original, claxx, new _ICounterpartFinder_261
				(this, sourceProvider));
		}

		private sealed class _ICounterpartFinder_261 : Db4objects.Drs.Inside.ICounterpartFinder
		{
			public _ICounterpartFinder_261(GenericReplicationSession _enclosing, Db4objects.Drs.Inside.IReplicationProviderInside
				 sourceProvider)
			{
				this._enclosing = _enclosing;
				this.sourceProvider = sourceProvider;
			}

			public object FindCounterpart(object original)
			{
				return this._enclosing.FindCounterpart(original, sourceProvider);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly Db4objects.Drs.Inside.IReplicationProviderInside sourceProvider;
		}

		private Db4objects.Drs.Inside.IReplicationProviderInside Other(Db4objects.Drs.Inside.IReplicationProviderInside
			 peer)
		{
			return peer == _providerA ? _providerB : _providerA;
		}

		private void ReplaceWithCounterparts(object[] objects, Db4objects.Drs.Inside.IReplicationProviderInside
			 sourceProvider)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				object @object = objects[i];
				if (@object == null)
				{
					continue;
				}
				Db4objects.Drs.Inside.IReplicationReference replicationReference = sourceProvider
					.ProduceReference(@object, null, null);
				if (replicationReference == null)
				{
					throw new System.Exception(sourceProvider + " cannot find ref for " + @object);
				}
				objects[i] = replicationReference.Counterpart();
			}
		}

		private void ResetProcessedUuids()
		{
			_processedUuids = new Db4objects.Db4o.Foundation.Hashtable4(SIZE);
		}

		private void StoreChangedCounterpartInDestination(Db4objects.Drs.Inside.IReplicationReference
			 reference, Db4objects.Drs.Inside.IReplicationProviderInside destination)
		{
			bool markedForReplicating = reference.IsMarkedForReplicating();
			if (!markedForReplicating)
			{
				return;
			}
			destination.StoreReplica(reference.Counterpart());
		}

		private void StoreChangedObjectsIn(Db4objects.Drs.Inside.IReplicationProviderInside
			 destination)
		{
			Db4objects.Drs.Inside.IReplicationProviderInside source = Other(destination);
			if (_directionTo == source)
			{
				return;
			}
			destination.VisitCachedReferences(new _IVisitor4_303(this, destination));
			source.VisitCachedReferences(new _IVisitor4_309(this, destination));
		}

		private sealed class _IVisitor4_303 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _IVisitor4_303(GenericReplicationSession _enclosing, Db4objects.Drs.Inside.IReplicationProviderInside
				 destination)
			{
				this._enclosing = _enclosing;
				this.destination = destination;
			}

			public void Visit(object obj)
			{
				this._enclosing.DeleteInDestination((Db4objects.Drs.Inside.IReplicationReference)
					obj, destination);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly Db4objects.Drs.Inside.IReplicationProviderInside destination;
		}

		private sealed class _IVisitor4_309 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _IVisitor4_309(GenericReplicationSession _enclosing, Db4objects.Drs.Inside.IReplicationProviderInside
				 destination)
			{
				this._enclosing = _enclosing;
				this.destination = destination;
			}

			public void Visit(object obj)
			{
				this._enclosing.StoreChangedCounterpartInDestination((Db4objects.Drs.Inside.IReplicationReference
					)obj, destination);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly Db4objects.Drs.Inside.IReplicationProviderInside destination;
		}
	}
}
