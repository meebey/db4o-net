/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// Provides a way to register event handlers for specific <see cref="IObjectContainer">IObjectContainer</see> events.<br/>
	/// EventRegistry methods represent events available for registering callbacks.
	/// EventRegistry instance can be obtained from <see cref="EventRegistryFactory">EventRegistryFactory</see>.
	/// <code>EventRegistry registry =  EventRegistryFactory.ForObjectContainer(container);</code>
	/// A new callback can be registered for an event with the following code:
	/// <code>
	/// private static void OnCreated(object sender, ObjectInfoEventArgs args)
	/// {
	/// Object obj = args.Object;
	/// if (obj is Pilot)
	/// {
	/// Console.WriteLine(obj.ToString());
	/// }
	/// }
	/// registry.Created+=new System.EventHandler&lt;ObjectInfoEventArgs&gt;(OnCreated);
	/// </code>
	/// <seealso cref="EventRegistryFactory">EventRegistryFactory</seealso>
	/// </summary>
	public interface IEventRegistry
	{
		/// <summary>
		/// This event is fired upon a query start and can be used to gather
		/// query statistics.
		/// </summary>
		/// <remarks>
		/// This event is fired upon a query start and can be used to gather
		/// query statistics.
		/// The query object is available from
		/// <see cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</see>
		/// event parameter.<br />
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> QueryStarted;

		/// <summary>
		/// This event is fired upon a query end and can be used to gather
		/// query statistics.
		/// </summary>
		/// <remarks>
		/// This event is fired upon a query end and can be used to gather
		/// query statistics.
		/// The query object is available from
		/// <see cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</see>
		/// event parameter.<br />
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.QueryEventArgs> QueryFinished;

		/// <summary>This event is fired before an object is saved for the first time.</summary>
		/// <remarks>
		/// This event is fired before an object is saved for the first time.
		/// The object can be obtained from
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel">Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel
		/// 	</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Store">Db4objects.Db4o.IObjectContainer.Store
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> Creating;

		/// <summary>This event is fired before an object is activated.</summary>
		/// <remarks>
		/// This event is fired before an object is activated.
		/// The object can be obtained from
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel">Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel
		/// 	</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Activate">Db4objects.Db4o.IObjectContainer.Activate
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> Activating;

		/// <summary>This event is fired before an object is updated.</summary>
		/// <remarks>
		/// This event is fired before an object is updated.
		/// The object can be obtained from
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel">Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel
		/// 	</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Store">Db4objects.Db4o.IObjectContainer.Store
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> Updating;

		/// <summary>This event is fired before an object is deleted.</summary>
		/// <remarks>
		/// This event is fired before an object is deleted.
		/// The object can be obtained from
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel">Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel
		/// 	</see>
		/// <br /><br />
		/// Note, that this event is not available in networked client/server
		/// mode and will throw an exception when attached to a client ObjectContainer.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Delete">Db4objects.Db4o.IObjectContainer.Delete
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> Deleting;

		/// <summary>This event is fired before an object is deactivated.</summary>
		/// <remarks>
		/// This event is fired before an object is deactivated.
		/// The object can be obtained from
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel">Db4objects.Db4o.Events.CancellableObjectEventArgs.Cancel
		/// 	</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Deactivate">Db4objects.Db4o.IObjectContainer.Deactivate
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CancellableObjectEventArgs> Deactivating;

		/// <summary>This event is fired after an object is activated.</summary>
		/// <remarks>
		/// This event is fired after an object is activated.
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter. <br /><br />
		/// The event can be used to trigger some post-activation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Activate">Db4objects.Db4o.IObjectContainer.Activate
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Activated;

		/// <summary>This event is fired after an object is created (saved for the first time).
		/// 	</summary>
		/// <remarks>
		/// This event is fired after an object is created (saved for the first time).
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-creation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Store">Db4objects.Db4o.IObjectContainer.Store
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Created;

		/// <summary>This event is fired after an object is updated.</summary>
		/// <remarks>
		/// This event is fired after an object is updated.
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-update
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Store">Db4objects.Db4o.IObjectContainer.Store
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Updated;

		/// <summary>This event is fired after an object is deleted.</summary>
		/// <remarks>
		/// This event is fired after an object is deleted.
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-deletion
		/// functionality.<br /><br />
		/// Note, that this event is not available in networked client/server
		/// mode and will throw an exception when attached to a client ObjectContainer.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Delete">Db4objects.Db4o.IObjectContainer.Delete
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Deleted;

		/// <summary>This event is fired after an object is deactivated.</summary>
		/// <remarks>
		/// This event is fired after an object is deactivated.
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-deactivation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Delete">Db4objects.Db4o.IObjectContainer.Delete
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Deactivated;

		/// <summary>This event is fired just before a transaction is committed.</summary>
		/// <remarks>
		/// This event is fired just before a transaction is committed.
		/// The transaction and a list of the modified objects can
		/// be obtained from the
		/// <see cref="Db4objects.Db4o.Events.CommitEventArgs">Db4objects.Db4o.Events.CommitEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// Committing event gives a user a chance to interrupt the commit
		/// and rollback the transaction.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CommitEventArgs">Db4objects.Db4o.Events.CommitEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Commit">Db4objects.Db4o.IObjectContainer.Commit
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> Committing;

		/// <summary>This event is fired after a transaction has been committed.</summary>
		/// <remarks>
		/// This event is fired after a transaction has been committed.
		/// The transaction and a list of the modified objects can
		/// be obtained from the
		/// <see cref="Db4objects.Db4o.Events.CommitEventArgs">Db4objects.Db4o.Events.CommitEventArgs
		/// 	</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-commit functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.CommitEventArgs">Db4objects.Db4o.Events.CommitEventArgs
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.IObjectContainer.Commit">Db4objects.Db4o.IObjectContainer.Commit
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.CommitEventArgs> Committed;

		/// <summary>This event is fired when a persistent object is instantiated.</summary>
		/// <remarks>
		/// This event is fired when a persistent object is instantiated.
		/// The object can be obtained from the
		/// <see cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</see>
		/// event parameter.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ObjectInfoEventArgs">Db4objects.Db4o.Events.ObjectInfoEventArgs
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectInfoEventArgs> Instantiated;

		/// <summary>This event is fired when a new class is registered with metadata.</summary>
		/// <remarks>
		/// This event is fired when a new class is registered with metadata.
		/// The class information can be obtained from
		/// <see cref="Db4objects.Db4o.Events.ClassEventArgs">Db4objects.Db4o.Events.ClassEventArgs
		/// 	</see>
		/// event parameter.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="Db4objects.Db4o.Events.ClassEventArgs">Db4objects.Db4o.Events.ClassEventArgs
		/// 	</seealso>
		event System.EventHandler<Db4objects.Db4o.Events.ClassEventArgs> ClassRegistered;

		/// <summary>
		/// This event is fired when the
		/// <see cref="Db4objects.Db4o.IObjectContainer.Close">Db4objects.Db4o.IObjectContainer.Close
		/// 	</see>
		/// is
		/// called.
		/// </summary>
		/// <returns>event</returns>
		event System.EventHandler<Db4objects.Db4o.Events.ObjectContainerEventArgs> Closing;
	}
}
