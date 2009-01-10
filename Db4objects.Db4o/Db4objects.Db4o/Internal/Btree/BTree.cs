/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTree : PersistentBase, ITransactionParticipant, IBTreeStructureListener
	{
		private const byte BtreeVersion = (byte)1;

		private const int DefragmentIncrementOffset = 1 + Const4.IntLength * 2;

		private readonly IIndexable4 _keyHandler;

		private BTreeNode _root;

		/// <summary>All instantiated nodes are held in this tree.</summary>
		/// <remarks>All instantiated nodes are held in this tree.</remarks>
		private TreeIntObject _nodes;

		private int _size;

		private IVisitor4 _removeListener;

		private sealed class _TransactionLocal_35 : TransactionLocal
		{
			public _TransactionLocal_35()
			{
			}

			// version byte
			// size, node size  
			public override object InitialValueFor(Transaction transaction)
			{
				return 0;
			}
		}

		private readonly TransactionLocal _sizeDeltaInTransaction = new _TransactionLocal_35
			();

		protected IQueue4 _processing;

		private int _nodeSize;

		internal int _halfNodeSize;

		private IBTreeStructureListener _structureListener;

		public BTree(Transaction trans, IIndexable4 keyHandler) : this(trans, 0, keyHandler
			)
		{
		}

		public BTree(Transaction trans, int id, IIndexable4 keyHandler) : this(trans, id, 
			keyHandler, Config(trans).BTreeNodeSize())
		{
		}

		public BTree(Transaction trans, int id, IIndexable4 keyHandler, int treeNodeSize)
		{
			if (null == keyHandler)
			{
				throw new ArgumentNullException();
			}
			_nodeSize = treeNodeSize;
			_halfNodeSize = _nodeSize / 2;
			_nodeSize = _halfNodeSize * 2;
			_keyHandler = keyHandler;
			if (id == 0)
			{
				SetStateDirty();
				_root = new BTreeNode(this, 0, true, 0, 0, 0);
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

		public virtual BTreeNode Root()
		{
			return _root;
		}

		public virtual int NodeSize()
		{
			return _nodeSize;
		}

		public virtual void Add(Transaction trans, object key)
		{
			KeyCantBeNull(key);
			IPreparedComparison preparedComparison = _keyHandler.PrepareComparison(trans.Context
				(), key);
			Add(trans, preparedComparison, key);
		}

		public virtual void Add(Transaction trans, IPreparedComparison preparedComparison
			, object key)
		{
			EnsureDirty(trans);
			BTreeNode rootOrSplit = _root.Add(trans, preparedComparison, key);
			if (rootOrSplit != null && rootOrSplit != _root)
			{
				_root = new BTreeNode(trans, _root, rootOrSplit);
				_root.Write(trans.SystemTransaction());
				AddNode(_root);
			}
		}

		// FIXME: Change the signature to return true, if object could be removed.
		public virtual void Remove(Transaction trans, object key)
		{
			KeyCantBeNull(key);
			IPreparedComparison preparedComparison = KeyHandler().PrepareComparison(trans.Context
				(), key);
			IEnumerator pointers = Search(trans, preparedComparison).Pointers();
			if (!pointers.MoveNext())
			{
				return;
			}
			BTreePointer first = (BTreePointer)pointers.Current;
			EnsureDirty(trans);
			BTreeNode node = first.Node();
			node.Remove(trans, preparedComparison, key, first.Index());
		}

		public virtual IBTreeRange Search(Transaction trans, object key)
		{
			KeyCantBeNull(key);
			return Search(trans, KeyHandler().PrepareComparison(trans.Context(), key));
		}

		private IBTreeRange Search(Transaction trans, IPreparedComparison preparedComparison
			)
		{
			EnsureActive(trans);
			// TODO: Optimize the following.
			//       Part of the search operates against the same nodes.
			//       As long as the bounds are on one node, the search
			//       should walk the nodes in one go.
			BTreeNodeSearchResult start = SearchLeaf(trans, preparedComparison, SearchTarget.
				Lowest);
			BTreeNodeSearchResult end = SearchLeaf(trans, preparedComparison, SearchTarget.Highest
				);
			return start.CreateIncludingRange(end);
		}

		private void KeyCantBeNull(object key)
		{
			if (null == key)
			{
				throw new ArgumentNullException();
			}
		}

		public virtual IIndexable4 KeyHandler()
		{
			return _keyHandler;
		}

		public virtual BTreeNodeSearchResult SearchLeaf(Transaction trans, object key, SearchTarget
			 target)
		{
			return SearchLeaf(trans, _keyHandler.PrepareComparison(trans.Context(), key), target
				);
		}

		public virtual BTreeNodeSearchResult SearchLeaf(Transaction trans, IPreparedComparison
			 preparedComparison, SearchTarget target)
		{
			EnsureActive(trans);
			return _root.SearchLeaf(trans, preparedComparison, target);
		}

		public virtual void Commit(Transaction transaction)
		{
			UpdateSize(transaction);
			CommitNodes(transaction);
			FinishTransaction(transaction);
		}

		private void UpdateSize(Transaction transaction)
		{
			ByRef sizeInTransaction = SizeIn(transaction);
			_size += (((int)sizeInTransaction.value));
			sizeInTransaction.value = 0;
		}

		private ByRef SizeIn(Transaction trans)
		{
			return trans.Get(_sizeDeltaInTransaction);
		}

		public virtual void CommitNodes(Transaction trans)
		{
			ProcessEachNode(new _IProcedure4_172(trans));
		}

		private sealed class _IProcedure4_172 : IProcedure4
		{
			public _IProcedure4_172(Transaction trans)
			{
				this.trans = trans;
			}

			public void Apply(object node)
			{
				((BTreeNode)node).Commit(trans);
			}

			private readonly Transaction trans;
		}

		private void ProcessEachNode(IProcedure4 action)
		{
			if (_nodes == null)
			{
				return;
			}
			ProcessAllNodes();
			while (_processing.HasNext())
			{
				action.Apply((BTreeNode)_processing.Next());
			}
			_processing = null;
		}

		public virtual void Rollback(Transaction trans)
		{
			RollbackNodes(trans);
			FinishTransaction(trans);
		}

		private void FinishTransaction(Transaction trans)
		{
			Transaction systemTransaction = trans.SystemTransaction();
			WriteAllNodes(systemTransaction);
			SetStateDirty();
			Write(systemTransaction);
			Purge();
		}

		private void RollbackNodes(Transaction trans)
		{
			ProcessEachNode(new _IProcedure4_201(trans));
		}

		private sealed class _IProcedure4_201 : IProcedure4
		{
			public _IProcedure4_201(Transaction trans)
			{
				this.trans = trans;
			}

			public void Apply(object node)
			{
				((BTreeNode)node).Rollback(trans);
			}

			private readonly Transaction trans;
		}

		private void WriteAllNodes(Transaction systemTransaction)
		{
			if (_nodes == null)
			{
				return;
			}
			_nodes.Traverse(new _IVisitor4_210(systemTransaction));
		}

		private sealed class _IVisitor4_210 : IVisitor4
		{
			public _IVisitor4_210(Transaction systemTransaction)
			{
				this.systemTransaction = systemTransaction;
			}

			public void Visit(object obj)
			{
				BTreeNode node = (BTreeNode)((TreeIntObject)obj).GetObject();
				node.Write(systemTransaction);
			}

			private readonly Transaction systemTransaction;
		}

		private void Purge()
		{
			if (_nodes == null)
			{
				return;
			}
			Tree temp = _nodes;
			_nodes = null;
			// FIXME: Caching doesn't work as expected. Disabled.
			//        Reimplement with LRU cache for all nodes,
			//        independant of _root.
			//        if(_cacheHeight > 0){
			//            _root.markAsCached(_cacheHeight);
			//        }else{
			_root.HoldChildrenAsIDs();
			AddNode(_root);
			//        }
			temp.Traverse(new _IVisitor4_240());
		}

		private sealed class _IVisitor4_240 : IVisitor4
		{
			public _IVisitor4_240()
			{
			}

			public void Visit(object obj)
			{
				BTreeNode node = (BTreeNode)((TreeIntObject)obj).GetObject();
				node.Purge();
			}
		}

		private void ProcessAllNodes()
		{
			_processing = new NonblockingQueue();
			_nodes.Traverse(new _IVisitor4_250(this));
		}

		private sealed class _IVisitor4_250 : IVisitor4
		{
			public _IVisitor4_250(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object node)
			{
				this._enclosing._processing.Add(((TreeIntObject)node).GetObject());
			}

			private readonly BTree _enclosing;
		}

		private void EnsureActive(Transaction trans)
		{
			if (!IsActive())
			{
				Read(trans.SystemTransaction());
			}
		}

		private void EnsureDirty(Transaction trans)
		{
			EnsureActive(trans);
			if (CanEnlistWithTransaction())
			{
				((LocalTransaction)trans).Enlist(this);
			}
			SetStateDirty();
		}

		protected virtual bool CanEnlistWithTransaction()
		{
			return true;
		}

		public override byte GetIdentifier()
		{
			return Const4.Btree;
		}

		public virtual void SetRemoveListener(IVisitor4 vis)
		{
			_removeListener = vis;
		}

		public override int OwnLength()
		{
			return 1 + Const4.ObjectLength + (Const4.IntLength * 2) + Const4.IdLength;
		}

		public virtual BTreeNode ProduceNode(int id)
		{
			TreeIntObject addtio = new TreeIntObject(id);
			_nodes = (TreeIntObject)Tree.Add(_nodes, addtio);
			TreeIntObject tio = (TreeIntObject)addtio.AddedOrExisting();
			BTreeNode node = (BTreeNode)tio.GetObject();
			if (node == null)
			{
				node = new BTreeNode(id, this);
				tio.SetObject(node);
				AddToProcessing(node);
			}
			return node;
		}

		internal virtual void AddNode(BTreeNode node)
		{
			_nodes = (TreeIntObject)Tree.Add(_nodes, new TreeIntObject(node.GetID(), node));
			AddToProcessing(node);
		}

		internal virtual void AddToProcessing(BTreeNode node)
		{
			if (_processing != null)
			{
				_processing.Add(node);
			}
		}

		internal virtual void RemoveNode(BTreeNode node)
		{
			_nodes = (TreeIntObject)_nodes.RemoveLike(new TreeInt(node.GetID()));
		}

		internal virtual void NotifyRemoveListener(object obj)
		{
			if (_removeListener != null)
			{
				_removeListener.Visit(obj);
			}
		}

		public override void ReadThis(Transaction a_trans, ByteArrayBuffer a_reader)
		{
			a_reader.IncrementOffset(1);
			// first byte is version, for possible future format changes
			_size = a_reader.ReadInt();
			_nodeSize = a_reader.ReadInt();
			_halfNodeSize = NodeSize() / 2;
			_root = ProduceNode(a_reader.ReadInt());
		}

		public override void WriteThis(Transaction trans, ByteArrayBuffer a_writer)
		{
			a_writer.WriteByte(BtreeVersion);
			a_writer.WriteInt(_size);
			a_writer.WriteInt(NodeSize());
			a_writer.WriteIDOf(trans, _root);
		}

		public virtual int Size(Transaction trans)
		{
			// This implementation of size will not work accurately for multiple
			// transactions. If two transactions call clear and both commit, _size
			// can end up negative.
			// For multiple transactions the size patches only are an estimate.
			EnsureActive(trans);
			return _size + (((int)SizeIn(trans).value));
		}

		public virtual void TraverseKeys(Transaction trans, IVisitor4 visitor)
		{
			EnsureActive(trans);
			if (_root == null)
			{
				return;
			}
			_root.TraverseKeys(trans, visitor);
		}

		public virtual void SizeChanged(Transaction transaction, BTreeNode node, int changeBy
			)
		{
			NotifyCountChanged(transaction, node, changeBy);
			ByRef sizeInTransaction = SizeIn(transaction);
			sizeInTransaction.value = (((int)sizeInTransaction.value)) + changeBy;
		}

		public virtual void Dispose(Transaction transaction)
		{
		}

		public virtual BTreePointer FirstPointer(Transaction trans)
		{
			EnsureActive(trans);
			if (null == _root)
			{
				return null;
			}
			return _root.FirstPointer(trans);
		}

		public virtual BTreePointer LastPointer(Transaction trans)
		{
			EnsureActive(trans);
			if (null == _root)
			{
				return null;
			}
			return _root.LastPointer(trans);
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTree DebugLoadFully(Transaction trans
			)
		{
			EnsureActive(trans);
			_root.DebugLoadFully(trans);
			return this;
		}

		private void TraverseAllNodes(Transaction trans, IVisitor4 command)
		{
			EnsureActive(trans);
			_root.TraverseAllNodes(trans, command);
		}

		public virtual void DefragIndex(DefragmentContextImpl context)
		{
			context.IncrementOffset(DefragmentIncrementOffset);
			context.CopyID();
		}

		public virtual void DefragIndexNode(DefragmentContextImpl context)
		{
			BTreeNode.DefragIndex(context, _keyHandler);
		}

		public virtual void DefragBTree(IDefragmentServices services)
		{
			DefragmentContextImpl.ProcessCopy(services, GetID(), new _ISlotCopyHandler_410(this
				));
			services.TraverseAllIndexSlots(this, new _IVisitor4_415(this, services));
		}

		private sealed class _ISlotCopyHandler_410 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_410(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ProcessCopy(DefragmentContextImpl context)
			{
				this._enclosing.DefragIndex(context);
			}

			private readonly BTree _enclosing;
		}

		private sealed class _IVisitor4_415 : IVisitor4
		{
			public _IVisitor4_415(BTree _enclosing, IDefragmentServices services)
			{
				this._enclosing = _enclosing;
				this.services = services;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				DefragmentContextImpl.ProcessCopy(services, id, new _ISlotCopyHandler_418(this));
			}

			private sealed class _ISlotCopyHandler_418 : ISlotCopyHandler
			{
				public _ISlotCopyHandler_418(_IVisitor4_415 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void ProcessCopy(DefragmentContextImpl context)
				{
					this._enclosing._enclosing.DefragIndexNode(context);
				}

				private readonly _IVisitor4_415 _enclosing;
			}

			private readonly BTree _enclosing;

			private readonly IDefragmentServices services;
		}

		public virtual int CompareKeys(IContext context, object key1, object key2)
		{
			IPreparedComparison preparedComparison = _keyHandler.PrepareComparison(context, key1
				);
			return preparedComparison.CompareTo(key2);
		}

		private static Config4Impl Config(Transaction trans)
		{
			if (null == trans)
			{
				throw new ArgumentNullException();
			}
			return trans.Container().ConfigImpl();
		}

		public override void Free(Transaction systemTrans)
		{
			FreeAllNodeIds(systemTrans, AllNodeIds(systemTrans));
			base.Free(systemTrans);
		}

		private void FreeAllNodeIds(Transaction systemTrans, IEnumerator allNodeIDs)
		{
			while (allNodeIDs.MoveNext())
			{
				int id = ((int)allNodeIDs.Current);
				systemTrans.SlotFreePointerOnCommit(id);
			}
		}

		public virtual IEnumerator AllNodeIds(Transaction systemTrans)
		{
			Collection4 allNodeIDs = new Collection4();
			TraverseAllNodes(systemTrans, new _IVisitor4_453(allNodeIDs));
			return allNodeIDs.GetEnumerator();
		}

		private sealed class _IVisitor4_453 : IVisitor4
		{
			public _IVisitor4_453(Collection4 allNodeIDs)
			{
				this.allNodeIDs = allNodeIDs;
			}

			public void Visit(object node)
			{
				allNodeIDs.Add(((BTreeNode)node).GetID());
			}

			private readonly Collection4 allNodeIDs;
		}

		public virtual IBTreeRange AsRange(Transaction trans)
		{
			return new BTreeRangeSingle(trans, this, FirstPointer(trans), null);
		}

		private void TraverseAllNodes(IVisitor4 visitor)
		{
			if (_nodes == null)
			{
				return;
			}
			_nodes.Traverse(new _IVisitor4_469(visitor));
		}

		private sealed class _IVisitor4_469 : IVisitor4
		{
			public _IVisitor4_469(IVisitor4 visitor)
			{
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				visitor.Visit(((TreeIntObject)obj).GetObject());
			}

			private readonly IVisitor4 visitor;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("BTree ");
			sb.Append(GetID());
			sb.Append(" Active Nodes: \n");
			TraverseAllNodes(new _IVisitor4_481(sb));
			return sb.ToString();
		}

		private sealed class _IVisitor4_481 : IVisitor4
		{
			public _IVisitor4_481(StringBuilder sb)
			{
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly StringBuilder sb;
		}

		public virtual void StructureListener(IBTreeStructureListener listener)
		{
			_structureListener = listener;
		}

		public virtual void NotifySplit(Transaction trans, BTreeNode originalNode, BTreeNode
			 newRightNode)
		{
			if (_structureListener != null)
			{
				_structureListener.NotifySplit(trans, originalNode, newRightNode);
			}
		}

		public virtual void NotifyDeleted(Transaction trans, BTreeNode node)
		{
			if (_structureListener != null)
			{
				_structureListener.NotifyDeleted(trans, node);
			}
		}

		public virtual void NotifyCountChanged(Transaction trans, BTreeNode node, int diff
			)
		{
			if (_structureListener != null)
			{
				_structureListener.NotifyCountChanged(trans, node, diff);
			}
		}

		public virtual IEnumerator Iterator(Transaction trans)
		{
			return new BTreeIterator(trans, this);
		}

		public virtual void Clear(Transaction transaction)
		{
			BTreePointer currentPointer = FirstPointer(transaction);
			while (currentPointer != null && currentPointer.IsValid())
			{
				BTreeNode node = currentPointer.Node();
				int index = currentPointer.Index();
				node.Remove(transaction, index);
				currentPointer = currentPointer.Next();
			}
		}
	}
}
