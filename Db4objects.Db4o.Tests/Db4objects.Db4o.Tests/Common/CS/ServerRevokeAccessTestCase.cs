/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ServerRevokeAccessTestCase : Db4oClientServerTestCase, IOptOutAllButNetworkingCS
	{
		private static readonly string ServerHostname = "127.0.0.1";

		public static void Main(string[] args)
		{
			new ServerRevokeAccessTestCase().RunAll();
		}

		#if !CF
		/// <exception cref="System.IO.IOException"></exception>
		public virtual void Test()
		{
			string user = "hohohi";
			string password = "hohoho";
			IObjectServer server = ClientServerFixture().Server();
			server.GrantAccess(user, password);
			IObjectContainer con = Db4oFactory.OpenClient(Config(), ServerHostname, ClientServerFixture
				().ServerPort(), user, password);
			Assert.IsNotNull(con);
			con.Close();
			server.Ext().RevokeAccess(user);
			Assert.Expect(typeof(Exception), new _ICodeBlock_40(this, user, password));
		}
		#endif // !CF

		private sealed class _ICodeBlock_40 : ICodeBlock
		{
			public _ICodeBlock_40(ServerRevokeAccessTestCase _enclosing, string user, string 
				password)
			{
				this._enclosing = _enclosing;
				this.user = user;
				this.password = password;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				Db4oFactory.OpenClient(this._enclosing.Config(), ServerRevokeAccessTestCase.ServerHostname
					, this._enclosing.ClientServerFixture().ServerPort(), user, password);
			}

			private readonly ServerRevokeAccessTestCase _enclosing;

			private readonly string user;

			private readonly string password;
		}

		private IConfiguration Config()
		{
			return ClientServerFixture().Config();
		}
	}
}
