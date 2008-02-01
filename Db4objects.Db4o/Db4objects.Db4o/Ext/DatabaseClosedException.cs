/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>db4o-specific exception.</summary>
	/// <remarks>
	/// db4o-specific exception. &lt;br&gt;&lt;br&gt;
	/// This exception is thrown when the object container required for
	/// the current operation was closed or failed to open.
	/// </remarks>
	/// <seealso cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</seealso>
	/// <seealso cref="IObjectContainer.Close">IObjectContainer.Close</seealso>
	[System.Serializable]
	public class DatabaseClosedException : Db4oException
	{
	}
}
