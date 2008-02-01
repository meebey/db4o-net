/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Config
{
	/// <summary>Client/Server configuration interface.</summary>
	/// <remarks>Client/Server configuration interface.</remarks>
	public interface IClientServerConfiguration
	{
		/// <summary>
		/// Sets the number of IDs to be pre-allocated in the database for new
		/// objects created on the client.
		/// </summary>
		/// <remarks>
		/// Sets the number of IDs to be pre-allocated in the database for new
		/// objects created on the client.
		/// This setting should be used on the client side. In embedded mode this setting
		/// has no effect.
		/// </remarks>
		/// <param name="prefetchIDCount">The number of IDs to be prefetched</param>
		void PrefetchIDCount(int prefetchIDCount);

		/// <summary>Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// 	</summary>
		/// <remarks>
		/// Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// This setting should be used on the server side. In embedded mode this setting
		/// has no effect.
		/// </remarks>
		/// <param name="prefetchObjectCount">The number of objects to be prefetched</param>
		void PrefetchObjectCount(int prefetchObjectCount);

		/// <summary>sets the MessageRecipient to receive Client Server messages.</summary>
		/// <remarks>
		/// sets the MessageRecipient to receive Client Server messages. &lt;br&gt;
		/// &lt;br&gt;
		/// This setting should be used on the server side.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="messageRecipient">the MessageRecipient to be used</param>
		void SetMessageRecipient(IMessageRecipient messageRecipient);

		/// <summary>returns the MessageSender for this Configuration context.</summary>
		/// <remarks>
		/// returns the MessageSender for this Configuration context.
		/// This setting should be used on the client side.
		/// </remarks>
		/// <returns>MessageSender</returns>
		IMessageSender GetMessageSender();

		/// <summary>
		/// configures the time a client waits for a message response
		/// from the server.
		/// </summary>
		/// <remarks>
		/// configures the time a client waits for a message response
		/// from the server. &lt;br&gt;
		/// &lt;br&gt;
		/// Default value: 600000ms (10 minutes)&lt;br&gt;
		/// &lt;br&gt;
		/// It is recommended to use the same values for
		/// <see cref="IClientServerConfiguration.TimeoutClientSocket">IClientServerConfiguration.TimeoutClientSocket
		/// 	</see>
		/// and
		/// <see cref="IClientServerConfiguration.TimeoutServerSocket">IClientServerConfiguration.TimeoutServerSocket
		/// 	</see>
		/// .
		/// &lt;br&gt;
		/// This setting can be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		void TimeoutClientSocket(int milliseconds);

		/// <summary>configures the timeout of the serverside socket.</summary>
		/// <remarks>
		/// configures the timeout of the serverside socket. &lt;br&gt;
		/// &lt;br&gt;
		/// The serverside handler waits for messages to arrive from the client.
		/// If no more messages arrive for the duration configured in this
		/// setting, the client will be disconnected.
		/// &lt;br&gt;
		/// Clients send PING messages to the server at an interval of
		/// Math.min(timeoutClientSocket(), timeoutServerSocket()) / 2
		/// and the server will respond to keep connections alive.
		/// &lt;br&gt;
		/// Decrease this setting if you want clients to disconnect faster.
		/// &lt;br&gt;
		/// Increase this setting if you have a large number of clients and long
		/// running queries and you are getting disconnected clients that you
		/// would like to wait even longer for a response from the server.
		/// &lt;br&gt;
		/// Default value: 600000ms (10 minutes)&lt;br&gt;
		/// &lt;br&gt;
		/// It is recommended to use the same values for
		/// <see cref="IClientServerConfiguration.TimeoutClientSocket">IClientServerConfiguration.TimeoutClientSocket
		/// 	</see>
		/// and
		/// <see cref="IClientServerConfiguration.TimeoutServerSocket">IClientServerConfiguration.TimeoutServerSocket
		/// 	</see>
		/// .
		/// &lt;br&gt;
		/// This setting can be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="milliseconds">time in milliseconds</param>
		void TimeoutServerSocket(int milliseconds);

		/// <summary>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// </summary>
		/// <remarks>
		/// configures the client messaging system to be single threaded
		/// or multithreaded.
		/// &lt;br&gt;&lt;br&gt;Recommended settings:&lt;br&gt;
		/// - &lt;code&gt;true&lt;/code&gt; for low resource systems.&lt;br&gt;
		/// - &lt;code&gt;false&lt;/code&gt; for best asynchronous performance and fast
		/// GUI response.
		/// &lt;br&gt;&lt;br&gt;Default value:&lt;br&gt;
		/// - .NET Compactframework: &lt;code&gt;true&lt;/code&gt;&lt;br&gt;
		/// - all other platforms: &lt;code&gt;false&lt;/code&gt;&lt;br&gt;&lt;br&gt;
		/// This setting can be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">the desired setting</param>
		void SingleThreadedClient(bool flag);

		/// <summary>Configures to batch messages between client and server.</summary>
		/// <remarks>
		/// Configures to batch messages between client and server. By default, batch
		/// mode is enabled.&lt;br&gt;&lt;br&gt;
		/// This setting can be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="flag">false, to turn message batching off.</param>
		void BatchMessages(bool flag);

		/// <summary>Configures the maximum memory buffer size for batched message.</summary>
		/// <remarks>
		/// Configures the maximum memory buffer size for batched message. If the
		/// size of batched messages is greater than &lt;code&gt;maxSize&lt;/code&gt;, batched
		/// messages will be sent to server.&lt;br&gt;&lt;br&gt;
		/// This setting can be used on both client and server.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="maxSize"></param>
		void MaxBatchQueueSize(int maxSize);
	}
}
