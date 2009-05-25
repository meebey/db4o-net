/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;

namespace Db4objects.Db4o
{
	/// <summary>
	/// Represents a local ObjectContainer attached to a
	/// database file.
	/// </summary>
	/// <remarks>
	/// Represents a local ObjectContainer attached to a
	/// database file.
	/// </remarks>
	/// <since>7.10</since>
	public interface IEmbeddedObjectContainer : IObjectContainer
	{
		/// <summary>opens a new ObjectContainer against the database file.</summary>
		/// <remarks>
		/// opens a new ObjectContainer against the database file.
		/// The ObjectContainer will have it's own transaction and
		/// it's own reference system.
		/// </remarks>
		/// <returns>the new ObjectContainer session.</returns>
		/// <since>7.10</since>
		IObjectContainer OpenSession();

		/// <summary>backs up a database file of an open ObjectContainer.</summary>
		/// <remarks>
		/// backs up a database file of an open ObjectContainer.
		/// <br /><br />While the backup is running, the ObjectContainer can continue to be
		/// used. Changes that are made while the backup is in progress, will be applied to
		/// the open ObjectContainer and to the backup.<br /><br />
		/// While the backup is running, the ObjectContainer should not be closed.<br /><br />
		/// If a file already exists at the specified path, it will be overwritten.<br /><br />
		/// The
		/// <see cref="Db4objects.Db4o.IO.IStorage">Db4objects.Db4o.IO.IStorage</see>
		/// used for backup is the one configured for this container.
		/// </remarks>
		/// <param name="path">a fully qualified path</param>
		/// <exception cref="Db4objects.Db4o.Ext.DatabaseClosedException">db4o database file was closed or failed to open.
		/// 	</exception>
		/// <exception cref="System.NotSupportedException">
		/// is thrown when the operation is not supported in current
		/// configuration/environment
		/// </exception>
		/// <exception cref="Db4objects.Db4o.Ext.Db4oIOException">I/O operation failed or was unexpectedly interrupted.
		/// 	</exception>
		void Backup(string path);
	}
}
