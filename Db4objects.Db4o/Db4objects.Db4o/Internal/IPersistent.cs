namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IPersistent
	{
		byte GetIdentifier();

		int OwnLength();

		void ReadThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader);

		void WriteThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer);
	}
}
