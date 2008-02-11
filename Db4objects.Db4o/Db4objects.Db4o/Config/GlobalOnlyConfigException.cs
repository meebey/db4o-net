/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// db4o-specific exception.&lt;br&gt;&lt;br&gt;
	/// This exception is thrown when a global configuration
	/// setting is attempted on an open object container.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.&lt;br&gt;&lt;br&gt;
	/// This exception is thrown when a global configuration
	/// setting is attempted on an open object container.
	/// </remarks>
	/// <seealso cref="IConfiguration.BlockSize">IConfiguration.BlockSize</seealso>
	/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
	/// <seealso cref="IConfiguration.Io">IConfiguration.Io</seealso>
	/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
	[System.Serializable]
	public class GlobalOnlyConfigException : Db4oException
	{
	}
}
