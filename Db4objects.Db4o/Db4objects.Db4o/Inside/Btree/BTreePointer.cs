namespace Db4objects.Db4o.Inside.Btree
{
	/// <exclude></exclude>
	public class BTreePointer
	{
		public static Db4objects.Db4o.Inside.Btree.BTreePointer Max(Db4objects.Db4o.Inside.Btree.BTreePointer
			 x, Db4objects.Db4o.Inside.Btree.BTreePointer y)
		{
			if (x == null)
			{
				return x;
			}
			if (y == null)
			{
				return y;
			}
			if (x.CompareTo(y) > 0)
			{
				return x;
			}
			return y;
		}

		public static Db4objects.Db4o.Inside.Btree.BTreePointer Min(Db4objects.Db4o.Inside.Btree.BTreePointer
			 x, Db4objects.Db4o.Inside.Btree.BTreePointer y)
		{
			if (x == null)
			{
				return y;
			}
			if (y == null)
			{
				return x;
			}
			if (x.CompareTo(y) < 0)
			{
				return x;
			}
			return y;
		}

		private readonly Db4objects.Db4o.Inside.Btree.BTreeNode _node;

		private readonly int _index;

		private readonly Db4objects.Db4o.Transaction _transaction;

		private readonly Db4objects.Db4o.YapReader _nodeReader;

		public BTreePointer(Db4objects.Db4o.Transaction transaction, Db4objects.Db4o.YapReader
			 nodeReader, Db4objects.Db4o.Inside.Btree.BTreeNode node, int index)
		{
			if (transaction == null || node == null)
			{
				throw new System.ArgumentNullException();
			}
			_transaction = transaction;
			_nodeReader = nodeReader;
			_node = node;
			_index = index;
		}

		public Db4objects.Db4o.Transaction Transaction()
		{
			return _transaction;
		}

		public int Index()
		{
			return _index;
		}

		public Db4objects.Db4o.Inside.Btree.BTreeNode Node()
		{
			return _node;
		}

		public object Key()
		{
			return Node().Key(Transaction(), NodeReader(), Index());
		}

		public object Value()
		{
			return Node().Value(NodeReader(), Index());
		}

		private Db4objects.Db4o.YapReader NodeReader()
		{
			return _nodeReader;
		}

		public virtual Db4objects.Db4o.Inside.Btree.BTreePointer Next()
		{
			int indexInMyNode = Index() + 1;
			while (indexInMyNode < Node().Count())
			{
				if (Node().IndexIsValid(Transaction(), indexInMyNode))
				{
					return new Db4objects.Db4o.Inside.Btree.BTreePointer(Transaction(), NodeReader(), 
						Node(), indexInMyNode);
				}
				indexInMyNode++;
			}
			int newIndex = -1;
			Db4objects.Db4o.Inside.Btree.BTreeNode nextNode = Node();
			Db4objects.Db4o.YapReader nextReader = null;
			while (newIndex == -1)
			{
				nextNode = nextNode.NextNode();
				if (nextNode == null)
				{
					return null;
				}
				nextReader = nextNode.PrepareRead(Transaction());
				newIndex = nextNode.FirstKeyIndex(Transaction());
			}
			return new Db4objects.Db4o.Inside.Btree.BTreePointer(Transaction(), nextReader, nextNode
				, newIndex);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is Db4objects.Db4o.Inside.Btree.BTreePointer))
			{
				return false;
			}
			Db4objects.Db4o.Inside.Btree.BTreePointer other = (Db4objects.Db4o.Inside.Btree.BTreePointer
				)obj;
			if (Index() != other.Index())
			{
				return false;
			}
			return Node().Equals(other.Node());
		}

		public override string ToString()
		{
			return "BTreePointer(index=" + Index() + ", node=" + Node() + ")";
		}

		public virtual int CompareTo(Db4objects.Db4o.Inside.Btree.BTreePointer y)
		{
			if (null == y)
			{
				throw new System.ArgumentNullException();
			}
			if (Btree() != y.Btree())
			{
				throw new System.ArgumentException();
			}
			return Btree().CompareKeys(Key(), y.Key());
		}

		private Db4objects.Db4o.Inside.Btree.BTree Btree()
		{
			return Node().Btree();
		}

		public static bool LessThan(Db4objects.Db4o.Inside.Btree.BTreePointer x, Db4objects.Db4o.Inside.Btree.BTreePointer
			 y)
		{
			return Db4objects.Db4o.Inside.Btree.BTreePointer.Min(x, y) == x && !Equals(x, y);
		}

		public static bool Equals(Db4objects.Db4o.Inside.Btree.BTreePointer x, Db4objects.Db4o.Inside.Btree.BTreePointer
			 y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		public virtual bool IsValid()
		{
			return Node().IndexIsValid(Transaction(), Index());
		}
	}
}
