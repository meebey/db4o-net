/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class ListenerRegistry
	{
		public static ListenerRegistry NewInstance()
		{
			return new ListenerRegistry();
		}

		private Collection4 _listeners;

		public virtual void Register(IListener listener)
		{
			if (_listeners == null)
			{
				_listeners = new Collection4();
			}
			_listeners.Add(listener);
		}

		public virtual void NotifyListeners(object @event)
		{
			if (_listeners == null)
			{
				return;
			}
			IEnumerator i = _listeners.GetEnumerator();
			while (i.MoveNext())
			{
				((IListener)i.Current).OnEvent(@event);
			}
		}
	}
}
