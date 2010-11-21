/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// extended client functionality for the
	/// <see cref="IExtObjectContainer">IExtObjectContainer</see>
	/// interface.
	/// <br /><br />Both
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenClient(string, int, string, string)">Db4o.openClient()
	/// 	</see>
	/// methods always
	/// return an <code>ExtClient</code> object so a cast is possible.<br /><br />
	/// The ObjectContainer functionality is split into multiple interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </summary>
	public interface IExtClient : IExtObjectContainer
	{
		/// <summary>checks if the client is currently connected to a server.</summary>
		/// <remarks>checks if the client is currently connected to a server.</remarks>
		/// <returns>true if the client is alive.</returns>
		bool IsAlive();
	}
}
