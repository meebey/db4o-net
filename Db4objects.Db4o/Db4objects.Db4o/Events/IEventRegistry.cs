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

	/// <summary>
	/// Provides a way to register event handlers for specific
	/// <see cref="ObjectContainer">ObjectContainer</see>
	/// events.
	/// </summary>
	/// <seealso cref="EventRegistryFactory">EventRegistryFactory</seealso>
	public interface IEventRegistry
	{
		/// <summary>
		/// Receives
		/// <see cref="QueryEventArgs">QueryEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.QueryEventHandler QueryStarted;

		/// <summary>
		/// Receives
		/// <see cref="QueryEventArgs">QueryEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.QueryEventHandler QueryFinished;

		/// <summary>
		/// Receives
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Creating;

		/// <summary>
		/// Receives
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Activating;

		/// <summary>
		/// Receives
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Updating;

		/// <summary>
		/// Receives
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deleting;

		/// <summary>
		/// Receives
		/// <see cref="CancellableObjectEventArgs">CancellableObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deactivating;

		/// <summary>
		/// Receives
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Activated;

		/// <summary>
		/// Receives
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Created;

		/// <summary>
		/// Receives
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Updated;

		/// <summary>
		/// Receives
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Deleted;

		/// <summary>
		/// Receives
		/// <see cref="ObjectEventArgs">ObjectEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Deactivated;

		/// <summary>
		/// Receives
		/// <see cref="CommitEventArgs">CommitEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CommitEventHandler Committing;

		/// <summary>
		/// Receives
		/// <see cref="CommitEventArgs">CommitEventArgs</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CommitEventHandler Committed;
	}
}
