using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Events;

namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// Provides an interface for getting an
	/// <see cref="IEventRegistry">IEventRegistry</see>
	/// from an
	/// <see cref="IObjectContainer">IObjectContainer</see>
	/// .
	/// </summary>
	public class EventRegistryFactory
	{
		/// <summary>
		/// Returns an
		/// <see cref="IEventRegistry">IEventRegistry</see>
		/// for registering events with the specified container.
		/// </summary>
		public static IEventRegistry ForObjectContainer(IObjectContainer objectContainer)
		{
			if (null == objectContainer)
			{
				throw new ArgumentNullException();
			}
			ObjectContainerBase container = ((ObjectContainerBase)objectContainer);
			ICallbacks callbacks = container.Callbacks();
			if (callbacks is IEventRegistry)
			{
				return (IEventRegistry)callbacks;
			}
			if (callbacks is NullCallbacks)
			{
				EventRegistryImpl impl = new EventRegistryImpl(container);
				container.Callbacks(impl);
				return impl;
			}
			throw new ArgumentException();
		}
	}
}
