/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */

using System.Collections.Generic;
using Db4objects.Db4o.Collections;
using Db4oUnit;


namespace Db4objects.Db4o.Tests.CLI2.Collections.Transparent.List
{
	public class ActivatableListTestCase : AbstractActivatableCollectionApiTestCase
	{
		public void TestAddRange()
		{
			SingleActivatableCollection().AddRange(ToBeAdded());
			Reopen();

			List<ICollectionElement> expected = NewPlainCollection();
			expected.AddRange(ToBeAdded());

			IteratorAssert.AreEqual(expected.GetEnumerator(), SingleCollection().GetEnumerator());
		}

		public void TestSortDefaultComparer()
		{
			IActivatableList<ICollectionElement> actual = SingleActivatableCollection();
			actual.Sort();

			List<ICollectionElement> expected = NewPlainCollection();
			expected.Sort();

			IteratorAssert.AreEqual(expected.GetEnumerator(), actual.GetEnumerator());
		}

		public void TestBinarySearch()
		{
			SingleActivatableCollection().Sort();
			Reopen();

			foreach (string name in Names)
			{
				Assert.IsGreaterOrEqual(0, SingleActivatableCollection().BinarySearch(new ActivatableElement(name)));
			}
		}

		private IActivatableList<ICollectionElement> SingleActivatableCollection()
		{
			return (IActivatableList<ICollectionElement>) SingleCollection();
		}

		//TODO: Add remaining 'helper' methods declared in List<T>

		private List<ICollectionElement> ToBeAdded()
		{
			return NewPlainCollection();
		}
	}
}
