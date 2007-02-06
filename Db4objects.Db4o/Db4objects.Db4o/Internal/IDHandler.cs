namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IDHandler : Db4objects.Db4o.Internal.PrimitiveIntHandler
	{
		public IDHandler(Db4objects.Db4o.Internal.ObjectContainerBase stream) : base(stream
			)
		{
		}

		public override void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers
			)
		{
			readers.CopyID(true, false);
		}
	}
}
