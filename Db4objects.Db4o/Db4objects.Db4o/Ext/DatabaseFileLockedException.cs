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
		public DatabaseFileLockedException()
		{
		}

		public DatabaseFileLockedException(System.Exception cause) : base(cause.Message, 
			cause)
		{
		}
	}
}
