/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Config
{
	/// <summary>factory to open C/S server and client implementations.</summary>
	/// <remarks>factory to open C/S server and client implementations.</remarks>
	/// <seealso cref="Db4oFactory.OpenClient(IConfiguration, string, int, string, string, INativeSocketFactory)
	/// 	">Db4oFactory.OpenClient(IConfiguration, string, int, string, string, INativeSocketFactory)
	/// 	</seealso>
	/// <seealso cref="Db4oFactory.OpenServer(IConfiguration, string, int, INativeSocketFactory)
	/// 	"></seealso>
	public interface IClientServerFactory
	{
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.InvalidPasswordException"></exception>
		IObjectContainer OpenClient(IConfiguration config, string hostName, int port, string
			 user, string password, INativeSocketFactory socketFactory);

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.IncompatibleFileFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseFileLockedException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseReadOnlyException"></exception>
		IObjectServer OpenServer(IConfiguration config, string databaseFileName, int port
			, INativeSocketFactory socketFactory);
	}
}
