/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
	public class PingTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new PingTestCase().RunAll();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().TimeoutClientSocket(1000);
			config.ClientServer().PingInterval(50);
		}

		internal PingTestCase.TestMessageRecipient recipient = new PingTestCase.TestMessageRecipient
			();

		public virtual void Test()
		{
			ClientServerFixture().Server().Ext().Configure().ClientServer().SetMessageRecipient
				(recipient);
			IExtObjectContainer client = ClientServerFixture().Db();
			IMessageSender sender = client.Configure().ClientServer().GetMessageSender();
			sender.Send(new PingTestCase.Data());
			IObjectSet os = client.Get(null);
			while (os.HasNext())
			{
				os.Next();
			}
			Assert.IsFalse(client.IsClosed());
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
