namespace Db4oUnit.Extensions.Fixtures
{
	public abstract class AbstractClientServerDb4oFixture : Db4oUnit.Extensions.Fixtures.AbstractDb4oFixture
	{
		protected static readonly string FILE = "Db4oClientServer.yap";

		protected static readonly string HOST = "localhost";

		protected const int PORT = unchecked((int)(0xdb40));

		protected static readonly string USERNAME = "db4o";

		protected static readonly string PASSWORD = USERNAME;

		private Db4objects.Db4o.IObjectServer _server;

		private readonly Sharpen.IO.File _yap;

		protected readonly int _port;

		public AbstractClientServerDb4oFixture(Db4oUnit.Extensions.Fixtures.IConfigurationSource
			 configSource, string fileName, int port) : base(configSource)
		{
			_yap = new Sharpen.IO.File(fileName);
			_port = port;
		}

		public AbstractClientServerDb4oFixture(Db4oUnit.Extensions.Fixtures.IConfigurationSource
			 configSource) : this(configSource, FILE, PORT)
		{
		}

		public override void Close()
		{
			_server.Close();
		}

		public override void Open()
		{
			_server = Db4objects.Db4o.Db4oFactory.OpenServer(Config(), _yap.GetAbsolutePath()
				, _port);
			_server.GrantAccess(USERNAME, PASSWORD);
		}

		public abstract override Db4objects.Db4o.Ext.IExtObjectContainer Db();

		protected override void DoClean()
		{
			_yap.Delete();
		}

		public virtual Db4objects.Db4o.IObjectServer Server()
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
		public override bool Accept(System.Type clazz)
		{
			if ((typeof(Db4oUnit.Extensions.Fixtures.IOptOutCS).IsAssignableFrom(clazz)) || !
				typeof(Db4oUnit.Extensions.IDb4oTestCase).IsAssignableFrom(clazz))
			{
				return false;
			}
			return true;
		}

		public override Db4objects.Db4o.Internal.LocalObjectContainer FileSession()
		{
			return (Db4objects.Db4o.Internal.LocalObjectContainer)_server.Ext().ObjectContainer
				();
		}

		public override void Defragment()
		{
			Defragment(FILE);
		}

		protected virtual Db4objects.Db4o.IObjectContainer OpenEmbeddedClient()
		{
			return _server.OpenClient(Config());
		}
	}
}
