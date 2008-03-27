/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the database file reaches the
	/// maximum allowed size.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when the database file reaches the
	/// maximum allowed size. Upon throwing the exception the database is
	/// switched to the read-only mode. <br />
	/// The maximum database size is configurable
	/// and can reach up to 254GB.
	/// </remarks>
	/// <seealso cref="IConfiguration.BlockSize">IConfiguration.BlockSize</seealso>
	[System.Serializable]
	public class DatabaseMaximumSizeReachedException : Db4oException
	{
	}
}
