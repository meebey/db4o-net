/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;

namespace Db4objects.Db4o.Internal.Btree
{
	/// <exclude></exclude>
	public class BTreeRemove : BTreeUpdate
	{
		public BTreeRemove(Transaction transaction, object obj) : base(transaction, obj)
		{
		}

		protected override void Committed(BTree btree)
		{
			btree.NotifyRemoveListener(GetObject());
		}

		public override string ToString()
		{
			return "(-) " + base.ToString();
		}

		public override bool IsRemove()
		{
			return true;
		}

		protected override object GetCommittedObject()
		{
			return No4.Instance;
		}

		protected override void AdjustSizeOnRemovalByOtherTransaction(BTree btree)
		{
			// The size was reduced for this entry, let's change back.
			btree.SizeChanged(_transaction, +1);
		}
	}
}
