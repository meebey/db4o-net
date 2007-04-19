using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Fieldindex;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public interface IIndexedNodeWithRange : IIndexedNode
	{
		IBTreeRange GetRange();
	}
}
