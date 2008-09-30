/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Internal.CS.Config
{
	/// <exclude></exclude>
	public class ClientServerFactoryImpl : IClientServerFactory
	{
		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="OldFormatException"></exception>
		/// <exception cref="InvalidPasswordException"></exception>
		public virtual IObjectContainer OpenClient(IConfiguration config, string hostName
			, int port, string user, string password, INativeSocketFactory socketFactory)
		{
			if (user == null || password == null)
			{
				throw new InvalidPasswordException();
			}
			NetworkSocket networkSocket = new NetworkSocket(socketFactory, hostName, port);
			return new ClientObjectContainer(config, networkSocket, user, password, true);
		}

		/// <exception cref="Db4oIOException"></exception>
		/// <exception cref="IncompatibleFileFormatException"></exception>
		/// <exception cref="OldFormatException"></exception>
		/// <exception cref="DatabaseFileLockedException"></exception>
		/// <exception cref="DatabaseReadOnlyException"></exception>
		public virtual IObjectServer OpenServer(IConfiguration config, string databaseFileName
			, int port, INativeSocketFactory socketFactory)
		{
			LocalObjectContainer container = (LocalObjectContainer)Db4oEmbedded.OpenFile(config
				, databaseFileName);
			if (container == null)
			{
				return null;
			}
			lock (container.Lock())
			{
				return new ObjectServerImpl(container, port, socketFactory);
			}
		}
	}
}
