/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientDisconnectTestCase : Db4oClientServerTestCase, IOptOutAllButNetworkingCS
	{
		public static void Main(string[] arguments)
		{
			new ClientDisconnectTestCase().RunClientServer();
		}

		public virtual void TestDisconnect()
		{
			IExtObjectContainer oc1 = OpenNewClient();
			IExtObjectContainer oc2 = OpenNewClient();
			try
			{
				ClientObjectContainer client1 = (ClientObjectContainer)oc1;
				ClientObjectContainer client2 = (ClientObjectContainer)oc2;
				client1.Socket().Close();
				Assert.IsFalse(oc1.IsClosed());
				Assert.Expect(typeof(Db4oException), new _ICodeBlock_27(this, client1));
				// It's ok for client2 to get something.
				client2.QueryByExample(null);
			}
			finally
			{
				oc1.Close();
				oc2.Close();
				Assert.IsTrue(oc1.IsClosed());
				Assert.IsTrue(oc2.IsClosed());
			}
		}

		private sealed class _ICodeBlock_27 : ICodeBlock
		{
			public _ICodeBlock_27(ClientDisconnectTestCase _enclosing, ClientObjectContainer 
				client1)
			{
				this._enclosing = _enclosing;
				this.client1 = client1;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				client1.QueryByExample(null);
			}

			private readonly ClientDisconnectTestCase _enclosing;

			private readonly ClientObjectContainer client1;
		}
	}
}
