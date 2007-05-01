using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;

namespace Db4objects.Db4o.Internal.Freespace
{
	internal class FreespaceIxLength : FreespaceIx
	{
		internal FreespaceIxLength(LocalObjectContainer file, MetaIndex metaIndex) : base
			(file, metaIndex)
		{
		}

		internal override void Add(int address, int length)
		{
			_index._handler.PrepareComparison(length);
			_indexTrans.Add(address, length);
		}

		internal override int Address()
		{
			return _visitor._key;
		}

		internal override int Length()
		{
			return _visitor._value;
		}

		internal override void Remove(int address, int length)
		{
			_index._handler.PrepareComparison(length);
			_indexTrans.Remove(address, length);
		}

		public override string ToString()
		{
			return _indexTrans.ToString();
		}
	}
}
