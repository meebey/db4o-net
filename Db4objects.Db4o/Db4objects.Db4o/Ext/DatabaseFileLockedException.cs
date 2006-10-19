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
	/// <seealso cref="Db4objects.Db4o.Db4o.OpenFile">Db4objects.Db4o.Db4o.OpenFile</seealso>
	[System.Serializable]
	public class DatabaseFileLockedException : System.Exception
	{
	}
}
