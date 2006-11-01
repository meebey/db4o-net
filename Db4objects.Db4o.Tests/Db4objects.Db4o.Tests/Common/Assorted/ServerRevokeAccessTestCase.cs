namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ServerRevokeAccessTestCase : Db4oUnit.ITestCase
	{
		internal static readonly string FILE = "ServerRevokeAccessTest.yap";

		internal const int SERVER_PORT = unchecked((int)(0xdb42));

		internal static readonly string SERVER_HOSTNAME = "localhost";

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Util.File4.Delete(FILE);
			Db4objects.Db4o.IObjectServer server = Db4objects.Db4o.Db4o.OpenServer(FILE, SERVER_PORT
				);
			try
			{
				string user = "hohohi";
				string password = "hohoho";
				server.GrantAccess(user, password);
				Db4objects.Db4o.IObjectContainer con = Db4objects.Db4o.Db4o.OpenClient(SERVER_HOSTNAME
					, SERVER_PORT, user, password);
				Db4oUnit.Assert.IsNotNull(con);
				con.Close();
				server.Ext().RevokeAccess(user);
				Db4oUnit.Assert.Expect(typeof(System.Exception), new _AnonymousInnerClass34(this, 
					user, password));
			}
			finally
			{
				server.Close();
			}
		}

		private sealed class _AnonymousInnerClass34 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass34(ServerRevokeAccessTestCase _enclosing, string user, 
				string password)
			{
				this._enclosing = _enclosing;
				this.user = user;
				this.password = password;
			}

			public void Run()
			{
				Db4objects.Db4o.Db4o.OpenClient(Db4objects.Db4o.Tests.Common.Assorted.ServerRevokeAccessTestCase
					.SERVER_HOSTNAME, Db4objects.Db4o.Tests.Common.Assorted.ServerRevokeAccessTestCase
					.SERVER_PORT, user, password);
			}

			private readonly ServerRevokeAccessTestCase _enclosing;

			private readonly string user;

			private readonly string password;
		}
	}
}
