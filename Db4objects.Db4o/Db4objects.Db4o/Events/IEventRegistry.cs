/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Events;

namespace Db4objects.Db4o.Events
{
	public delegate void QueryEventHandler(object sender, Db4objects.Db4o.Events.QueryEventArgs
		 args);

	public delegate void CancellableObjectEventHandler(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
		 args);

	public delegate void ObjectEventHandler(object sender, Db4objects.Db4o.Events.ObjectEventArgs
		 args);

	public delegate void CommitEventHandler(object sender, Db4objects.Db4o.Events.CommitEventArgs
		 args);

	public delegate void ClassEventHandler(object sender, Db4objects.Db4o.Events.ClassEventArgs
		 args);

	public delegate void ObjectContainerEventHandler(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
		 args);

	/// <summary>
	/// Provides a way to register event handlers for specific <see cref="IObjectContainer">IObjectContainer</see> events.<br/>
	/// EventRegistry methods represent events available for registering callbacks.
	/// EventRegistry instance can be obtained from <see cref="EventRegistryFactory">EventRegistryFactory</see>.
	/// <code>EventRegistry registry =  EventRegistryFactory.ForObjectContainer(container);</code>
	/// A new callback can be registered for an event with the following code:
	/// <code>
	/// private static void OnCreated(object sender, ObjectEventArgs args)
	/// {
	/// Object obj = args.Object;
	/// if (obj is Pilot)
	/// {
	/// Console.WriteLine(obj.ToString());
	/// }
	/// }
	/// registry.Created+=new ObjectEventHandler(OnCreated);
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
		/// <see cref="QueryEventArgs">QueryEventArgs</see>
		/// event parameter.<br />
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="QueryEventArgs">QueryEventArgs</seealso>
		event Db4objects.Db4o.Events.QueryEventHandler QueryStarted;

		/// <summary>
		/// This event is fired upon a query end and can be used to gather
		/// query statistics.
		/// </summary>
		/// <remarks>
		/// This event is fired upon a query end and can be used to gather
		/// query statistics.
		/// The query object is available from
		/// <see cref="QueryEventArgs">QueryEventArgs</see>
		/// event parameter.<br />
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="QueryEventArgs">QueryEventArgs</seealso>
		event Db4objects.Db4o.Events.QueryEventHandler QueryFinished;

		/// <summary>This event is fired before an object is saved for the first time.</summary>
		/// <remarks>
		/// This event is fired before an object is saved for the first time.
		/// The object can be obtained from
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="CancellableObjectEventArgs.Cancel">CancellableObjectEventArgs.Cancel</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CancellableObjectEventArgs">CancellableObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Creating;

		/// <summary>This event is fired before an object is activated.</summary>
		/// <remarks>
		/// This event is fired before an object is activated.
		/// The object can be obtained from
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="CancellableObjectEventArgs.Cancel">CancellableObjectEventArgs.Cancel</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CancellableObjectEventArgs">CancellableObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Activate">IObjectContainer.Activate</seealso>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Activating;

		/// <summary>This event is fired before an object is updated.</summary>
		/// <remarks>
		/// This event is fired before an object is updated.
		/// The object can be obtained from
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="CancellableObjectEventArgs.Cancel">CancellableObjectEventArgs.Cancel</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CancellableObjectEventArgs">CancellableObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Updating;

		/// <summary>This event is fired before an object is deleted.</summary>
		/// <remarks>
		/// This event is fired before an object is deleted.
		/// The object can be obtained from
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="CancellableObjectEventArgs.Cancel">CancellableObjectEventArgs.Cancel</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CancellableObjectEventArgs">CancellableObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Delete">IObjectContainer.Delete</seealso>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deleting;

		/// <summary>This event is fired before an object is deactivated.</summary>
		/// <remarks>
		/// This event is fired before an object is deactivated.
		/// The object can be obtained from
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// event parameter. The action can be cancelled using
		/// <see cref="CancellableObjectEventArgs.Cancel">CancellableObjectEventArgs.Cancel</see>
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CancellableObjectEventArgs">CancellableObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Deactivate">IObjectContainer.Deactivate</seealso>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deactivating;

		/// <summary>This event is fired after an object is activated.</summary>
		/// <remarks>
		/// This event is fired after an object is activated.
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter. <br /><br />
		/// The event can be used to trigger some post-activation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Activate">IObjectContainer.Activate</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Activated;

		/// <summary>This event is fired after an object is created (saved for the first time).
		/// 	</summary>
		/// <remarks>
		/// This event is fired after an object is created (saved for the first time).
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-creation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Created;

		/// <summary>This event is fired after an object is updated.</summary>
		/// <remarks>
		/// This event is fired after an object is updated.
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-update
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Store">IObjectContainer.Store</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Updated;

		/// <summary>This event is fired after an object is deleted.</summary>
		/// <remarks>
		/// This event is fired after an object is deleted.
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-deletion
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Delete">IObjectContainer.Delete</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Deleted;

		/// <summary>This event is fired after an object is deactivated.</summary>
		/// <remarks>
		/// This event is fired after an object is deactivated.
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-deactivation
		/// functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Delete">IObjectContainer.Delete</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Deactivated;

		/// <summary>This event is fired after a transaction has been committed.</summary>
		/// <remarks>
		/// This event is fired after a transaction has been committed.
		/// The transaction and a list of the modified objects can
		/// be obtained from the
		/// <see cref="CommitEventArgs">CommitEventArgs</see>
		/// event parameter.<br /><br />
		/// Committing event gives a user a chance to interrupt the commit
		/// and rollback the transaction.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CommitEventArgs">CommitEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Commit">IObjectContainer.Commit</seealso>
		event Db4objects.Db4o.Events.CommitEventHandler Committing;

		/// <summary>This event is fired after a transaction has been committed.</summary>
		/// <remarks>
		/// This event is fired after a transaction has been committed.
		/// The transaction and a list of the modified objects can
		/// be obtained from the
		/// <see cref="CommitEventArgs">CommitEventArgs</see>
		/// event parameter.<br /><br />
		/// The event can be used to trigger some post-commit functionality.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="CommitEventArgs">CommitEventArgs</seealso>
		/// <seealso cref="IObjectContainer.Commit">IObjectContainer.Commit</seealso>
		event Db4objects.Db4o.Events.CommitEventHandler Committed;

		/// <summary>This event is fired when a persistent object is instantiated.</summary>
		/// <remarks>
		/// This event is fired when a persistent object is instantiated.
		/// The object can be obtained from the
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// event parameter.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ObjectEventArgs">ObjectEventArgs</seealso>
		event Db4objects.Db4o.Events.ObjectEventHandler Instantiated;

		/// <summary>This event is fired when a new class is registered with metadata.</summary>
		/// <remarks>
		/// This event is fired when a new class is registered with metadata.
		/// The class information can be obtained from
		/// <see cref="ClassEventArgs">ClassEventArgs</see>
		/// event parameter.
		/// </remarks>
		/// <returns>event</returns>
		/// <seealso cref="ClassEventArgs">ClassEventArgs</seealso>
		event Db4objects.Db4o.Events.ClassEventHandler ClassRegistered;

		/// <summary>
		/// This event is fired when the
		/// <see cref="IObjectContainer.Close">IObjectContainer.Close</see>
		/// is
		/// called.
		/// </summary>
		/// <returns>event</returns>
		event Db4objects.Db4o.Events.ObjectContainerEventHandler Closing;
	}
}
