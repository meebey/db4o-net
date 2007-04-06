using System.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Fieldindex;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	public interface IIndexedNode : IEnumerable
	{
		bool IsResolved();

		IIndexedNode Resolve();

		BTree GetIndex();

		int ResultSize();

		TreeInt ToTreeInt();
	}
}
