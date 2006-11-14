namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ServerRevokeAccessTestCase : Db4oUnit.ITestCase
	{
		internal static readonly string FILE = "ServerRevokeAccessTest.yap";

		internal const int SERVER_PORT = unchecked((int)(0xdb42));

		internal static readonly string SERVER_HOSTNAME = "localhost";

		#if !CF_1_0 && !CF_2_0
		public virtual void Test()
		{
			Db4objects.Db4o.Foundation.IO.File4.Delete(FILE);
			Db4objects.Db4o.IObjectServer server = Db4objects.Db4o.Db4oFactory.OpenServer(FILE
				, SERVER_PORT);
			try
			{
				string user = "hohohi";
				string password = "hohoho";
				server.GrantAccess(user, password);
				Db4objects.Db4o.IObjectContainer con = Db4objects.Db4o.Db4oFactory.OpenClient(SERVER_HOSTNAME
					, SERVER_PORT, user, password);
				Db4oUnit.Assert.IsNotNull(con);
				con.Close();
				server.Ext().RevokeAccess(user);
				Db4oUnit.Assert.Expect(typeof(System.Exception), new _AnonymousInnerClass37(this, 
					user, password));
			}
			finally
			{
				server.Close();
			}
		}
		#endif // !CF_1_0 && !CF_2_0

		private sealed class _AnonymousInnerClass37 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass37(ServerRevokeAccessTestCase _enclosing, string user, 
				string password)
			{
				this._enclosing = _enclosing;
				this.user = user;
				this.password = password;
			}

			public void Run()
			{
				Db4objects.Db4o.Db4oFactory.OpenClient(Db4objects.Db4o.Tests.Common.Assorted.ServerRevokeAccessTestCase
					.SERVER_HOSTNAME, Db4objects.Db4o.Tests.Common.Assorted.ServerRevokeAccessTestCase
					.SERVER_PORT, user, password);
			}

			private readonly ServerRevokeAccessTestCase _enclosing;

			private readonly string user;

			private readonly string password;
		}
	}
}
