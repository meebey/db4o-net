/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Messaging
{
	/// <summary>message sender for client/server messaging.</summary>
	/// <remarks>
	/// message sender for client/server messaging.
	/// &lt;br&gt;&lt;br&gt;db4o allows using the client/server TCP connection to send
	/// messages from the client to the server. Any object that can be
	/// stored to a db4o database file may be used as a message.&lt;br&gt;&lt;br&gt;
	/// For an example see Reference documentation: &lt;br&gt;
	/// http://developer.db4o.com/Resources/view.aspx/Reference/Client-Server/Messaging&lt;br&gt;
	/// http://developer.db4o.com/Resources/view.aspx/Reference/Client-Server/Remote_Code_Execution&lt;br&gt;&lt;br&gt;
	/// &lt;b&gt;See Also:&lt;/b&gt;&lt;br&gt;
	/// <see cref="IClientServerConfiguration.GetMessageSender">IClientServerConfiguration.GetMessageSender
	/// 	</see>
	/// ,&lt;br&gt;
	/// <see cref="IMessageRecipient">IMessageRecipient</see>
	/// ,&lt;br&gt;
	/// <see cref="IClientServerConfiguration.SetMessageRecipient">IClientServerConfiguration.SetMessageRecipient
	/// 	</see>
	/// </remarks>
	public interface IMessageSender
	{
		/// <summary>sends a message to the server.</summary>
		/// <remarks>sends a message to the server.</remarks>
		/// <param name="obj">the message parameter, any object may be used.</param>
		void Send(object obj);
	}
}
