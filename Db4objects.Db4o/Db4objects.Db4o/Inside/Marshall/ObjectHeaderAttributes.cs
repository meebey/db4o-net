namespace Db4objects.Db4o.Inside.Marshall
{
	/// <exclude></exclude>
	public abstract class ObjectHeaderAttributes
	{
		public abstract void AddBaseLength(int length);

		public abstract void AddPayLoadLength(int length);

		public abstract void PrepareIndexedPayLoadEntry(Db4objects.Db4o.Transaction trans
			);
	}
}
