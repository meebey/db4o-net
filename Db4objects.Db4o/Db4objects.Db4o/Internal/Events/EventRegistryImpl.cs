/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
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

		protected System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> _queryStarted;

		protected System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> _queryFinished;

		protected System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> 
			_creating;

		protected System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> 
			_activating;

		protected System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> 
			_updating;

		protected System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> 
			_deleting;

		protected System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> 
			_deactivating;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _created;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _activated;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _updated;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _deleted;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _deactivated;

		protected System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> _committing;

		protected System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> _committed;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> _instantiated;

		protected System.EventHandler<Db4objects.Db4o.Events.ClassEventArgs> _classRegistered;

		protected System.EventHandler<Db4objects.Db4o.Events.ObjectContainerEventArgs> _closing;

		public EventRegistryImpl(IInternalObjectContainer container)
		{
			_container = container;
		}

		// Callbacks implementation
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

		public virtual void ObjectOnActivate(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _activated, obj);
		}

		public virtual void ObjectOnNew(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _created, obj);
		}

		public virtual void ObjectOnUpdate(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _updated, obj);
		}

		public virtual void ObjectOnDelete(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _deleted, obj);
		}

		public virtual void ClassOnRegistered(ClassMetadata clazz)
		{
			EventPlatform.TriggerClassEvent(_classRegistered, clazz);
		}

		public virtual void ObjectOnDeactivate(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _deactivated, obj);
		}

		public virtual void ObjectOnInstantiate(Transaction transaction, IObjectInfo obj)
		{
			EventPlatform.TriggerObjectInfoEvent(transaction, _instantiated, obj);
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

		public virtual event System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> QueryFinished
		{
			add
			{
				_queryFinished = (System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs>)System.Delegate.Combine
					(_queryFinished, value);
			}
			remove
			{
				_queryFinished = (System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs>)System.Delegate.Remove
					(_queryFinished, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> QueryStarted
		{
			add
			{
				_queryStarted = (System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs>)System.Delegate.Combine
					(_queryStarted, value);
			}
			remove
			{
				_queryStarted = (System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs>)System.Delegate.Remove
					(_queryStarted, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
			 Creating
		{
			add
			{
				_creating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Combine(_creating, value);
			}
			remove
			{
				_creating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Remove(_creating, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
			 Activating
		{
			add
			{
				_activating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Combine(_activating, value);
			}
			remove
			{
				_activating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Remove(_activating, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
			 Updating
		{
			add
			{
				_updating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Combine(_updating, value);
			}
			remove
			{
				_updating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Remove(_updating, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
			 Deleting
		{
			add
			{
				_deleting = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Combine(_deleting, value);
			}
			remove
			{
				_deleting = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Remove(_deleting, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
			 Deactivating
		{
			add
			{
				_deactivating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Combine(_deactivating, value);
			}
			remove
			{
				_deactivating = (System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs>
					)System.Delegate.Remove(_deactivating, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Created
		{
			add
			{
				_created = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Combine
					(_created, value);
			}
			remove
			{
				_created = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Remove
					(_created, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Activated
		{
			add
			{
				_activated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Combine
					(_activated, value);
			}
			remove
			{
				_activated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Remove
					(_activated, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Updated
		{
			add
			{
				_updated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Combine
					(_updated, value);
			}
			remove
			{
				_updated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Remove
					(_updated, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Deleted
		{
			add
			{
				_deleted = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Combine
					(_deleted, value);
			}
			remove
			{
				_deleted = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Remove
					(_deleted, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Deactivated
		{
			add
			{
				_deactivated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Combine
					(_deactivated, value);
			}
			remove
			{
				_deactivated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)System.Delegate.Remove
					(_deactivated, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> 
			Committing
		{
			add
			{
				_committing = (System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs>)System.Delegate.Combine
					(_committing, value);
			}
			remove
			{
				_committing = (System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs>)System.Delegate.Remove
					(_committing, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> 
			Committed
		{
			add
			{
				_committed = (System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs>)System.Delegate.Combine
					(_committed, value);
				OnCommittedListener();
			}
			remove
			{
				_committed = (System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs>)System.Delegate.Remove
					(_committed, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ClassEventArgs> ClassRegistered
		{
			add
			{
				_classRegistered = (System.EventHandler<Db4objects.Db4o.Events.ClassEventArgs>)System.Delegate.Combine
					(_classRegistered, value);
			}
			remove
			{
				_classRegistered = (System.EventHandler<Db4objects.Db4o.Events.ClassEventArgs>)System.Delegate.Remove
					(_classRegistered, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>
			 Instantiated
		{
			add
			{
				_instantiated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)
					System.Delegate.Combine(_instantiated, value);
			}
			remove
			{
				_instantiated = (System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs>)
					System.Delegate.Remove(_instantiated, value);
			}
		}

		public virtual event System.EventHandler<Db4objects.Db4o.Events.ObjectContainerEventArgs>
			 Closing
		{
			add
			{
				_closing = (System.EventHandler<Db4objects.Db4o.Events.ObjectContainerEventArgs>)
					System.Delegate.Combine(_closing, value);
			}
			remove
			{
				_closing = (System.EventHandler<Db4objects.Db4o.Events.ObjectContainerEventArgs>)
					System.Delegate.Remove(_closing, value);
			}
		}

		protected virtual void OnCommittedListener()
		{
			// TODO: notify the server that we are interested in 
			// committed callbacks
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
