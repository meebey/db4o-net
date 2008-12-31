/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	/// <summary>the db4o server interface.</summary>
	/// <remarks>
	/// the db4o server interface.
	/// <br /><br />- db4o servers can be opened with
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenServer">Db4objects.Db4o.Db4oFactory.OpenServer
	/// 	</see>
	/// .<br />
	/// - Direct in-memory connections to servers can be made with
	/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
	/// 	</see>
	/// <br />
	/// - TCP connections are available through
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenClient">Db4objects.Db4o.Db4oFactory.OpenClient
	/// 	</see>
	/// .
	/// <br /><br />Before connecting clients over TCP, you have to
	/// <see cref="Db4objects.Db4o.IObjectServer.GrantAccess">Db4objects.Db4o.IObjectServer.GrantAccess
	/// 	</see>
	/// to the username and password combination
	/// that you want to use.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Db4oFactory.OpenServer">Db4o.openServer</seealso>
	/// <seealso cref="Db4objects.Db4o.Ext.IExtObjectServer">ExtObjectServer for extended functionality
	/// 	</seealso>
	public interface IObjectServer
	{
		/// <summary>
		/// closes the
		/// <see cref="Db4objects.Db4o.IObjectServer"></see>
		/// and writes all cached data.
		/// <br /><br />
		/// </summary>
		/// <returns>
		/// true - denotes that the last instance connected to the
		/// used database file was closed.
		/// </returns>
		bool Close();

		/// <summary>
		/// returns an
		/// <see cref="Db4objects.Db4o.IObjectServer"></see>
		/// with extended functionality.
		/// <br /><br />Use this method as a convenient accessor to extended methods.
		/// Every
		/// <see cref="Db4objects.Db4o.IObjectServer"></see>
		/// can be casted to an
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectServer">Db4objects.Db4o.Ext.IExtObjectServer
		/// 	</see>
		/// .
		/// <br /><br />The functionality is split to two interfaces to allow newcomers to
		/// focus on the essential methods.
		/// </summary>
		IExtObjectServer Ext();

		/// <summary>grants client access to the specified user with the specified password.</summary>
		/// <remarks>
		/// grants client access to the specified user with the specified password.
		/// <br /><br />If the user already exists, the password is changed to
		/// the specified password.<br /><br />
		/// </remarks>
		/// <param name="userName">the name of the user</param>
		/// <param name="password">the password to be used</param>
		void GrantAccess(string userName, string password);

		/// <summary>opens a client against this server.</summary>
		/// <remarks>
		/// opens a client against this server.
		/// <br /><br />A client opened with this method operates within the same VM
		/// as the server. Since an embedded client can use direct communication, without
		/// an in-between socket connection, performance will be better than a client
		/// opened with
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenClient">Db4objects.Db4o.Db4oFactory.OpenClient
		/// 	</see>
		/// <br /><br />Every client has it's own transaction and uses it's own cache
		/// for it's own version of all peristent objects.
		/// </remarks>
		IObjectContainer OpenClient();

		/// <summary>
		/// See
		/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
		/// 	</see>
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">IConfiguration</see>
		/// instance to be obtained via
		/// <see cref="Db4objects.Db4o.Db4oFactory.NewConfiguration">Db4objects.Db4o.Db4oFactory.NewConfiguration
		/// 	</see>
		/// </param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
		/// </returns>
		IObjectContainer OpenClient(IConfiguration config);
	}
}
