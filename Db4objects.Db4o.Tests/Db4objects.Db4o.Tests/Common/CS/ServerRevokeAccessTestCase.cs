/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerRevokeAccessTestCase : Db4oClientServerTestCase
	{
		private static readonly string SERVER_HOSTNAME = "127.0.0.1";

		public static void Main(string[] args)
		{
			new ServerRevokeAccessTestCase().RunAll();
		}

		#if !CF_1_0 && !CF_2_0
		public virtual void Test()
		{
			string user = "hohohi";
			string password = "hohoho";
			IObjectServer server = ClientServerFixture().Server();
			server.GrantAccess(user, password);
			IObjectContainer con = Db4oFactory.OpenClient(SERVER_HOSTNAME, ClientServerFixture
				().ServerPort(), user, password);
			Assert.IsNotNull(con);
			con.Close();
			server.Ext().RevokeAccess(user);
			Assert.Expect(typeof(Exception), new _ICodeBlock_36(this, user, password));
		}
		#endif // !CF_1_0 && !CF_2_0

		private sealed class _ICodeBlock_36 : ICodeBlock
		{
			public _ICodeBlock_36(ServerRevokeAccessTestCase _enclosing, string user, string 
				password)
			{
				this._enclosing = _enclosing;
				this.user = user;
				this.password = password;
			}

			public void Run()
			{
				Db4oFactory.OpenClient(ServerRevokeAccessTestCase.SERVER_HOSTNAME, this._enclosing
					.ClientServerFixture().ServerPort(), user, password);
			}

			private readonly ServerRevokeAccessTestCase _enclosing;

			private readonly string user;

			private readonly string password;
		}
	}
}
