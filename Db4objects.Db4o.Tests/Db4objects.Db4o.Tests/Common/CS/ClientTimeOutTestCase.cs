/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientTimeOutTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new ClientTimeOutTestCase().RunAll();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().TimeoutClientSocket(1000);
		}

		internal ClientTimeOutTestCase.TestMessageRecipient recipient = new ClientTimeOutTestCase.TestMessageRecipient
			();

		public virtual void Test()
		{
			if (IsMTOC())
			{
				return;
			}
			ClientServerFixture().Server().Ext().Configure().ClientServer().SetMessageRecipient
				(recipient);
			IExtObjectContainer client = ClientServerFixture().Db();
			IMessageSender sender = client.Configure().ClientServer().GetMessageSender();
			sender.Send(new ClientTimeOutTestCase.Data());
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_42(this, client));
		}

		private sealed class _ICodeBlock_42 : ICodeBlock
		{
			public _ICodeBlock_42(ClientTimeOutTestCase _enclosing, IExtObjectContainer client
				)
			{
				this._enclosing = _enclosing;
				this.client = client;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				IObjectSet os = client.Get(null);
				while (os.HasNext())
				{
					os.Next();
				}
			}

			private readonly ClientTimeOutTestCase _enclosing;

			private readonly IExtObjectContainer client;
		}

		public class TestMessageRecipient : IMessageRecipient
		{
			public virtual void ProcessMessage(IObjectContainer con, object message)
			{
				Cool.SleepIgnoringInterruption(3000);
			}
		}

		public class Data
		{
		}
	}
}
