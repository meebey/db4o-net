/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Tests.Common.Btree;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeSearchTestCase : AbstractDb4oTestCase, IOptOutDefragSolo, IOptOutCS
	{
		protected const int BtreeNodeSize = 4;

		public static void Main(string[] arguments)
		{
			new BTreeSearchTestCase().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			CycleIntKeys(new int[] { 3, 5, 7, 10, 11, 12, 14, 15, 17, 20, 21, 25 });
		}

		/// <exception cref="Exception"></exception>
		private void CycleIntKeys(int[] values)
		{
			BTree btree = BTreeAssert.CreateIntKeyBTree(Stream(), 0, BtreeNodeSize);
			for (int i = 0; i < 5; i++)
			{
				btree = CycleIntKeys(btree, values);
			}
		}

		/// <exception cref="Exception"></exception>
		private BTree CycleIntKeys(BTree btree, int[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				btree.Add(Trans(), values[i]);
			}
			ExpectKeysSearch(Trans(), btree, values);
			btree.Commit(Trans());
			int id = btree.GetID();
			Stream().Commit(Trans());
			Reopen();
			btree = BTreeAssert.CreateIntKeyBTree(Stream(), id, BtreeNodeSize);
			ExpectKeysSearch(Trans(), btree, values);
			for (int i = 0; i < values.Length; i++)
			{
				btree.Remove(Trans(), values[i]);
			}
			BTreeAssert.AssertEmpty(Trans(), btree);
			btree.Commit(Trans());
			BTreeAssert.AssertEmpty(Trans(), btree);
			return btree;
		}

		private void ExpectKeysSearch(Transaction trans, BTree btree, int[] keys)
		{
			int lastValue = int.MinValue;
			for (int i = 0; i < keys.Length; i++)
			{
				if (keys[i] != lastValue)
				{
					ExpectingVisitor expectingVisitor = BTreeAssert.CreateExpectingVisitor(keys[i], IntArrays4
						.Occurences(keys, keys[i]));
					IBTreeRange range = btree.Search(trans, keys[i]);
					BTreeAssert.TraverseKeys(range, expectingVisitor);
					expectingVisitor.AssertExpectations();
					lastValue = keys[i];
				}
			}
		}
	}
}
