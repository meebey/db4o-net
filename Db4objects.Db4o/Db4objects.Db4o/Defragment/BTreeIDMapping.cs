namespace Db4objects.Db4o.Defragment
{
	/// <summary>BTree mapping for IDs during a defragmentation run.</summary>
	/// <remarks>BTree mapping for IDs during a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class BTreeIDMapping : Db4objects.Db4o.Defragment.AbstractContextIDMapping
	{
		private string _fileName;

		private Db4objects.Db4o.Internal.LocalObjectContainer _mappingDb;

		private Db4objects.Db4o.Internal.Btree.BTree _idTree;

		private Db4objects.Db4o.Internal.Mapping.MappedIDPair _cache = new Db4objects.Db4o.Internal.Mapping.MappedIDPair
			(0, 0);

		private Db4objects.Db4o.Defragment.BTreeIDMapping.BTreeSpec _treeSpec = null;

		private int _commitFrequency = 0;

		private int _insertCount = 0;

		public BTreeIDMapping(string fileName) : this(fileName, null, 0)
		{
		}

		public BTreeIDMapping(string fileName, int nodeSize, int cacheHeight, int commitFrequency
			) : this(fileName, new Db4objects.Db4o.Defragment.BTreeIDMapping.BTreeSpec(nodeSize
			, cacheHeight), commitFrequency)
		{
		}

		private BTreeIDMapping(string fileName, Db4objects.Db4o.Defragment.BTreeIDMapping.BTreeSpec
			 treeSpec, int commitFrequency)
		{
			_fileName = fileName;
			_treeSpec = treeSpec;
			_commitFrequency = commitFrequency;
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
			Db4objects.Db4o.Internal.Btree.IBTreeRange range = _idTree.Search(Trans(), new Db4objects.Db4o.Internal.Mapping.MappedIDPair
				(oldID, 0));
			System.Collections.IEnumerator pointers = range.Pointers();
			if (pointers.MoveNext())
			{
				Db4objects.Db4o.Internal.Btree.BTreePointer pointer = (Db4objects.Db4o.Internal.Btree.BTreePointer
					)pointers.Current;
				_cache = (Db4objects.Db4o.Internal.Mapping.MappedIDPair)pointer.Key();
				return _cache.Mapped();
			}
			if (lenient)
			{
				return MapLenient(oldID, range);
			}
			return 0;
		}

		private int MapLenient(int oldID, Db4objects.Db4o.Internal.Btree.IBTreeRange range
			)
		{
			range = range.Smaller();
			Db4objects.Db4o.Internal.Btree.BTreePointer pointer = range.LastPointer();
			if (pointer == null)
			{
				return 0;
			}
			Db4objects.Db4o.Internal.Mapping.MappedIDPair mappedIDs = (Db4objects.Db4o.Internal.Mapping.MappedIDPair
				)pointer.Key();
			return mappedIDs.Mapped() + (oldID - mappedIDs.Orig());
		}

		protected override void MapNonClassIDs(int origID, int mappedID)
		{
			_cache = new Db4objects.Db4o.Internal.Mapping.MappedIDPair(origID, mappedID);
			_idTree.Add(Trans(), _cache);
			if (_commitFrequency > 0)
			{
				_insertCount++;
				if (_commitFrequency == _insertCount)
				{
					_idTree.Commit(Trans());
					_insertCount = 0;
				}
			}
		}

		public override void Open()
		{
			_mappingDb = Db4objects.Db4o.Defragment.DefragContextImpl.FreshYapFile(_fileName);
			Db4objects.Db4o.Internal.IX.IIndexable4 handler = new Db4objects.Db4o.Internal.Mapping.MappedIDPairHandler
				(_mappingDb);
			_idTree = (_treeSpec == null ? new Db4objects.Db4o.Internal.Btree.BTree(Trans(), 
				0, handler) : new Db4objects.Db4o.Internal.Btree.BTree(Trans(), 0, handler, _treeSpec
				.NodeSize(), _treeSpec.CacheHeight()));
		}

		public override void Close()
		{
			_mappingDb.Close();
		}

		private Db4objects.Db4o.Internal.Transaction Trans()
		{
			return _mappingDb.GetSystemTransaction();
		}

		private class BTreeSpec
		{
			private int _nodeSize;

			private int _cacheHeight;

			public BTreeSpec(int nodeSize, int cacheHeight)
			{
				_nodeSize = nodeSize;
				_cacheHeight = cacheHeight;
			}

			public virtual int NodeSize()
			{
				return _nodeSize;
			}

			public virtual int CacheHeight()
			{
				return _cacheHeight;
			}
		}
	}
}
