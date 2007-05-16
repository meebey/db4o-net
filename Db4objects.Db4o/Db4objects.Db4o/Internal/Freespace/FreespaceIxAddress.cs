/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;

namespace Db4objects.Db4o.Internal.Freespace
{
	internal class FreespaceIxAddress : FreespaceIx
	{
		internal FreespaceIxAddress(LocalObjectContainer file, MetaIndex metaIndex) : base
			(file, metaIndex)
		{
		}

		internal override void Add(int address, int length)
		{
			_index._handler.PrepareComparison(address);
			_indexTrans.Add(length, address);
		}

		internal override int Address()
		{
			return _visitor._value;
		}

		internal override int Length()
		{
			return _visitor._key;
		}

		internal override void Remove(int address, int length)
		{
			_index._handler.PrepareComparison(address);
			_indexTrans.Remove(length, address);
		}
	}
}
