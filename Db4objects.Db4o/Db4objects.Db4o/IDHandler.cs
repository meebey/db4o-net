namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class IDHandler : Db4objects.Db4o.PrimitiveIntHandler
	{
		public IDHandler(Db4objects.Db4o.YapStream stream) : base(stream)
		{
		}

		public override void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers)
		{
			readers.CopyID(true, false);
		}
	}
}
