using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;

namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractClientServerDb4oFixture : AbstractDb4oFixture
	{
		protected static readonly string FILE = "Db4oClientServer.yap";

		protected static readonly string HOST = "localhost";

		protected const int PORT = unchecked((int)(0xdb40));

		protected static readonly string USERNAME = "db4o";

		protected static readonly string PASSWORD = USERNAME;

		private IObjectServer _server;

		private readonly Sharpen.IO.File _yap;

		protected readonly int _port;

		public AbstractClientServerDb4oFixture(IConfigurationSource configSource, string 
			fileName, int port) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
			_port = port;
		}

		public AbstractClientServerDb4oFixture(IConfigurationSource configSource) : this(
			configSource, FILE, PORT)
		{
		}

		public override void Close()
		{
			if (null != _server)
			{
				_server.Close();
				_server = null;
			}
		}

		public override void Open()
		{
			_server = Db4oFactory.OpenServer(Config(), _yap.GetAbsolutePath(), _port);
			_server.GrantAccess(USERNAME, PASSWORD);
		}

		public abstract override IExtObjectContainer Db();

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
			if ((typeof(IOptOutCS).IsAssignableFrom(clazz)) || !typeof(IDb4oTestCase).IsAssignableFrom
				(clazz))
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

		protected virtual IObjectContainer OpenEmbeddedClient()
		{
			return _server.OpenClient(Config());
		}
	}
}
