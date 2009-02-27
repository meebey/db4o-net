/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;

namespace Db4objects.Db4o
{
	/// <summary>Factory class to open db4o instances in embedded
	/// mode.</summary>
	/// <remarks> Factory class to open db4o instances in embedded mode.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.CS.Db4oClientServer"> Db4objects.Db4o.CS.Db4oClientServer in
	/// Db4objects.Db4o.CS.dll for methods to open db4o servers and db4o
	/// clients.</seealso>
	/// <since>7.5</since>
	public class Db4oEmbedded
	{
		/// <summary>
		/// Creates a fresh
		/// <see cref="Db4objects.Db4o.Config.IEmbeddedConfiguration">IEmbeddedConfiguration</see>
		/// instance.
		/// </summary>
		/// <returns>a fresh, independent configuration with all options set to their default values
		/// 	</returns>
		public static IEmbeddedConfiguration NewConfiguration()
		{
			return new EmbeddedConfigurationImpl(Db4oFactory.NewConfiguration());
		}

		/// <summary>
		/// opens an
		/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
		/// on the specified database file for local use.
		/// <br/>
		/// <br/>
		/// A database file can only be opened once, subsequent attempts to
		/// open another
		/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
		/// against the same file will result in a
		/// <see cref="Db4objects.Db4o.Ext.DatabaseFileLockedException"> DatabaseFileLockedException</see>
		/// .
		/// <br/>
		/// <br/>
		/// Database files can only be accessed for readwrite access from one
		/// process at one time. All versions except for db4o mobile edition
		/// use an internal mechanism to lock the database file for other
		/// processes.
		/// <br/>
		/// <br/>
		/// </summary>
		/// <param name="config">
		/// a custom
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">IConfiguration</see>
		/// instance to be obtained via
		/// <see cref="newConfiguration">newConfiguration</see>
		/// </param>
		/// <param name="databaseFileName">an absolute or relative path to the database
		/// file</param>
		/// <returns>
		/// an open
		/// <see cref="Db4objects.Db4o.IObjectContainer">IObjectContainer</see>
		/// </returns>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.ReadOnly">
		/// Db4objects.Db4o.Config.IConfiguration.ReadOnly</seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Encrypt"> Db4objects.Db4o.Config.IConfiguration.Encrypt
		/// </seealso>
		/// <seealso cref="Db4objects.Db4o.Config.IConfiguration.Password">
		/// Db4objects.Db4o.Config.IConfiguration.Password</seealso>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException"> I/O operation failed or was unexpectedly
		/// interrupted.</exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseFileLockedException"> the required database file is locked by
		/// another process.</exception>
		/// <exception cref="Db4objects.Db4o.Ext.IncompatibleFileFormatException">
		/// runtime
		/// <see cref="Db4objects.Db4o.Config.IConfiguration">configuration</see>
		/// is not compatible with the configuration of the database file.
		/// </exception>
		/// <exception cref="Db4objects.Db4o.Ext.OldFormatException">
		/// open operation failed because the database file is in old format
		/// and
		/// <see cref="Db4objects.Db4o.Config.IConfiguration.AllowVersionUpdates">
		/// Db4objects.Db4o.Config.IConfiguration.AllowVersionUpdates</see>
		/// is set to false.
		/// </exception>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseReadOnlyException"> database was configured as read-only.
		/// </exception>
		public static IObjectContainer OpenFile(IEmbeddedConfiguration config, string databaseFileName
			)
		{
			if (null == config)
			{
				throw new ArgumentNullException();
			}
			return ObjectContainerFactory.OpenObjectContainer(((ILegacyConfigurationProvider)
				config).Legacy(), databaseFileName);
		}
	}
}
