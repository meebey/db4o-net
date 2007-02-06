namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTree : Db4objects.Db4o.Internal.PersistentBase, Db4objects.Db4o.Internal.ITransactionParticipant
	{
		private const byte BTREE_VERSION = (byte)1;

		private const int DEFRAGMENT_INCREMENT_OFFSET = 1 + Db4objects.Db4o.Internal.Const4
			.INT_LENGTH * 2;

		private readonly Db4objects.Db4o.Internal.IX.IIndexable4 _keyHandler;

		internal readonly Db4objects.Db4o.Internal.IX.IIndexable4 _valueHandler;

		private Db4objects.Db4o.Internal.Btree.BTreeNode _root;

		/// <summary>All instantiated nodes are held in this tree.</summary>
		/// <remarks>All instantiated nodes are held in this tree.</remarks>
		private Db4objects.Db4o.Internal.TreeIntObject _nodes;

		private int _size;

		private Db4objects.Db4o.Foundation.IVisitor4 _removeListener;

		private Db4objects.Db4o.Foundation.Hashtable4 _sizesByTransaction;

		protected Db4objects.Db4o.Foundation.Queue4 _processing;

		private int _nodeSize;

		internal int _halfNodeSize;

		private readonly int _cacheHeight;

		public BTree(Db4objects.Db4o.Internal.Transaction trans, int id, Db4objects.Db4o.Internal.IX.IIndexable4
			 keyHandler) : this(trans, id, keyHandler, null)
		{
		}

		public BTree(Db4objects.Db4o.Internal.Transaction trans, int id, Db4objects.Db4o.Internal.IX.IIndexable4
			 keyHandler, Db4objects.Db4o.Internal.IX.IIndexable4 valueHandler) : this(trans, 
			id, keyHandler, valueHandler, Config(trans).BTreeNodeSize(), Config(trans).BTreeCacheHeight
			())
		{
		}

		public BTree(Db4objects.Db4o.Internal.Transaction trans, int id, Db4objects.Db4o.Internal.IX.IIndexable4
			 keyHandler, Db4objects.Db4o.Internal.IX.IIndexable4 valueHandler, int treeNodeSize
			, int treeCacheHeight)
		{
			if (null == keyHandler)
			{
				throw new System.ArgumentNullException();
			}
			_nodeSize = treeNodeSize;
			_halfNodeSize = _nodeSize / 2;
			_nodeSize = _halfNodeSize * 2;
			_cacheHeight = treeCacheHeight;
			_keyHandler = keyHandler;
			_valueHandler = (valueHandler == null) ? Db4objects.Db4o.Internal.Null.INSTANCE : 
				valueHandler;
			_sizesByTransaction = new Db4objects.Db4o.Foundation.Hashtable4();
			if (id == 0)
			{
				SetStateDirty();
				_root = new Db4objects.Db4o.Internal.Btree.BTreeNode(this, 0, true, 0, 0, 0);
				_root.Write(trans.SystemTransaction());
				AddNode(_root);
				Write(trans.SystemTransaction());
			}
			else
			{
				SetID(id);
				SetStateDeactivated();
			}
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreeNode Root()
		{
			return _root;
		}

		public virtual int NodeSize()
		{
			return _nodeSize;
		}

		public virtual void Add(Db4objects.Db4o.Internal.Transaction trans, object key)
		{
			Add(trans, key, null);
		}

		public virtual void Add(Db4objects.Db4o.Internal.Transaction trans, object key, object
			 value)
		{
			KeyCantBeNull(key);
			_keyHandler.PrepareComparison(key);
			_valueHandler.PrepareComparison(value);
			EnsureDirty(trans);
			Db4objects.Db4o.Internal.Btree.BTreeNode rootOrSplit = _root.Add(trans);
			if (rootOrSplit != null && rootOrSplit != _root)
			{
				_root = new Db4objects.Db4o.Internal.Btree.BTreeNode(trans, _root, rootOrSplit);
				_root.Write(trans.SystemTransaction());
				AddNode(_root);
			}
		}

		public virtual void Remove(Db4objects.Db4o.Internal.Transaction trans, object key
			)
		{
			KeyCantBeNull(key);
			System.Collections.IEnumerator pointers = Search(trans, key).Pointers();
			if (!pointers.MoveNext())
			{
				return;
			}
			Db4objects.Db4o.Internal.Btree.BTreePointer first = (Db4objects.Db4o.Internal.Btree.BTreePointer
				)pointers.Current;
			EnsureDirty(trans);
			Db4objects.Db4o.Internal.Btree.BTreeNode node = first.Node();
			node.Remove(trans, first.Index());
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange Search(Db4objects.Db4o.Internal.Transaction
			 trans, object key)
		{
			KeyCantBeNull(key);
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult start = SearchLeaf(trans, key
				, Db4objects.Db4o.Internal.Btree.SearchTarget.LOWEST);
			Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult end = SearchLeaf(trans, key, 
				Db4objects.Db4o.Internal.Btree.SearchTarget.HIGHEST);
			return start.CreateIncludingRange(end);
		}

		private void KeyCantBeNull(object key)
		{
			if (null == key)
			{
				throw new System.ArgumentNullException();
			}
		}

		public virtual Db4objects.Db4o.Internal.IX.IIndexable4 KeyHandler()
		{
			return _keyHandler;
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult SearchLeaf(Db4objects.Db4o.Internal.Transaction
			 trans, object key, Db4objects.Db4o.Internal.Btree.SearchTarget target)
		{
			EnsureActive(trans);
			_keyHandler.PrepareComparison(key);
			return _root.SearchLeaf(trans, target);
		}

		public virtual void Commit(Db4objects.Db4o.Internal.Transaction trans)
		{
			Db4objects.Db4o.Internal.Transaction systemTransaction = trans.SystemTransaction(
				);
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff != null)
			{
				_size += ((int)sizeDiff);
			}
			_sizesByTransaction.Remove(trans);
			if (_nodes != null)
			{
				ProcessAllNodes();
				while (_processing.HasNext())
				{
					((Db4objects.Db4o.Internal.Btree.BTreeNode)_processing.Next()).Commit(trans);
				}
				_processing = null;
				WriteAllNodes(systemTransaction, true);
			}
			SetStateDirty();
			Write(systemTransaction);
			Purge();
		}

		public virtual void Rollback(Db4objects.Db4o.Internal.Transaction trans)
		{
			Db4objects.Db4o.Internal.Transaction systemTransaction = trans.SystemTransaction(
				);
			_sizesByTransaction.Remove(trans);
			if (_nodes == null)
			{
				return;
			}
			ProcessAllNodes();
			while (_processing.HasNext())
			{
				((Db4objects.Db4o.Internal.Btree.BTreeNode)_processing.Next()).Rollback(trans);
			}
			_processing = null;
			WriteAllNodes(systemTransaction, false);
			SetStateDirty();
			Write(systemTransaction);
			Purge();
		}

		private void WriteAllNodes(Db4objects.Db4o.Internal.Transaction systemTransaction
			, bool setDirty)
		{
			if (_nodes == null)
			{
				return;
			}
			_nodes.Traverse(new _AnonymousInnerClass202(this, setDirty, systemTransaction));
		}

		private sealed class _AnonymousInnerClass202 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass202(BTree _enclosing, bool setDirty, Db4objects.Db4o.Internal.Transaction
				 systemTransaction)
			{
				this._enclosing = _enclosing;
				this.setDirty = setDirty;
				this.systemTransaction = systemTransaction;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode node = (Db4objects.Db4o.Internal.Btree.BTreeNode
					)((Db4objects.Db4o.Internal.TreeIntObject)obj).GetObject();
				if (setDirty)
				{
					node.SetStateDirty();
				}
				node.Write(systemTransaction);
			}

			private readonly BTree _enclosing;

			private readonly bool setDirty;

			private readonly Db4objects.Db4o.Internal.Transaction systemTransaction;
		}

		private void Purge()
		{
			if (_nodes == null)
			{
				return;
			}
			Db4objects.Db4o.Foundation.Tree temp = _nodes;
			_nodes = null;
			if (_cacheHeight > 0)
			{
				_root.MarkAsCached(_cacheHeight);
			}
			else
			{
				_root.HoldChildrenAsIDs();
				AddNode(_root);
			}
			temp.Traverse(new _AnonymousInnerClass229(this));
		}

		private sealed class _AnonymousInnerClass229 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass229(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode node = (Db4objects.Db4o.Internal.Btree.BTreeNode
					)((Db4objects.Db4o.Internal.TreeIntObject)obj).GetObject();
				node.Purge();
			}

			private readonly BTree _enclosing;
		}

		private void ProcessAllNodes()
		{
			_processing = new Db4objects.Db4o.Foundation.Queue4();
			_nodes.Traverse(new _AnonymousInnerClass239(this));
		}

		private sealed class _AnonymousInnerClass239 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass239(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing._processing.Add(((Db4objects.Db4o.Internal.TreeIntObject)obj).GetObject
					());
			}

			private readonly BTree _enclosing;
		}

		private void EnsureActive(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (!IsActive())
			{
				Read(trans.SystemTransaction());
			}
		}

		private void EnsureDirty(Db4objects.Db4o.Internal.Transaction trans)
		{
			EnsureActive(trans);
			trans.Enlist(this);
			SetStateDirty();
		}

		public override byte GetIdentifier()
		{
			return Db4objects.Db4o.Internal.Const4.BTREE;
		}

		public virtual void SetRemoveListener(Db4objects.Db4o.Foundation.IVisitor4 vis)
		{
			_removeListener = vis;
		}

		public override int OwnLength()
		{
			return 1 + Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + (Db4objects.Db4o.Internal.Const4
				.INT_LENGTH * 2) + Db4objects.Db4o.Internal.Const4.ID_LENGTH;
		}

		internal virtual Db4objects.Db4o.Internal.Btree.BTreeNode ProduceNode(int id)
		{
			Db4objects.Db4o.Internal.TreeIntObject addtio = new Db4objects.Db4o.Internal.TreeIntObject
				(id);
			_nodes = (Db4objects.Db4o.Internal.TreeIntObject)Db4objects.Db4o.Foundation.Tree.
				Add(_nodes, addtio);
			Db4objects.Db4o.Internal.TreeIntObject tio = (Db4objects.Db4o.Internal.TreeIntObject
				)addtio.AddedOrExisting();
			Db4objects.Db4o.Internal.Btree.BTreeNode node = (Db4objects.Db4o.Internal.Btree.BTreeNode
				)tio.GetObject();
			if (node == null)
			{
				node = new Db4objects.Db4o.Internal.Btree.BTreeNode(id, this);
				tio.SetObject(node);
				AddToProcessing(node);
			}
			return node;
		}

		internal virtual void AddNode(Db4objects.Db4o.Internal.Btree.BTreeNode node)
		{
			_nodes = (Db4objects.Db4o.Internal.TreeIntObject)Db4objects.Db4o.Foundation.Tree.
				Add(_nodes, new Db4objects.Db4o.Internal.TreeIntObject(node.GetID(), node));
			AddToProcessing(node);
		}

		internal virtual void AddToProcessing(Db4objects.Db4o.Internal.Btree.BTreeNode node
			)
		{
			if (_processing != null)
			{
				_processing.Add(node);
			}
		}

		internal virtual void RemoveNode(Db4objects.Db4o.Internal.Btree.BTreeNode node)
		{
			_nodes = (Db4objects.Db4o.Internal.TreeIntObject)_nodes.RemoveLike(new Db4objects.Db4o.Internal.TreeInt
				(node.GetID()));
		}

		internal virtual void NotifyRemoveListener(object obj)
		{
			if (_removeListener != null)
			{
				_removeListener.Visit(obj);
			}
		}

		public override void ReadThis(Db4objects.Db4o.Internal.Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			a_reader.IncrementOffset(1);
			_size = a_reader.ReadInt();
			_nodeSize = a_reader.ReadInt();
			_halfNodeSize = NodeSize() / 2;
			_root = ProduceNode(a_reader.ReadInt());
		}

		public override void WriteThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 a_writer)
		{
			a_writer.Append(BTREE_VERSION);
			a_writer.WriteInt(_size);
			a_writer.WriteInt(NodeSize());
			a_writer.WriteIDOf(trans, _root);
		}

		public virtual int Size(Db4objects.Db4o.Internal.Transaction trans)
		{
			EnsureActive(trans);
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff != null)
			{
				return _size + ((int)sizeDiff);
			}
			return _size;
		}

		public virtual void TraverseKeys(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			EnsureActive(trans);
			if (_root == null)
			{
				return;
			}
			_root.TraverseKeys(trans, visitor);
		}

		public virtual void TraverseValues(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			EnsureActive(trans);
			if (_root == null)
			{
				return;
			}
			_root.TraverseValues(trans, visitor);
		}

		public virtual void SizeChanged(Db4objects.Db4o.Internal.Transaction trans, int changeBy
			)
		{
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff == null)
			{
				_sizesByTransaction.Put(trans, changeBy);
				return;
			}
			_sizesByTransaction.Put(trans, ((int)sizeDiff) + changeBy);
		}

		public virtual void Dispose(Db4objects.Db4o.Internal.Transaction transaction)
		{
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreePointer FirstPointer(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			EnsureActive(trans);
			if (null == _root)
			{
				return null;
			}
			return _root.FirstPointer(trans);
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTreePointer LastPointer(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			EnsureActive(trans);
			if (null == _root)
			{
				return null;
			}
			return _root.LastPointer(trans);
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTree DebugLoadFully(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			EnsureActive(trans);
			_root.DebugLoadFully(trans);
			return this;
		}

		private void TraverseAllNodes(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			EnsureActive(trans);
			_root.TraverseAllNodes(trans, command);
		}

		public virtual void DefragIndex(Db4objects.Db4o.Internal.ReaderPair readers)
		{
			readers.IncrementOffset(DEFRAGMENT_INCREMENT_OFFSET);
			readers.CopyID();
		}

		public virtual void DefragIndexNode(Db4objects.Db4o.Internal.ReaderPair readers)
		{
			Db4objects.Db4o.Internal.Btree.BTreeNode.DefragIndex(readers, _keyHandler, _valueHandler
				);
		}

		public virtual void DefragBTree(Db4objects.Db4o.Internal.Mapping.IDefragContext context
			)
		{
			Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, GetID(), new _AnonymousInnerClass400
				(this));
			Db4objects.Db4o.CorruptionException[] exc = { null };
			try
			{
				context.TraverseAllIndexSlots(this, new _AnonymousInnerClass407(this, context, exc
					));
			}
			catch (System.Exception e)
			{
				if (exc[0] != null)
				{
					throw exc[0];
				}
				throw;
			}
		}

		private sealed class _AnonymousInnerClass400 : Db4objects.Db4o.Internal.ISlotCopyHandler
		{
			public _AnonymousInnerClass400(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
			{
				this._enclosing.DefragIndex(readers);
			}

			private readonly BTree _enclosing;
		}

		private sealed class _AnonymousInnerClass407 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass407(BTree _enclosing, Db4objects.Db4o.Internal.Mapping.IDefragContext
				 context, Db4objects.Db4o.CorruptionException[] exc)
			{
				this._enclosing = _enclosing;
				this.context = context;
				this.exc = exc;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				try
				{
					Db4objects.Db4o.Internal.ReaderPair.ProcessCopy(context, id, new _AnonymousInnerClass411
						(this));
				}
				catch (Db4objects.Db4o.CorruptionException e)
				{
					exc[0] = e;
					throw new System.Exception();
				}
			}

			private sealed class _AnonymousInnerClass411 : Db4objects.Db4o.Internal.ISlotCopyHandler
			{
				public _AnonymousInnerClass411(_AnonymousInnerClass407 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void ProcessCopy(Db4objects.Db4o.Internal.ReaderPair readers)
				{
					this._enclosing._enclosing.DefragIndexNode(readers);
				}

				private readonly _AnonymousInnerClass407 _enclosing;
			}

			private readonly BTree _enclosing;

			private readonly Db4objects.Db4o.Internal.Mapping.IDefragContext context;

			private readonly Db4objects.Db4o.CorruptionException[] exc;
		}

		public virtual int CompareKeys(object key1, object key2)
		{
			_keyHandler.PrepareComparison(key2);
			return _keyHandler.CompareTo(key1);
		}

		private static Db4objects.Db4o.Internal.Config4Impl Config(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			if (null == trans)
			{
				throw new System.ArgumentNullException();
			}
			return trans.Stream().ConfigImpl();
		}

		public virtual void Free(Db4objects.Db4o.Internal.Transaction systemTrans)
		{
			FreeAllNodeIds(systemTrans, AllNodeIds(systemTrans));
		}

		private void FreeAllNodeIds(Db4objects.Db4o.Internal.Transaction systemTrans, System.Collections.IEnumerator
			 allNodeIDs)
		{
			while (allNodeIDs.MoveNext())
			{
				int id = ((int)allNodeIDs.Current);
				systemTrans.SlotFreePointerOnCommit(id);
			}
		}

		public virtual System.Collections.IEnumerator AllNodeIds(Db4objects.Db4o.Internal.Transaction
			 systemTrans)
		{
			Db4objects.Db4o.Foundation.Collection4 allNodeIDs = new Db4objects.Db4o.Foundation.Collection4
				();
			TraverseAllNodes(systemTrans, new _AnonymousInnerClass455(this, allNodeIDs));
			return allNodeIDs.GetEnumerator();
		}

		private sealed class _AnonymousInnerClass455 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass455(BTree _enclosing, Db4objects.Db4o.Foundation.Collection4
				 allNodeIDs)
			{
				this._enclosing = _enclosing;
				this.allNodeIDs = allNodeIDs;
			}

			public void Visit(object node)
			{
				allNodeIDs.Add(((Db4objects.Db4o.Internal.Btree.BTreeNode)node).GetID());
			}

			private readonly BTree _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 allNodeIDs;
		}

		public virtual Db4objects.Db4o.Internal.Btree.IBTreeRange AsRange(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeRangeSingle(trans, this, FirstPointer
				(trans), null);
		}
	}
}
