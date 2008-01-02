/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class PingTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new PingTestCase().RunAll();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().TimeoutClientSocket(1000);
		}

		internal PingTestCase.TestMessageRecipient recipient = new PingTestCase.TestMessageRecipient
			();

		public virtual void Test()
		{
			ClientServerFixture().Server().Ext().Configure().ClientServer().SetMessageRecipient
				(recipient);
			IExtObjectContainer client = ClientServerFixture().Db();
			IMessageSender sender = client.Configure().ClientServer().GetMessageSender();
			if (IsMTOC())
			{
				Assert.Expect(typeof(NotSupportedException), new _ICodeBlock_35(this, sender));
				return;
			}
			sender.Send(new PingTestCase.Data());
			IObjectSet os = client.Get(null);
			while (os.HasNext())
			{
				os.Next();
			}
			Assert.IsFalse(client.IsClosed());
		}

		private sealed class _ICodeBlock_35 : ICodeBlock
		{
			public _ICodeBlock_35(PingTestCase _enclosing, IMessageSender sender)
			{
				this._enclosing = _enclosing;
				this.sender = sender;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				sender.Send(new PingTestCase.Data());
			}

			private readonly PingTestCase _enclosing;

			private readonly IMessageSender sender;
		}

		public class TestMessageRecipient : IMessageRecipient
		{
			public virtual void ProcessMessage(IMessageContext con, object message)
			{
				Cool.SleepIgnoringInterruption(3000);
			}
		}

		public class Data
		{
		}
	}
}
