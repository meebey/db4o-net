/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Collections.Generic;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Collections;
using Db4objects.Db4o.Tests.Jre12.Collections;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Tests.Jre12.Collections
{
	/// <decaf.ignore.jdk11></decaf.ignore.jdk11>
	public class BigSetTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		private static readonly BigSetTestCase.Item ItemOne = new BigSetTestCase.Item("one"
			);

		private static readonly BigSetTestCase.Item[] items = new BigSetTestCase.Item[] { 
			new BigSetTestCase.Item("one"), new BigSetTestCase.Item("two"), new BigSetTestCase.Item
			("three") };

		public class Holder<E>
		{
			public ISet<E> _set;
		}

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is BigSetTestCase.Item))
				{
					return false;
				}
				BigSetTestCase.Item other = (BigSetTestCase.Item)obj;
				if (_name == null)
				{
					return other._name == null;
				}
				return _name.Equals(other._name);
			}
		}

		public virtual void TestTypeHandlerInstalled()
		{
			ITypeHandler4 typeHandler = Container().Handlers().ConfiguredTypeHandler(Reflector
				().ForClass(NewBigSet().GetType()));
			Assert.IsInstanceOf(typeof(BigSetTypeHandler), typeHandler);
		}

		public virtual void TestEmptySet()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			Assert.AreEqual(0, set.Count);
		}

		public virtual void TestSize()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			set.Add(ItemOne);
			Assert.AreEqual(1, set.Count);
			set.Remove(ItemOne);
			Assert.AreEqual(0, set.Count);
			BigSetTestCase.Item itemTwo = new BigSetTestCase.Item("two");
			set.Add(itemTwo);
			set.Add(new BigSetTestCase.Item("three"));
			Assert.AreEqual(2, set.Count);
			set.Remove(itemTwo);
			Assert.AreEqual(1, set.Count);
		}

		public virtual void TestContains()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			set.Add(ItemOne);
			Assert.IsTrue(set.Contains(ItemOne));
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestPersistence()
		{
			BigSetTestCase.Holder<BigSetTestCase.Item> holder = new BigSetTestCase.Holder<BigSetTestCase.Item
				>();
			holder._set = NewBigSet();
			ISet<BigSetTestCase.Item> set = holder._set;
			set.Add(ItemOne);
			Store(holder);
			Reopen();
			holder = (BigSetTestCase.Holder<BigSetTestCase.Item>)RetrieveOnlyInstance(holder.
				GetType());
			set = holder._set;
			AssertSinglePersistentItem(set);
		}

		private void AssertSinglePersistentItem(ISet<BigSetTestCase.Item> set)
		{
			BigSetTestCase.Item expectedItem = (BigSetTestCase.Item)RetrieveOnlyInstance(typeof(
				BigSetTestCase.Item));
			Assert.IsNotNull(set);
			Assert.AreEqual(1, set.Count);
			IEnumerator setIterator = set.GetEnumerator();
			Assert.IsNotNull(setIterator);
			Assert.IsTrue(setIterator.MoveNext());
			BigSetTestCase.Item actualItem = (BigSetTestCase.Item)setIterator.Current;
			Assert.AreSame(expectedItem, actualItem);
		}

		public virtual void TestAddAllContainsAll()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			IList<BigSetTestCase.Item> collection = ItemList();
			Assert.IsTrue(set.AddAll(collection));
			Assert.IsTrue(set.ContainsAll(collection));
			Assert.IsFalse(set.AddAll(collection));
			Assert.AreEqual(collection.Count, set.Count);
		}

		public virtual void TestRemove()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			IList<BigSetTestCase.Item> collection = ItemList();
			set.AddAll(collection);
			BigSetTestCase.Item first = collection[0];
			set.Remove(first);
			Assert.IsTrue(collection.Remove(first));
			Assert.IsFalse(collection.Remove(first));
			Assert.IsTrue(set.ContainsAll(collection));
			Assert.IsFalse(set.Contains(first));
		}

		public virtual void TestRemoveAll()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			IList<BigSetTestCase.Item> collection = ItemList();
			set.AddAll(collection);
			Assert.IsTrue(set.RemoveAll(collection));
			Assert.AreEqual(0, set.Count);
			Assert.IsFalse(set.RemoveAll(collection));
		}

		public virtual void TestIsEmpty()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			Assert.IsTrue(set.IsEmpty);
			set.Add(ItemOne);
			Assert.IsFalse(set.IsEmpty);
			set.Remove(ItemOne);
			Assert.IsTrue(set.IsEmpty);
		}

		public virtual void TestIterator()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			ICollection<BigSetTestCase.Item> collection = ItemList();
			set.AddAll(collection);
			IEnumerator i = set.GetEnumerator();
			Assert.IsNotNull(i);
			IteratorAssert.SameContent(collection.GetEnumerator(), i);
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestDelete()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			set.Add(ItemOne);
			Db().Store(set);
			Db().Commit();
			BTree bTree = BTree(set);
			BTreeAssert.AssertAllSlotsFreed(FileTransaction(), bTree, new _ICodeBlock_174(this
				, set));
			Assert.Expect(typeof(InvalidOperationException), new _ICodeBlock_180(set));
		}

		private sealed class _ICodeBlock_174 : ICodeBlock
		{
			public _ICodeBlock_174(BigSetTestCase _enclosing, ISet<BigSetTestCase.Item> set)
			{
				this._enclosing = _enclosing;
				this.set = set;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Delete(set);
				this._enclosing.Db().Commit();
			}

			private readonly BigSetTestCase _enclosing;

			private readonly ISet<BigSetTestCase.Item> set;
		}

		private sealed class _ICodeBlock_180 : ICodeBlock
		{
			public _ICodeBlock_180(ISet<BigSetTestCase.Item> set)
			{
				this.set = set;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				set.Add(BigSetTestCase.ItemOne);
			}

			private readonly ISet<BigSetTestCase.Item> set;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestDefragment()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			set.Add(ItemOne);
			Db().Store(set);
			Db().Commit();
			Defragment();
			set = (ISet<BigSetTestCase.Item>)RetrieveOnlyInstance(set.GetType());
			AssertSinglePersistentItem(set);
		}

		public virtual void TestClear()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			set.Add(ItemOne);
			set.Clear();
			Assert.AreEqual(0, set.Count);
		}

		private IList<BigSetTestCase.Item> ItemList()
		{
			IList<BigSetTestCase.Item> c = new List<BigSetTestCase.Item>();
			for (int i = 0; i < items.Length; i++)
			{
				c.Add(items[i]);
			}
			return c;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestGetInternalImplementation()
		{
			ISet<BigSetTestCase.Item> set = NewBigSet();
			BTree bTree = BTree(set);
			Assert.IsNotNull(bTree);
		}

		private ISet<BigSetTestCase.Item> NewBigSet()
		{
			return CollectionFactory.ForObjectContainer(Db()).NewBigSet<BigSetTestCase.Item>(
				);
		}

		/// <exception cref="MemberAccessException"></exception>
		public static BTree BTree(ISet<BigSetTestCase.Item> set)
		{
			return (BTree)Reflection4.GetFieldValue(set, "_bTree");
		}

		private LocalTransaction FileTransaction()
		{
			return ((LocalTransaction)Trans());
		}
	}
}
