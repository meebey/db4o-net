/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Config
{
	/// <summary>factory to open C/S server and client implementations.</summary>
	/// <remarks>factory to open C/S server and client implementations.</remarks>
	/// <seealso cref="Db4oFactory.OpenClient">Db4oFactory.OpenClient</seealso>
	/// <seealso cref="Db4oFactory.OpenServer"></seealso>
	public interface IClientServerFactory
	{
		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="OldFormatException"></exception>
		/// <exception cref="InvalidPasswordException"></exception>
		IObjectContainer OpenClient(IConfiguration config, string hostName, int port, string
			 user, string password, INativeSocketFactory socketFactory);

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="IncompatibleFileFormatException"></exception>
		/// <exception cref="OldFormatException"></exception>
		/// <exception cref="DatabaseFileLockedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		IObjectServer OpenServer(IConfiguration config, string databaseFileName, int port
			, INativeSocketFactory socketFactory);
	}
}
