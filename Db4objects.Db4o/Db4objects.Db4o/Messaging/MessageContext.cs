/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Messaging;
using Db4objects.Db4o.Messaging.Internal;

namespace Db4objects.Db4o.Messaging
{
	/// <summary>Additional message-related information.</summary>
	/// <remarks>Additional message-related information.</remarks>
	public abstract class MessageContext
	{
		/// <summary>The context associated to the current message.</summary>
		/// <remarks>
		/// The context associated to the current message.
		/// Only valid during
		/// <see cref="IMessageRecipient.ProcessMessage">IMessageRecipient.ProcessMessage</see>
		/// </remarks>
		public static MessageContext Current
		{
			get
			{
				return MessageContextInfrastructure.Context();
			}
		}

		/// <summary>The sender of the current message.</summary>
		/// <remarks>
		/// The sender of the current message.
		/// The reference can be used to send a reply to it.
		/// </remarks>
		/// <returns></returns>
		public abstract IMessageSender Sender
		{
			get;
		}
	}
}
