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
			Store(A(), CreateList());
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
			SimpleListHolder SimpleListHolder = (SimpleListHolder)GetOneInstance(B(), typeof(
				SimpleListHolder));
			SimpleItem fooBaby = new SimpleItem(SimpleListHolder, "foobaby");
			B().Provider().StoreNew(fooBaby);
			SimpleListHolder.Add(fooBaby);
			SimpleItem foo = GetItem(SimpleListHolder, "foo");
			foo.SetChild(fooBaby);
			B().Provider().Update(foo);
			B().Provider().Update(SimpleListHolder);
		}

		private void ReplicateAndTest(IDrsFixture source, IDrsFixture target)
		{
			ReplicateAll(source.Provider(), target.Provider());
			EnsureContents(target, (SimpleListHolder)GetOneInstance(source, typeof(SimpleListHolder
				)));
		}

		private void Store(IDrsFixture fixture, SimpleListHolder list)
		{
			ITestableReplicationProviderInside provider = fixture.Provider();
			provider.StoreNew(list);
			provider.StoreNew(GetItem(list, "foo"));
			provider.StoreNew(GetItem(list, "foobar"));
			provider.Commit();
			EnsureContents(fixture, list);
		}

		private void EnsureContents(IDrsFixture actualFixture, SimpleListHolder expected)
		{
			SimpleListHolder actual = (SimpleListHolder)GetOneInstance(actualFixture, typeof(
				SimpleListHolder));
			IList expectedList = expected.GetList();
			IList actualList = actual.GetList();
			AssertListWithCycles(expectedList, actualList);
		}

		private void AssertListWithCycles(IList expectedList, IList actualList)
		{
			Assert.AreEqual(expectedList.Count, actualList.Count);
			for (int i = 0; i < expectedList.Count; ++i)
			{
				SimpleItem expected = (SimpleItem)expectedList[i];
				SimpleItem actual = (SimpleItem)actualList[i];
				AssertItem(expected, actual);
			}
			AssertCycle(actualList, "foo", "bar", 1);
			AssertCycle(actualList, "foo", "foobar", 1);
			AssertCycle(actualList, "foo", "baz", 2);
		}

		private void AssertCycle(IList list, string childName, string parentName, int level
			)
		{
			SimpleItem foo = GetItem(list, childName);
			SimpleItem bar = GetItem(list, parentName);
			Assert.IsNotNull(foo);
			Assert.IsNotNull(bar);
			Assert.AreSame(foo, bar.GetChild(level));
			Assert.AreSame(foo.GetParent(), bar.GetParent());
		}

		private void AssertItem(SimpleItem expected, SimpleItem actual)
		{
			if (expected == null)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.AreEqual(expected.GetValue(), actual.GetValue());
			AssertItem(expected.GetChild(), actual.GetChild());
		}

		private SimpleItem GetItem(SimpleListHolder holder, string tbf)
		{
			return GetItem(holder.GetList(), tbf);
		}

		private SimpleItem GetItem(IList list, string tbf)
		{
			int itemIndex = list.IndexOf(new SimpleItem(tbf));
			return (SimpleItem)(itemIndex >= 0 ? list[itemIndex] : null);
		}

		public virtual SimpleListHolder CreateList()
		{
			// list : {foo, bar, baz, foobar}
			//
			// baz -----+
			//          |
			//         bar --> foo
			//                  ^
			//                  |
			// foobar ----------+
			SimpleListHolder listHolder = new SimpleListHolder();
			SimpleItem foo = new SimpleItem(listHolder, "foo");
			SimpleItem bar = new SimpleItem(listHolder, "bar", foo);
			listHolder.Add(foo);
			listHolder.Add(bar);
			listHolder.Add(new SimpleItem(listHolder, "baz", bar));
			listHolder.Add(new SimpleItem(listHolder, "foobar", foo));
			return listHolder;
		}
	}
}
