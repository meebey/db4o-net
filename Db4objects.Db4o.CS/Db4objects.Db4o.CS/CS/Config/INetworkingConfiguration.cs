/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;

namespace Db4objects.Db4o.CS.Config
{
	/// <since>7.5</since>
	public interface INetworkingConfiguration
	{
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
		/// <see cref="INetworkingConfiguration.TimeoutClientSocket">INetworkingConfiguration.TimeoutClientSocket
		/// 	</see>
		/// and
		/// <see cref="INetworkingConfiguration.TimeoutServerSocket">INetworkingConfiguration.TimeoutServerSocket
		/// 	</see>
		/// .
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		int TimeoutClientSocket
		{
			set;
		}

		/// <summary>configures the timeout of the serverside socket.</summary>
		/// <remarks>
		/// configures the timeout of the serverside socket. <br />
		/// <br />
		/// The serverside handler waits for messages to arrive from the client.
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
		/// <see cref="INetworkingConfiguration.TimeoutClientSocket">INetworkingConfiguration.TimeoutClientSocket
		/// 	</see>
		/// and
		/// <see cref="INetworkingConfiguration.TimeoutServerSocket">INetworkingConfiguration.TimeoutServerSocket
		/// 	</see>
		/// .
		/// <br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
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
		/// - .NET Compactframework: <code>true</code><br />
		/// - all other platforms: <code>false</code><br /><br />
		/// This setting can be used on both client and server.<br /><br />
		/// </remarks>
		/// <param name="flag">the desired setting</param>
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
		/// <param name="flag">false, to turn message batching off.</param>
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
		/// <param name="maxSize"></param>
		int MaxBatchQueueSize
		{
			set;
		}
	}
}
