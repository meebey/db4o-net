namespace Db4objects.Db4o.Events
{
	public delegate void QueryEventHandler(object sender, QueryEventArgs args);

	public delegate void CancellableObjectEventHandler(object sender, CancellableObjectEventArgs
		 args);

	public delegate void ObjectEventHandler(object sender, ObjectEventArgs args);

	/// <summary>
	/// Provides a way to register event handlers for specific
	/// <see cref="ObjectContainer">ObjectContainer</see>
	/// events.
	/// </summary>
	/// <seealso cref="Db4objects.Db4o.Events.EventRegistryFactory">Db4objects.Db4o.Events.EventRegistryFactory
	/// 	</seealso>
	public interface IEventRegistry
	{
		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event QueryEventHandler QueryStarted;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event QueryEventHandler QueryFinished;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event CancellableObjectEventHandler Creating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event CancellableObjectEventHandler Activating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event CancellableObjectEventHandler Updating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event CancellableObjectEventHandler Deleting;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event CancellableObjectEventHandler Deactivating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event ObjectEventHandler Activated;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event ObjectEventHandler Created;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event ObjectEventHandler Updated;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event ObjectEventHandler Deleted;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event ObjectEventHandler Deactivated;
	}
}
