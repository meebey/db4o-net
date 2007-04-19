using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public interface IPersistent
	{
		byte GetIdentifier();

		int OwnLength();

		void ReadThis(Transaction trans, Db4objects.Db4o.Internal.Buffer reader);

		void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer writer);
	}
}
