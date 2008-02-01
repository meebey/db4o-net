/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Ext
{
	/// <summary>extended functionality for the ObjectServer interface.</summary>
	/// <remarks>
	/// extended functionality for the ObjectServer interface.
	/// &lt;br&gt;&lt;br&gt;Every ObjectServer also always is an ExtObjectServer
	/// so a cast is possible.&lt;br&gt;&lt;br&gt;
	/// <see cref="IObjectServer.Ext">IObjectServer.Ext</see>
	/// is a convenient method to perform the cast.&lt;br&gt;&lt;br&gt;
	/// The functionality is split to two interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </remarks>
	public interface IExtObjectServer : IObjectServer
	{
		/// <summary>backs up the database file used by the ObjectServer.</summary>
		/// <remarks>
		/// backs up the database file used by the ObjectServer.
		/// &lt;br&gt;&lt;br&gt;While the backup is running, the ObjectServer can continue to be
		/// used. Changes that are made while the backup is in progress, will be applied to
		/// the open ObjectServer and to the backup.&lt;br&gt;&lt;br&gt;
		/// While the backup is running, the ObjectContainer should not be closed.&lt;br&gt;&lt;br&gt;
		/// If a file already exists at the specified path, it will be overwritten.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="path">a fully qualified path</param>
		/// <exception cref="IOException"></exception>
		void Backup(string path);

		/// <summary>returns the number of connected clients.</summary>
		/// <remarks>returns the number of connected clients.</remarks>
		int ClientCount();

		/// <summary>
		/// returns the
		/// <see cref="IConfiguration">IConfiguration</see>
		/// context for this ObjectServer.
		/// &lt;br&gt;&lt;br&gt;
		/// Upon opening an ObjectServer with any of the factory methods in the
		/// <see cref="Db4oFactory">Db4oFactory</see>
		/// class, the global
		/// <see cref="IConfiguration">IConfiguration</see>
		/// context
		/// is copied into the ObjectServer. The
		/// <see cref="IConfiguration">IConfiguration</see>
		/// can be modified individually for
		/// each ObjectServer without any effects on the global settings.&lt;br&gt;&lt;br&gt;
		/// </summary>
		/// <returns>the Configuration context for this ObjectServer</returns>
		/// <seealso cref="Db4oFactory.Configure">Db4oFactory.Configure</seealso>
		IConfiguration Configure();

		/// <summary>returns the ObjectContainer used by the server.</summary>
		/// <remarks>
		/// returns the ObjectContainer used by the server.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <returns>the ObjectContainer used by the server</returns>
		IObjectContainer ObjectContainer();

		/// <summary>removes client access permissions for the specified user.</summary>
		/// <remarks>
		/// removes client access permissions for the specified user.
		/// &lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="userName">the name of the user</param>
		void RevokeAccess(string userName);

		/// <returns>The local port this server uses, 0 if disconnected or in embedded mode</returns>
		int Port();
	}
}
