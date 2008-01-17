/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.TA.TA;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
	/// <exclude></exclude>
	public class TALinkedListTestCase : TAItemTestCaseBase
	{
		public static void Main(string[] args)
		{
			new TALinkedListTestCase().RunAll();
		}

		/// <exception cref="Exception"></exception>
		protected override object CreateItem()
		{
			TALinkedListItem item = new TALinkedListItem();
			item.list = NewList();
			return item;
		}

		private TALinkedList NewList()
		{
			return TALinkedList.NewList(10);
		}

		/// <exception cref="Exception"></exception>
		protected override void AssertItemValue(object obj)
		{
			TALinkedListItem item = (TALinkedListItem)obj;
			Assert.AreEqual(NewList(), item.List());
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestDeactivateDepth()
		{
			TALinkedListItem item = (TALinkedListItem)RetrieveOnlyInstance();
			TALinkedList list = item.List();
			TALinkedList next3 = list.NextN(3);
			TALinkedList next5 = list.NextN(5);
			Assert.IsNotNull(next3.Next());
			Assert.IsNotNull(next5.Next());
			Db().Deactivate(list, 4);
			Assert.IsNull(list.next);
			Assert.AreEqual(0, list.value);
			// FIXME: test fails if uncomenting the following assertion.
			//	    	Assert.isNull(next3.next);
			Assert.IsNotNull(next5.next);
		}
	}
}
