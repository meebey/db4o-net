/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ComplexListTestCase : DrsTestCase
	{
		public virtual void Test()
		{
			//TODO: Fix the following exception and remove the "if" line
			if (A().GetType().FullName.IndexOf("HsqlMemoryFixture") >= 0 || B().GetType().FullName
				.IndexOf("HsqlMemoryFixture") >= 0)
			{
				return;
			}
			Store(A(), CreateList("foo.list"));
			ReplicateAndTest(A(), B());
			RoundTripTest();
		}

		private void RoundTripTest()
		{
			ChangeInProviderB();
			ReplicateAndTest(B(), A());
		}

		private void ChangeInProviderB()
		{
			ListHolder listHolder = (ListHolder)GetOneInstance(B(), typeof(ListHolder));
			Item fooBaby = new Item("foobaby", listHolder);
			B().Provider().StoreNew(fooBaby);
			listHolder.Add(fooBaby);
			Item foo = GetItem(listHolder, "foo");
			foo.SetChild(fooBaby);
			B().Provider().Update(foo);
			B().Provider().Update(listHolder);
		}

		private void ReplicateAndTest(IDrsFixture source, IDrsFixture target)
		{
			ReplicateAll(source.Provider(), target.Provider());
			EnsureContents(target, (ListHolder)GetOneInstance(source, typeof(ListHolder)));
		}

		private void Store(IDrsFixture fixture, ListHolder list)
		{
			ITestableReplicationProviderInside provider = fixture.Provider();
			provider.StoreNew(list);
			provider.StoreNew(GetItem(list, "foo"));
			provider.StoreNew(GetItem(list, "foobar"));
			provider.Commit();
			EnsureContents(fixture, list);
		}

		private void EnsureContents(IDrsFixture actualFixture, ListHolder expected)
		{
			ListHolder actual = (ListHolder)GetOneInstance(actualFixture, typeof(ListHolder));
			Assert.AreEqual(expected.GetName(), actual.GetName());
			IList expectedList = expected.GetList();
			IList actualList = actual.GetList();
			AssertListWithCycles(expectedList, actualList);
		}

		private void AssertListWithCycles(IList expectedList, IList actualList)
		{
			Assert.AreEqual(expectedList.Count, actualList.Count);
			for (int i = 0; i < expectedList.Count; ++i)
			{
				Item expected = (Item)expectedList[i];
				Item actual = (Item)actualList[i];
				AssertItem(expected, actual);
			}
			AssertCycle(actualList, "foo", "bar", 1);
			AssertCycle(actualList, "foo", "foobar", 1);
			AssertCycle(actualList, "foo", "baz", 2);
		}

		private void AssertCycle(IList list, string childName, string parentName, int level
			)
		{
			Item foo = GetItem(list, childName);
			Item bar = GetItem(list, parentName);
			Assert.IsNotNull(foo);
			Assert.IsNotNull(bar);
			Assert.AreSame(foo, bar.Child(level));
			Assert.AreSame(foo.Parent(), bar.Parent());
		}

		private void AssertItem(Item expected, Item actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.AreEqual(expected.GetName(), actual.GetName());
			AssertItem(expected.Child(), actual.Child());
		}

		private Item GetItem(ListHolder holder, string tbf)
		{
			return GetItem(holder.GetList(), tbf);
		}

		private Item GetItem(IList list, string tbf)
		{
			int itemIndex = list.IndexOf(new Item(tbf));
			return (Item)(itemIndex >= 0 ? list[itemIndex] : null);
		}

		public virtual ListHolder CreateList(string name)
		{
			// list : {foo, bar, baz, foobar}
			//
			// baz -----+
			//          |
			//         bar --> foo
			//                  ^
			//                  |
			// foobar ----------+
			ListHolder listHolder = NewList(name);
			Item foo = new Item("foo", listHolder);
			Item bar = new Item("bar", foo, listHolder);
			listHolder.Add(foo);
			listHolder.Add(bar);
			listHolder.Add(new Item("baz", bar, listHolder));
			listHolder.Add(new Item("foobar", foo, listHolder));
			return listHolder;
		}

		private ListHolder NewList(string name)
		{
			ListHolder holder = new ListHolder(name);
			holder.SetList(new ArrayList());
			return holder;
		}
	}
}
