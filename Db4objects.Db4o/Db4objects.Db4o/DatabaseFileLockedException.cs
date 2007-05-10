using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o
{
	/// <summary>
	/// this Exception is thrown during any of the db4o open calls
	/// if the database file is locked by another process.
	/// </summary>
	/// <remarks>
	/// this Exception is thrown during any of the db4o open calls
	/// if the database file is locked by another process.
	/// </remarks>
	/// <seealso cref="Db4oFactory.OpenFile">Db4oFactory.OpenFile</seealso>
	[System.Serializable]
	public class DatabaseFileLockedException : Db4oException
	{
		public DatabaseFileLockedException(string databaseDescription) : base(databaseDescription
			)
		{
		}

		public DatabaseFileLockedException(string databaseDescription, Exception cause) : 
			base(databaseDescription, cause)
		{
		}
	}
}
