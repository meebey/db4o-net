/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

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
using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Inside.Traversal;

namespace Db4objects.Drs.Inside
{
	internal class InstanceReplicationPreparer : IVisitor
	{
		private readonly IReplicationProviderInside _providerA;

		private readonly IReplicationProviderInside _providerB;

		private readonly IReplicationProvider _directionTo;

		private readonly IReplicationEventListener _listener;

		private readonly bool _isReplicatingOnlyDeletions;

		private readonly long _lastReplicationVersion;

		private readonly Hashtable4 _uuidsProcessedInSession;

		private readonly ITraverser _traverser;

		private readonly ReplicationReflector _reflector;

		private readonly Db4objects.Drs.Inside.ICollectionHandler _collectionHandler;

		/// <summary>
		/// Purpose: handle circular references
		/// TODO Big Refactoring: Evolve this to handle ALL reference logic (!) and remove it from the providers.
		/// </summary>
		/// <remarks>
		/// Purpose: handle circular references
		/// TODO Big Refactoring: Evolve this to handle ALL reference logic (!) and remove it from the providers.
		/// </remarks>
		private readonly Hashtable4 _objectsPreparedToReplicate = new Hashtable4(10000);

		/// <summary>
		/// key = object originated from one provider
		/// value = the counterpart ReplicationReference of the original object
		/// </summary>
		private Hashtable4 _counterpartRefsByOriginal = new Hashtable4(10000);

		private readonly ReplicationEventImpl _event;

		private readonly ObjectStateImpl _stateInA;

		private readonly ObjectStateImpl _stateInB;

		private object _obj;

		private object _referencingObject;

		private string _fieldName;

		internal InstanceReplicationPreparer(IReplicationProviderInside providerA, IReplicationProviderInside
			 providerB, IReplicationProvider directionTo, IReplicationEventListener listener
			, bool isReplicatingOnlyDeletions, long lastReplicationVersion, Hashtable4 uuidsProcessedInSession
			, ITraverser traverser, ReplicationReflector reflector, Db4objects.Drs.Inside.ICollectionHandler
			 collectionHandler)
		{
			_event = new ReplicationEventImpl();
			_stateInA = _event._stateInProviderA;
			_stateInB = _event._stateInProviderB;
			_providerA = providerA;
			_providerB = providerB;
			_directionTo = directionTo;
			_listener = listener;
			_isReplicatingOnlyDeletions = isReplicatingOnlyDeletions;
			_lastReplicationVersion = lastReplicationVersion;
			_uuidsProcessedInSession = uuidsProcessedInSession;
			_traverser = traverser;
			_reflector = reflector;
			_collectionHandler = collectionHandler;
		}

		public bool Visit(object obj)
		{
			if (_objectsPreparedToReplicate.Get(obj) != null)
			{
				return false;
			}
			if (IsValueType(obj))
			{
				return true;
			}
			_objectsPreparedToReplicate.Put(obj, obj);
			return PrepareObjectToBeReplicated(obj, null, null);
		}

		private bool IsValueType(object o)
		{
			return ReplicationPlatform.IsValueType(o);
		}

		private bool PrepareObjectToBeReplicated(object obj, object referencingObject, string
			 fieldName)
		{
			//TODO Optimization: keep track of the peer we are traversing to avoid having to look in both.
			_obj = obj;
			_referencingObject = referencingObject;
			_fieldName = fieldName;
			IReplicationReference refA = _providerA.ProduceReference(_obj, _referencingObject
				, _fieldName);
			IReplicationReference refB = _providerB.ProduceReference(_obj, _referencingObject
				, _fieldName);
			if (refA == null && refB == null)
			{
				throw new Exception(string.Empty + _obj.GetType() + " " + _obj + " must be stored in one of the databases being replicated."
					);
			}
			//FIXME: Use db4o's standard for throwing exceptions.
			if (refA != null && refB != null)
			{
				throw new Exception(string.Empty + _obj.GetType() + " " + _obj + " cannot be referenced by both databases being replicated."
					);
			}
			//FIXME: Use db4o's standard for throwing exceptions.
			IReplicationProviderInside owner = refA == null ? _providerB : _providerA;
			IReplicationReference ownerRef = refA == null ? refB : refA;
			IReplicationProviderInside other = Other(owner);
			Db4oUUID uuid = ownerRef.Uuid();
			IReplicationReference otherRef = other.ProduceReferenceByUUID(uuid, _obj.GetType(
				));
			if (refA == null)
			{
				refA = otherRef;
			}
			else
			{
				refB = otherRef;
			}
			//TODO for circular referenced object, otherRef should not be null in the subsequent pass.
			//But db4o always return null. A bug. check!
			if (otherRef == null)
			{
				//Object is only present in one ReplicationProvider. Missing in the other. Could have been deleted or never replicated.
				if (WasProcessed(uuid))
				{
					return false;
				}
				MarkAsProcessed(uuid);
				long creationTime = ownerRef.Uuid().GetLongPart();
				if (creationTime > _lastReplicationVersion)
				{
					//if it was created after the last time two ReplicationProviders were replicated it has to be treated as new.
					if (_isReplicatingOnlyDeletions)
					{
						return false;
					}
					return HandleNewObject(_obj, ownerRef, owner, other, _referencingObject, _fieldName
						, true, false);
				}
				else
				{
					// if it was created before the last time two ReplicationProviders were replicated it has to be treated as deleted.
					return HandleMissingObjectInOther(_obj, ownerRef, owner, other, _referencingObject
						, _fieldName);
				}
			}
			if (_isReplicatingOnlyDeletions)
			{
				return false;
			}
			ownerRef.SetCounterpart(otherRef.Object());
			if (WasProcessed(uuid))
			{
				return false;
			}
			//Has to be done AFTER the counterpart is set because object yet to be replicated might reference the current one, replicated previously.
			MarkAsProcessed(uuid);
			object objectA = refA.Object();
			object objectB = refB.Object();
			bool changedInA = _providerA.WasModifiedSinceLastReplication(refA);
			//System.out.println("changedInA = " + changedInA);
			bool changedInB = _providerB.WasModifiedSinceLastReplication(refB);
			//System.out.println("changedInB = " + changedInB);
			if (!changedInA && !changedInB)
			{
				return false;
			}
			bool conflict = false;
			if (changedInA && changedInB)
			{
				conflict = true;
			}
			if (changedInA && _directionTo == _providerA)
			{
				conflict = true;
			}
			if (changedInB && _directionTo == _providerB)
			{
				conflict = true;
			}
			object prevailing = _obj;
			_providerA.Activate(objectA);
			_providerB.Activate(objectB);
			_event.ResetAction();
			_event._isConflict = conflict;
			_event._creationDate = TimeStampIdGenerator.IdToMilliseconds(uuid.GetLongPart());
			_stateInA.SetAll(objectA, false, changedInA, TimeStampIdGenerator.IdToMilliseconds
				(ownerRef.Version()));
			_stateInB.SetAll(objectB, false, changedInB, TimeStampIdGenerator.IdToMilliseconds
				(otherRef.Version()));
			_listener.OnReplicate(_event);
			if (conflict)
			{
				if (!_event._actionWasChosen)
				{
					ThrowReplicationConflictException();
				}
				if (_event._actionChosenState == null)
				{
					return false;
				}
				if (_event._actionChosenState == _stateInA)
				{
					prevailing = objectA;
				}
				if (_event._actionChosenState == _stateInB)
				{
					prevailing = objectB;
				}
			}
			else
			{
				if (_event._actionWasChosen)
				{
					if (_event._actionChosenState == _stateInA)
					{
						prevailing = objectA;
					}
					if (_event._actionChosenState == _stateInB)
					{
						prevailing = objectB;
					}
					if (_event._actionChosenState == null)
					{
						return false;
					}
				}
				else
				{
					if (changedInA)
					{
						prevailing = objectA;
					}
					if (changedInB)
					{
						prevailing = objectB;
					}
				}
			}
			IReplicationProviderInside prevailingPeer = prevailing == objectA ? _providerA : 
				_providerB;
			if (_directionTo == prevailingPeer)
			{
				return false;
			}
			if (!conflict)
			{
				prevailingPeer.Activate(prevailing);
			}
			//Already activated if there was a conflict.
			if (prevailing != _obj)
			{
				otherRef.SetCounterpart(_obj);
				otherRef.MarkForReplicating();
				MarkAsNotProcessed(uuid);
				_traverser.ExtendTraversalTo(prevailing);
			}
			else
			{
				//Now we start traversing objects on the other peer! Is that cool or what? ;)
				ownerRef.MarkForReplicating();
			}
			return !_event._actionShouldStopTraversal;
		}

		private void MarkAsNotProcessed(Db4oUUID uuid)
		{
			_uuidsProcessedInSession.Remove(uuid);
		}

		private void MarkAsProcessed(Db4oUUID uuid)
		{
			if (_uuidsProcessedInSession.Get(uuid) != null)
			{
				throw new Exception("illegal state");
			}
			_uuidsProcessedInSession.Put(uuid, uuid);
		}

		//Using this Hashtable4 as a Set.
		private bool WasProcessed(Db4oUUID uuid)
		{
			return _uuidsProcessedInSession.Get(uuid) != null;
		}

		private IReplicationProviderInside Other(IReplicationProviderInside peer)
		{
			return peer == _providerA ? _providerB : _providerA;
		}

		private bool HandleMissingObjectInOther(object obj, IReplicationReference ownerRef
			, IReplicationProviderInside owner, IReplicationProviderInside other, object referencingObject
			, string fieldName)
		{
			bool isConflict = false;
			bool wasModified = owner.WasModifiedSinceLastReplication(ownerRef);
			if (wasModified)
			{
				isConflict = true;
			}
			if (_directionTo == other)
			{
				isConflict = true;
			}
			object prevailing = null;
			//by default, deletion prevails
			if (isConflict)
			{
				owner.Activate(obj);
			}
			_event.ResetAction();
			_event._isConflict = isConflict;
			_event._creationDate = TimeStampIdGenerator.IdToMilliseconds(ownerRef.Uuid().GetLongPart
				());
			long modificationDate = TimeStampIdGenerator.IdToMilliseconds(ownerRef.Version());
			if (owner == _providerA)
			{
				_stateInA.SetAll(obj, false, wasModified, modificationDate);
				_stateInB.SetAll(null, false, false, -1);
			}
			else
			{
				//owner == _providerB
				_stateInA.SetAll(null, false, false, -1);
				_stateInB.SetAll(obj, false, wasModified, modificationDate);
			}
			_listener.OnReplicate(_event);
			if (isConflict && !_event._actionWasChosen)
			{
				ThrowReplicationConflictException();
			}
			if (_event._actionWasChosen)
			{
				if (_event._actionChosenState == null)
				{
					return false;
				}
				if (_event._actionChosenState == _stateInA)
				{
					prevailing = _stateInA.GetObject();
				}
				if (_event._actionChosenState == _stateInB)
				{
					prevailing = _stateInB.GetObject();
				}
			}
			if (prevailing == null)
			{
				//Deletion has prevailed.
				if (_directionTo == other)
				{
					return false;
				}
				ownerRef.MarkForDeleting();
				return !_event._actionShouldStopTraversal;
			}
			bool needsToBeActivated = !isConflict;
			//Already activated if there was a conflict.
			return HandleNewObject(obj, ownerRef, owner, other, referencingObject, fieldName, 
				needsToBeActivated, true);
		}

		private bool HandleNewObject(object obj, IReplicationReference ownerRef, IReplicationProviderInside
			 owner, IReplicationProviderInside other, object referencingObject, string fieldName
			, bool needsToBeActivated, bool listenerAlreadyNotified)
		{
			if (_directionTo == owner)
			{
				return false;
			}
			if (needsToBeActivated)
			{
				owner.Activate(obj);
			}
			if (!listenerAlreadyNotified)
			{
				_event.ResetAction();
				_event._isConflict = false;
				_event._creationDate = TimeStampIdGenerator.IdToMilliseconds(ownerRef.Uuid().GetLongPart
					());
				if (owner == _providerA)
				{
					_stateInA.SetAll(obj, true, false, -1);
					_stateInB.SetAll(null, false, false, -1);
				}
				else
				{
					_stateInA.SetAll(null, false, false, -1);
					_stateInB.SetAll(obj, true, false, -1);
				}
				_listener.OnReplicate(_event);
				if (_event._actionWasChosen)
				{
					if (_event._actionChosenState == null)
					{
						return false;
					}
					if (_event._actionChosenState.GetObject() != obj)
					{
						return false;
					}
				}
			}
			object counterpart = EmptyClone(owner, obj);
			ownerRef.SetCounterpart(counterpart);
			ownerRef.MarkForReplicating();
			IReplicationReference otherRef = other.ReferenceNewObject(counterpart, ownerRef, 
				GetCounterpartRef(referencingObject), fieldName);
			PutCounterpartRef(obj, otherRef);
			if (_event._actionShouldStopTraversal)
			{
				return false;
			}
			return true;
		}

		private void ThrowReplicationConflictException()
		{
			throw new ReplicationConflictException("A replication conflict ocurred and the ReplicationEventListener, if any, did not choose which state should override the other."
				);
		}

		private object EmptyClone(IReplicationProviderInside sourceProvider, object obj)
		{
			if (obj == null)
			{
				return null;
			}
			IReflectClass claxx = ReflectClass(obj);
			//		if (claxx.isSecondClass()) return obj;
			if (_reflector.IsSecondClass(claxx))
			{
				throw new Exception("IllegalState");
			}
			//		if (claxx.isArray()) return arrayClone(obj, claxx, sourceProvider); //Copy arrayClone() from GenericReplicationSession if necessary.
			if (claxx.IsArray())
			{
				throw new Exception("IllegalState");
			}
			//Copy arrayClone() from GenericReplicationSession if necessary.
			if (_collectionHandler.CanHandleClass(claxx))
			{
				return CollectionClone(sourceProvider, obj, claxx);
			}
			object result = claxx.NewInstance();
			if (result == null)
			{
				throw new Exception("Unable to create a new instance of " + obj.GetType());
			}
			//FIXME Use db4o's standard for throwing exceptions.
			return result;
		}

		private IReflectClass ReflectClass(object obj)
		{
			return _reflector.ForObject(obj);
		}

		private object CollectionClone(IReplicationProviderInside sourceProvider, object 
			original, IReflectClass claxx)
		{
			return _collectionHandler.EmptyClone(sourceProvider, original, claxx);
		}

		private IReplicationReference GetCounterpartRef(object original)
		{
			return (IReplicationReference)_counterpartRefsByOriginal.Get(original);
		}

		private void PutCounterpartRef(object obj, IReplicationReference otherRef)
		{
			if (_counterpartRefsByOriginal.Get(obj) != null)
			{
				throw new Exception("illegal state");
			}
			_counterpartRefsByOriginal.Put(obj, otherRef);
		}
	}
}
