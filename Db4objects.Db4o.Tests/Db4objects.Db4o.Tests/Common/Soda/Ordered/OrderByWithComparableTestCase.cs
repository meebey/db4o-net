/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Soda.Ordered;

namespace Db4objects.Db4o.Tests.Common.Soda.Ordered
{
	public class OrderByWithComparableTestCase : AbstractDb4oTestCase
	{
		public class ItemComparable : IComparable
		{
			public int _id;

			public ItemComparable(int id)
			{
				_id = id;
			}

			public virtual int CompareTo(object other)
			{
				OrderByWithComparableTestCase.ItemComparable cmp = (OrderByWithComparableTestCase.ItemComparable
					)other;
				if (_id == cmp._id)
				{
					return 0;
				}
				return _id < cmp._id ? -1 : 1;
			}

			public virtual int Id()
			{
				return _id;
			}
		}

		public class Item
		{
			public int _id;

			public OrderByWithComparableTestCase.ItemComparable _itemCmp;

			public Item(int id, OrderByWithComparableTestCase.ItemComparable itemCmp)
			{
				_id = id;
				_itemCmp = itemCmp;
			}

			public virtual OrderByWithComparableTestCase.ItemComparable ItemCmp()
			{
				return _itemCmp;
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new OrderByWithComparableTestCase.Item(1, new OrderByWithComparableTestCase.ItemComparable
				(1)));
			Store(new OrderByWithComparableTestCase.Item(2, null));
			Store(new OrderByWithComparableTestCase.Item(3, new OrderByWithComparableTestCase.ItemComparable
				(2)));
			Store(new OrderByWithComparableTestCase.Item(4, null));
		}

		public virtual void TestOrderByWithEnums()
		{
			IQuery query = NewQuery();
			query.Constrain(typeof(OrderByWithComparableTestCase.Item));
			query.Descend("_id").Constrain(1).Or(query.Descend("_id").Constrain(3));
			query.Descend("_itemCmp").OrderAscending();
			IObjectSet result = query.Execute();
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(1, ((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp().
				Id());
			Assert.AreEqual(2, ((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp().
				Id());
		}

		public virtual void TestOrderByWithNullValues()
		{
			IQuery query = NewQuery();
			query.Constrain(typeof(OrderByWithComparableTestCase.Item));
			query.Descend("_itemCmp").OrderAscending();
			IObjectSet result = query.Execute();
			Assert.AreEqual(4, result.Count);
			Assert.IsNull(((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp());
			Assert.IsNull(((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp());
			Assert.AreEqual(1, ((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp().
				Id());
			Assert.AreEqual(2, ((OrderByWithComparableTestCase.Item)result.Next()).ItemCmp().
				Id());
		}
	}
}
