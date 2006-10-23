namespace Db4objects.Db4o
{
	/// <summary>Field MetaData to be stored to the database file.</summary>
	/// <remarks>
	/// Field MetaData to be stored to the database file.
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaField : Db4objects.Db4o.IInternal4
	{
		public string name;

		public Db4objects.Db4o.MetaIndex index;

		public MetaField()
		{
		}

		public MetaField(string name_)
		{
			name = name_;
		}
	}
}
