/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o
{
	/// <summary>factory class to start db4o database engines.</summary>
	/// <remarks>
	/// factory class to start db4o database engines.
	/// <br /><br />This class provides static methods to<br />
	/// - open single-user databases
	/// <see cref="OpenFile(string)">OpenFile(string)</see>
	/// <br />
	/// - open db4o servers
	/// <see cref="#openServer(String,int)">#openServer(String,int)</see>
	/// <br />
	/// - connect to db4o servers
	/// <see cref="#openClient(String,int,String,String)">#openClient(String,int,String,String)
	/// 	</see>
	/// <br />
	/// - provide access to the global configuration context
	/// <see cref="Configure()">Configure()</see>
	/// <br />
	/// - print the version number of this db4o version
	/// <see cref="Main(java.lang.String[])">Main(java.lang.String[])</see>
	/// 
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Ext.ExtDb4oFactory">ExtDb4o for extended functionality.
	/// 	</seealso>
	public class Db4oFactory
	{
		internal static readonly Config4Impl i_config = new Config4Impl();

		static Db4oFactory()
		{
			Platform4.GetDefaultConfiguration(i_config);
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
		/// <see cref="IConfiguration">IConfiguration</see>
		/// context
		/// for the running CLR session.
		/// <br/><br/>
		/// The
		/// <see cref="IConfiguration">IConfiguration</see>
		/// can be overriden in each
		/// <see cref="IExtObjectContainer.Configure">ObjectContainer</see>
		/// .<br/><br/>
		/// </summary>
		/// <returns>
		/// the global
		/// <see cref="IConfiguration">configuration</see>
		/// context
		/// 
		/// </returns>
		public static IConfiguration Configure()
		{
			return i_config;
		}

		/// <summary>
		/// Creates a fresh
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">IConfiguration</see>
		/// instance.
		/// </summary>
		/// <returns>a fresh, independent configuration with all options set to their default values
		/// 	</returns>
		[System.ObsoleteAttribute(@"Use Db4oEmbedded.NewConfiguration() instead.")]
		public static IConfiguration NewConfiguration()
		{
			Config4Impl config = new Config4Impl();
			Platform4.GetDefaultConfiguration(config);
			return config;
		}

		/// <summary>
		/// Creates a clone of the global db4o
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">IConfiguration</see>
		/// .
		/// </summary>
		/// <returns>
		/// a fresh configuration with all option values set to the values
		/// currently configured for the global db4o configuration context
		/// </returns>
		[System.ObsoleteAttribute(@"use explicit configuration via Db4oEmbedded.NewConfiguration() instead"
			)]
		public static IConfiguration CloneConfiguration()
		{
			return (Config4Impl)((IDeepClone)Db4oFactory.Configure()).DeepClone(null);
		}

		/// <summary>
		/// Operates just like
		/// <see cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</see>
		/// , but uses
		/// the global db4o
		/// <see cref="IConfiguration">IConfiguration</see>
		/// context.
		/// opens an
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// on the specified database file for local use.
		/// <br/><br/>A database file can only be opened once, subsequent attempts to open
		/// another
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// against the same file will result in
		/// a
		/// <see cref="DatabaseFileLockedException">DatabaseFileLockedException</see>
		/// .<br/><br/>
		/// Database files can only be accessed for readwrite access from one process
		/// at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br/><br/>
		/// 
		/// </summary>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// 
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		/// <exception cref="Db4oIOException">
		/// I/O operation failed or was unexpectedly interrupted.
		/// 
		/// </exception>
		/// <exception cref="DatabaseFileLockedException">
		/// the required database file is locked by
		/// another process.
		/// 
		/// </exception>
		/// <exception cref="IncompatibleFileFormatException">
		/// runtime
		/// <see cref="IConfiguration">configuration</see>
		/// is not compatible
		/// with the configuration of the database file.
		/// 
		/// </exception>
		/// <exception cref="OldFormatException">
		/// open operation failed because the database file
		/// is in old format and
		/// <see cref="IConfiguration.AllowVersionUpdates">
		/// IConfiguration.AllowVersionUpdates
		/// </see>
		/// is set to false.
		/// </exception>
		/// <exception cref="DatabaseReadOnlyException">
		/// database was configured as read-only.
		/// </exception>
		public static IObjectContainer OpenFile(string databaseFileName)
		{
			return Db4oFactory.OpenFile(CloneConfiguration(), databaseFileName);
		}

		/// <summary>
		/// opens an
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// on the specified database file for local use.
		/// <br/><br/>A database file can only be opened once, subsequent attempts to open
		/// another
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// against the same file will result in
		/// a
		/// <see cref="DatabaseFileLockedException">DatabaseFileLockedException</see>
		/// .<br/><br/>
		/// Database files can only be accessed for readwrite access from one process
		/// at one time. All versions except for db4o mobile edition use an
		/// internal mechanism to lock the database file for other processes.
		/// <br/><br/>
		/// 
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="IConfiguration">IConfiguration</see>
		/// instance to be obtained via
		/// <see cref="Db4oFactory.NewConfiguration">Db4oFactory.NewConfiguration</see>
		/// 
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database file</param>
		/// <returns>
		/// an open
		/// <see cref="IObjectContainer">IObjectContainer</see>
		/// 
		/// </returns>
		/// <seealso cref="IConfiguration.ReadOnly">IConfiguration.ReadOnly</seealso>
		/// <seealso cref="IConfiguration.Encrypt">IConfiguration.Encrypt</seealso>
		/// <seealso cref="IConfiguration.Password">IConfiguration.Password</seealso>
		/// <exception cref="Db4oIOException">
		/// I/O operation failed or was unexpectedly interrupted.
		/// 
		/// </exception>
		/// <exception cref="DatabaseFileLockedException">
		/// the required database file is locked by
		/// another process.
		/// 
		/// </exception>
		/// <exception cref="IncompatibleFileFormatException">
		/// runtime
		/// <see cref="IConfiguration">configuration</see>
		/// is not compatible
		/// with the configuration of the database file.
		/// 
		/// </exception>
		/// <exception cref="OldFormatException">
		/// open operation failed because the database file
		/// is in old format and
		/// <see cref="IConfiguration.AllowVersionUpdates">
		/// IConfiguration.AllowVersionUpdates
		/// 
		/// </see>
		/// 
		/// is set to false.
		/// 
		/// </exception>
		/// <exception cref="DatabaseReadOnlyException">
		/// database was configured as read-only.
		/// 
		/// </exception>
		public static IObjectContainer OpenFile(IConfiguration config, string databaseFileName
			)
		{
			return ObjectContainerFactory.OpenObjectContainer(config, databaseFileName);
		}

		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseFileLockedException"></exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException"></exception>
		protected static IObjectContainer OpenMemoryFile1(IConfiguration config, MemoryFile
			 memoryFile)
		{
			Config4Impl.AssertIsNotTainted(config);
			if (memoryFile == null)
			{
				memoryFile = new MemoryFile();
			}
			IObjectContainer oc = new InMemoryObjectContainer(config, memoryFile);
			Db4objects.Db4o.Internal.Messages.LogMsg(config, 5, "Memory File");
			return oc;
		}

		internal static IReflector Reflector()
		{
			return i_config.Reflector();
		}

		/// <summary>returns the version name of the used db4o version.</summary>
		/// <remarks>
		/// returns the version name of the used db4o version.
		/// <br /><br />
		/// </remarks>
		/// <returns>version information as a <code>String</code>.</returns>
		public static string Version()
		{
			return "db4o " + Db4oVersion.Name;
		}
	}
}
