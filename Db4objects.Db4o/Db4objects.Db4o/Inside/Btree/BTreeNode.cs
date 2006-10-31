namespace Db4objects.Db4o.Inside.Btree
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
	public class BTreeNode : Db4objects.Db4o.YapMeta
	{
		private const int COUNT_LEAF_AND_3_LINK_LENGTH = (Db4objects.Db4o.YapConst.INT_LENGTH
			 * 4) + 1;

		private const int SLOT_LEADING_LENGTH = Db4objects.Db4o.YapConst.LEADING_LENGTH +
			 COUNT_LEAF_AND_3_LINK_LENGTH;

		internal readonly Db4objects.Db4o.Inside.Btree.BTree _btree;

		private int _count;

		private bool _isLeaf;

		private object[] _keys;

		/// <summary>Can contain BTreeNode or Integer for ID of BTreeNode</summary>
		private object[] _children;

		/// <summary>Only used for leafs</summary>
		private object[] _values;

		private int _parentID;

		private int _previousID;

		private int _nextID;

		private bool _cached;

		private bool _dead;

		public BTreeNode(Db4objects.Db4o.Inside.Btree.BTree btree, int count, bool isLeaf
			, int parentID, int previousID, int nextID)
		{
			_btree = btree;
			_parentID = parentID;
			_previousID = previousID;
			_nextID = nextID;
			_count = count;
			_isLeaf = isLeaf;
			PrepareArrays();
			SetStateDirty();
		}

		public BTreeNode(int id, Db4objects.Db4o.Inside.Btree.BTree btree)
		{
			_btree = btree;
			SetID(id);
			SetStateDeactivated();
		}

		public BTreeNode(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTreeNode
			 firstChild, Db4objects.Db4o.Inside.Btree.BTreeNode secondChild) : this(firstChild
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

		public virtual Db4objects.Db4o.Inside.Btree.BTree Btree()
		{
			return _btree;
		}

		/// <returns>
		/// the split node if this node is split
		/// or this if the first key has changed
		/// </returns>
		public virtual Db4objects.Db4o.Inside.Btree.BTreeNode Add(Db4objects.Db4o.Transaction
			 trans)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
			Db4objects.Db4o.Inside.Btree.Searcher s = Search(reader);
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
				if (HandlesValues())
				{
					_values[s.Cursor()] = ValueHandler().Current();
				}
			}
			else
			{
				Db4objects.Db4o.Inside.Btree.BTreeNode childNode = Child(reader, s.Cursor());
				Db4objects.Db4o.Inside.Btree.BTreeNode childNodeOrSplit = childNode.Add(trans);
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
			if (_count >= _btree.NodeSize())
			{
				return Split(trans);
			}
			if (s.Cursor() == 0)
			{
				return this;
			}
			return null;
		}

		private Db4objects.Db4o.Inside.Btree.BTreeAdd NewAddPatch(Db4objects.Db4o.Transaction
			 trans)
		{
			SizeIncrement(trans);
			return new Db4objects.Db4o.Inside.Btree.BTreeAdd(trans, CurrentKey());
		}

		private object CurrentKey()
		{
			return KeyHandler().Current();
		}

		private void CancelRemoval(Db4objects.Db4o.Transaction trans, int index)
		{
			Db4objects.Db4o.Inside.Btree.BTreeUpdate patch = (Db4objects.Db4o.Inside.Btree.BTreeUpdate
				)KeyPatch(index);
			Db4objects.Db4o.Inside.Btree.BTreeUpdate nextPatch = patch.RemoveFor(trans);
			_keys[index] = NewCancelledRemoval(trans, patch.GetObject(), nextPatch);
			SizeIncrement(trans);
		}

		private Db4objects.Db4o.Inside.Btree.BTreePatch NewCancelledRemoval(Db4objects.Db4o.Transaction
			 trans, object originalObject, Db4objects.Db4o.Inside.Btree.BTreeUpdate existingPatches
			)
		{
			return new Db4objects.Db4o.Inside.Btree.BTreeCancelledRemoval(trans, originalObject
				, CurrentKey(), existingPatches);
		}

		private void SizeIncrement(Db4objects.Db4o.Transaction trans)
		{
			_btree.SizeChanged(trans, 1);
		}

		private bool WasRemoved(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.Searcher
			 s)
		{
			if (!s.FoundMatch())
			{
				return false;
			}
			Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(trans, s.Cursor());
			return patch != null && patch.IsRemove();
		}

		internal virtual Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult SearchLeaf(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.Inside.Btree.SearchTarget target)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
			Db4objects.Db4o.Inside.Btree.Searcher s = Search(reader, target);
			if (!_isLeaf)
			{
				return Child(reader, s.Cursor()).SearchLeaf(trans, target);
			}
			if (!s.FoundMatch() || target == Db4objects.Db4o.Inside.Btree.SearchTarget.ANY ||
				 target == Db4objects.Db4o.Inside.Btree.SearchTarget.HIGHEST)
			{
				return new Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult(trans, reader, Btree
					(), s, this);
			}
			if (target == Db4objects.Db4o.Inside.Btree.SearchTarget.LOWEST)
			{
				Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult res = FindLowestLeafMatch(trans
					, s.Cursor() - 1);
				if (res != null)
				{
					return res;
				}
				return CreateMatchingSearchResult(trans, reader, s.Cursor());
			}
			throw new System.InvalidOperationException();
		}

		private Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult FindLowestLeafMatch(Db4objects.Db4o.Transaction
			 trans, int index)
		{
			return FindLowestLeafMatch(trans, PrepareRead(trans), index);
		}

		private Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult FindLowestLeafMatch(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader reader, int index)
		{
			if (index >= 0)
			{
				if (!CompareInReadModeEquals(reader, index))
				{
					return null;
				}
				if (index > 0)
				{
					Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult res = FindLowestLeafMatch(trans
						, reader, index - 1);
					if (res != null)
					{
						return res;
					}
					return CreateMatchingSearchResult(trans, reader, index);
				}
			}
			Db4objects.Db4o.Inside.Btree.BTreeNode node = PreviousNode();
			if (node != null)
			{
				Db4objects.Db4o.YapReader nodeReader = node.PrepareRead(trans);
				Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult res = node.FindLowestLeafMatch
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

		private bool CompareInReadModeEquals(Db4objects.Db4o.YapReader reader, int index)
		{
			return CompareInReadMode(reader, index) == 0;
		}

		private Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult CreateMatchingSearchResult
			(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader reader, int index)
		{
			return new Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult(trans, reader, Btree
				(), this, index, true);
		}

		public virtual bool CanWrite()
		{
			return _keys != null;
		}

		internal virtual Db4objects.Db4o.Inside.Btree.BTreeNode Child(int index)
		{
			if (_children[index] is Db4objects.Db4o.Inside.Btree.BTreeNode)
			{
				return (Db4objects.Db4o.Inside.Btree.BTreeNode)_children[index];
			}
			return _btree.ProduceNode(((int)_children[index]));
		}

		internal virtual Db4objects.Db4o.Inside.Btree.BTreeNode Child(Db4objects.Db4o.YapReader
			 reader, int index)
		{
			if (ChildLoaded(index))
			{
				return (Db4objects.Db4o.Inside.Btree.BTreeNode)_children[index];
			}
			Db4objects.Db4o.Inside.Btree.BTreeNode child = _btree.ProduceNode(ChildID(reader, 
				index));
			if (_children != null)
			{
				if (_cached || child.CanWrite())
				{
					_children[index] = child;
				}
			}
			return child;
		}

		private int ChildID(Db4objects.Db4o.YapReader reader, int index)
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
				return ((Db4objects.Db4o.Inside.Btree.BTreeNode)_children[index]).GetID();
			}
			return ((int)_children[index]);
		}

		private bool ChildLoaded(int index)
		{
			if (_children == null)
			{
				return false;
			}
			return _children[index] is Db4objects.Db4o.Inside.Btree.BTreeNode;
		}

		private bool ChildCanSupplyFirstKey(int index)
		{
			if (!ChildLoaded(index))
			{
				return false;
			}
			return ((Db4objects.Db4o.Inside.Btree.BTreeNode)_children[index]).CanWrite();
		}

		internal virtual void Commit(Db4objects.Db4o.Transaction trans)
		{
			CommitOrRollback(trans, true);
		}

		internal virtual void CommitOrRollback(Db4objects.Db4o.Transaction trans, bool isCommit
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
			bool vals = HandlesValues();
			object[] tempKeys = new object[_btree.NodeSize()];
			object[] tempValues = vals ? new object[_btree.NodeSize()] : null;
			int count = 0;
			for (int i = 0; i < _count; i++)
			{
				object key = _keys[i];
				Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(i);
				if (patch != null)
				{
					key = isCommit ? patch.Commit(trans, _btree) : patch.Rollback(trans, _btree);
				}
				if (key != Db4objects.Db4o.Foundation.No4.INSTANCE)
				{
					tempKeys[count] = key;
					if (vals)
					{
						tempValues[count] = _values[i];
					}
					count++;
				}
			}
			_keys = tempKeys;
			_values = tempValues;
			_count = count;
			if (FreeIfEmpty(trans))
			{
				return;
			}
			if (_keys[0] != keyZero)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		private bool FreeIfEmpty(Db4objects.Db4o.Transaction trans)
		{
			return FreeIfEmpty(trans, _count);
		}

		private bool FreeIfEmpty(Db4objects.Db4o.Transaction trans, int count)
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
			if (!(obj is Db4objects.Db4o.Inside.Btree.BTreeNode))
			{
				return false;
			}
			Db4objects.Db4o.Inside.Btree.BTreeNode other = (Db4objects.Db4o.Inside.Btree.BTreeNode
				)obj;
			return GetID() == other.GetID();
		}

		private void Free(Db4objects.Db4o.Transaction trans)
		{
			_dead = true;
			if (!IsRoot())
			{
				Db4objects.Db4o.Inside.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.RemoveChild(trans, this);
			}
			PointPreviousTo(trans, _nextID);
			PointNextTo(trans, _previousID);
			trans.SystemTransaction().SlotFreePointerOnCommit(GetID());
			_btree.RemoveNode(this);
		}

		internal virtual void HoldChildrenAsIDs()
		{
			if (_children == null)
			{
				return;
			}
			for (int i = 0; i < _count; i++)
			{
				if (_children[i] is Db4objects.Db4o.Inside.Btree.BTreeNode)
				{
					_children[i] = ((Db4objects.Db4o.Inside.Btree.BTreeNode)_children[i]).GetID();
				}
			}
		}

		private void RemoveChild(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTreeNode
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
						PrepareValues();
					}
					return;
				}
			}
			throw new System.InvalidOperationException("child not found");
		}

		private void KeyChanged(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Btree.BTreeNode
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

		private void TellParentAboutChangedKey(Db4objects.Db4o.Transaction trans)
		{
			if (!IsRoot())
			{
				Db4objects.Db4o.Inside.Btree.BTreeNode parent = _btree.ProduceNode(_parentID);
				parent.KeyChanged(trans, this);
			}
		}

		private bool IsDirty(Db4objects.Db4o.Transaction trans)
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

		private void Compare(Db4objects.Db4o.Inside.Btree.Searcher s, Db4objects.Db4o.YapReader
			 reader)
		{
			if (CanWrite())
			{
				s.ResultIs(CompareInWriteMode(s.Cursor()));
			}
			else
			{
				s.ResultIs(CompareInReadMode(reader, s.Cursor()));
			}
		}

		private int CompareInWriteMode(int index)
		{
			return KeyHandler().CompareTo(Key(index));
		}

		private int CompareInReadMode(Db4objects.Db4o.YapReader reader, int index)
		{
			if (CanWrite())
			{
				return CompareInWriteMode(index);
			}
			SeekKey(reader, index);
			return KeyHandler().CompareTo(KeyHandler().ReadIndexEntry(reader));
		}

		public virtual int Count()
		{
			return _count;
		}

		private int EntryLength()
		{
			int len = KeyHandler().LinkLength();
			if (_isLeaf)
			{
				if (HandlesValues())
				{
					len += ValueHandler().LinkLength();
				}
			}
			else
			{
				len += Db4objects.Db4o.YapConst.ID_LENGTH;
			}
			return len;
		}

		public virtual int FirstKeyIndex(Db4objects.Db4o.Transaction trans)
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

		public virtual int LastKeyIndex(Db4objects.Db4o.Transaction trans)
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

		public virtual bool IndexIsValid(Db4objects.Db4o.Transaction trans, int index)
		{
			if (!CanWrite())
			{
				return true;
			}
			Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return true;
			}
			return patch.Key(trans) != Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		private object FirstKey(Db4objects.Db4o.Transaction trans)
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
			return Db4objects.Db4o.YapConst.BTREE_NODE;
		}

		private bool HandlesValues()
		{
			return _btree._valueHandler != Db4objects.Db4o.Null.INSTANCE;
		}

		private void PrepareInsert(int pos)
		{
			if (pos < 0)
			{
				throw new System.ArgumentException("pos " + pos);
			}
			if (pos > LastIndex())
			{
				_count++;
				return;
			}
			int len = _count - pos;
			System.Array.Copy(_keys, pos, _keys, pos + 1, len);
			if (_values != null)
			{
				System.Array.Copy(_values, pos, _values, pos + 1, len);
			}
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
			if (_values != null)
			{
				System.Array.Copy(_values, pos + 1, _values, pos, len);
				_values[_count] = null;
			}
			if (_children != null)
			{
				System.Array.Copy(_children, pos + 1, _children, pos, len);
				_children[_count] = null;
			}
		}

		internal virtual object Key(int index)
		{
			Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return _keys[index];
			}
			return patch.GetObject();
		}

		internal virtual object Key(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader, int index)
		{
			if (CanWrite())
			{
				return Key(trans, index);
			}
			SeekKey(reader, index);
			return KeyHandler().ReadIndexEntry(reader);
		}

		internal virtual object Key(Db4objects.Db4o.Transaction trans, int index)
		{
			Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				return _keys[index];
			}
			return patch.Key(trans);
		}

		private Db4objects.Db4o.Inside.Btree.BTreePatch KeyPatch(int index)
		{
			if (_keys[index] is Db4objects.Db4o.Inside.Btree.BTreePatch)
			{
				return (Db4objects.Db4o.Inside.Btree.BTreePatch)_keys[index];
			}
			return null;
		}

		private Db4objects.Db4o.Inside.Btree.BTreePatch KeyPatch(Db4objects.Db4o.Transaction
			 trans, int index)
		{
			if (_keys[index] is Db4objects.Db4o.Inside.Btree.BTreePatch)
			{
				return ((Db4objects.Db4o.Inside.Btree.BTreePatch)_keys[index]).ForTransaction(trans
					);
			}
			return null;
		}

		private Db4objects.Db4o.Inside.IX.IIndexable4 KeyHandler()
		{
			return _btree._keyHandler;
		}

		internal virtual void MarkAsCached(int height)
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
				if (_children[i] is Db4objects.Db4o.Inside.Btree.BTreeNode)
				{
					((Db4objects.Db4o.Inside.Btree.BTreeNode)_children[i]).MarkAsCached(height);
				}
			}
		}

		public override int OwnLength()
		{
			return SLOT_LEADING_LENGTH + (_count * EntryLength()) + Db4objects.Db4o.YapConst.
				BRACKETS_BYTES;
		}

		internal virtual Db4objects.Db4o.YapReader PrepareRead(Db4objects.Db4o.Transaction
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
			Db4objects.Db4o.YapReader reader = trans.i_file.ReadReaderByID(trans.SystemTransaction
				(), GetID());
			ReadNodeHeader(reader);
			return reader;
		}

		internal virtual void PrepareWrite(Db4objects.Db4o.Transaction trans)
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
			if (_isLeaf)
			{
				PrepareValues();
			}
			else
			{
				_children = new object[_btree.NodeSize()];
			}
		}

		private void PrepareValues()
		{
			if (HandlesValues())
			{
				_values = new object[_btree.NodeSize()];
			}
		}

		private void ReadNodeHeader(Db4objects.Db4o.YapReader reader)
		{
			_count = reader.ReadInt();
			byte leafByte = reader.ReadByte();
			_isLeaf = (leafByte == 1);
			_parentID = reader.ReadInt();
			_previousID = reader.ReadInt();
			_nextID = reader.ReadInt();
		}

		public override void ReadThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader)
		{
			ReadNodeHeader(reader);
			PrepareArrays();
			bool isInner = !_isLeaf;
			bool vals = HandlesValues() && _isLeaf;
			for (int i = 0; i < _count; i++)
			{
				_keys[i] = KeyHandler().ReadIndexEntry(reader);
				if (vals)
				{
					_values[i] = ValueHandler().ReadIndexEntry(reader);
				}
				else
				{
					if (isInner)
					{
						_children[i] = reader.ReadInt();
					}
				}
			}
		}

		public virtual void Remove(Db4objects.Db4o.Transaction trans, int index)
		{
			if (!_isLeaf)
			{
				throw new System.InvalidOperationException();
			}
			PrepareWrite(trans);
			Db4objects.Db4o.Inside.Btree.BTreePatch patch = KeyPatch(index);
			if (patch == null)
			{
				_keys[index] = NewRemovePatch(trans);
				KeyChanged(trans, index);
				return;
			}
			Db4objects.Db4o.Inside.Btree.BTreePatch transPatch = patch.ForTransaction(trans);
			if (transPatch != null)
			{
				if (transPatch.IsAdd())
				{
					CancelAdding(trans, index);
					return;
				}
			}
			else
			{
				if (!patch.IsAdd())
				{
					((Db4objects.Db4o.Inside.Btree.BTreeUpdate)patch).Append(NewRemovePatch(trans));
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
			Db4objects.Db4o.Inside.Btree.BTreeNode node = NextNode();
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

		private void CancelAdding(Db4objects.Db4o.Transaction trans, int index)
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

		private void SizeDecrement(Db4objects.Db4o.Transaction trans)
		{
			_btree.SizeChanged(trans, -1);
		}

		private int LastIndex()
		{
			return _count - 1;
		}

		private Db4objects.Db4o.Inside.Btree.BTreeUpdate NewRemovePatch(Db4objects.Db4o.Transaction
			 trans)
		{
			_btree.SizeChanged(trans, -1);
			return new Db4objects.Db4o.Inside.Btree.BTreeRemove(trans, CurrentKey());
		}

		private void KeyChanged(Db4objects.Db4o.Transaction trans, int index)
		{
			if (index == 0)
			{
				TellParentAboutChangedKey(trans);
			}
		}

		internal virtual void Rollback(Db4objects.Db4o.Transaction trans)
		{
			CommitOrRollback(trans, false);
		}

		private Db4objects.Db4o.Inside.Btree.Searcher Search(Db4objects.Db4o.YapReader reader
			)
		{
			return Search(reader, Db4objects.Db4o.Inside.Btree.SearchTarget.ANY);
		}

		private Db4objects.Db4o.Inside.Btree.Searcher Search(Db4objects.Db4o.YapReader reader
			, Db4objects.Db4o.Inside.Btree.SearchTarget target)
		{
			Db4objects.Db4o.Inside.Btree.Searcher s = new Db4objects.Db4o.Inside.Btree.Searcher
				(target, _count);
			while (s.Incomplete())
			{
				Compare(s, reader);
			}
			return s;
		}

		private void SeekAfterKey(Db4objects.Db4o.YapReader reader, int ix)
		{
			SeekKey(reader, ix);
			reader._offset += KeyHandler().LinkLength();
		}

		private void SeekChild(Db4objects.Db4o.YapReader reader, int ix)
		{
			SeekAfterKey(reader, ix);
		}

		private void SeekKey(Db4objects.Db4o.YapReader reader, int ix)
		{
			reader._offset = SLOT_LEADING_LENGTH + (EntryLength() * ix);
		}

		private void SeekValue(Db4objects.Db4o.YapReader reader, int ix)
		{
			if (HandlesValues())
			{
				SeekAfterKey(reader, ix);
			}
			else
			{
				SeekKey(reader, ix);
			}
		}

		private Db4objects.Db4o.Inside.Btree.BTreeNode Split(Db4objects.Db4o.Transaction 
			trans)
		{
			Db4objects.Db4o.Inside.Btree.BTreeNode res = new Db4objects.Db4o.Inside.Btree.BTreeNode
				(_btree, _btree._halfNodeSize, _isLeaf, _parentID, GetID(), _nextID);
			System.Array.Copy(_keys, _btree._halfNodeSize, res._keys, 0, _btree._halfNodeSize
				);
			for (int i = _btree._halfNodeSize; i < _keys.Length; i++)
			{
				_keys[i] = null;
			}
			if (_values != null)
			{
				res._values = new object[_btree.NodeSize()];
				System.Array.Copy(_values, _btree._halfNodeSize, res._values, 0, _btree._halfNodeSize
					);
				for (int i = _btree._halfNodeSize; i < _values.Length; i++)
				{
					_values[i] = null;
				}
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

		private void PointNextTo(Db4objects.Db4o.Transaction trans, int id)
		{
			if (_nextID != 0)
			{
				NextNode().SetPreviousID(trans, id);
			}
		}

		private void PointPreviousTo(Db4objects.Db4o.Transaction trans, int id)
		{
			if (_previousID != 0)
			{
				PreviousNode().SetNextID(trans, id);
			}
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTreeNode PreviousNode()
		{
			if (_previousID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_previousID);
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTreeNode NextNode()
		{
			if (_nextID == 0)
			{
				return null;
			}
			return _btree.ProduceNode(_nextID);
		}

		internal virtual Db4objects.Db4o.Inside.Btree.BTreePointer FirstPointer(Db4objects.Db4o.Transaction
			 trans)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
			if (_isLeaf)
			{
				int index = FirstKeyIndex(trans);
				if (index == -1)
				{
					return null;
				}
				return new Db4objects.Db4o.Inside.Btree.BTreePointer(trans, reader, this, index);
			}
			for (int i = 0; i < _count; i++)
			{
				Db4objects.Db4o.Inside.Btree.BTreePointer childFirstPointer = Child(reader, i).FirstPointer
					(trans);
				if (childFirstPointer != null)
				{
					return childFirstPointer;
				}
			}
			return null;
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTreePointer LastPointer(Db4objects.Db4o.Transaction
			 trans)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
			if (_isLeaf)
			{
				int index = LastKeyIndex(trans);
				if (index == -1)
				{
					return null;
				}
				return new Db4objects.Db4o.Inside.Btree.BTreePointer(trans, reader, this, index);
			}
			for (int i = _count - 1; i >= 0; i--)
			{
				Db4objects.Db4o.Inside.Btree.BTreePointer childLastPointer = Child(reader, i).LastPointer
					(trans);
				if (childLastPointer != null)
				{
					return childLastPointer;
				}
			}
			return null;
		}

		internal virtual void Purge()
		{
			if (_dead)
			{
				_keys = null;
				_values = null;
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
				if (_keys[i] is Db4objects.Db4o.Inside.Btree.BTreePatch)
				{
					HoldChildrenAsIDs();
					_btree.AddNode(this);
					return;
				}
			}
		}

		private void SetParentID(Db4objects.Db4o.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_parentID = id;
			SetStateDirty();
		}

		private void SetPreviousID(Db4objects.Db4o.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_previousID = id;
			SetStateDirty();
		}

		private void SetNextID(Db4objects.Db4o.Transaction trans, int id)
		{
			PrepareWrite(trans);
			_nextID = id;
			SetStateDirty();
		}

		public virtual void TraverseKeys(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
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

		public virtual void TraverseValues(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			if (!HandlesValues())
			{
				TraverseKeys(trans, visitor);
				return;
			}
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
			if (_isLeaf)
			{
				for (int i = 0; i < _count; i++)
				{
					if (Key(trans, reader, i) != Db4objects.Db4o.Foundation.No4.INSTANCE)
					{
						visitor.Visit(Value(reader, i));
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					Child(reader, i).TraverseValues(trans, visitor);
				}
			}
		}

		internal virtual object Value(int index)
		{
			return _values[index];
		}

		internal virtual object Value(Db4objects.Db4o.YapReader reader, int index)
		{
			if (_values != null)
			{
				return _values[index];
			}
			SeekValue(reader, index);
			return ValueHandler().ReadIndexEntry(reader);
		}

		private Db4objects.Db4o.Inside.IX.IIndexable4 ValueHandler()
		{
			return _btree._valueHandler;
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

		public override void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 a_writer)
		{
			int count = 0;
			int startOffset = a_writer._offset;
			a_writer.IncrementOffset(COUNT_LEAF_AND_3_LINK_LENGTH);
			if (_isLeaf)
			{
				bool vals = HandlesValues();
				for (int i = 0; i < _count; i++)
				{
					object obj = Key(trans, i);
					if (obj != Db4objects.Db4o.Foundation.No4.INSTANCE)
					{
						count++;
						KeyHandler().WriteIndexEntry(a_writer, obj);
						if (vals)
						{
							ValueHandler().WriteIndexEntry(a_writer, _values[i]);
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < _count; i++)
				{
					if (ChildCanSupplyFirstKey(i))
					{
						Db4objects.Db4o.Inside.Btree.BTreeNode child = (Db4objects.Db4o.Inside.Btree.BTreeNode
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

		public virtual void DebugLoadFully(Db4objects.Db4o.Transaction trans)
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
				((Db4objects.Db4o.Inside.Btree.BTreeNode)_children[i]).DebugLoadFully(trans);
			}
		}

		public static void DefragIndex(Db4objects.Db4o.ReaderPair readers, Db4objects.Db4o.Inside.IX.IIndexable4
			 keyHandler, Db4objects.Db4o.Inside.IX.IIndexable4 valueHandler)
		{
			int count = readers.ReadInt();
			byte leafByte = readers.ReadByte();
			bool isLeaf = (leafByte == 1);
			bool handlesValues = (valueHandler != null) && isLeaf;
			readers.CopyID();
			readers.CopyID();
			readers.CopyID();
			for (int i = 0; i < count; i++)
			{
				keyHandler.DefragIndexEntry(readers);
				if (handlesValues)
				{
					valueHandler.DefragIndexEntry(readers);
				}
				else
				{
					if (!isLeaf)
					{
						readers.CopyID();
					}
				}
			}
		}

		public virtual bool IsLeaf()
		{
			return _isLeaf;
		}

		/// <summary>This traversal goes over all nodes, not just leafs</summary>
		internal virtual void TraverseAllNodes(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			Db4objects.Db4o.YapReader reader = PrepareRead(trans);
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
