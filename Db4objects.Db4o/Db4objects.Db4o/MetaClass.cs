namespace Db4objects.Db4o
{
	/// <summary>
	/// Class metadata to be stored to the database file
	/// Don't obfuscate.
	/// </summary>
	/// <remarks>
	/// Class metadata to be stored to the database file
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaClass : Db4objects.Db4o.IInternal4
	{
		/// <summary>persistent field, don't touch</summary>
		public string name;

		/// <summary>persistent field, don't touch</summary>
		public Db4objects.Db4o.MetaField[] fields;
	}
}
