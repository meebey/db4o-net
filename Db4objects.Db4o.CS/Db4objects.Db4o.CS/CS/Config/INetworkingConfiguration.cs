/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Foundation.Network;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.CS.Config
{
	/// <summary>Configuration interface for networking configuration settings.</summary>
	/// <remarks>Configuration interface for networking configuration settings.</remarks>
	/// <since>7.5</since>
	public interface INetworkingConfiguration
	{
		IClientServerFactory ClientServerFactory
		{
			get;
			set;
		}

		/// <summary>
		/// configures the time a client waits for a message response
		/// from the server.
		/// </summary>
		/// <remarks>
		/// configures the time a client waits for a message response
		/// from the server. <br />
		/// <br />
		/// Default value: 600000ms (10 minutes)<br />
		/// <br />
		/// It is recommended to use the same values for
		/// <see cref="TimeoutClientSocket(int)">TimeoutClientSocket(int)</see>
		/// and
		/// <see cref="TimeoutServerSocket(int)">TimeoutServerSocket(int)</see>
		/// .
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value>time in milliseconds</value>
		int TimeoutClientSocket
		{
			set;
		}

		/// <summary>configures the timeout of the server side socket.</summary>
		/// <remarks>
		/// configures the timeout of the server side socket. <br />
		/// <br />
		/// The server side handler waits for messages to arrive from the client.
		/// If no more messages arrive for the duration configured in this
		/// setting, the client will be disconnected.
		/// <br />
		/// Clients send PING messages to the server at an interval of
		/// Math.min(timeoutClientSocket(), timeoutServerSocket()) / 2
		/// and the server will respond to keep connections alive.
		/// <br />
		/// Decrease this setting if you want clients to disconnect faster.
		/// <br />
		/// Increase this setting if you have a large number of clients and long
		/// running queries and you are getting disconnected clients that you
		/// would like to wait even longer for a response from the server.
		/// <br />
		/// Default value: 600000ms (10 minutes)<br />
		/// <br />
		/// It is recommended to use the same values for
		/// <see cref="TimeoutClientSocket(int)">TimeoutClientSocket(int)</see>
		/// and
		/// <see cref="TimeoutServerSocket(int)">TimeoutServerSocket(int)</see>
		/// .
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value>time in milliseconds</value>
		int TimeoutServerSocket
		{
			set;
		}

		/// <summary>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// </summary>
		/// <remarks>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// <br /><br />Recommended settings:<br />
		/// - <code>true</code> for low resource systems.<br />
		/// - <code>false</code> for best asynchronous performance and fast
		/// GUI response.
		/// <br /><br />Default value:<br />
		/// - .NET Compact Framework: <code>true</code><br />
		/// - all other platforms: <code>false</code><br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value>the desired setting</value>
		bool SingleThreadedClient
		{
			set;
		}

		/// <summary>Configures to batch messages between client and server.</summary>
		/// <remarks>
		/// Configures to batch messages between client and server. By default, batch
		/// mode is enabled.<br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value>false, to turn message batching off.</value>
		bool BatchMessages
		{
			set;
		}

		/// <summary>Configures the maximum memory buffer size for batched message.</summary>
		/// <remarks>
		/// Configures the maximum memory buffer size for batched message. If the
		/// size of batched messages is greater than <code>maxSize</code>, batched
		/// messages will be sent to server.<br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value></value>
		int MaxBatchQueueSize
		{
			set;
		}

		/// <summary>sets the MessageRecipient to receive Client Server messages.</summary>
		/// <remarks>
		/// sets the MessageRecipient to receive Client Server messages. <br />
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <value>the MessageRecipient to be used</value>
		IMessageRecipient MessageRecipient
		{
			set;
		}

		/// <since>7.11</since>
		/// <since>7.11</since>
		ISocket4Factory SocketFactory
		{
			get;
			set;
		}
	}
}
