namespace Db4objects.Db4o.Inside.Classindex
{
	/// <exclude></exclude>
	public class BTreeClassIndexStrategy : Db4objects.Db4o.Inside.Classindex.AbstractClassIndexStrategy
	{
		private Db4objects.Db4o.Inside.Btree.BTree _btreeIndex;

		public BTreeClassIndexStrategy(Db4objects.Db4o.YapClass yapClass) : base(yapClass
			)
		{
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTree Btree()
		{
			return _btreeIndex;
		}

		public override int EntryCount(Db4objects.Db4o.Transaction ta)
		{
			return _btreeIndex != null ? _btreeIndex.Size(ta) : 0;
		}

		public override void Initialize(Db4objects.Db4o.YapStream stream)
		{
			CreateBTreeIndex(stream, 0);
		}

		public override void Purge()
		{
		}

		public override void Read(Db4objects.Db4o.YapStream stream, int indexID)
		{
			ReadBTreeIndex(stream, indexID);
		}

		public override int Write(Db4objects.Db4o.Transaction trans)
		{
			if (_btreeIndex == null)
			{
				return 0;
			}
			_btreeIndex.Write(trans);
			return _btreeIndex.GetID();
		}

		public override void TraverseAll(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			if (_btreeIndex != null)
			{
				_btreeIndex.TraverseKeys(ta, command);
			}
		}

		private void CreateBTreeIndex(Db4objects.Db4o.YapStream stream, int btreeID)
		{
			if (stream.IsClient())
			{
				return;
			}
			_btreeIndex = ((Db4objects.Db4o.YapFile)stream).CreateBTreeClassIndex(btreeID);
			_btreeIndex.SetRemoveListener(new _AnonymousInnerClass61(this, stream));
		}

		private sealed class _AnonymousInnerClass61 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass61(BTreeClassIndexStrategy _enclosing, Db4objects.Db4o.YapStream
				 stream)
			{
				this._enclosing = _enclosing;
				this.stream = stream;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				Db4objects.Db4o.YapObject yo = stream.GetYapObject(id);
				if (yo != null)
				{
					stream.RemoveReference(yo);
				}
			}

			private readonly BTreeClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.YapStream stream;
		}

		private void ReadBTreeIndex(Db4objects.Db4o.YapStream stream, int indexId)
		{
			if (!stream.IsClient() && _btreeIndex == null)
			{
				CreateBTreeIndex(stream, indexId);
			}
		}

		protected override void InternalAdd(Db4objects.Db4o.Transaction trans, int id)
		{
			_btreeIndex.Add(trans, id);
		}

		protected override void InternalRemove(Db4objects.Db4o.Transaction ta, int id)
		{
			_btreeIndex.Remove(ta, id);
		}

		public override void DontDelete(Db4objects.Db4o.Transaction transaction, int id)
		{
		}

		public override void DefragReference(Db4objects.Db4o.YapClass yapClass, Db4objects.Db4o.ReaderPair
			 readers, int classIndexID)
		{
			int newID = -classIndexID;
			readers.WriteInt(newID);
		}

		public override int Id()
		{
			return _btreeIndex.GetID();
		}

		public override System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Transaction
			 trans)
		{
			return _btreeIndex.AllNodeIds(trans);
		}

		public override void DefragIndex(Db4objects.Db4o.ReaderPair readers)
		{
			_btreeIndex.DefragIndex(readers);
		}

		public override void DefragIndexNode(Db4objects.Db4o.ReaderPair readers)
		{
			_btreeIndex.DefragIndexNode(readers);
		}
	}
}
