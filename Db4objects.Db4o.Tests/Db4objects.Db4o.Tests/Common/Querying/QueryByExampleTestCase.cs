/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class QueryByExampleTestCase : AbstractDb4oTestCase
	{
		internal const int COUNT = 100;

		internal static QueryByExampleTestCase.LinkedList list = QueryByExampleTestCase.LinkedList
			.NewLongCircularList();

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		public static void Main(string[] args)
		{
			new QueryByExampleTestCase().RunSolo();
		}

		protected override void Store()
		{
			Store(list);
		}

		public virtual void TestDefaultQueryModeIsIdentity()
		{
			QueryByExampleTestCase.Item itemOne = new QueryByExampleTestCase.Item("one");
			QueryByExampleTestCase.Item itemTwo = new QueryByExampleTestCase.Item("two");
			Store(itemOne);
			Store(itemTwo);
		}

		public virtual void TestByExample()
		{
			IQuery q = Db().Query();
			q.Constrain(list).ByExample();
			IObjectSet result = q.Execute();
			Assert.AreEqual(COUNT, result.Size());
		}

		public virtual void TestByIdentity()
		{
			IQuery q = Db().Query();
			q.Constrain(typeof(QueryByExampleTestCase.LinkedList));
			IObjectSet result = q.Execute();
			Assert.AreEqual(COUNT, result.Size());
			while (result.HasNext())
			{
				Db().Delete(result.Next());
			}
			q = Db().Query();
			q.Constrain(typeof(QueryByExampleTestCase.LinkedList));
			result = q.Execute();
			Assert.AreEqual(0, result.Size());
			QueryByExampleTestCase.LinkedList newList = QueryByExampleTestCase.LinkedList.NewLongCircularList
				();
			Db().Set(newList);
			q = Db().Query();
			q.Constrain(newList);
			result = q.Execute();
			Assert.AreEqual(1, result.Size());
		}

		public virtual void TestClassConstraint()
		{
			IQuery q = Db().Query();
			q.Constrain(typeof(QueryByExampleTestCase.LinkedList));
			IObjectSet result = q.Execute();
			Assert.AreEqual(COUNT, result.Size());
			q = Db().Query();
			q.Constrain(typeof(QueryByExampleTestCase.LinkedList)).ByExample();
			result = q.Execute();
			Assert.AreEqual(100, result.Size());
		}

		public class LinkedList
		{
			public QueryByExampleTestCase.LinkedList _next;

			[System.NonSerialized]
			public int _depth;

			public static QueryByExampleTestCase.LinkedList NewLongCircularList()
			{
				QueryByExampleTestCase.LinkedList head = new QueryByExampleTestCase.LinkedList();
				QueryByExampleTestCase.LinkedList tail = head;
				for (int i = 1; i < COUNT; i++)
				{
					tail._next = new QueryByExampleTestCase.LinkedList();
					tail = tail._next;
					tail._depth = i;
				}
				tail._next = head;
				return head;
			}

			public override string ToString()
			{
				return "List[" + _depth + "]";
			}
		}
	}
}
