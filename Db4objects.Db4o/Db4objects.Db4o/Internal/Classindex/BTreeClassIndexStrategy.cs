namespace Db4objects.Db4o.Internal.Classindex
{
	/// <exclude></exclude>
	public class BTreeClassIndexStrategy : Db4objects.Db4o.Internal.Classindex.AbstractClassIndexStrategy
	{
		private Db4objects.Db4o.Internal.Btree.BTree _btreeIndex;

		public BTreeClassIndexStrategy(Db4objects.Db4o.Internal.ClassMetadata yapClass) : 
			base(yapClass)
		{
		}

		public virtual Db4objects.Db4o.Internal.Btree.BTree Btree()
		{
			return _btreeIndex;
		}

		public override int EntryCount(Db4objects.Db4o.Internal.Transaction ta)
		{
			return _btreeIndex != null ? _btreeIndex.Size(ta) : 0;
		}

		public override void Initialize(Db4objects.Db4o.Internal.ObjectContainerBase stream
			)
		{
			CreateBTreeIndex(stream, 0);
		}

		public override void Purge()
		{
		}

		public override void Read(Db4objects.Db4o.Internal.ObjectContainerBase stream, int
			 indexID)
		{
			ReadBTreeIndex(stream, indexID);
		}

		public override int Write(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (_btreeIndex == null)
			{
				return 0;
			}
			_btreeIndex.Write(trans);
			return _btreeIndex.GetID();
		}

		public override void TraverseAll(Db4objects.Db4o.Internal.Transaction ta, Db4objects.Db4o.Foundation.IVisitor4
			 command)
		{
			if (_btreeIndex != null)
			{
				_btreeIndex.TraverseKeys(ta, command);
			}
		}

		private void CreateBTreeIndex(Db4objects.Db4o.Internal.ObjectContainerBase stream
			, int btreeID)
		{
			if (stream.IsClient())
			{
				return;
			}
			_btreeIndex = ((Db4objects.Db4o.Internal.LocalObjectContainer)stream).CreateBTreeClassIndex
				(btreeID);
			_btreeIndex.SetRemoveListener(new _AnonymousInnerClass61(this, stream));
		}

		private sealed class _AnonymousInnerClass61 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass61(BTreeClassIndexStrategy _enclosing, Db4objects.Db4o.Internal.ObjectContainerBase
				 stream)
			{
				this._enclosing = _enclosing;
				this.stream = stream;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				Db4objects.Db4o.Internal.ObjectReference yo = stream.GetYapObject(id);
				if (yo != null)
				{
					stream.RemoveReference(yo);
				}
			}

			private readonly BTreeClassIndexStrategy _enclosing;

			private readonly Db4objects.Db4o.Internal.ObjectContainerBase stream;
		}

		private void ReadBTreeIndex(Db4objects.Db4o.Internal.ObjectContainerBase stream, 
			int indexId)
		{
			if (!stream.IsClient() && _btreeIndex == null)
			{
				CreateBTreeIndex(stream, indexId);
			}
		}

		protected override void InternalAdd(Db4objects.Db4o.Internal.Transaction trans, int
			 id)
		{
			_btreeIndex.Add(trans, id);
		}

		protected override void InternalRemove(Db4objects.Db4o.Internal.Transaction ta, int
			 id)
		{
			_btreeIndex.Remove(ta, id);
		}

		public override void DontDelete(Db4objects.Db4o.Internal.Transaction transaction, 
			int id)
		{
		}

		public override void DefragReference(Db4objects.Db4o.Internal.ClassMetadata yapClass
			, Db4objects.Db4o.Internal.ReaderPair readers, int classIndexID)
		{
			int newID = -classIndexID;
			readers.WriteInt(newID);
		}

		public override int Id()
		{
			return _btreeIndex.GetID();
		}

		public override System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return _btreeIndex.AllNodeIds(trans);
		}

		public override void DefragIndex(Db4objects.Db4o.Internal.ReaderPair readers)
		{
			_btreeIndex.DefragIndex(readers);
		}

		public static Db4objects.Db4o.Internal.Btree.BTree Btree(Db4objects.Db4o.Internal.ClassMetadata
			 clazz)
		{
			Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy index = clazz.Index();
			if (!(index is Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy))
			{
				throw new System.InvalidOperationException();
			}
			return ((Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy)index).Btree
				();
		}

		public static System.Collections.IEnumerator Iterate(Db4objects.Db4o.Internal.ClassMetadata
			 clazz, Db4objects.Db4o.Internal.Transaction trans)
		{
			return Btree(clazz).AsRange(trans).Keys();
		}
	}
}
