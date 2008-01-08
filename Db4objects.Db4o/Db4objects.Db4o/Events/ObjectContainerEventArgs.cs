/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;

namespace Db4objects.Db4o.Events
{
	/// <summary>Arguments for container related events.</summary>
	/// <remarks>Arguments for container related events.</remarks>
	/// <seealso cref="IEventRegistry">IEventRegistry</seealso>
	public class ObjectContainerEventArgs : EventArgs
	{
		private readonly IObjectContainer _container;

		public ObjectContainerEventArgs(IObjectContainer container)
		{
			_container = container;
		}

		public virtual IObjectContainer ObjectContainer
		{
			get
			{
				return _container;
			}
		}
	}
}
