/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Mapping;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTree : PersistentBase, ITransactionParticipant
	{
		private const byte BTREE_VERSION = (byte)1;

		private const int DEFRAGMENT_INCREMENT_OFFSET = 1 + Const4.INT_LENGTH * 2;

		private readonly IIndexable4 _keyHandler;

		private BTreeNode _root;

		/// <summary>All instantiated nodes are held in this tree.</summary>
		/// <remarks>All instantiated nodes are held in this tree.</remarks>
		private TreeIntObject _nodes;

		private int _size;

		private IVisitor4 _removeListener;

		private Hashtable4 _sizesByTransaction;

		protected IQueue4 _processing;

		private int _nodeSize;

		internal int _halfNodeSize;

		private readonly int _cacheHeight;

		public BTree(Transaction trans, int id, IIndexable4 keyHandler) : this(trans, id, 
			keyHandler, Config(trans).BTreeNodeSize(), Config(trans).BTreeCacheHeight())
		{
		}

		public BTree(Transaction trans, int id, IIndexable4 keyHandler, int treeNodeSize, 
			int treeCacheHeight)
		{
			if (null == keyHandler)
			{
				throw new ArgumentNullException();
			}
			_nodeSize = treeNodeSize;
			_halfNodeSize = _nodeSize / 2;
			_nodeSize = _halfNodeSize * 2;
			_cacheHeight = treeCacheHeight;
			_keyHandler = keyHandler;
			_sizesByTransaction = new Hashtable4();
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
			_keyHandler.PrepareComparison(key);
			EnsureDirty(trans);
			BTreeNode rootOrSplit = _root.Add(trans, key);
			if (rootOrSplit != null && rootOrSplit != _root)
			{
				_root = new BTreeNode(trans, _root, rootOrSplit);
				_root.Write(trans.SystemTransaction());
				AddNode(_root);
			}
		}

		public virtual void Remove(Transaction trans, object key)
		{
			KeyCantBeNull(key);
			IEnumerator pointers = Search(trans, key).Pointers();
			if (!pointers.MoveNext())
			{
				return;
			}
			BTreePointer first = (BTreePointer)pointers.Current;
			EnsureDirty(trans);
			BTreeNode node = first.Node();
			node.Remove(trans, key, first.Index());
		}

		public virtual IBTreeRange Search(Transaction trans, object key)
		{
			KeyCantBeNull(key);
			BTreeNodeSearchResult start = SearchLeaf(trans, key, SearchTarget.LOWEST);
			BTreeNodeSearchResult end = SearchLeaf(trans, key, SearchTarget.HIGHEST);
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
			EnsureActive(trans);
			_keyHandler.PrepareComparison(key);
			return _root.SearchLeaf(trans, target);
		}

		public virtual void Commit(Transaction trans)
		{
			Transaction systemTransaction = trans.SystemTransaction();
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff != null)
			{
				_size += ((int)sizeDiff);
			}
			_sizesByTransaction.Remove(trans);
			if (_nodes != null)
			{
				CommitNodes(trans);
				WriteAllNodes(systemTransaction);
			}
			SetStateDirty();
			Write(systemTransaction);
			Purge();
		}

		public virtual void CommitNodes(Transaction trans)
		{
			if (_nodes == null)
			{
				return;
			}
			ProcessAllNodes();
			while (_processing.HasNext())
			{
				((BTreeNode)_processing.Next()).Commit(trans);
			}
			_processing = null;
		}

		public virtual void Rollback(Transaction trans)
		{
			Transaction systemTransaction = trans.SystemTransaction();
			_sizesByTransaction.Remove(trans);
			if (_nodes == null)
			{
				return;
			}
			ProcessAllNodes();
			while (_processing.HasNext())
			{
				((BTreeNode)_processing.Next()).Rollback(trans);
			}
			_processing = null;
			WriteAllNodes(systemTransaction);
			SetStateDirty();
			Write(systemTransaction);
			Purge();
		}

		private void WriteAllNodes(Transaction systemTransaction)
		{
			if (_nodes == null)
			{
				return;
			}
			_nodes.Traverse(new _IVisitor4_196(this, systemTransaction));
		}

		private sealed class _IVisitor4_196 : IVisitor4
		{
			public _IVisitor4_196(BTree _enclosing, Transaction systemTransaction)
			{
				this._enclosing = _enclosing;
				this.systemTransaction = systemTransaction;
			}

			public void Visit(object obj)
			{
				BTreeNode node = (BTreeNode)((TreeIntObject)obj).GetObject();
				node.Write(systemTransaction);
			}

			private readonly BTree _enclosing;

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
			if (_cacheHeight > 0)
			{
				_root.MarkAsCached(_cacheHeight);
			}
			else
			{
				_root.HoldChildrenAsIDs();
				AddNode(_root);
			}
			temp.Traverse(new _IVisitor4_220(this));
		}

		private sealed class _IVisitor4_220 : IVisitor4
		{
			public _IVisitor4_220(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				BTreeNode node = (BTreeNode)((TreeIntObject)obj).GetObject();
				node.Purge();
			}

			private readonly BTree _enclosing;
		}

		private void ProcessAllNodes()
		{
			_processing = new NonblockingQueue();
			_nodes.Traverse(new _IVisitor4_230(this));
		}

		private sealed class _IVisitor4_230 : IVisitor4
		{
			public _IVisitor4_230(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing._processing.Add(((TreeIntObject)obj).GetObject());
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
			return Const4.BTREE;
		}

		public virtual void SetRemoveListener(IVisitor4 vis)
		{
			_removeListener = vis;
		}

		public override int OwnLength()
		{
			return 1 + Const4.OBJECT_LENGTH + (Const4.INT_LENGTH * 2) + Const4.ID_LENGTH;
		}

		internal virtual BTreeNode ProduceNode(int id)
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

		public override void ReadThis(Transaction a_trans, BufferImpl a_reader)
		{
			a_reader.IncrementOffset(1);
			_size = a_reader.ReadInt();
			_nodeSize = a_reader.ReadInt();
			_halfNodeSize = NodeSize() / 2;
			_root = ProduceNode(a_reader.ReadInt());
		}

		public override void WriteThis(Transaction trans, BufferImpl a_writer)
		{
			a_writer.WriteByte(BTREE_VERSION);
			a_writer.WriteInt(_size);
			a_writer.WriteInt(NodeSize());
			a_writer.WriteIDOf(trans, _root);
		}

		public virtual int Size(Transaction trans)
		{
			EnsureActive(trans);
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff != null)
			{
				return _size + ((int)sizeDiff);
			}
			return _size;
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

		public virtual void SizeChanged(Transaction trans, int changeBy)
		{
			object sizeDiff = _sizesByTransaction.Get(trans);
			if (sizeDiff == null)
			{
				_sizesByTransaction.Put(trans, changeBy);
				return;
			}
			_sizesByTransaction.Put(trans, ((int)sizeDiff) + changeBy);
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
			context.IncrementOffset(DEFRAGMENT_INCREMENT_OFFSET);
			context.CopyID();
		}

		public virtual void DefragIndexNode(DefragmentContextImpl context)
		{
			BTreeNode.DefragIndex(context, _keyHandler);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public virtual void DefragBTree(IDefragmentServices services)
		{
			DefragmentContextImpl.ProcessCopy(services, GetID(), new _ISlotCopyHandler_389(this
				));
			CorruptionException[] corruptx = new CorruptionException[] { null };
			IOException[] iox = new IOException[] { null };
			try
			{
				services.TraverseAllIndexSlots(this, new _IVisitor4_397(this, services, corruptx, 
					iox));
			}
			catch (Exception e)
			{
				if (corruptx[0] != null)
				{
					throw corruptx[0];
				}
				if (iox[0] != null)
				{
					throw iox[0];
				}
				throw;
			}
		}

		private sealed class _ISlotCopyHandler_389 : ISlotCopyHandler
		{
			public _ISlotCopyHandler_389(BTree _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="CorruptionException"></exception>
			public void ProcessCopy(DefragmentContextImpl context)
			{
				this._enclosing.DefragIndex(context);
			}

			private readonly BTree _enclosing;
		}

		private sealed class _IVisitor4_397 : IVisitor4
		{
			public _IVisitor4_397(BTree _enclosing, IDefragmentServices services, CorruptionException
				[] corruptx, IOException[] iox)
			{
				this._enclosing = _enclosing;
				this.services = services;
				this.corruptx = corruptx;
				this.iox = iox;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				try
				{
					DefragmentContextImpl.ProcessCopy(services, id, new _ISlotCopyHandler_401(this));
				}
				catch (CorruptionException e)
				{
					corruptx[0] = e;
					throw new Exception();
				}
				catch (IOException e)
				{
					iox[0] = e;
					throw new Exception();
				}
			}

			private sealed class _ISlotCopyHandler_401 : ISlotCopyHandler
			{
				public _ISlotCopyHandler_401(_IVisitor4_397 _enclosing)
				{
					this._enclosing = _enclosing;
				}

				public void ProcessCopy(DefragmentContextImpl context)
				{
					this._enclosing._enclosing.DefragIndexNode(context);
				}

				private readonly _IVisitor4_397 _enclosing;
			}

			private readonly BTree _enclosing;

			private readonly IDefragmentServices services;

			private readonly CorruptionException[] corruptx;

			private readonly IOException[] iox;
		}

		public virtual int CompareKeys(object key1, object key2)
		{
			_keyHandler.PrepareComparison(key2);
			return _keyHandler.CompareTo(key1);
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
			TraverseAllNodes(systemTrans, new _IVisitor4_452(this, allNodeIDs));
			return allNodeIDs.GetEnumerator();
		}

		private sealed class _IVisitor4_452 : IVisitor4
		{
			public _IVisitor4_452(BTree _enclosing, Collection4 allNodeIDs)
			{
				this._enclosing = _enclosing;
				this.allNodeIDs = allNodeIDs;
			}

			public void Visit(object node)
			{
				allNodeIDs.Add(((BTreeNode)node).GetID());
			}

			private readonly BTree _enclosing;

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
			_nodes.Traverse(new _IVisitor4_468(this, visitor));
		}

		private sealed class _IVisitor4_468 : IVisitor4
		{
			public _IVisitor4_468(BTree _enclosing, IVisitor4 visitor)
			{
				this._enclosing = _enclosing;
				this.visitor = visitor;
			}

			public void Visit(object obj)
			{
				visitor.Visit(((TreeIntObject)obj).GetObject());
			}

			private readonly BTree _enclosing;

			private readonly IVisitor4 visitor;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("BTree ");
			sb.Append(GetID());
			sb.Append(" Active Nodes: \n");
			TraverseAllNodes(new _IVisitor4_480(this, sb));
			return sb.ToString();
		}

		private sealed class _IVisitor4_480 : IVisitor4
		{
			public _IVisitor4_480(BTree _enclosing, StringBuilder sb)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(object obj)
			{
				sb.Append(obj);
				sb.Append("\n");
			}

			private readonly BTree _enclosing;

			private readonly StringBuilder sb;
		}
	}
}
