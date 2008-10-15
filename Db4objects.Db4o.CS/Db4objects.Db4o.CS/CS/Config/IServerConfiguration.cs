/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.CS.Config
{
	/// <summary>Configuration interface for db4o servers.</summary>
	/// <remarks>Configuration interface for db4o servers.</remarks>
	/// <since>7.5</since>
	public interface IServerConfiguration : IFileConfigurationProvider, INetworkingConfigurationProvider
		, ICommonConfigurationProvider
	{
		/// <summary>sets the MessageRecipient to receive Client Server messages.</summary>
		/// <remarks>
		/// sets the MessageRecipient to receive Client Server messages. <br />
		/// <br />
		/// This setting should be used on the server side.<br /><br />
		/// </remarks>
		/// <param name="messageRecipient">the MessageRecipient to be used</param>
		IMessageRecipient MessageRecipient
		{
			set;
		}
	}
}
