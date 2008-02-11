/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Btree;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeAssert
	{
		public static ExpectingVisitor CreateExpectingVisitor(int value, int count)
		{
			int[] values = new int[count];
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = value;
			}
			return new ExpectingVisitor(IntArrays4.ToObjectArray(values));
		}

		public static ExpectingVisitor CreateExpectingVisitor(int[] keys)
		{
			return new ExpectingVisitor(IntArrays4.ToObjectArray(keys));
		}

		private static ExpectingVisitor CreateSortedExpectingVisitor(int[] keys)
		{
			return new ExpectingVisitor(IntArrays4.ToObjectArray(keys), true, false);
		}

		public static void TraverseKeys(IBTreeRange result, IVisitor4 visitor)
		{
			IEnumerator i = result.Keys();
			while (i.MoveNext())
			{
				visitor.Visit(i.Current);
			}
		}

		public static void AssertKeys(Transaction transaction, BTree btree, int[] keys)
		{
			ExpectingVisitor visitor = CreateExpectingVisitor(keys);
			btree.TraverseKeys(transaction, visitor);
			visitor.AssertExpectations();
		}

		public static void AssertEmpty(Transaction transaction, BTree tree)
		{
			ExpectingVisitor visitor = new ExpectingVisitor(new object[0]);
			tree.TraverseKeys(transaction, visitor);
			visitor.AssertExpectations();
			Assert.AreEqual(0, tree.Size(transaction));
		}

		public static void DumpKeys(Transaction trans, BTree tree)
		{
			tree.TraverseKeys(trans, new _IVisitor4_51());
		}

		private sealed class _IVisitor4_51 : IVisitor4
		{
			public _IVisitor4_51()
			{
			}

			public void Visit(object obj)
			{
				Sharpen.Runtime.Out.WriteLine(obj);
			}
		}

		public static ExpectingVisitor CreateExpectingVisitor(int expectedID)
		{
			return CreateExpectingVisitor(expectedID, 1);
		}

		public static int FillSize(BTree btree)
		{
			return btree.NodeSize() + 1;
		}

		public static int[] NewBTreeNodeSizedArray(BTree btree, int value)
		{
			return IntArrays4.Fill(new int[FillSize(btree)], value);
		}

		public static void AssertRange(int[] expectedKeys, IBTreeRange range)
		{
			Assert.IsNotNull(range);
			ExpectingVisitor visitor = CreateSortedExpectingVisitor(expectedKeys);
			TraverseKeys(range, visitor);
			visitor.AssertExpectations();
		}

		public static BTree CreateIntKeyBTree(ObjectContainerBase stream, int id, int nodeSize
			)
		{
			return new BTree(stream.SystemTransaction(), id, new IntHandler(stream), nodeSize
				, stream.ConfigImpl().BTreeCacheHeight());
		}

		public static BTree CreateIntKeyBTree(ObjectContainerBase stream, int id, int treeCacheHeight
			, int nodeSize)
		{
			return new BTree(stream.SystemTransaction(), id, new IntHandler(stream), nodeSize
				, treeCacheHeight);
		}

		public static void AssertSingleElement(Transaction trans, BTree btree, object element
			)
		{
			Assert.AreEqual(1, btree.Size(trans));
			IBTreeRange result = btree.Search(trans, element);
			ExpectingVisitor expectingVisitor = new ExpectingVisitor(new object[] { element }
				);
			BTreeAssert.TraverseKeys(result, expectingVisitor);
			expectingVisitor.AssertExpectations();
			expectingVisitor = new ExpectingVisitor(new object[] { element });
			btree.TraverseKeys(trans, expectingVisitor);
			expectingVisitor.AssertExpectations();
		}
	}
}
