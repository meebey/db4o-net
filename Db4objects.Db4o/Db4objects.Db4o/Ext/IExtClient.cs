/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Messaging;

namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// extended client functionality for the
	/// <see cref="IExtObjectContainer">IExtObjectContainer</see>
	/// interface.
	/// &lt;br&gt;&lt;br&gt;Both
	/// <see cref="Db4oFactory.OpenClient">Db4o.openClient()</see>
	/// methods always
	/// return an &lt;code&gt;ExtClient&lt;/code&gt; object so a cast is possible.&lt;br&gt;&lt;br&gt;
	/// The ObjectContainer functionality is split into multiple interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </summary>
	public interface IExtClient : IExtObjectContainer
	{
		/// <summary>requests opening a different server database file for this client session.
		/// 	</summary>
		/// <remarks>
		/// requests opening a different server database file for this client session.
		/// &lt;br&gt;&lt;br&gt;
		/// This method can be used to switch between database files from the client
		/// side while not having to open a new socket connection or closing the
		/// current one.
		/// &lt;br&gt;&lt;br&gt;
		/// If the database file does not exist on the server, it will be created.
		/// &lt;br&gt;&lt;br&gt;
		/// A typical usecase:&lt;br&gt;
		/// The main database file is used for login, user and rights management only.
		/// Only one single db4o server session needs to be run. Multiple satellite
		/// database files are used for different applications or multiple user circles.
		/// Storing the data to multiple database files has the following advantages:&lt;br&gt;
		/// - easier rights management&lt;br&gt;
		/// - easier backup&lt;br&gt;
		/// - possible later load balancing to multiple servers&lt;br&gt;
		/// - better performance of smaller individual database files&lt;br&gt;
		/// - special debugging database files can be used
		/// &lt;br&gt;&lt;br&gt;
		/// User authorization to the alternative database file will not be checked.
		/// &lt;br&gt;&lt;br&gt;
		/// All persistent references to objects that are currently in memory
		/// are discarded during the switching process.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		/// <param name="fileName">the fully qualified path of the requested database file.</param>
		void SwitchToFile(string fileName);

		/// <summary>
		/// requests switching back to the main database file after a previous call
		/// to &lt;code&gt;switchToFile(String fileName)&lt;/code&gt;.
		/// </summary>
		/// <remarks>
		/// requests switching back to the main database file after a previous call
		/// to &lt;code&gt;switchToFile(String fileName)&lt;/code&gt;.
		/// &lt;br&gt;&lt;br&gt;
		/// All persistent references to objects that are currently in memory
		/// are discarded during the switching process.&lt;br&gt;&lt;br&gt;
		/// </remarks>
		void SwitchToMainFile();

		/// <summary>checks if the client is currently connected to a server.</summary>
		/// <remarks>checks if the client is currently connected to a server.</remarks>
		/// <returns>true if the client is alive.</returns>
		bool IsAlive();

		/// <summary>
		/// Dispatches any pending messages to
		/// the currently configured
		/// <see cref="IMessageRecipient">IMessageRecipient</see>
		/// .
		/// </summary>
		/// <param name="maxTimeSlice">how long before the method returns leaving messages on the queue for later dispatching
		/// 	</param>
		void DispatchPendingMessages(long maxTimeSlice);
	}
}
