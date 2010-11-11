/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

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

		private HashSet4 _processedUuids = new HashSet4(Size);

		private bool _isReplicatingOnlyDeletions;

		public GenericReplicationSession(IReplicationProviderInside _peerA, IReplicationProviderInside
			 _peerB) : this(_peerA, _peerB, new DefaultReplicationEventListener())
		{
		}

		public GenericReplicationSession(IReplicationProvider providerA, IReplicationProvider
			 providerB, IReplicationEventListener listener) : this(providerA, providerB, listener
			, null)
		{
		}

		public GenericReplicationSession(IReplicationProvider providerA, IReplicationProvider
			 providerB, IReplicationEventListener listener, IReflector reflector)
		{
			//null means bidirectional replication.
			_reflector = new ReplicationReflector(providerA, providerB, reflector);
			_collectionHandler = new CollectionHandlerImpl(_reflector);
			_traverser = new GenericTraverser(_reflector, _collectionHandler);
			_providerA = (IReplicationProviderInside)providerA;
			_providerB = (IReplicationProviderInside)providerB;
			_listener = listener;
			RunIsolated(new _IBlock4_73(this));
		}

		private sealed class _IBlock4_73 : IBlock4
		{
			public _IBlock4_73(GenericReplicationSession _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing._providerA.StartReplicationTransaction(this._enclosing._providerB
					.GetSignature());
				this._enclosing._providerB.StartReplicationTransaction(this._enclosing._providerA
					.GetSignature());
				if (this._enclosing._providerA.GetLastReplicationVersion() != this._enclosing._providerB
					.GetLastReplicationVersion())
				{
					throw new Exception("Version numbers must be the same but (" + this._enclosing._providerA
						.GetName() + ":" + this._enclosing._providerA.GetLastReplicationVersion() + ")  ("
						 + this._enclosing._providerB.GetName() + ":" + this._enclosing._providerB.GetLastReplicationVersion
						() + ")");
				}
				this._enclosing._lastReplicationVersion = this._enclosing._providerA.GetLastReplicationVersion
					();
			}

			private readonly GenericReplicationSession _enclosing;
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

		private void RunIsolated(IBlock4 block)
		{
			_providerA.RunIsolated(new _IBlock4_108(this, block));
		}

		private sealed class _IBlock4_108 : IBlock4
		{
			public _IBlock4_108(GenericReplicationSession _enclosing, IBlock4 block)
			{
				this._enclosing = _enclosing;
				this.block = block;
			}

			public void Run()
			{
				this._enclosing._providerB.RunIsolated(new _IBlock4_110(block));
			}

			private sealed class _IBlock4_110 : IBlock4
			{
				public _IBlock4_110(IBlock4 block)
				{
					this.block = block;
				}

				public void Run()
				{
					block.Run();
				}

				private readonly IBlock4 block;
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IBlock4 block;
		}

		public void Commit()
		{
			RunIsolated(new _IBlock4_120(this));
		}

		private sealed class _IBlock4_120 : IBlock4
		{
			public _IBlock4_120(GenericReplicationSession _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				long maxVersion = Math.Max(this._enclosing._providerA.GetCurrentVersion(), this._enclosing
					._providerB.GetCurrentVersion());
				this._enclosing._providerA.SyncVersionWithPeer(maxVersion);
				this._enclosing._providerB.SyncVersionWithPeer(maxVersion);
				maxVersion++;
				this._enclosing._providerA.CommitReplicationTransaction(maxVersion);
				this._enclosing._providerB.CommitReplicationTransaction(maxVersion);
			}

			private readonly GenericReplicationSession _enclosing;
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
				CopyStateAcross(_providerA, _providerB);
				CopyStateAcross(_providerB, _providerA);
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
			// TODO: Write tests for rollback.
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
			 sourceProvider, IReplicationProviderInside targetProvider)
		{
			IReflectClass componentType = _reflector.GetComponentType(claxx);
			int[] dimensions = _reflector.ArrayDimensions(original);
			object result = _reflector.NewArrayInstance(componentType, dimensions);
			object[] flatContents = _reflector.ArrayContents(original);
			//TODO Optimize: Copy the structure without flattening. Do this in ReflectArray.
			if (!(_reflector.IsValueType(claxx) || _reflector.IsValueType(componentType)))
			{
				ReplaceWithCounterparts(flatContents, sourceProvider, targetProvider);
			}
			_reflector.ArrayShape(flatContents, 0, result, dimensions, 0);
			return result;
		}

		private void CopyFieldValuesAcross(object src, object dest, IReflectClass claxx, 
			IReplicationProviderInside sourceProvider, IReplicationProviderInside targetProvider
			)
		{
			IEnumerator fields = FieldIterators.PersistentFields(claxx);
			while (fields.MoveNext())
			{
				IReflectField field = (IReflectField)fields.Current;
				object value = field.Get(src);
				field.Set(dest, FindCounterpart(value, sourceProvider, targetProvider));
			}
			IReflectClass superclass = claxx.GetSuperclass();
			if (superclass == null)
			{
				return;
			}
			CopyFieldValuesAcross(src, dest, superclass, sourceProvider, targetProvider);
		}

		private void CopyStateAcross(IReplicationProviderInside sourceProvider, IReplicationProviderInside
			 targetProvider)
		{
			if (_directionTo == sourceProvider)
			{
				return;
			}
			sourceProvider.VisitCachedReferences(new _IVisitor4_218(this, sourceProvider, targetProvider
				));
		}

		private sealed class _IVisitor4_218 : IVisitor4
		{
			public _IVisitor4_218(GenericReplicationSession _enclosing, IReplicationProviderInside
				 sourceProvider, IReplicationProviderInside targetProvider)
			{
				this._enclosing = _enclosing;
				this.sourceProvider = sourceProvider;
				this.targetProvider = targetProvider;
			}

			public void Visit(object obj)
			{
				this._enclosing.CopyStateAcross((IReplicationReference)obj, sourceProvider, targetProvider
					);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IReplicationProviderInside sourceProvider;

			private readonly IReplicationProviderInside targetProvider;
		}

		private void CopyStateAcross(IReplicationReference sourceRef, IReplicationProviderInside
			 sourceProvider, IReplicationProviderInside targetProvider)
		{
			if (!sourceRef.IsMarkedForReplicating())
			{
				return;
			}
			CopyStateAcross(sourceRef.Object(), sourceRef.Counterpart(), sourceProvider, targetProvider
				);
		}

		private void CopyStateAcross(object source, object dest, IReplicationProviderInside
			 sourceProvider, IReplicationProviderInside targetProvider)
		{
			IReflectClass claxx = _reflector.ForObject(source);
			CopyFieldValuesAcross(source, dest, claxx, sourceProvider, targetProvider);
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
			, IReplicationProviderInside targetProvider)
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
				return ArrayClone(value, claxx, sourceProvider, targetProvider);
			}
			if (Platform4.IsTransient(claxx))
			{
				return null;
			}
			// TODO: make it a warning
			if (_reflector.IsValueType(claxx))
			{
				return value;
			}
			if (_collectionHandler.CanHandle(value))
			{
				return CollectionClone(value, claxx, sourceProvider, targetProvider);
			}
			//if value is a Collection, result should be found by passing in just the value
			IReplicationReference @ref = sourceProvider.ProduceReference(value, null, null);
			if (@ref == null)
			{
				throw new InvalidOperationException("unable to find the ref of " + value + " of class "
					 + value.GetType());
			}
			object result = @ref.Counterpart();
			if (result != null)
			{
				return result;
			}
			IReplicationReference targetRef = targetProvider.ProduceReferenceByUUID(@ref.Uuid
				(), value.GetType());
			if (targetRef == null)
			{
				throw new InvalidOperationException("unable to find the counterpart of " + value 
					+ " of class " + value.GetType());
			}
			return targetRef.Object();
		}

		private object CollectionClone(object original, IReflectClass claxx, IReplicationProviderInside
			 sourceProvider, IReplicationProviderInside targetProvider)
		{
			return _collectionHandler.CloneWithCounterparts(sourceProvider, original, claxx, 
				new _ICounterpartFinder_274(this, sourceProvider, targetProvider));
		}

		private sealed class _ICounterpartFinder_274 : ICounterpartFinder
		{
			public _ICounterpartFinder_274(GenericReplicationSession _enclosing, IReplicationProviderInside
				 sourceProvider, IReplicationProviderInside targetProvider)
			{
				this._enclosing = _enclosing;
				this.sourceProvider = sourceProvider;
				this.targetProvider = targetProvider;
			}

			public object FindCounterpart(object original)
			{
				return this._enclosing.FindCounterpart(original, sourceProvider, targetProvider);
			}

			private readonly GenericReplicationSession _enclosing;

			private readonly IReplicationProviderInside sourceProvider;

			private readonly IReplicationProviderInside targetProvider;
		}

		private IReplicationProviderInside Other(IReplicationProviderInside peer)
		{
			return peer == _providerA ? _providerB : _providerA;
		}

		private void ReplaceWithCounterparts(object[] objects, IReplicationProviderInside
			 sourceProvider, IReplicationProviderInside targetProvider)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				object @object = objects[i];
				if (@object == null)
				{
					continue;
				}
				objects[i] = FindCounterpart(@object, sourceProvider, targetProvider);
			}
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
			destination.VisitCachedReferences(new _IVisitor4_306(this, destination));
			source.VisitCachedReferences(new _IVisitor4_312(this, destination));
		}

		private sealed class _IVisitor4_306 : IVisitor4
		{
			public _IVisitor4_306(GenericReplicationSession _enclosing, IReplicationProviderInside
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

		private sealed class _IVisitor4_312 : IVisitor4
		{
			public _IVisitor4_312(GenericReplicationSession _enclosing, IReplicationProviderInside
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
