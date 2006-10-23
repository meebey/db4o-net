namespace Db4objects.Db4o.Ext
{
	/// <summary>
	/// extended client functionality for the
	/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer">ExtObjectContainer</see>
	/// interface.
	/// <br /><br />Both
	/// <see cref="Db4objects.Db4o.Db4o.OpenClient">Db4o.openClient()</see>
	/// methods always
	/// return an <code>ExtClient</code> object so a cast is possible.<br /><br />
	/// The ObjectContainer functionality is split into multiple interfaces to allow newcomers to
	/// focus on the essential methods.
	/// </summary>
	public interface IExtClient : Db4objects.Db4o.Ext.IExtObjectContainer
	{
		/// <summary>requests opening a different server database file for this client session.
		/// 	</summary>
		/// <remarks>
		/// requests opening a different server database file for this client session.
		/// <br /><br />
		/// This method can be used to switch between database files from the client
		/// side while not having to open a new socket connection or closing the
		/// current one.
		/// <br /><br />
		/// If the database file does not exist on the server, it will be created.
		/// <br /><br />
		/// A typical usecase:<br />
		/// The main database file is used for login, user and rights management only.
		/// Only one single db4o server session needs to be run. Multiple satellite
		/// database files are used for different applications or multiple user circles.
		/// Storing the data to multiple database files has the following advantages:<br />
		/// - easier rights management<br />
		/// - easier backup<br />
		/// - possible later load balancing to multiple servers<br />
		/// - better performance of smaller individual database files<br />
		/// - special debugging database files can be used
		/// <br /><br />
		/// User authorization to the alternative database file will not be checked.
		/// <br /><br />
		/// All persistent references to objects that are currently in memory
		/// are discarded during the switching process.<br /><br />
		/// </remarks>
		/// <param name="fileName">the fully qualified path of the requested database file.</param>
		void SwitchToFile(string fileName);

		/// <summary>
		/// requests switching back to the main database file after a previous call
		/// to <code>switchToFile(String fileName)</code>.
		/// </summary>
		/// <remarks>
		/// requests switching back to the main database file after a previous call
		/// to <code>switchToFile(String fileName)</code>.
		/// <br /><br />
		/// All persistent references to objects that are currently in memory
		/// are discarded during the switching process.<br /><br />
		/// </remarks>
		void SwitchToMainFile();

		/// <summary>checks if the client is currently connected to a server.</summary>
		/// <remarks>checks if the client is currently connected to a server.</remarks>
		/// <returns>true if the client is alive.</returns>
		bool IsAlive();
	}
}
