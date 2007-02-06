namespace Db4objects.Db4o.Internal.Fieldindex
{
	internal sealed class IndexedPathIterator : Db4objects.Db4o.Foundation.CompositeIterator4
	{
		private Db4objects.Db4o.Internal.Fieldindex.IndexedPath _path;

		public IndexedPathIterator(Db4objects.Db4o.Internal.Fieldindex.IndexedPath path, 
			System.Collections.IEnumerator iterator) : base(iterator)
		{
			_path = path;
		}

		protected override System.Collections.IEnumerator NextIterator(object current)
		{
			Db4objects.Db4o.Internal.Btree.FieldIndexKey key = (Db4objects.Db4o.Internal.Btree.FieldIndexKey
				)current;
			return _path.Search(key.ParentID()).Keys();
		}
	}
}
