namespace Db4objects.Db4o
{
	/// <summary>factory class to start db4o database engines.</summary>
	/// <remarks>
	/// factory class to start db4o database engines.
	/// <br /><br />This class provides static methods to<br />
	/// - open single-user databases
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4objects.Db4o.Db4oFactory.OpenFile
	/// 	</see>
	/// <br />
	/// - open db4o servers
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenServer">Db4objects.Db4o.Db4oFactory.OpenServer
	/// 	</see>
	/// <br />
	/// - connect to db4o servers
	/// <see cref="Db4objects.Db4o.Db4oFactory.OpenClient">Db4objects.Db4o.Db4oFactory.OpenClient
	/// 	</see>
	/// <br />
	/// - provide access to the global configuration context
	/// <see cref="Db4objects.Db4o.Db4oFactory.Configure">Db4objects.Db4o.Db4oFactory.Configure
	/// 	</see>
	/// <br />
	/// - print the version number of this db4o version
	/// <see cref="Db4objects.Db4o.Db4oFactory.Main">Db4objects.Db4o.Db4oFactory.Main</see>
	/// 
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Ext.ExtDb4oFactory">ExtDb4o for extended functionality.
	/// 	</seealso>
	public class Db4oFactory
	{
		internal static readonly Db4objects.Db4o.Config4Impl i_config = new Db4objects.Db4o.Config4Impl
			();

		private static Db4objects.Db4o.Sessions i_sessions = new Db4objects.Db4o.Sessions
			();

		static Db4oFactory()
		{
			Db4objects.Db4o.Platform4.GetDefaultConfiguration(i_config);
		}

		/// <summary>prints the version name of this db4o version to <code>System.out</code>.
		/// 	</summary>
		/// <remarks>prints the version name of this db4o version to <code>System.out</code>.
		/// 	</remarks>
		public static void Main(string[] args)
		{
			Sharpen.Runtime.Out.WriteLine(Version());
		}

		/// <summary>
		/// returns the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// context
		/// for the running JVM session.
		/// <br /><br />
		/// The
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// can be overriden in each
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer.Configure">ObjectContainer</see>
		/// .<br /><br />
		/// </summary>
		/// <returns>
		/// the global
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">configuration</see>
		/// context
		/// </returns>
		public static Db4objects.Db4o.Config.IConfiguration Configure()
		{
			return i_config;
		}

		/// <summary>
		/// Creates a fresh
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// instance.
		/// </summary>
		/// <returns>a fresh, independent configuration with all options set to their default values
		/// 	</returns>
		public static Db4objects.Db4o.Config.IConfiguration NewConfiguration()
		{
			Db4objects.Db4o.Config4Impl config = new Db4objects.Db4o.Config4Impl();
			Db4objects.Db4o.Platform4.GetDefaultConfiguration(config);
			return config;
		}

		/// <summary>
		/// Creates a clone of the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// .
		/// </summary>
		/// <returns>
		/// a fresh configuration with all option values set to the values
		/// currently configured for the global db4o configuration context
		/// </returns>
		public static Db4objects.Db4o.Config.IConfiguration CloneConfiguration()
		{
			return (Db4objects.Db4o.Config4Impl)((Db4objects.Db4o.Foundation.IDeepClone)Db4objects.Db4o.Db4oFactory
				.Configure()).DeepClone(null);
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenClient">Db4objects.Db4o.Db4oFactory.OpenClient
		/// 	</see>
		/// , but uses
		/// the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// client and connects it to the specified named server and port.
		/// <br /><br />
		/// The server needs to
		/// <see cref="Db4objects.Db4o.IObjectServer.GrantAccess">allow access</see>
		/// for the specified user and password.
		/// <br /><br />
		/// A client
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// can be cast to
		/// <see cref="Db4objects.Db4o.Ext.IExtClient">ExtClient</see>
		/// to use extended
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer">ExtObjectContainer</see>
		/// 
		/// and
		/// <see cref="Db4objects.Db4o.Ext.IExtClient">ExtClient</see>
		/// methods.
		/// <br /><br />
		/// </summary>
		/// <param name="hostName">the host name</param>
		/// <param name="port">the port the server is using</param>
		/// <param name="user">the user name</param>
		/// <param name="password">the user password</param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.IObjectServer.GrantAccess">Db4objects.Db4o.IObjectServer.GrantAccess
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectContainer OpenClient(string hostName, int port
			, string user, string password)
		{
			return OpenClient(Db4objects.Db4o.Db4oFactory.CloneConfiguration(), hostName, port
				, user, password);
		}

		/// <summary>
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// client and connects it to the specified named server and port.
		/// <br /><br />
		/// The server needs to
		/// <see cref="Db4objects.Db4o.IObjectServer.GrantAccess">allow access</see>
		/// for the specified user and password.
		/// <br /><br />
		/// A client
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// can be cast to
		/// <see cref="Db4objects.Db4o.Ext.IExtClient">ExtClient</see>
		/// to use extended
		/// <see cref="Db4objects.Db4o.Ext.IExtObjectContainer">ExtObjectContainer</see>
		/// 
		/// and
		/// <see cref="Db4objects.Db4o.Ext.IExtClient">ExtClient</see>
		/// methods.
		/// <br /><br />
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4objects.Db4o.Db4oFactory.NewConfiguration">Db4objects.Db4o.Db4oFactory.NewConfiguration
		/// 	</see>
		/// </param>
		/// <param name="hostName">the host name</param>
		/// <param name="port">the port the server is using</param>
		/// <param name="user">the user name</param>
		/// <param name="password">the user password</param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.IObjectServer.GrantAccess">Db4objects.Db4o.IObjectServer.GrantAccess
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectContainer OpenClient(Db4objects.Db4o.Config.IConfiguration
			 config, string hostName, int port, string user, string password)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				return new Db4objects.Db4o.CS.YapClient(config, new Db4objects.Db4o.Foundation.Network.YapSocketReal
					(hostName, port), user, password, true);
			}
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4objects.Db4o.Db4oFactory.OpenFile
		/// 	</see>
		/// , but uses
		/// the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// on the specified database file for local use.
		/// <br /><br />Subsidiary calls with the same database file name will return the same
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// object.<br /><br />
		/// Every call to <code>openFile()</code> requires a corresponding
		/// <see cref="Db4objects.Db4o.IObjectContainer.Close">ObjectContainer.close</see>
		/// .<br /><br />
		/// Database files can only be accessed for readwrite access from one process
		/// (one Java VM) at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br /><br />
		/// </summary>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ReadOnly">Db4objects.Db4o.Config.IConfiguration.ReadOnly
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt">Db4objects.Db4o.Config.IConfiguration.Encrypt
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectContainer OpenFile(string databaseFileName)
		{
			return OpenFile(CloneConfiguration(), databaseFileName);
		}

		/// <summary>
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// on the specified database file for local use.
		/// <br /><br />Subsidiary calls with the same database file name will return the same
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// object.<br /><br />
		/// Every call to <code>openFile()</code> requires a corresponding
		/// <see cref="Db4objects.Db4o.IObjectContainer.Close">ObjectContainer.close</see>
		/// .<br /><br />
		/// Database files can only be accessed for readwrite access from one process
		/// (one Java VM) at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br /><br />
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4objects.Db4o.Db4oFactory.NewConfiguration">Db4objects.Db4o.Db4oFactory.NewConfiguration
		/// 	</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">ObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ReadOnly">Db4objects.Db4o.Config.IConfiguration.ReadOnly
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt">Db4objects.Db4o.Config.IConfiguration.Encrypt
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectContainer OpenFile(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				return i_sessions.Open(config, databaseFileName);
			}
		}

		protected static Db4objects.Db4o.IObjectContainer OpenMemoryFile1(Db4objects.Db4o.Config.IConfiguration
			 config, Db4objects.Db4o.Ext.MemoryFile memoryFile)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				if (memoryFile == null)
				{
					memoryFile = new Db4objects.Db4o.Ext.MemoryFile();
				}
				Db4objects.Db4o.IObjectContainer oc = null;
				try
				{
					oc = new Db4objects.Db4o.YapMemoryFile(config, memoryFile);
				}
				catch (System.Exception t)
				{
					Db4objects.Db4o.Messages.LogErr(i_config, 4, "Memory File", t);
					return null;
				}
				Db4objects.Db4o.Platform4.PostOpen(oc);
				Db4objects.Db4o.Messages.LogMsg(i_config, 5, "Memory File");
				return oc;
			}
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4objects.Db4o.Db4oFactory.OpenServer">Db4objects.Db4o.Db4oFactory.OpenServer
		/// 	</see>
		/// , but uses
		/// the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// context.
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectServer">ObjectServer</see>
		/// on the specified database file and port.
		/// <br /><br />
		/// If the server does not need to listen on a port because it will only be used
		/// in embedded mode with
		/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
		/// 	</see>
		/// , specify '0' as the
		/// port number.
		/// </summary>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <param name="port">
		/// the port to be used, or 0, if the server should not open a port,
		/// because it will only be used with
		/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
		/// 	</see>
		/// </param>
		/// <returns>
		/// an
		/// <see cref="Db4objects.Db4o.IObjectServer">ObjectServer</see>
		/// listening
		/// on the specified port.
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ReadOnly">Db4objects.Db4o.Config.IConfiguration.ReadOnly
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt">Db4objects.Db4o.Config.IConfiguration.Encrypt
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectServer OpenServer(string databaseFileName, int
			 port)
		{
			return OpenServer(CloneConfiguration(), databaseFileName, port);
		}

		/// <summary>
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectServer">ObjectServer</see>
		/// on the specified database file and port.
		/// <br /><br />
		/// If the server does not need to listen on a port because it will only be used
		/// in embedded mode with
		/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
		/// 	</see>
		/// , specify '0' as the
		/// port number.
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">Configuration</see>
		/// instance to be obtained via
		/// <see cref="Db4objects.Db4o.Db4oFactory.NewConfiguration">Db4objects.Db4o.Db4oFactory.NewConfiguration
		/// 	</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <param name="port">
		/// the port to be used, or 0, if the server should not open a port,
		/// because it will only be used with
		/// <see cref="Db4objects.Db4o.IObjectServer.OpenClient">Db4objects.Db4o.IObjectServer.OpenClient
		/// 	</see>
		/// </param>
		/// <returns>
		/// an
		/// <see cref="Db4objects.Db4o.IObjectServer">ObjectServer</see>
		/// listening
		/// on the specified port.
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ReadOnly">Db4objects.Db4o.Config.IConfiguration.ReadOnly
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt">Db4objects.Db4o.Config.IConfiguration.Encrypt
		/// 	</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">Db4objects.Db4o.Config.IConfiguration.Password
		/// 	</seealso>
		public static Db4objects.Db4o.IObjectServer OpenServer(Db4objects.Db4o.Config.IConfiguration
			 config, string databaseFileName, int port)
		{
			lock (Db4objects.Db4o.Inside.Global4.Lock)
			{
				Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)OpenFile(config, databaseFileName
					);
				if (stream == null)
				{
					return null;
				}
				lock (stream.Lock())
				{
					return new Db4objects.Db4o.CS.YapServer(stream, port);
				}
			}
		}

		internal static Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return i_config.Reflector();
		}

		internal static void ForEachSession(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			i_sessions.ForEach(visitor);
		}

		internal static void SessionStopped(Db4objects.Db4o.Session a_session)
		{
			i_sessions.Remove(a_session);
		}

		/// <summary>returns the version name of the used db4o version.</summary>
		/// <remarks>
		/// returns the version name of the used db4o version.
		/// <br /><br />
		/// </remarks>
		/// <returns>version information as a <code>String</code>.</returns>
		public static string Version()
		{
			return "db4o " + Db4objects.Db4o.Db4oVersion.NAME;
		}
	}
}
