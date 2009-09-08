/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.CS.Config;

namespace Db4objects.Db4o.CS.Config
{
	/// <summary>factory to open C/S server and client implementations.</summary>
	/// <remarks>factory to open C/S server and client implementations.</remarks>
	/// <seealso cref="Db4objects.Db4o.Db4oFactory.OpenClient(Db4objects.Db4o.Config.IConfiguration, string, int, string, string)
	/// 	">Db4objects.Db4o.Db4oFactory.OpenClient(Db4objects.Db4o.Config.IConfiguration, string, int, string, string)
	/// 	</seealso>
	/// <seealso cref="Db4objects.Db4o.Db4oFactory.OpenServer(Db4objects.Db4o.Config.IConfiguration, string, int)
	/// 	"></seealso>
	public interface IClientServerFactory
	{
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.InvalidPasswordException"></exception>
		IObjectContainer OpenClient(IClientConfiguration config, string hostName, int port
			, string user, string password);

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.IncompatibleFileFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseFileLockedException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseReadOnlyException"></exception>
		IObjectServer OpenServer(IServerConfiguration config, string databaseFileName, int
			 port);
	}
}
