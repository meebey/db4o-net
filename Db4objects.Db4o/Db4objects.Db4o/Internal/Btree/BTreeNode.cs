namespace Db4objects.Db4o.Internal.Btree
{
	/// <summary>
	/// We work with BTreeNode in two states:
	/// - deactivated: never read, no valid members, ID correct or 0 if new
	/// - write: real representation of keys, values and children in arrays
	/// The write state can be detected with canWrite().
	/// </summary>
	/// <remarks>
	/// We work with BTreeNode in two states:
	/// - deactivated: never read, no valid members, ID correct or 0 if new
	/// - write: real representation of keys, values and children in arrays
	/// The write state can be detected with canWrite(). States can be changed
	/// as needed with prepareRead() and prepareWrite().
	/// </remarks>
	/// <exclude></exclude>
	public sealed class BTreeNode : Db4objects.Db4o.Internal.PersistentBase
	{
		private const int COUNT_LEAF_AND_3_LINK_LENGTH = (Db4objects.Db4o.Internal.Const4
			.INT_LENGTH * 4) + 1;

		private const int SLOT_LEADING_LENGTH = Db4objects.Db4o.Internal.Const4.LEADING_LENGTH
			 + COUNT_LEAF_AND_3_LINK_LENGTH;

		internal readonly Db4objects.Db4o.Internal.Btree.BTree _btree;

		private int _count;

		private bool _isLeaf;

		private object[] _keys;

		/// <summary>Can contain BTreeNode or Integer for ID of BTreeNode</summary>
		private object[] _children;

		private int _parentID;

		private int _previousID;

		private int _nextID;

		private bool _cached;

		private bool _dead;

		public BTreeNode(Db4objects.Db4o.Internal.Btree.BTree btree, int count, bool isLeaf
			, int parentID, int previousID, int nextID)
		{
			_btree = btree;
			_parentID = parentID;
			_previousID = previousID;
			_nextID = nextID;
			_count = count;
			_isLeaf = isLeaf;
			PrepareArrays();
		}

		public BTreeNode(int id, Db4objects.Db4o.Internal.Btree.BTree btree)
		{
			_btree = btree;
			SetID(id);
			SetStateDeactivated();
		}

		public BTreeNode(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode
			 firstChild, Db4objects.Db4o.Internal.Btree.BTreeNode secondChild) : this(firstChild
			._btree, 2, false, 0, 0, 0)
		{
			_keys[0] = firstChild._keys[0];
			_children[0] = firstChild;
			_keys[1] = secondChild._keys[0];
			_children[1] = secondChild;
			Write(trans.SystemTransaction());
			firstChild.SetParentID(trans, GetID());
			secondChild.SetParentID(trans, GetID());
		}

		public Db4objects.Db4o.Internal.Btree.BTree Btree()
		{
			return _btree;
		}

		/// <returns>
		/// the split node if this node is split
		/// or this if the first key has changed
		/// </returns>
		public Db4objects.Db4o.Internal.Btree.BTreeNode Add(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			Db4objects.Db4o.Internal.Btree.Searcher s = Search(reader);
			if (_isLeaf)
			{
				PrepareWrite(trans);
				if (WasRemoved(trans, s))
				{
					CancelRemoval(trans, s.Cursor());
					return null;
				}
				if (s.Count() > 0 && !s.BeforeFirst())
				{
					s.MoveForward();
				}
				PrepareInsert(s.Cursor());
				_keys[s.Cursor()] = NewAddPatch(trans);
			}
			else
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode childNode = Child(reader, s.Cursor());
				Db4objects.Db4o.Internal.Btree.BTreeNode childNodeOrSplit = childNode.Add(trans);
				if (childNodeOrSplit == null)
				{
					return null;
				}
				PrepareWrite(trans);
				_keys[s.Cursor()] = childNode._keys[0];
				if (childNode != childNodeOrSplit)
				{
					int splitCursor = s.Cursor() + 1;
					PrepareInsert(splitCursor);
					_keys[splitCursor] = childNodeOrSplit._keys[0];
					_children[splitCursor] = childNodeOrSplit;
				}
			}
			if (MustSplit())
			{
				return Split(trans);
			}
			if (s.Cursor() == 0)
			{
				return this;
			}
			return null;
		}

		private bool MustSplit()
		{
			return _count >= _btree.NodeSize();
		}

		private Db4objects.Db4o.Internal.Btree.BTreeAdd NewAddPatch(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			SizeIncrement(trans);
			return new Db4objects.Db4o.Internal.Btree.BTreeAdd(trans, CurrentKey());
		}

		private object CurrentKey()
		{
			return KeyHandler().Current();
		}

		private void CancelRemoval(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			Db4objects.Db4o.Internal.Btree.BTreeUpdate patch = (Db4objects.Db4o.Internal.Btree.BTreeUpdate
				)KeyPatch(index);
			Db4objects.Db4o.Internal.Btree.BTreeUpdate nextPatch = patch.RemoveFor(trans);
			_keys[index] = NewCancelledRemoval(trans, patch.GetObject(), nextPatch);
			SizeIncrement(trans);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePatch NewCancelledRemoval(Db4objects.Db4o.Internal.Transaction
			 trans, object originalObject, Db4objects.Db4o.Internal.Btree.BTreeUpdate existingPatches
			)
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeCancelledRemoval(trans, originalObject
				, CurrentKey(), existingPatches);
		}

		private void SizeIncrement(Db4objects.Db4o.Internal.Transaction trans)
		{
			_btree.SizeChanged(trans, 1);
		}

		private bool WasRemoved(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.Searcher
			 s)
		{
			if (!s.FoundMatch())
			{
				return false;
			}
			Db4objects.Db4o.Internal.Btree.BTreePatch patch = KeyPatch(trans, s.Cursor());
			return patch != null && patch.IsRemove();
		}

		internal Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult SearchLeaf(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Btree.SearchTarget target)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			Db4objects.Db4o.Internal.Btree.Searcher s = Search(reader, target);
			if (!_isLeaf)
			{
				return Child(reader, s.Cursor()).SearchLeaf(trans, target);
			}
			if (!s.FoundMatch() || target == Db4objects.Db4o.Internal.Btree.SearchTarget.ANY 
				|| target == Db4objects.Db4o.Internal.Btree.SearchTarget.HIGHEST)
			{
				return new Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult(trans, reader, Btree
					(), s, this);
			}
			if (target == Db4objects.Db4o.Internal.Btree.SearchTarget.LOWEST)
			{
				Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult res = FindLowestLeafMatch(trans
					, s.Cursor() - 1);
				if (res != null)
				{
					return res;
				}
				return CreateMatchingSearchResult(trans, reader, s.Cursor());
			}
			throw new System.InvalidOperationException();
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult FindLowestLeafMatch(
			Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			return FindLowestLeafMatch(trans, PrepareRead(trans), index);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult FindLowestLeafMatch(
			Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer reader
			, int index)
		{
			if (index >= 0)
			{
				if (!CompareEquals(reader, index))
				{
					return null;
				}
				if (index > 0)
				{
					Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult res = FindLowestLeafMatch(trans
						, reader, index - 1);
					if (res != null)
					{
						return res;
					}
					return CreateMatchingSearchResult(trans, reader, index);
				}
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode node = PreviousNode();
			if (node != null)
			{
				Db4objects.Db4o.Internal.Buffer nodeReader = node.PrepareRead(trans);
				Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult res = node.FindLowestLeafMatch
					(trans, nodeReader, node.LastIndex());
				if (res != null)
				{
					return res;
				}
			}
			if (index < 0)
			{
				return null;
			}
			return CreateMatchingSearchResult(trans, reader, index);
		}

		private bool CompareEquals(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			if (CanWrite())
			{
				return CompareInWriteMode(index) == 0;
			}
			return CompareInReadMode(reader, index) == 0;
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult CreateMatchingSearchResult
			(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer reader
			, int index)
		{
			return new Db4objects.Db4o.Internal.Btree.BTreeNodeSearchResult(trans, reader, Btree
				(), this, index, true);
		}

		public bool CanWrite()
		{
			return _keys != null;
		}

		internal Db4objects.Db4o.Internal.Btree.BTreeNode Child(int index)
		{
			if (_children[index] is Db4objects.Db4o.Internal.Btree.BTreeNode)
			{
				return (Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index];
			}
			return _btree.ProduceNode(((int)_children[index]));
		}

		internal Db4objects.Db4o.Internal.Btree.BTreeNode Child(Db4objects.Db4o.Internal.Buffer
			 reader, int index)
		{
			if (ChildLoaded(index))
			{
				return (Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index];
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode child = _btree.ProduceNode(ChildID(reader
				, index));
			if (_children != null)
			{
				if (_cached || child.CanWrite())
				{
					_children[index] = child;
				}
			}
			return child;
		}

		private int ChildID(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			if (_children == null)
			{
				SeekChild(reader, index);
				return reader.ReadInt();
			}
			return ChildID(index);
		}

		private int ChildID(int index)
		{
			if (ChildLoaded(index))
			{
				return ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index]).GetID();
			}
			return ((int)_children[index]);
		}

		private bool ChildLoaded(int index)
		{
			if (_children == null)
			{
				return false;
			}
			return _children[index] is Db4objects.Db4o.Internal.Btree.BTreeNode;
		}

		private bool ChildCanSupplyFirstKey(int index)
		{
			if (!ChildLoaded(index))
			{
				return false;
			}
			return ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[index]).CanWrite();
		}

		internal void Commit(Db4objects.Db4o.Internal.Transaction trans)
		{
			CommitOrRollback(trans, true);
		}

		internal void CommitOrRollback(Db4objects.Db4o.Internal.Transaction trans, bool isCommit
			)
		{
			if (_dead)
			{
				return;
			}
			_cached = false;
			if (!_isLeaf)
			{
				return;
			}
			if (!IsDirty(trans))
			{
				return;
			}
			object keyZero = _keys[0];
			object[] tempKeys = new object[_btree.NodeSize()];
			int count = 0;
			for (int i = 0; i < _count; i++)
			{
				object key = _keys[i];
				Db4objects.Db4o.Internal.Btree.BTreePatch patch = KeyPatch(i);
				if (patch != null)
				{
					key = isCommit ? patch.Commit(trans, _btree) : patch.Rollback(trans, _btree);
				}
				if (key != Db4objects.Db4o.Foundation.No4.INSTANCE)
				{
					tempKeys[count] = key;
					count++;
				}
			}
			_keys = tempKeys;
			_count = count;
			if (FreeIfEmpty(trans))
			{
				return;
			}
			SetStateDirty();
			if (_keys[0] != keyZero)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		private bool FreeIfEmpty(Db4objects.Db4o.Internal.Transaction trans)
		{
			return FreeIfEmpty(trans, _count);
		}

		private bool FreeIfEmpty(Db4objects.Db4o.Internal.Transaction trans, int count)
		{
			if (count > 0)
			{
				return false;
			}
			if (IsRoot())
			{
				return false;
			}
			Free(trans);
			return true;
		}

		private bool IsRoot()
		{
			return _btree.Root() == this;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is Db4objects.Db4o.Internal.Btree.BTreeNode))
			{
				return false;
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode other = (Db4objects.Db4o.Internal.Btree.BTreeNode
				)obj;
			return GetID() == other.GetID();
		}

		public override int GetHashCode()
		{
			return GetID();
		}

		private void Free(Db4objects.Db4o.Internal.Transaction trans)
		{
			_dead = true;
			if (!IsRoot())
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.RemoveChild(trans, this);
			}
			PointPreviousTo(trans, _nextID);
			PointNextTo(trans, _previousID);
			trans.SystemTransaction().SlotFreePointerOnCommit(GetID());
			_btree.RemoveNode(this);
		}

		internal void HoldChildrenAsIDs()
		{
			if (_children == null)
			{
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_children[i] is Db4objects.Db4o.Internal.Btree.BTreeNode)
				{
					_children[i] = ((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).GetID();
				}
			}
		}

		private void RemoveChild(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode
			 child)
		{
			PrepareWrite(trans);
			int id = child.GetID();
			for (int i = 0; i < _count; i++)
			{
				if (ChildID(i) == id)
				{
					if (FreeIfEmpty(trans, _count - 1))
					{
						return;
					}
					Remove(i);
					if (i <= 1)
					{
						TellParentAboutChangedKey(trans);
					}
					if (_count == 0)
					{
						_isLeaf = true;
					}
					return;
				}
			}
			throw new System.InvalidOperationException("child not found");
		}

		private void KeyChanged(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Btree.BTreeNode
			 child)
		{
			PrepareWrite(trans);
			int id = child.GetID();
			for (int i = 0; i < _count; i++)
			{
				if (ChildID(i) == id)
				{
					_keys[i] = child._keys[0];
					_children[i] = child;
					KeyChanged(trans, i);
					return;
				}
			}
			throw new System.InvalidOperationException("child not found");
		}

		private void TellParentAboutChangedKey(Db4objects.Db4o.Internal.Transaction trans
			)
		{
			if (!IsRoot())
			{
				Db4objects.Db4o.Internal.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.KeyChanged(trans, this);
			}
		}

		private bool IsDirty(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (!CanWrite())
			{
				return false;
			}
			for (int i = 0; i < _count; i++)
			{
				if (KeyPatch(trans, i) != null)
				{
					return true;
				}
			}
			return false;
		}

		private int CompareInWriteMode(int index)
		{
			return KeyHandler().CompareTo(Key(index));
		}

		private int CompareInReadMode(Db4objects.Db4o.Internal.Buffer reader, int index)
		{
			SeekKey(reader, index);
			return KeyHandler().CompareTo(KeyHandler().ReadIndexEntry(reader));
		}

		public int Count()
		{
			return _count;
		}

		private int EntryLength()
		{
			int len = KeyHandler().LinkLength();
			if (!_isLeaf)
			{
				len += Db4objects.Db4o.Internal.Const4.ID_LENGTH;
			}
			return len;
		}

		public int FirstKeyIndex(Db4objects.Db4o.Internal.Transaction trans)
		{
			for (int ix = 0; ix < _count; ix++)
			{
				if (IndexIsValid(trans, ix))
				{
					return ix;
				}
			}
			return -1;
		}

		public int LastKeyIndex(Db4objects.Db4o.Internal.Transaction trans)
		{
			for (int ix = _count - 1; ix >= 0; ix--)
			{
				if (IndexIsValid(trans, ix))
				{
					return ix;
				}
			}
			return -1;
		}

		public bool IndexIsValid(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			if (!CanWrite())
			{
				return true;
			}
			Db4objects.Db4o.Internal.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return true;
			}
			return patch.Key(trans) != Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		private object FirstKey(Db4objects.Db4o.Internal.Transaction trans)
		{
			int index = FirstKeyIndex(trans);
			if (-1 == index)
			{
				return Db4objects.Db4o.Foundation.No4.INSTANCE;
			}
			return Key(trans, index);
		}

		public override byte GetIdentifier()
		{
			return Db4objects.Db4o.Internal.Const4.BTREE_NODE;
		}

		private void PrepareInsert(int pos)
		{
			if (pos > LastIndex())
			{
				_count++;
				return;
			}
			int len = _count - pos;
			System.Array.Copy(_keys, pos, _keys, pos + 1, len);
			if (_children != null)
			{
				System.Array.Copy(_children, pos, _children, pos + 1, len);
			}
			_count++;
		}

		private void Remove(int pos)
		{
			int len = _count - pos;
			_count--;
			System.Array.Copy(_keys, pos + 1, _keys, pos, len);
			_keys[_count] = null;
			if (_children != null)
			{
				System.Array.Copy(_children, pos + 1, _children, pos, len);
				_children[_count] = null;
			}
		}

		internal object Key(int index)
		{
			object obj = _keys[index];
			if (obj is Db4objects.Db4o.Internal.Btree.BTreePatch)
			{
				return ((Db4objects.Db4o.Internal.Btree.BTreePatch)obj).GetObject();
			}
			return obj;
		}

		internal object Key(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader, int index)
		{
			if (CanWrite())
			{
				return Key(trans, index);
			}
			SeekKey(reader, index);
			return KeyHandler().ReadIndexEntry(reader);
		}

		internal object Key(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			Db4objects.Db4o.Internal.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return _keys[index];
			}
			return patch.Key(trans);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePatch KeyPatch(int index)
		{
			object obj = _keys[index];
			if (obj is Db4objects.Db4o.Internal.Btree.BTreePatch)
			{
				return (Db4objects.Db4o.Internal.Btree.BTreePatch)obj;
			}
			return null;
		}

		private Db4objects.Db4o.Internal.Btree.BTreePatch KeyPatch(Db4objects.Db4o.Internal.Transaction
			 trans, int index)
		{
			object obj = _keys[index];
			if (obj is Db4objects.Db4o.Internal.Btree.BTreePatch)
			{
				return ((Db4objects.Db4o.Internal.Btree.BTreePatch)obj).ForTransaction(trans);
			}
			return null;
		}

		private Db4objects.Db4o.Internal.IX.IIndexable4 KeyHandler()
		{
			return _btree.KeyHandler();
		}

		internal void MarkAsCached(int height)
		{
			_cached = true;
			_btree.AddNode(this);
			if (_isLeaf || (_children == null))
			{
				return;
			}
			height--;
			if (height < 1)
			{
				HoldChildrenAsIDs();
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_children[i] is Db4objects.Db4o.Internal.Btree.BTreeNode)
				{
					((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).MarkAsCached(height);
				}
			}
		}

		public override int OwnLength()
		{
			return SLOT_LEADING_LENGTH + (_count * EntryLength()) + Db4objects.Db4o.Internal.Const4
				.BRACKETS_BYTES;
		}

		internal Db4objects.Db4o.Internal.Buffer PrepareRead(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			if (CanWrite())
			{
				return null;
			}
			if (IsNew())
			{
				return null;
			}
			if (_cached)
			{
				Read(trans.SystemTransaction());
				_btree.AddToProcessing(this);
				return null;
			}
			Db4objects.Db4o.Internal.Buffer reader = trans.i_file.ReadReaderByID(trans.SystemTransaction
				(), GetID());
			ReadNodeHeader(reader);
			return reader;
		}

		internal void PrepareWrite(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (_dead)
			{
				return;
			}
			if (CanWrite())
			{
				SetStateDirty();
				return;
			}
			Read(trans.SystemTransaction());
			SetStateDirty();
			_btree.AddToProcessing(this);
		}

		private void PrepareArrays()
		{
			if (CanWrite())
			{
				return;
			}
			_keys = new object[_btree.NodeSize()];
			if (!_isLeaf)
			{
				_children = new object[_btree.NodeSize()];
			}
		}

		private void ReadNodeHeader(Db4objects.Db4o.Internal.Buffer reader)
		{
			_count = reader.ReadInt();
			byte leafByte = reader.ReadByte();
			_isLeaf = (leafByte == 1);
			_parentID = reader.ReadInt();
			_previousID = reader.ReadInt();
			_nextID = reader.ReadInt();
		}

		public override void ReadThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			ReadNodeHeader(reader);
			PrepareArrays();
			bool isInner = !_isLeaf;
			for (int i = 0; i < _count; i++)
			{
				_keys[i] = KeyHandler().ReadIndexEntry(reader);
				if (isInner)
				{
					_children[i] = reader.ReadInt();
				}
			}
		}

		public void Remove(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			if (!_isLeaf)
			{
				throw new System.InvalidOperationException();
			}
			PrepareWrite(trans);
			Db4objects.Db4o.Internal.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				_keys[index] = NewRemovePatch(trans);
				KeyChanged(trans, index);
				return;
			}
			Db4objects.Db4o.Internal.Btree.BTreePatch transPatch = patch.ForTransaction(trans
				);
			if (transPatch != null)
			{
				if (transPatch.IsAdd())
				{
					CancelAdding(trans, index);
					return;
				}
				if (transPatch.IsCancelledRemoval())
				{
					Db4objects.Db4o.Internal.Btree.BTreeRemove removePatch = NewRemovePatch(trans, transPatch
						.GetObject());
					_keys[index] = ((Db4objects.Db4o.Internal.Btree.BTreeUpdate)patch).ReplacePatch(transPatch
						, removePatch);
					KeyChanged(trans, index);
					return;
				}
			}
			else
			{
				if (!patch.IsAdd())
				{
					((Db4objects.Db4o.Internal.Btree.BTreeUpdate)patch).Append(NewRemovePatch(trans));
					return;
				}
			}
			if (index != LastIndex())
			{
				if (CompareInWriteMode(index + 1) != 0)
				{
					return;
				}
				Remove(trans, index + 1);
				return;
			}
			Db4objects.Db4o.Internal.Btree.BTreeNode node = NextNode();
			if (node == null)
			{
				return;
			}
			node.PrepareWrite(trans);
			if (node.CompareInWriteMode(0) != 0)
			{
				return;
			}
			node.Remove(trans, 0);
		}

		private void CancelAdding(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			_btree.NotifyRemoveListener(KeyPatch(index).GetObject());
			if (FreeIfEmpty(trans, _count - 1))
			{
				SizeDecrement(trans);
				return;
			}
			Remove(index);
			KeyChanged(trans, index);
			SizeDecrement(trans);
		}

		private void SizeDecrement(Db4objects.Db4o.Internal.Transaction trans)
		{
			_btree.SizeChanged(trans, -1);
		}

		private int LastIndex()
		{
			return _count - 1;
		}

		private Db4objects.Db4o.Internal.Btree.BTreeRemove NewRemovePatch(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return NewRemovePatch(trans, CurrentKey());
		}

		private Db4objects.Db4o.Internal.Btree.BTreeRemove NewRemovePatch(Db4objects.Db4o.Internal.Transaction
			 trans, object key)
		{
			_btree.SizeChanged(trans, -1);
			return new Db4objects.Db4o.Internal.Btree.BTreeRemove(trans, key);
		}

		private void KeyChanged(Db4objects.Db4o.Internal.Transaction trans, int index)
		{
			if (index == 0)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		internal void Rollback(Db4objects.Db4o.Internal.Transaction trans)
		{
			CommitOrRollback(trans, false);
		}

		private Db4objects.Db4o.Internal.Btree.Searcher Search(Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			return Search(reader, Db4objects.Db4o.Internal.Btree.SearchTarget.ANY);
		}

		private Db4objects.Db4o.Internal.Btree.Searcher Search(Db4objects.Db4o.Internal.Buffer
			 reader, Db4objects.Db4o.Internal.Btree.SearchTarget target)
		{
			Db4objects.Db4o.Internal.Btree.Searcher s = new Db4objects.Db4o.Internal.Btree.Searcher
				(target, _count);
			if (CanWrite())
			{
				while (s.Incomplete())
				{
					s.ResultIs(CompareInWriteMode(s.Cursor()));
				}
			}
			else
			{
				while (s.Incomplete())
				{
					s.ResultIs(CompareInReadMode(reader, s.Cursor()));
				}
			}
			return s;
		}

		private void SeekAfterKey(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			SeekKey(reader, ix);
			reader._offset += KeyHandler().LinkLength();
		}

		private void SeekChild(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			SeekAfterKey(reader, ix);
		}

		private void SeekKey(Db4objects.Db4o.Internal.Buffer reader, int ix)
		{
			reader._offset = SLOT_LEADING_LENGTH + (EntryLength() * ix);
		}

		private Db4objects.Db4o.Internal.Btree.BTreeNode Split(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			Db4objects.Db4o.Internal.Btree.BTreeNode res = new Db4objects.Db4o.Internal.Btree.BTreeNode
				(_btree, _btree._halfNodeSize, _isLeaf, _parentID, GetID(), _nextID);
			System.Array.Copy(_keys, _btree._halfNodeSize, res._keys, 0, _btree._halfNodeSize
				);
			for (int i = _btree._halfNodeSize; i < _keys.Length; i++)
			{
				_keys[i] = null;
			}
			if (_children != null)
			{
				res._children = new object[_btree.NodeSize()];
				System.Array.Copy(_children, _btree._halfNodeSize, res._children, 0, _btree._halfNodeSize
					);
				for (int i = _btree._halfNodeSize; i < _children.Length; i++)
				{
					_children[i] = null;
				}
			}
			_count = _btree._halfNodeSize;
			res.Write(trans.SystemTransaction());
			_btree.AddNode(res);
			int splitID = res.GetID();
			PointNextTo(trans, splitID);
			SetNextID(trans, splitID);
			if (_children != null)
			{
				for (int i = 0; i < _btree._halfNodeSize; i++)
				{
					if (res._children[i] == null)
					{
						break;
					}
					res.Child(i).SetParentID(trans, splitID);
				}
			}
			return res;
		}

		private void PointNextTo(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			if (_nextID != 0)
			{
				NextNode().SetPreviousID(trans, id);
			}
		}

		private void PointPreviousTo(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			if (_previousID != 0)
			{
				PreviousNode().SetNextID(trans, id);
			}
		}

		public Db4objects.Db4o.Internal.Btree.BTreeNode PreviousNode()
		{
			if (_previousID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_previousID);
		}

		public Db4objects.Db4o.Internal.Btree.BTreeNode NextNode()
		{
			if (_nextID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_nextID);
		}

		internal Db4objects.Db4o.Internal.Btree.BTreePointer FirstPointer(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				return LeafFirstPointer(trans, reader);
			}
			return BranchFirstPointer(trans, reader);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer BranchFirstPointer(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			for (int i = 0; i < _count; i++)
			{
				Db4objects.Db4o.Internal.Btree.BTreePointer childFirstPointer = Child(reader, i).
					FirstPointer(trans);
				if (childFirstPointer != null)
				{
					return childFirstPointer;
				}
			}
			return null;
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer LeafFirstPointer(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			int index = FirstKeyIndex(trans);
			if (index == -1)
			{
				return null;
			}
			return new Db4objects.Db4o.Internal.Btree.BTreePointer(trans, reader, this, index
				);
		}

		public Db4objects.Db4o.Internal.Btree.BTreePointer LastPointer(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				return LeafLastPointer(trans, reader);
			}
			return BranchLastPointer(trans, reader);
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer BranchLastPointer(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			for (int i = _count - 1; i >= 0; i--)
			{
				Db4objects.Db4o.Internal.Btree.BTreePointer childLastPointer = Child(reader, i).LastPointer
					(trans);
				if (childLastPointer != null)
				{
					return childLastPointer;
				}
			}
			return null;
		}

		private Db4objects.Db4o.Internal.Btree.BTreePointer LeafLastPointer(Db4objects.Db4o.Internal.Transaction
			 trans, Db4objects.Db4o.Internal.Buffer reader)
		{
			int index = LastKeyIndex(trans);
			if (index == -1)
			{
				return null;
			}
			return new Db4objects.Db4o.Internal.Btree.BTreePointer(trans, reader, this, index
				);
		}

		internal void Purge()
		{
			if (_dead)
			{
				_keys = null;
				_children = null;
				return;
			}
			if (_cached)
			{
				return;
			}
			if (!CanWrite())
			{
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_keys[i] is Db4objects.Db4o.Internal.Btree.BTreePatch)
				{
					HoldChildrenAsIDs();
					_btree.AddNode(this);
					return;
				}
			}
		}

		private void SetParentID(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_parentID = id;
		}

		private void SetPreviousID(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_previousID = id;
		}

		private void SetNextID(Db4objects.Db4o.Internal.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_nextID = id;
		}

		public void TraverseKeys(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			if (_isLeaf)
			{
				for (int i = 0; i < _count; i++)
				{
					object obj = Key(trans, reader, i);
					if (obj != Db4objects.Db4o.Foundation.No4.INSTANCE)
					{
						visitor.Visit(obj);
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					Child(reader, i).TraverseKeys(trans, visitor);
				}
			}
		}

		public override bool WriteObjectBegin()
		{
			if (_dead)
			{
				return false;
			}
			if (!CanWrite())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public override void WriteThis(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 a_writer)
		{
			int count = 0;
			int startOffset = a_writer._offset;
			a_writer.IncrementOffset(COUNT_LEAF_AND_3_LINK_LENGTH);
			if (_isLeaf)
			{
				for (int i = 0; i < _count; i++)
				{
					object obj = Key(trans, i);
					if (obj != Db4objects.Db4o.Foundation.No4.INSTANCE)
					{
						count++;
						KeyHandler().WriteIndexEntry(a_writer, obj);
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					if (ChildCanSupplyFirstKey(i))
					{
						Db4objects.Db4o.Internal.Btree.BTreeNode child = (Db4objects.Db4o.Internal.Btree.BTreeNode
							)_children[i];
						object childKey = child.FirstKey(trans);
						if (childKey != Db4objects.Db4o.Foundation.No4.INSTANCE)
						{
							count++;
							KeyHandler().WriteIndexEntry(a_writer, childKey);
							a_writer.WriteIDOf(trans, child);
						}
					}
					else
					{
						count++;
						KeyHandler().WriteIndexEntry(a_writer, Key(i));
						a_writer.WriteIDOf(trans, _children[i]);
					}
				}
			}
			int endOffset = a_writer._offset;
			a_writer._offset = startOffset;
			a_writer.WriteInt(count);
			a_writer.Append(_isLeaf ? (byte)1 : (byte)0);
			a_writer.WriteInt(_parentID);
			a_writer.WriteInt(_previousID);
			a_writer.WriteInt(_nextID);
			a_writer._offset = endOffset;
		}

		public override string ToString()
		{
			if (_count == 0)
			{
				return "Node " + GetID() + " not loaded";
			}
			string str = "\nBTreeNode";
			str += "\nid: " + GetID();
			str += "\nparent: " + _parentID;
			str += "\nprevious: " + _previousID;
			str += "\nnext: " + _nextID;
			str += "\ncount:" + _count;
			str += "\nleaf:" + _isLeaf + "\n";
			if (CanWrite())
			{
				str += " { ";
				bool first = true;
				for (int i = 0; i < _count; i++)
				{
					if (_keys[i] != null)
					{
						if (!first)
						{
							str += ", ";
						}
						str += _keys[i].ToString();
						first = false;
					}
				}
				str += " }";
			}
			return str;
		}

		public void DebugLoadFully(Db4objects.Db4o.Internal.Transaction trans)
		{
			PrepareWrite(trans);
			if (_isLeaf)
			{
				return;
			}
			for (int i = 0; i < _count; ++i)
			{
				if (_children[i] is int)
				{
					_children[i] = Btree().ProduceNode(((int)_children[i]));
				}
				((Db4objects.Db4o.Internal.Btree.BTreeNode)_children[i]).DebugLoadFully(trans);
			}
		}

		public static void DefragIndex(Db4objects.Db4o.Internal.ReaderPair readers, Db4objects.Db4o.Internal.IX.IIndexable4
			 keyHandler)
		{
			int count = readers.ReadInt();
			byte leafByte = readers.ReadByte();
			bool isLeaf = (leafByte == 1);
			readers.CopyID();
			readers.CopyID();
			readers.CopyID();
			for (int i = 0; i < count; i++)
			{
				keyHandler.DefragIndexEntry(readers);
				if (!isLeaf)
				{
					readers.CopyID();
				}
			}
		}

		public bool IsLeaf()
		{
			return _isLeaf;
		}

		/// <summary>This traversal goes over all nodes, not just leafs</summary>
		internal void TraverseAllNodes(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			Db4objects.Db4o.Internal.Buffer reader = PrepareRead(trans);
			command.Visit(this);
			if (_isLeaf)
			{
				return;
			}
			for (int childIdx = 0; childIdx < _count; childIdx++)
			{
				Child(reader, childIdx).TraverseAllNodes(trans, command);
			}
		}
	}
}
