/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerRevokeAccessTestCase : ITestCase
	{
		internal static readonly string FILE = "ServerRevokeAccessTest.yap";

		internal const int SERVER_PORT = unchecked((int)(0xdb42));

		internal static readonly string SERVER_HOSTNAME = "localhost";

		#if !CF_1_0 && !CF_2_0
		public virtual void Test()
		{
			File4.Delete(FILE);
			IObjectServer server = Db4oFactory.OpenServer(FILE, SERVER_PORT);
			try
			{
				string user = "hohohi";
				string password = "hohoho";
				server.GrantAccess(user, password);
				IObjectContainer con = Db4oFactory.OpenClient(SERVER_HOSTNAME, SERVER_PORT, user, 
					password);
				Assert.IsNotNull(con);
				con.Close();
				server.Ext().RevokeAccess(user);
				Assert.Expect(typeof(Exception), new _ICodeBlock_37(this, user, password));
			}
			finally
			{
				server.Close();
			}
		}
		#endif // !CF_1_0 && !CF_2_0

		private sealed class _ICodeBlock_37 : ICodeBlock
		{
			public _ICodeBlock_37(ServerRevokeAccessTestCase _enclosing, string user, string 
				password)
			{
				this._enclosing = _enclosing;
				this.user = user;
				this.password = password;
			}

			public void Run()
			{
				Db4oFactory.OpenClient(ServerRevokeAccessTestCase.SERVER_HOSTNAME, ServerRevokeAccessTestCase
					.SERVER_PORT, user, password);
			}

			private readonly ServerRevokeAccessTestCase _enclosing;

			private readonly string user;

			private readonly string password;
		}
	}
}
