namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IPersistent
	{
		/// <moveto>
		/// new com.db4o.internal.Persistent interface
		/// all four of the following abstract methods
		/// </moveto>
		byte GetIdentifier();

		int OwnLength();

		void ReadThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader);

		void WriteThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer);
	}
}
