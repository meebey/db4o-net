namespace Db4objects.Db4o
{
	/// <summary>The index record that is written to the database file.</summary>
	/// <remarks>
	/// The index record that is written to the database file.
	/// Don't obfuscate.
	/// </remarks>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class MetaIndex : Db4objects.Db4o.IInternal4
	{
		public int indexAddress;

		public int indexEntries;

		public int indexLength;

		private const int patchAddress = 0;

		private const int patchEntries = 0;

		private const int patchLength = 0;

		public virtual void Read(Db4objects.Db4o.YapReader reader)
		{
			indexAddress = reader.ReadInt();
			indexEntries = reader.ReadInt();
			indexLength = reader.ReadInt();
			reader.ReadInt();
			reader.ReadInt();
			reader.ReadInt();
		}

		public virtual void Write(Db4objects.Db4o.YapReader writer)
		{
			writer.WriteInt(indexAddress);
			writer.WriteInt(indexEntries);
			writer.WriteInt(indexLength);
			writer.WriteInt(patchAddress);
			writer.WriteInt(patchEntries);
			writer.WriteInt(patchLength);
		}

		public virtual void Free(Db4objects.Db4o.YapFile file)
		{
			file.Free(indexAddress, indexLength);
			indexAddress = 0;
			indexLength = 0;
		}
	}
}
