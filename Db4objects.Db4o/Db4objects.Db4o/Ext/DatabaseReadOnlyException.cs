/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a write operation is attempted
	/// on a database in a read-only mode.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a write operation is attempted
	/// on a database in a read-only mode.
	/// </remarks>
	/// <seealso cref="IConfiguration.ReadOnly"></seealso>
	[System.Serializable]
	public class DatabaseReadOnlyException : Db4oException
	{
	}
}
