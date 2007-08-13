/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MUserMessage : MsgObject, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			if (MessageRecipient() != null)
			{
				Unmarshall();
				MessageRecipient().ProcessMessage(Transaction().ObjectContainer(), ReadObjectFromPayLoad
					());
			}
			return true;
		}

		private IMessageRecipient MessageRecipient()
		{
			return Config().MessageRecipient();
		}
	}
}
