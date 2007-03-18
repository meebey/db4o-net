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
	/// <seealso cref="Db4objects.Db4o.Db4oFactory.OpenFile">Db4objects.Db4o.Db4oFactory.OpenFile
	/// 	</seealso>
	[System.Serializable]
	public class DatabaseFileLockedException : System.Exception
	{
		private string _databaseDescription;

		public DatabaseFileLockedException(string databaseDescription) : this(databaseDescription
			, null)
		{
		}

		public DatabaseFileLockedException(string databaseDescription, System.Exception cause
			) : base(Message(databaseDescription), cause)
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
