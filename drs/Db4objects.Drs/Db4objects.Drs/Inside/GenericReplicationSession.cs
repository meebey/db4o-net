/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Inside.Traversal;

namespace Db4objects.Drs.Inside
{
	public sealed class GenericReplicationSession : IReplicationSession
	{
		private const int Size = 10000;

		private readonly ReplicationReflector _reflector;

		private readonly Db4objects.Drs.Inside.ICollectionHandler _collectionHandler;

		private IReplicationProviderInside _providerA;

		private IReplicationProviderInside _providerB;

		private IReplicationProvider _directionTo;

		private readonly IReplicationEventListener _listener;

		private readonly ITraverser _traverser;

		private long _lastReplicationVersion;

		private Hashtable4 _processedUuids;

		private bool _isReplicatingOnlyDeletions;

		public GenericReplicationSession(IReplicationProviderInside _peerA, IReplicationProviderInside
			 _peerB) : this(_peerA, _peerB, new DefaultReplicationEventListener())
		{
		}

		public GenericReplicationSession(IReplicationProvider providerA, IReplicationProvider
			 providerB, IReplicationEventListener listener)
		{
			//null means bidirectional replication.
			_reflector = new ReplicationReflector(providerA, providerB);
			_collectionHandler = new CollectionHandlerImpl(_reflector);
			_traverser = new GenericTraverser(_reflector, _collectionHandler);
			_providerA = (IReplicationProviderInside)providerA;
			_providerB = (IReplicationProviderInside)providerB;
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
						throw new Exception("Version numbers must be the same");
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

		public IReplicationProvider ProviderA()
		{
			return _providerA;
		}

		public IReplicationProvider ProviderB()
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

		public void ReplicateDeletions(Type extent)
		{
			ReplicateDeletions(extent, _providerA);
			ReplicateDeletions(extent, _providerB);
		}

		private void ReplicateDeletions(Type extent, IReplicationProviderInside provider)
		{
			_isReplicatingOnlyDeletions = true;
			try
			{
				IEnumerator instances = provider.GetStoredObjects(extent).GetEnumerator();
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

		public void SetDirection(IReplicationProvider replicateFrom, IReplicationProvider
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
			_traverser.TraverseGraph(root, new InstanceReplicationPreparer(_providerA, _providerB
				, _directionTo, _listener, _isReplicatingOnlyDeletions, _lastReplicationVersion, 
				_processedUuids, _traverser, _reflector, _collectionHandler));
		}

		private object ArrayClone(object original, IReflectClass claxx, IReplicationProviderInside
			 sourceProvider)
		{
			IReflectClass componentType = _reflector.GetComponentType(claxx);
			int[] dimensions = _reflector.ArrayDimensions(original);
			object result = _reflector.NewArrayInstance(componentType, dimensions);
			object[] flatContents = _reflector.ArrayContents(original);
			//TODO Optimize: Copy the structure without flattening. Do this in ReflectArray.
			if (!(_reflector.IsSecondClass(claxx) || _reflector.IsSecondClass(componentType))
				)
			{
				ReplaceWithCounterparts(flatContents, sourceProvider);
			}
			_reflector.ArrayShape(flatContents, 0, result, dimensions, 0);
			return result;
		}

		private void CopyFieldValuesAcross(object src, object dest, IReflectClass claxx, 
			IReplicationProviderInside sourceProvider)
		{
			IEnumerator fields = FieldIterators.PersistentFields(claxx);
			while (fields.MoveNext())
			{
				IReflectField field = (IReflectField)fields.Current;
				object value = field.Get(src);
				field.Set(dest, FindCounterpart(value, sourceProvider));
			}
			IReflectClass superclass = claxx.GetSuperclass();
			if (superclass == null)
			{
				return;
			}
			CopyFieldValuesAcross(src, dest, superclass, sourceProvider);
		}

		private void CopyStateAcross(IReplicationProviderInside sourceProvider)
		{
			if (_directionTo == sourceProvider)
			{
				return;
			}
			sourceProvider.VisitCachedReferences(new _IVisitor4_202(this, sourceProvider));
		}

		private sealed class _IVisitor4_202 : IVisitor4
		{
			public _IVisitor4_202(GenericReplicationSession _enclosing, IReplicationProviderInside
				 sourceProvider)
			{
				this._enclosing = _enclosing;
				this.sourceProvider = sourceProvider;
			}

			public void Visit(object obj)
			{
				this._enclosing.CopyStateAcross((IReplicationReference)obj, sourceProvider);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IReplicationProviderInside sourceProvider;
		}

		private void CopyStateAcross(IReplicationReference sourceRef, IReplicationProviderInside
			 sourceProvider)
		{
			if (!sourceRef.IsMarkedForReplicating())
			{
				return;
			}
			CopyStateAcross(sourceRef.Object(), sourceRef.Counterpart(), sourceProvider);
		}

		private void CopyStateAcross(object source, object dest, IReplicationProviderInside
			 sourceProvider)
		{
			IReflectClass claxx = _reflector.ForObject(source);
			CopyFieldValuesAcross(source, dest, claxx, sourceProvider);
		}

		private void DeleteInDestination(IReplicationReference reference, IReplicationProviderInside
			 destination)
		{
			if (!reference.IsMarkedForDeleting())
			{
				return;
			}
			destination.ReplicateDeletion(reference.Uuid());
		}

		private object FindCounterpart(object value, IReplicationProviderInside sourceProvider
			)
		{
			if (value == null)
			{
				return null;
			}
			// TODO: need to clone and findCounterpart of each reference object in the
			// struct
			if (ReplicationPlatform.IsValueType(value))
			{
				return value;
			}
			IReflectClass claxx = _reflector.ForObject(value);
			if (claxx.IsArray())
			{
				return ArrayClone(value, claxx, sourceProvider);
			}
			if (Platform4.IsTransient(claxx))
			{
				return null;
			}
			// TODO: make it a warning
			if (_reflector.IsSecondClass(claxx))
			{
				return value;
			}
			if (_collectionHandler.CanHandle(value))
			{
				return CollectionClone(value, claxx, sourceProvider);
			}
			//if value is a Collection, result should be found by passing in just the value
			IReplicationReference @ref = sourceProvider.ProduceReference(value, null, null);
			if (@ref == null)
			{
				throw new ArgumentNullException("unable to find the ref of " + value + " of class "
					 + value.GetType());
			}
			object result = @ref.Counterpart();
			if (result == null)
			{
				throw new ArgumentNullException("unable to find the counterpart of " + value + " of class "
					 + value.GetType());
			}
			return result;
		}

		private object CollectionClone(object original, IReflectClass claxx, IReplicationProviderInside
			 sourceProvider)
		{
			return _collectionHandler.CloneWithCounterparts(sourceProvider, original, claxx, 
				new _ICounterpartFinder_253(this, sourceProvider));
		}

		private sealed class _ICounterpartFinder_253 : ICounterpartFinder
		{
			public _ICounterpartFinder_253(GenericReplicationSession _enclosing, IReplicationProviderInside
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

			private readonly IReplicationProviderInside sourceProvider;
		}

		private IReplicationProviderInside Other(IReplicationProviderInside peer)
		{
			return peer == _providerA ? _providerB : _providerA;
		}

		private void ReplaceWithCounterparts(object[] objects, IReplicationProviderInside
			 sourceProvider)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				object @object = objects[i];
				if (@object == null)
				{
					continue;
				}
				objects[i] = FindCounterpart(@object, sourceProvider);
			}
		}

		private void ResetProcessedUuids()
		{
			_processedUuids = new Hashtable4(Size);
		}

		private void StoreChangedCounterpartInDestination(IReplicationReference reference
			, IReplicationProviderInside destination)
		{
			//System.out.println("reference = " + reference);
			bool markedForReplicating = reference.IsMarkedForReplicating();
			//System.out.println("markedForReplicating = " + markedForReplicating);
			if (!markedForReplicating)
			{
				return;
			}
			destination.StoreReplica(reference.Counterpart());
		}

		private void StoreChangedObjectsIn(IReplicationProviderInside destination)
		{
			IReplicationProviderInside source = Other(destination);
			if (_directionTo == source)
			{
				return;
			}
			destination.VisitCachedReferences(new _IVisitor4_289(this, destination));
			source.VisitCachedReferences(new _IVisitor4_295(this, destination));
		}

		private sealed class _IVisitor4_289 : IVisitor4
		{
			public _IVisitor4_289(GenericReplicationSession _enclosing, IReplicationProviderInside
				 destination)
			{
				this._enclosing = _enclosing;
				this.destination = destination;
			}

			public void Visit(object obj)
			{
				this._enclosing.DeleteInDestination((IReplicationReference)obj, destination);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IReplicationProviderInside destination;
		}

		private sealed class _IVisitor4_295 : IVisitor4
		{
			public _IVisitor4_295(GenericReplicationSession _enclosing, IReplicationProviderInside
				 destination)
			{
				this._enclosing = _enclosing;
				this.destination = destination;
			}

			public void Visit(object obj)
			{
				this._enclosing.StoreChangedCounterpartInDestination((IReplicationReference)obj, 
					destination);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IReplicationProviderInside destination;
		}
	}
}
