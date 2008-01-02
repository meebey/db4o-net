/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class MessagingTestCaseBase : ITestCase, IOptOutCS
	{
		public sealed class MessageCollector : IMessageRecipient
		{
			public readonly Collection4 messages = new Collection4();

			public void ProcessMessage(IMessageContext context, object message)
			{
				messages.Add(message);
			}
		}

		protected virtual IMessageSender MessageSender(IObjectContainer client)
		{
			return client.Ext().Configure().ClientServer().GetMessageSender();
		}

		protected virtual IObjectContainer OpenClient(string clientId, IObjectServer server
			)
		{
			server.GrantAccess(clientId, "p");
			return Db4oFactory.OpenClient("127.0.0.1", server.Ext().Port(), clientId, "p");
		}

		protected virtual IObjectServer OpenServerWith(IMessageRecipient recipient)
		{
			IConfiguration config = Db4oFactory.NewConfiguration();
			config.Io(new MemoryIoAdapter());
			config.ClientServer().SetMessageRecipient(recipient);
			return Db4oFactory.OpenServer(config, "nofile", unchecked((int)(0xdb40)));
		}

		protected virtual void SetMessageRecipient(IObjectContainer container, IMessageRecipient
			 recipient)
		{
			container.Ext().Configure().ClientServer().SetMessageRecipient(recipient);
		}
	}
}
