/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
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
		private readonly IInternalObjectContainer _container;

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

		protected Db4objects.Db4o.Events.ObjectContainerEventHandler _closing;

		public EventRegistryImpl(IInternalObjectContainer container)
		{
			_container = container;
		}

		public virtual void QueryOnFinished(Transaction transaction, IQuery query)
		{
			EventPlatform.TriggerQueryEvent(transaction, _queryFinished, query);
		}

		public virtual void QueryOnStarted(Transaction transaction, IQuery query)
		{
			EventPlatform.TriggerQueryEvent(transaction, _queryStarted, query);
		}

		public virtual bool ObjectCanNew(Transaction transaction, object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(transaction, _creating, obj
				);
		}

		public virtual bool ObjectCanActivate(Transaction transaction, object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(transaction, _activating, 
				obj);
		}

		public virtual bool ObjectCanUpdate(Transaction transaction, object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(transaction, _updating, obj
				);
		}

		public virtual bool ObjectCanDelete(Transaction transaction, object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(transaction, _deleting, obj
				);
		}

		public virtual bool ObjectCanDeactivate(Transaction transaction, object obj)
		{
			return EventPlatform.TriggerCancellableObjectEventArgs(transaction, _deactivating
				, obj);
		}

		public virtual void ObjectOnActivate(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _activated, obj);
		}

		public virtual void ObjectOnNew(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _created, obj);
		}

		public virtual void ObjectOnUpdate(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _updated, obj);
		}

		public virtual void ObjectOnDelete(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _deleted, obj);
		}

		public virtual void ClassOnRegistered(ClassMetadata clazz)
		{
			EventPlatform.TriggerClassEvent(_classRegistered, clazz);
		}

		public virtual void ObjectOnDeactivate(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _deactivated, obj);
		}

		public virtual void ObjectOnInstantiate(Transaction transaction, object obj)
		{
			EventPlatform.TriggerObjectEvent(transaction, _instantiated, obj);
		}

		public virtual void CommitOnStarted(Transaction transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
			EventPlatform.TriggerCommitEvent(transaction, _committing, objectInfoCollections);
		}

		public virtual void CommitOnCompleted(Transaction transaction, CallbackObjectInfoCollections
			 objectInfoCollections)
		{
			EventPlatform.TriggerCommitEvent(transaction, _committed, objectInfoCollections);
		}

		public virtual void CloseOnStarted(IObjectContainer container)
		{
			EventPlatform.TriggerObjectContainerEvent(container, _closing);
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

		public virtual event Db4objects.Db4o.Events.ObjectContainerEventHandler Closing
		{
			add
			{
				_closing = (Db4objects.Db4o.Events.ObjectContainerEventHandler)System.Delegate.Combine
					(_closing, value);
			}
			remove
			{
				_closing = (Db4objects.Db4o.Events.ObjectContainerEventHandler)System.Delegate.Remove
					(_closing, value);
			}
		}

		protected virtual void OnCommittedListener()
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

		public virtual bool CaresAboutDeleting()
		{
			return EventPlatform.HasListeners(_deleting);
		}

		public virtual bool CaresAboutDeleted()
		{
			return EventPlatform.HasListeners(_deleted);
		}
	}
}
