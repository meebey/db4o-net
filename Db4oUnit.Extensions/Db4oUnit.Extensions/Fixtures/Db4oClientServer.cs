/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
		protected static readonly string File = "Db4oClientServer.yap";

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

		public Db4oClientServer(IConfigurationSource configSource, string fileName, bool 
			embeddedClient, string label) : base(configSource)
		{
			_file = new Sharpen.IO.File(fileName);
			_embeddedClient = embeddedClient;
			_label = label;
		}

		public Db4oClientServer(IConfigurationSource configSource, bool embeddedClient, string
			 label) : this(configSource, FilePath(), embeddedClient, label)
		{
		}

		/// <exception cref="System.Exception"></exception>
		public override void Open(Type testCaseClass)
		{
			OpenServer();
			IConfiguration config = CloneConfiguration();
			ApplyFixtureConfiguration(testCaseClass, config);
			_objectContainer = _embeddedClient ? OpenEmbeddedClient().Ext() : Db4oFactory.OpenClient
				(config, Host, _port, Username, Password).Ext();
		}

		public virtual IExtObjectContainer OpenNewClient()
		{
			return _embeddedClient ? OpenEmbeddedClient().Ext() : Db4oFactory.OpenClient(CloneConfiguration
				(), Host, _port, Username, Password).Ext();
		}

		/// <exception cref="System.Exception"></exception>
		private void OpenServer()
		{
			_serverConfig = CloneConfiguration();
			_server = Db4oFactory.OpenServer(_serverConfig, _file.GetAbsolutePath(), -1);
			_port = _server.Ext().Port();
			_server.GrantAccess(Username, Password);
		}

		/// <exception cref="System.Exception"></exception>
		public override void Close()
		{
			if (null != _objectContainer)
			{
				_objectContainer.Close();
				_objectContainer = null;
			}
			CloseServer();
		}

		/// <exception cref="System.Exception"></exception>
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
			return _server.OpenClient(CloneConfiguration());
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
