namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class BTreeNodeSearchResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

		private readonly Db4objects.Db4o.Inside.Btree.BTree _btree;

		private readonly Db4objects.Db4o.Inside.Btree.BTreePointer _pointer;

		private readonly bool _foundMatch;

		internal BTreeNodeSearchResult(Db4objects.Db4o.Transaction transaction, Db4objects.Db4o.Inside.Btree.BTree
			 btree, Db4objects.Db4o.Inside.Btree.BTreePointer pointer, bool foundMatch)
		{
			if (null == transaction || null == btree)
			{
				throw new System.ArgumentNullException();
			}
			_transaction = transaction;
			_btree = btree;
			_pointer = pointer;
			_foundMatch = foundMatch;
		}

		internal BTreeNodeSearchResult(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 nodeReader, Db4objects.Db4o.Inside.Btree.BTree btree, Db4objects.Db4o.Inside.Btree.BTreeNode
			 node, int cursor, bool foundMatch) : this(trans, btree, PointerOrNull(trans, nodeReader
			, node, cursor), foundMatch)
		{
		}

		internal BTreeNodeSearchResult(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 nodeReader, Db4objects.Db4o.Inside.Btree.BTree btree, Db4objects.Db4o.Inside.Btree.Searcher
			 searcher, Db4objects.Db4o.Inside.Btree.BTreeNode node) : this(trans, btree, NextPointerIf
			(PointerOrNull(trans, nodeReader, node, searcher.Cursor()), searcher.IsGreater()
			), searcher.FoundMatch())
		{
		}

		private static Db4objects.Db4o.Inside.Btree.BTreePointer NextPointerIf(Db4objects.Db4o.Inside.Btree.BTreePointer
			 pointer, bool condition)
		{
			if (null == pointer)
			{
				return null;
			}
			if (condition)
			{
				return pointer.Next();
			}
			return pointer;
		}

		private static Db4objects.Db4o.Inside.Btree.BTreePointer PointerOrNull(Db4objects.Db4o.Transaction
			 trans, Db4objects.Db4o.YapReader nodeReader, Db4objects.Db4o.Inside.Btree.BTreeNode
			 node, int cursor)
		{
			return node == null ? null : new Db4objects.Db4o.Inside.Btree.BTreePointer(trans, 
				nodeReader, node, cursor);
		}

		public virtual Db4objects.Db4o.Inside.Btree.IBTreeRange CreateIncludingRange(Db4objects.Db4o.Inside.Btree.BTreeNodeSearchResult
			 end)
		{
			Db4objects.Db4o.Inside.Btree.BTreePointer firstPointer = FirstValidPointer();
			Db4objects.Db4o.Inside.Btree.BTreePointer endPointer = end._foundMatch ? end._pointer
				.Next() : end.FirstValidPointer();
			return new Db4objects.Db4o.Inside.Btree.BTreeRangeSingle(_transaction, _btree, firstPointer
				, endPointer);
		}

		private Db4objects.Db4o.Inside.Btree.BTreePointer FirstValidPointer()
		{
			if (null == _pointer)
			{
				return null;
			}
			if (_pointer.IsValid())
			{
				return _pointer;
			}
			return _pointer.Next();
		}
	}
}
