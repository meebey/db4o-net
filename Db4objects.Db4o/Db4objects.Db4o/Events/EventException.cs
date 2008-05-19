/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Events
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// Exception thrown during event dispatching if a client
	/// provided event handler throws.&lt;br/&gt;&lt;br/&gt;
	/// The exception threw by the client can be retrieved by
	/// calling EventException#getCause().
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// Exception thrown during event dispatching if a client
	/// provided event handler throws.&lt;br/&gt;&lt;br/&gt;
	/// The exception threw by the client can be retrieved by
	/// calling EventException#getCause().
	/// </remarks>
	[System.Serializable]
	public class EventException : Db4oException
	{
		public EventException(Exception exc) : base(exc)
		{
		}
	}
}
