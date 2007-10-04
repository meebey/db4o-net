/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Util;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public class Db4oClientServer : AbstractDb4oFixture, IDb4oClientServerFixture
	{
		protected static readonly string FILE = "Db4oClientServer.yap";

		protected static readonly string HOST = "localhost";

		protected static readonly string USERNAME = "db4o";

		protected static readonly string PASSWORD = USERNAME;

		private IObjectServer _server;

		private readonly Sharpen.IO.File _yap;

		private bool _embeddedClient;

		private IExtObjectContainer _objectContainer;

		private string _label;

		private int _port;

		private IConfiguration _serverConfig;

		public Db4oClientServer(IConfigurationSource configSource, string fileName, bool 
			embeddedClient, string label) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
			_embeddedClient = embeddedClient;
			_label = label;
		}

		public Db4oClientServer(IConfigurationSource configSource, bool embeddedClient, string
			 label) : this(configSource, FilePath(), embeddedClient, label)
		{
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
			_serverConfig = CloneDb4oConfiguration(Config());
			_server = Db4oFactory.OpenServer(_serverConfig, _yap.GetAbsolutePath(), -1);
			_port = _server.Ext().Port();
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
			if (!typeof(AbstractDb4oTestCase).IsAssignableFrom(clazz))
			{
				return false;
			}
			if (typeof(IOptOutCS).IsAssignableFrom(clazz))
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

		public override void Defragment()
		{
			Defragment(FilePath());
		}

		private IObjectContainer OpenEmbeddedClient()
		{
			return _server.OpenClient(Config());
		}

		private Config4Impl CloneDb4oConfiguration(IConfiguration config)
		{
			return (Config4Impl)((Config4Impl)config).DeepClone(this);
		}

		public override string GetLabel()
		{
			return _label;
		}

		public virtual int ServerPort()
		{
			return _port;
		}

		private static string FilePath()
		{
			return CrossPlatformServices.DatabasePath(FILE);
		}

		public override void ConfigureAtRuntime(IRuntimeConfigureAction action)
		{
			action.Apply(Config());
			action.Apply(_serverConfig);
		}
	}
}
