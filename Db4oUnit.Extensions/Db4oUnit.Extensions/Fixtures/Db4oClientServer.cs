/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Threading;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oClientServer : AbstractDb4oFixture, IDb4oClientServerFixture
	{
		private const int ThreadpoolTimeout = 3000;

		protected static readonly string File = "Db4oClientServer.db4o";

		public static readonly string Host = "127.0.0.1";

		public static readonly string Username = "db4o";

		public static readonly string Password = Username;

		private IObjectServer _server;

		private readonly Sharpen.IO.File _file;

		private bool _embeddedClient;

		private IExtObjectContainer _objectContainer;

		private string _label;

		private int _port;

		private IConfiguration _serverConfig;

		private readonly IClientServerFactory _csFactory;

		public Db4oClientServer(IClientServerFactory csFactory, bool embeddedClient, string
			 label)
		{
			_csFactory = csFactory != null ? csFactory : DefaultClientServerFactory();
			_file = new Sharpen.IO.File(FilePath());
			_embeddedClient = embeddedClient;
			_label = label;
		}

		private IClientServerFactory DefaultClientServerFactory()
		{
			return ((Config4Impl)Config()).ClientServerFactory();
		}

		public Db4oClientServer(bool embeddedClient, string label) : this(null, embeddedClient
			, label)
		{
		}

		/// <exception cref="System.Exception"></exception>
		public override void Open(IDb4oTestCase testInstance)
		{
			OpenServerFor(testInstance);
			OpenClientFor(testInstance);
			ListenToUncaughtExceptions();
		}

		private void ListenToUncaughtExceptions()
		{
			ListenToUncaughtExceptions(ServerThreadPool());
			IThreadPool4 clientThreadPool = ClientThreadPool();
			if (null != clientThreadPool)
			{
				ListenToUncaughtExceptions(clientThreadPool);
			}
		}

		private IThreadPool4 ClientThreadPool()
		{
			return ThreadPoolFor(_objectContainer);
		}

		private IThreadPool4 ServerThreadPool()
		{
			return ThreadPoolFor(_server.Ext().ObjectContainer());
		}

		/// <exception cref="System.Exception"></exception>
		private void OpenClientFor(IDb4oTestCase testInstance)
		{
			IConfiguration config = ClientConfigFor(testInstance);
			_objectContainer = OpenClientWith(config);
		}

		/// <exception cref="System.Exception"></exception>
		private IConfiguration ClientConfigFor(IDb4oTestCase testInstance)
		{
			if (testInstance is ICustomClientServerConfiguration)
			{
				IConfiguration customServerConfig = NewConfiguration();
				((ICustomClientServerConfiguration)testInstance).ConfigureClient(customServerConfig
					);
				return customServerConfig;
			}
			IConfiguration config = CloneConfiguration();
			ApplyFixtureConfiguration(testInstance, config);
			return config;
		}

		private IExtObjectContainer OpenSocketClient(IConfiguration config)
		{
			return _csFactory.OpenClient(config, Host, _port, Username, Password, new PlainSocketFactory
				()).Ext();
		}

		public virtual IExtObjectContainer OpenNewClient()
		{
			return OpenClientWith(CloneConfiguration());
		}

		private IExtObjectContainer OpenClientWith(IConfiguration config)
		{
			return _embeddedClient ? OpenEmbeddedClient().Ext() : OpenSocketClient(config);
		}

		/// <exception cref="System.Exception"></exception>
		private void OpenServerFor(IDb4oTestCase testInstance)
		{
			_serverConfig = ServerConfigFor(testInstance);
			_server = _csFactory.OpenServer(_serverConfig, _file.GetAbsolutePath(), -1, new PlainSocketFactory
				());
			_port = _server.Ext().Port();
			_server.GrantAccess(Username, Password);
		}

		/// <exception cref="System.Exception"></exception>
		private IConfiguration ServerConfigFor(IDb4oTestCase testInstance)
		{
			if (testInstance is ICustomClientServerConfiguration)
			{
				IConfiguration customServerConfig = NewConfiguration();
				((ICustomClientServerConfiguration)testInstance).ConfigureServer(customServerConfig
					);
				return customServerConfig;
			}
			return CloneConfiguration();
		}

		/// <exception cref="System.Exception"></exception>
		public override void Close()
		{
			if (null != _objectContainer)
			{
				IThreadPool4 clientThreadPool = ClientThreadPool();
				_objectContainer.Close();
				_objectContainer = null;
				if (null != clientThreadPool)
				{
					clientThreadPool.Join(ThreadpoolTimeout);
				}
			}
			CloseServer();
		}

		/// <exception cref="System.Exception"></exception>
		private void CloseServer()
		{
			if (null != _server)
			{
				IThreadPool4 serverThreadPool = ServerThreadPool();
				_server.Close();
				_server = null;
				if (null != serverThreadPool)
				{
					serverThreadPool.Join(ThreadpoolTimeout);
				}
			}
		}

		public override IExtObjectContainer Db()
		{
			return _objectContainer;
		}

		protected override void DoClean()
		{
			_file.Delete();
		}

		public virtual IObjectServer Server()
		{
			return _server;
		}

		public virtual bool EmbeddedClients()
		{
			return _embeddedClient;
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
			if (!typeof(IDb4oTestCase).IsAssignableFrom(clazz))
			{
				return false;
			}
			if (typeof(IOptOutCS).IsAssignableFrom(clazz))
			{
				return false;
			}
			if (!_embeddedClient && (typeof(IOptOutNetworkingCS).IsAssignableFrom(clazz)))
			{
				return false;
			}
			if (_embeddedClient && (typeof(IOptOutAllButNetworkingCS).IsAssignableFrom(clazz)
				))
			{
				return false;
			}
			return true;
		}

		public override LocalObjectContainer FileSession()
		{
			return (LocalObjectContainer)_server.Ext().ObjectContainer();
		}

		/// <exception cref="System.Exception"></exception>
		public override void Defragment()
		{
			Defragment(FilePath());
		}

		private IObjectContainer OpenEmbeddedClient()
		{
			return _server.OpenClient();
		}

		public override string Label()
		{
			return BuildLabel(_label);
		}

		public virtual int ServerPort()
		{
			return _port;
		}

		private static string FilePath()
		{
			return CrossPlatformServices.DatabasePath(File);
		}

		public override void ConfigureAtRuntime(IRuntimeConfigureAction action)
		{
			action.Apply(Config());
			action.Apply(_serverConfig);
		}
	}
}
