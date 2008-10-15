/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.CS.Config
{
	/// <summary>Configuration interface for db4o clients.</summary>
	/// <remarks>Configuration interface for db4o clients.</remarks>
	/// <since>7.5</since>
	public interface IClientConfiguration : INetworkingConfigurationProvider, ICommonConfigurationProvider
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
		int PrefetchIDCount
		{
			set;
		}

		/// <summary>Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// 	</summary>
		/// <remarks>
		/// Sets the number of objects to be prefetched for an ObjectSet in C/S mode.
		/// This setting should be used on the server side. In embedded mode this setting
		/// has no effect.
		/// </remarks>
		/// <param name="prefetchObjectCount">The number of objects to be prefetched</param>
		int PrefetchObjectCount
		{
			set;
		}

		/// <summary>returns the MessageSender for this Configuration context.</summary>
		/// <remarks>
		/// returns the MessageSender for this Configuration context.
		/// This setting should be used on the client side.
		/// </remarks>
		/// <returns>MessageSender</returns>
		IMessageSender MessageSender
		{
			get;
		}
	}
}
