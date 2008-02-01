/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Messaging
{
	/// <summary>message recipient for client/server messaging.</summary>
	/// <remarks>
	/// message recipient for client/server messaging.
	/// &lt;br&gt;&lt;br&gt;db4o allows using the client/server TCP connection to send
	/// messages from the client to the server. Any object that can be
	/// stored to a db4o database file may be used as a message.&lt;br&gt;&lt;br&gt;
	/// For an example see Reference documentation: &lt;br&gt;
	/// http://developer.db4o.com/Resources/view.aspx/Reference/Client-Server/Messaging&lt;br&gt;
	/// http://developer.db4o.com/Resources/view.aspx/Reference/Client-Server/Remote_Code_Execution&lt;br&gt;&lt;br&gt;
	/// &lt;b&gt;See Also:&lt;/b&gt;&lt;br&gt;
	/// <see cref="IClientServerConfiguration.SetMessageRecipient">ClientServerConfiguration.setMessageRecipient(MessageRecipient)
	/// 	</see>
	/// , &lt;br&gt;
	/// <see cref="IMessageSender">IMessageSender</see>
	/// ,&lt;br&gt;
	/// <see cref="IClientServerConfiguration.GetMessageSender">IClientServerConfiguration.GetMessageSender
	/// 	</see>
	/// ,&lt;br&gt;
	/// <see cref="MessageRecipientWithContext">MessageRecipientWithContext</see>
	/// &lt;br&gt;
	/// </remarks>
	public interface IMessageRecipient
	{
		/// <summary>the method called upon the arrival of messages.</summary>
		/// <remarks>the method called upon the arrival of messages.</remarks>
		/// <param name="context">contextual information for the message.</param>
		/// <param name="message">the message received.</param>
		void ProcessMessage(IMessageContext context, object message);
	}
}
