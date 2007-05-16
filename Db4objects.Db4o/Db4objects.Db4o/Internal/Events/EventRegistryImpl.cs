/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Events;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Internal.Events
{
	/// <exclude></exclude>
	public class EventRegistryImpl : ICallbacks, IEventRegistry
	{
		private readonly ObjectContainerBase _container;

		protected Db4objects.Db4o.Events.QueryEventHandler _queryStarted;

		protected Db4objects.Db4o.Events.QueryEventHandler _queryFinished;

		protected Db4objects.Db4o.Events.CancellableObjectEventHandler _creating;

		protected Db4objects.Db4o.Events.CancellableObjectEventHandler _activating;

		protected Db4objects.Db4o.Events.CancellableObjectEventHandler _updating;

		protected Db4objects.Db4o.Events.CancellableObjectEventHandler _deleting;

		protected Db4objects.Db4o.Events.CancellableObjectEventHandler _deactivating;

		protected Db4objects.Db4o.Events.ObjectEventHandler _created;

		protected Db4objects.Db4o.Events.ObjectEventHandler _activated;

		protected Db4objects.Db4o.Events.ObjectEventHandler _updated;

		protected Db4objects.Db4o.Events.ObjectEventHandler _deleted;

		protected Db4objects.Db4o.Events.ObjectEventHandler _deactivated;

		protected Db4objects.Db4o.Events.CommitEventHandler _committing;

		protected Db4objects.Db4o.Events.CommitEventHandler _committed;

		protected Db4objects.Db4o.Events.ObjectEventHandler _instantiated;

		protected Db4objects.Db4o.Events.ClassEventHandler _classRegistered;

		public EventRegistryImpl(ObjectContainerBase container)
		{
			_container = container;
		}

		public virtual void QueryOnFinished(IQuery query)
		{
			EventPlatform.TriggerQueryEvent(_queryFinished, query);
		}

		public virtual void QueryOnStarted(IQuery query)
		{
			EventPlatform.TriggerQueryEvent(_queryStarted, query);
		}

		public virtual bool ObjectCanNew(object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(_creating, obj);
		}

		public virtual bool ObjectCanActivate(object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(_activating, obj);
		}

		public virtual bool ObjectCanUpdate(object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(_updating, obj);
		}

		public virtual bool ObjectCanDelete(object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(_deleting, obj);
		}

		public virtual bool ObjectCanDeactivate(object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(_deactivating, obj);
		}

		public virtual void ObjectOnActivate(object obj)
		{
			EventPlatform.TriggerObjectEvent(_activated, obj);
		}

		public virtual void ObjectOnNew(object obj)
		{
			EventPlatform.TriggerObjectEvent(_created, obj);
		}

		public virtual void ObjectOnUpdate(object obj)
		{
			EventPlatform.TriggerObjectEvent(_updated, obj);
		}

		public virtual void ObjectOnDelete(object obj)
		{
			EventPlatform.TriggerObjectEvent(_deleted, obj);
		}

		public virtual void ClassOnRegistered(ClassMetadata clazz)
		{
			EventPlatform.TriggerClassEvent(_classRegistered, clazz);
		}

		public virtual void ObjectOnDeactivate(object obj)
		{
			EventPlatform.TriggerObjectEvent(_deactivated, obj);
		}

		public virtual void ObjectOnInstantiate(object obj)
		{
			EventPlatform.TriggerObjectEvent(_instantiated, obj);
		}

		public virtual void CommitOnStarted(object transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
			EventPlatform.TriggerCommitEvent(_committing, transaction, objectInfoCollections);
		}

		public virtual void CommitOnCompleted(object transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
			EventPlatform.TriggerCommitEvent(_committed, transaction, objectInfoCollections);
		}

		public virtual event Db4objects.Db4o.Events.QueryEventHandler QueryFinished
		{
			add
			{
				_queryFinished = (Db4objects.Db4o.Events.QueryEventHandler)System.Delegate.Combine
					(_queryFinished, value);
			}
			remove
			{
				_queryFinished = (Db4objects.Db4o.Events.QueryEventHandler)System.Delegate.Remove
					(_queryFinished, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.QueryEventHandler QueryStarted
		{
			add
			{
				_queryStarted = (Db4objects.Db4o.Events.QueryEventHandler)System.Delegate.Combine
					(_queryStarted, value);
			}
			remove
			{
				_queryStarted = (Db4objects.Db4o.Events.QueryEventHandler)System.Delegate.Remove(
					_queryStarted, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CancellableObjectEventHandler Creating
		{
			add
			{
				_creating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Combine
					(_creating, value);
			}
			remove
			{
				_creating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Remove
					(_creating, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CancellableObjectEventHandler Activating
		{
			add
			{
				_activating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Combine
					(_activating, value);
			}
			remove
			{
				_activating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Remove
					(_activating, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CancellableObjectEventHandler Updating
		{
			add
			{
				_updating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Combine
					(_updating, value);
			}
			remove
			{
				_updating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Remove
					(_updating, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CancellableObjectEventHandler Deleting
		{
			add
			{
				_deleting = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Combine
					(_deleting, value);
			}
			remove
			{
				_deleting = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Remove
					(_deleting, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CancellableObjectEventHandler Deactivating
		{
			add
			{
				_deactivating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Combine
					(_deactivating, value);
			}
			remove
			{
				_deactivating = (Db4objects.Db4o.Events.CancellableObjectEventHandler)System.Delegate.Remove
					(_deactivating, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Created
		{
			add
			{
				_created = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine(_created
					, value);
			}
			remove
			{
				_created = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove(_created
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Activated
		{
			add
			{
				_activated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine(_activated
					, value);
			}
			remove
			{
				_activated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove(_activated
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Updated
		{
			add
			{
				_updated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine(_updated
					, value);
			}
			remove
			{
				_updated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove(_updated
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Deleted
		{
			add
			{
				_deleted = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine(_deleted
					, value);
			}
			remove
			{
				_deleted = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove(_deleted
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Deactivated
		{
			add
			{
				_deactivated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine
					(_deactivated, value);
			}
			remove
			{
				_deactivated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove(
					_deactivated, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CommitEventHandler Committing
		{
			add
			{
				_committing = (Db4objects.Db4o.Events.CommitEventHandler)System.Delegate.Combine(
					_committing, value);
			}
			remove
			{
				_committing = (Db4objects.Db4o.Events.CommitEventHandler)System.Delegate.Remove(_committing
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.CommitEventHandler Committed
		{
			add
			{
				_committed = (Db4objects.Db4o.Events.CommitEventHandler)System.Delegate.Combine(_committed
					, value);
				OnCommittedListener();
			}
			remove
			{
				_committed = (Db4objects.Db4o.Events.CommitEventHandler)System.Delegate.Remove(_committed
					, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ClassEventHandler ClassRegistered
		{
			add
			{
				_classRegistered = (Db4objects.Db4o.Events.ClassEventHandler)System.Delegate.Combine
					(_classRegistered, value);
			}
			remove
			{
				_classRegistered = (Db4objects.Db4o.Events.ClassEventHandler)System.Delegate.Remove
					(_classRegistered, value);
			}
		}

		public virtual event Db4objects.Db4o.Events.ObjectEventHandler Instantiated
		{
			add
			{
				_instantiated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Combine
					(_instantiated, value);
			}
			remove
			{
				_instantiated = (Db4objects.Db4o.Events.ObjectEventHandler)System.Delegate.Remove
					(_instantiated, value);
			}
		}

		private void OnCommittedListener()
		{
			_container.OnCommittedListener();
		}

		public virtual bool CaresAboutCommitting()
		{
			return EventPlatform.HasListeners(_committing);
		}

		public virtual bool CaresAboutCommitted()
		{
			return EventPlatform.HasListeners(_committed);
		}
	}
}
