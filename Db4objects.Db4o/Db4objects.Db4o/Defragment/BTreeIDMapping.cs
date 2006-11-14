namespace Db4objects.Db4o.Defragment
{
	/// <summary>BTree mapping for IDs during a defragmentation run.</summary>
	/// <remarks>BTree mapping for IDs during a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	internal class BTreeIDMapping : Db4objects.Db4o.Defragment.AbstractContextIDMapping
	{
		private string _fileName;

		private Db4objects.Db4o.YapFile _mappingDb;

		private Db4objects.Db4o.Inside.Btree.BTree _idTree;

		private Db4objects.Db4o.Inside.Mapping.MappedIDPair _cache = new Db4objects.Db4o.Inside.Mapping.MappedIDPair
			(0, 0);

		public BTreeIDMapping(string fileName)
		{
			_fileName = fileName;
		}

		public override int MappedID(int oldID, bool lenient)
		{
			if (_cache.Orig() == oldID)
			{
				return _cache.Mapped();
			}
			int classID = MappedClassID(oldID);
			if (classID != 0)
			{
				return classID;
			}
			Db4objects.Db4o.Inside.Btree.IBTreeRange range = _idTree.Search(Trans(), new Db4objects.Db4o.Inside.Mapping.MappedIDPair
				(oldID, 0));
			System.Collections.IEnumerator pointers = range.Pointers();
			if (pointers.MoveNext())
			{
				Db4objects.Db4o.Inside.Btree.BTreePointer pointer = (Db4objects.Db4o.Inside.Btree.BTreePointer
					)pointers.Current;
				_cache = (Db4objects.Db4o.Inside.Mapping.MappedIDPair)pointer.Key();
				return _cache.Mapped();
			}
			if (lenient)
			{
				return MapLenient(oldID, range);
			}
			return 0;
		}

		private int MapLenient(int oldID, Db4objects.Db4o.Inside.Btree.IBTreeRange range)
		{
			range = range.Smaller();
			Db4objects.Db4o.Inside.Btree.BTreePointer pointer = range.LastPointer();
			if (pointer == null)
			{
				return 0;
			}
			Db4objects.Db4o.Inside.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Inside.Mapping.MappedIDPair
				)pointer.Key();
			return mappedIDs.Mapped() + (oldID - mappedIDs.Orig());
		}

		protected override void MapNonClassIDs(int origID, int mappedID)
		{
			_cache = new Db4objects.Db4o.Inside.Mapping.MappedIDPair(origID, mappedID);
			_idTree.Add(Trans(), _cache);
		}

		public override void Open()
		{
			_mappingDb = Db4objects.Db4o.Defragment.DefragContextImpl.FreshYapFile(_fileName);
			Db4objects.Db4o.Inside.IX.IIndexable4 handler = new Db4objects.Db4o.Inside.Mapping.MappedIDPairHandler
				(_mappingDb);
			_idTree = new Db4objects.Db4o.Inside.Btree.BTree(Trans(), 0, handler);
		}

		public override void Close()
		{
			_mappingDb.Close();
		}

		private Db4objects.Db4o.Transaction Trans()
		{
			return _mappingDb.GetSystemTransaction();
		}
	}
}
