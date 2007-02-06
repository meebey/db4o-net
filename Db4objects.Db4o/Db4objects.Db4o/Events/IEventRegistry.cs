namespace Db4objects.Db4o.Events
{
	public delegate void QueryEventHandler(object sender, Db4objects.Db4o.Events.QueryEventArgs
		 args);

	public delegate void CancellableObjectEventHandler(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
		 args);

	public delegate void ObjectEventHandler(object sender, Db4objects.Db4o.Events.ObjectEventArgs
		 args);

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
		event Db4objects.Db4o.Events.QueryEventHandler QueryStarted;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.QueryEventArgs">Db4objects.Db4o.Events.QueryEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.QueryEventHandler QueryFinished;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Creating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Activating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Updating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deleting;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.CancellableObjectEventArgs">Db4objects.Db4o.Events.CancellableObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.CancellableObjectEventHandler Deactivating;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Activated;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// .
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Created;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Updated;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Deleted;

		/// <summary>
		/// Receives
		/// <see cref="Db4objects.Db4o.Events.ObjectEventArgs">Db4objects.Db4o.Events.ObjectEventArgs
		/// 	</see>
		/// </summary>
		/// <returns></returns>
		event Db4objects.Db4o.Events.ObjectEventHandler Deactivated;
	}
}
