/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Tests.Common.CS;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientTimeOutTestCase : Db4oClientServerTestCase, IOptOutAllButNetworkingCS
	{
		private const int Timeout = 500;

		internal static bool _clientWasBlocked;

		internal ClientTimeOutTestCase.TestMessageRecipient recipient = new ClientTimeOutTestCase.TestMessageRecipient
			();

		public static void Main(string[] args)
		{
			new ClientTimeOutTestCase().RunAll();
		}

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().TimeoutClientSocket(Timeout);
		}

		public virtual void TestKeptAliveClient()
		{
			ClientTimeOutTestCase.Item item = new ClientTimeOutTestCase.Item("one");
			Store(item);
			Cool.SleepIgnoringInterruption(Timeout * 2);
			Assert.AreSame(item, RetrieveOnlyInstance(typeof(ClientTimeOutTestCase.Item)));
		}

		public virtual void TestTimedoutAndClosedClient()
		{
			Store(new ClientTimeOutTestCase.Item("one"));
			ClientServerFixture().Server().Ext().Configure().ClientServer().SetMessageRecipient
				(recipient);
			IExtObjectContainer client = ClientServerFixture().Db();
			IMessageSender sender = client.Configure().ClientServer().GetMessageSender();
			_clientWasBlocked = false;
			sender.Send(new ClientTimeOutTestCase.Data());
			long start = Runtime.CurrentTimeMillis();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_58(this, client));
			long stop = Runtime.CurrentTimeMillis();
			long duration = stop - start;
			Assert.IsGreaterOrEqual(Timeout / 2, duration);
			Assert.IsTrue(_clientWasBlocked);
		}

		private sealed class _ICodeBlock_58 : ICodeBlock
		{
			public _ICodeBlock_58(ClientTimeOutTestCase _enclosing, IExtObjectContainer client
				)
			{
				this._enclosing = _enclosing;
				this.client = client;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				client.QueryByExample(null);
			}

			private readonly ClientTimeOutTestCase _enclosing;

			private readonly IExtObjectContainer client;
		}

		public class TestMessageRecipient : IMessageRecipient
		{
			public virtual void ProcessMessage(IMessageContext con, object message)
			{
				_clientWasBlocked = true;
				Cool.SleepIgnoringInterruption(Timeout * 3);
			}
		}

		public class Data
		{
		}
	}
}
