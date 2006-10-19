namespace Db4objects.Db4o.Inside.Freespace
{
	internal class FreespaceIxLength : Db4objects.Db4o.Inside.Freespace.FreespaceIx
	{
		internal FreespaceIxLength(Db4objects.Db4o.YapFile file, Db4objects.Db4o.MetaIndex
			 metaIndex) : base(file, metaIndex)
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
	}
}
