/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oClientServer : AbstractDb4oFixture, IDb4oClientServerFixture
	{
		private const int DEFAULT_PORT = unchecked((int)(0xdb40));

		protected static readonly string FILE = "Db4oClientServer.yap";

		protected static readonly string HOST = "localhost";

		protected static readonly string USERNAME = "db4o";

		protected static readonly string PASSWORD = USERNAME;

		private IObjectServer _server;

		private readonly Sharpen.IO.File _yap;

		private bool _embeddedClient;

		private IExtObjectContainer _objectContainer;

		protected static readonly int _port = FindFreePort();

		public Db4oClientServer(IConfigurationSource configSource, string fileName, bool 
			embeddedClient) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
			_embeddedClient = embeddedClient;
		}

		public Db4oClientServer(IConfigurationSource configSource, bool embeddedClient) : 
			this(configSource, FILE, embeddedClient)
		{
		}

		private static int FindFreePort()
		{
			try
			{
				return FindFreePort(DEFAULT_PORT);
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			try
			{
				return FindFreePort(0);
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			throw new InvalidOperationException("Could not allocate a usable port");
		}

		private static int FindFreePort(int port)
		{
			ServerSocket4 server = new ServerSocket4(port);
			port = server.GetLocalPort();
			server.Close();
			Cool.SleepIgnoringInterruption(3);
			return port;
		}

		public override void Open()
		{
			OpenServer();
			_objectContainer = _embeddedClient ? OpenEmbeddedClient().Ext() : Db4oFactory.OpenClient
				(Config(), HOST, _port, USERNAME, PASSWORD).Ext();
		}

		public virtual IExtObjectContainer OpenNewClient()
		{
			return _embeddedClient ? OpenEmbeddedClient().Ext() : Db4oFactory.OpenClient(CloneDb4oConfiguration
				((Config4Impl)Config()), HOST, _port, USERNAME, PASSWORD).Ext();
		}

		private void OpenServer()
		{
			_server = Db4oFactory.OpenServer(Config(), _yap.GetAbsolutePath(), _port);
			_server.GrantAccess(USERNAME, PASSWORD);
		}

		public override void Close()
		{
			if (null != _objectContainer)
			{
				_objectContainer.Close();
				_objectContainer = null;
			}
			CloseServer();
		}

		private void CloseServer()
		{
			if (null != _server)
			{
				_server.Close();
				_server = null;
			}
		}

		public override IExtObjectContainer Db()
		{
			return _objectContainer;
		}

		protected override void DoClean()
		{
			_yap.Delete();
		}

		public virtual IObjectServer Server()
		{
			return _server;
		}

		/// <summary>
		/// Does not accept a clazz which is assignable from OptOutCS, or not
		/// assignable from Db4oTestCase.
		/// </summary>
		/// <remarks>
		/// Does not accept a clazz which is assignable from OptOutCS, or not
		/// assignable from Db4oTestCase.
		/// </remarks>
		/// <returns>
		/// returns false if the clazz is assignable from OptOutCS, or not
		/// assignable from Db4oTestCase. Otherwise, returns true.
		/// </returns>
		public override bool Accept(Type clazz)
		{
			if ((typeof(IOptOutCS).IsAssignableFrom(clazz)) || !typeof(AbstractDb4oTestCase).
				IsAssignableFrom(clazz))
			{
				return false;
			}
			return true;
		}

		public override LocalObjectContainer FileSession()
		{
			return (LocalObjectContainer)_server.Ext().ObjectContainer();
		}

		public override void Defragment()
		{
			Defragment(FILE);
		}

		private IObjectContainer OpenEmbeddedClient()
		{
			return _server.OpenClient(Config());
		}

		private Config4Impl CloneDb4oConfiguration(Config4Impl config)
		{
			return (Config4Impl)config.DeepClone(this);
		}

		public override string GetLabel()
		{
			return "C/S" + (_embeddedClient ? " Embedded" : string.Empty);
		}

		public virtual int ServerPort()
		{
			return _port;
		}
	}
}
