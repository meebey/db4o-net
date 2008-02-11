/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeNodeTestCase : BTreeTestCaseBase
	{
		public static void Main(string[] args)
		{
			new BTreeNodeTestCase().RunSolo();
		}

		private readonly int[] keys = new int[] { -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 7, 9
			 };

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			base.Db4oSetupAfterStore();
			Add(keys);
			Commit();
		}

		public virtual void TestLastKeyIndex()
		{
			BTreeNode node = Node(3);
			Assert.AreEqual(1, node.LastKeyIndex(Trans()));
			Transaction trans = NewTransaction();
			_btree.Add(trans, 5);
			Assert.AreEqual(1, node.LastKeyIndex(Trans()));
			_btree.Commit(trans);
			Assert.AreEqual(2, node.LastKeyIndex(Trans()));
		}

		private BTreeNode Node(int value)
		{
			IBTreeRange range = Search(value);
			IEnumerator i = range.Pointers();
			i.MoveNext();
			BTreePointer firstPointer = (BTreePointer)i.Current;
			BTreeNode node = firstPointer.Node();
			node.DebugLoadFully(SystemTrans());
			return node;
		}

		public virtual void TestLastPointer()
		{
			BTreeNode node = Node(3);
			BTreePointer lastPointer = node.LastPointer(Trans());
			AssertPointerKey(4, lastPointer);
		}
	}
}
