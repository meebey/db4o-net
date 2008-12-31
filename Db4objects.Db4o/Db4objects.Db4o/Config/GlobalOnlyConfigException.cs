/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a global configuration
	/// setting is attempted on an open object container.
	/// </summary>
	/// <remarks>
	/// db4o-specific exception.<br /><br />
	/// This exception is thrown when a global configuration
	/// setting is attempted on an open object container.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.BlockSize">Db4objects.Db4o.Config.IConfiguration.BlockSize
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt">Db4objects.Db4o.Config.IConfiguration.Encrypt
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Io">Db4objects.Db4o.Config.IConfiguration.Io
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
	/// 	</seealso>
	[System.Serializable]
	public class GlobalOnlyConfigException : Db4oException
	{
	}
}
