/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Fieldindex;

namespace Db4objects.Db4o.Internal.Fieldindex
{
	internal sealed class IndexedPathIterator : CompositeIterator4
	{
		private IndexedPath _path;

		public IndexedPathIterator(IndexedPath path, IEnumerator iterator) : base(iterator
			)
		{
			_path = path;
		}

		protected override IEnumerator NextIterator(object current)
		{
			FieldIndexKey key = (FieldIndexKey)current;
			return _path.Search(key.ParentID()).Keys();
		}
	}
}
