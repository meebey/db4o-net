using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.Marshall
{
	/// <exclude></exclude>
	public abstract class ObjectHeaderAttributes
	{
		public abstract void AddBaseLength(int length);

		public abstract void AddPayLoadLength(int length);

		public abstract void PrepareIndexedPayLoadEntry(Transaction trans);
	}
}
