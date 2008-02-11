/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Mapping;

namespace Db4objects.Db4o.Defragment
{
	/// <summary>BTree mapping for IDs during a defragmentation run.</summary>
	/// <remarks>BTree mapping for IDs during a defragmentation run.</remarks>
	/// <seealso cref="Db4objects.Db4o.Defragment.Defragment">Db4objects.Db4o.Defragment.Defragment
	/// 	</seealso>
	public class BTreeIDMapping : AbstractContextIDMapping
	{
		private string _fileName;

		private LocalObjectContainer _mappingDb;

		private BTree _idTree;

		private MappedIDPair _cache = new MappedIDPair(0, 0);

		private BTreeIDMapping.BTreeSpec _treeSpec = null;

		private int _commitFrequency = 0;

		private int _insertCount = 0;

		/// <summary>Will maintain the ID mapping as a BTree in the file with the given path.
		/// 	</summary>
		/// <remarks>
		/// Will maintain the ID mapping as a BTree in the file with the given path.
		/// If a file exists in this location, it will be DELETED.
		/// Node size and cache height of the tree will be the default values used by
		/// the BTree implementation. The tree will never commit.
		/// </remarks>
		/// <param name="fileName">The location where the BTree file should be created.</param>
		public BTreeIDMapping(string fileName) : this(fileName, null, 0)
		{
		}

		/// <summary>Will maintain the ID mapping as a BTree in the file with the given path.
		/// 	</summary>
		/// <remarks>
		/// Will maintain the ID mapping as a BTree in the file with the given path.
		/// If a file exists in this location, it will be DELETED.
		/// </remarks>
		/// <param name="fileName">The location where the BTree file should be created.</param>
		/// <param name="nodeSize">The size of a BTree node</param>
		/// <param name="cacheHeight">The height of the BTree node cache</param>
		/// <param name="commitFrequency">The number of inserts after which a commit should be issued (&lt;=0: never commit)
		/// 	</param>
		public BTreeIDMapping(string fileName, int nodeSize, int cacheHeight, int commitFrequency
			) : this(fileName, new BTreeIDMapping.BTreeSpec(nodeSize, cacheHeight), commitFrequency
			)
		{
		}

		private BTreeIDMapping(string fileName, BTreeIDMapping.BTreeSpec treeSpec, int commitFrequency
			)
		{
			// <=0 : never commit
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
			IBTreeRange range = _idTree.Search(Trans(), new MappedIDPair(oldID, 0));
			IEnumerator pointers = range.Pointers();
			if (pointers.MoveNext())
			{
				BTreePointer pointer = (BTreePointer)pointers.Current;
				_cache = (MappedIDPair)pointer.Key();
				return _cache.Mapped();
			}
			if (lenient)
			{
				return MapLenient(oldID, range);
			}
			return 0;
		}

		private int MapLenient(int oldID, IBTreeRange range)
		{
			range = range.Smaller();
			BTreePointer pointer = range.LastPointer();
			if (pointer == null)
			{
				return 0;
			}
			MappedIDPair mappedIDs = (MappedIDPair)pointer.Key();
			return mappedIDs.Mapped() + (oldID - mappedIDs.Orig());
		}

		protected override void MapNonClassIDs(int origID, int mappedID)
		{
			_cache = new MappedIDPair(origID, mappedID);
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
			_mappingDb = DefragmentServicesImpl.FreshYapFile(_fileName, 1);
			IIndexable4 handler = new MappedIDPairHandler(_mappingDb);
			_idTree = (_treeSpec == null ? new BTree(Trans(), 0, handler) : new BTree(Trans()
				, 0, handler, _treeSpec.NodeSize(), _treeSpec.CacheHeight()));
		}

		public override void Close()
		{
			_mappingDb.Close();
		}

		private Transaction Trans()
		{
			return _mappingDb.SystemTransaction();
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
