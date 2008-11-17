/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Config;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.CS.Config
{
	/// <summary>Configuration interface for db4o servers.</summary>
	/// <remarks>Configuration interface for db4o servers.</remarks>
	/// <since>7.5</since>
	public interface IServerConfiguration : IFileConfigurationProvider, INetworkingConfigurationProvider
		, ICommonConfigurationProvider
	{
	}
}
