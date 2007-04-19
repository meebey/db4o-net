using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Db4objects.Db4o.Ext
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
		private string _databaseDescription;

		public DatabaseFileLockedException(string databaseDescription) : this(databaseDescription
			, null)
		{
		}

		public DatabaseFileLockedException(string databaseDescription, Exception cause) : 
			base(Message(databaseDescription), cause)
		{
			_databaseDescription = databaseDescription;
		}

		public virtual string DatabaseDescription()
		{
			return _databaseDescription;
		}

		private static string Message(string databaseDescription)
		{
			return "Database locked: '" + databaseDescription + "'";
		}
	}
}
