namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// Provides an interface for getting an
	/// <see cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
	/// 	</see>
	/// from an
	/// <see cref="Db4objects.Db4o.IObjectContainer">Db4objects.Db4o.IObjectContainer</see>
	/// .
	/// </summary>
	public class EventRegistryFactory
	{
		/// <summary>
		/// Returns an
		/// <see cref="Db4objects.Db4o.Events.IEventRegistry">Db4objects.Db4o.Events.IEventRegistry
		/// 	</see>
		/// for registering events with the specified container.
		/// </summary>
		public static Db4objects.Db4o.Events.IEventRegistry ForObjectContainer(Db4objects.Db4o.IObjectContainer
			 container)
		{
			if (null == container)
			{
				throw new System.ArgumentNullException("container");
			}
			Db4objects.Db4o.Internal.ObjectContainerBase stream = ((Db4objects.Db4o.Internal.ObjectContainerBase
				)container);
			Db4objects.Db4o.Internal.Callbacks.ICallbacks callbacks = stream.Callbacks();
			if (callbacks is Db4objects.Db4o.Events.IEventRegistry)
			{
				return (Db4objects.Db4o.Events.IEventRegistry)callbacks;
			}
			if (callbacks is Db4objects.Db4o.Internal.Callbacks.NullCallbacks)
			{
				Db4objects.Db4o.Internal.Events.EventRegistryImpl impl = new Db4objects.Db4o.Internal.Events.EventRegistryImpl
					();
				stream.Callbacks(impl);
				return impl;
			}
			throw new System.ArgumentException("container callbacks already in use");
		}
	}
}
