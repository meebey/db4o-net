/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Messaging.Internal;

namespace Db4objects.Db4o.Messaging.Internal
{
	public class MessageContextInfrastructure
	{
		public static readonly ContextVariable contextProvider = new ContextVariable(typeof(
			IMessageContextProvider));

		public static MessageContext Context()
		{
			IMessageContextProvider provider = (IMessageContextProvider)contextProvider.Value;
			if (provider == null)
			{
				return null;
			}
			return provider.MessageContext();
		}
	}
}
