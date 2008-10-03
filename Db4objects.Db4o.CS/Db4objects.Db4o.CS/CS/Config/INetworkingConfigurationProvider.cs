/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;

namespace Db4objects.Db4o.CS.Config
{
	/// <since>7.5</since>
	public interface INetworkingConfigurationProvider
	{
		/// <summary>Networking configuration.</summary>
		/// <remarks>Networking configuration.</remarks>
		INetworkingConfiguration Networking
		{
			get;
		}
	}
}
