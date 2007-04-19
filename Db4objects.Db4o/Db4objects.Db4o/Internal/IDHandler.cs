using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class IDHandler : PrimitiveIntHandler
	{
		public IDHandler(ObjectContainerBase stream) : base(stream)
		{
		}

		public override void DefragIndexEntry(ReaderPair readers)
		{
			readers.CopyID(true, false);
		}
	}
}
