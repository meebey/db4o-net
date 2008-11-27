/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Events
{
	internal class EventInfo
	{
		public EventInfo(string eventFirerName, IProcedure4 eventListenerSetter)
		{
			_listenerSetter = eventListenerSetter;
			_eventFirerName = eventFirerName;
		}

		public virtual IProcedure4 ListenerSetter()
		{
			return _listenerSetter;
		}

		public virtual string EventFirerName()
		{
			return _eventFirerName;
		}

		private readonly IProcedure4 _listenerSetter;

		private readonly string _eventFirerName;
	}
}
