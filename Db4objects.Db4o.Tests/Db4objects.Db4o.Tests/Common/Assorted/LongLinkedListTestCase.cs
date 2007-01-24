namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class LongLinkedListTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private const int COUNT = 1000;

		public class LinkedList
		{
			public Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList _next;

			public int _depth;
		}

		private static Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList
			 NewLongCircularList()
		{
			Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList head = new 
				Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList();
			Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList tail = head;
			for (int i = 1; i < COUNT; i++)
			{
				tail._next = new Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList
					();
				tail = tail._next;
				tail._depth = i;
			}
			tail._next = head;
			return head;
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase().RunSolo();
		}

		protected override void Store()
		{
			Store(NewLongCircularList());
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList)
				);
			q.Descend("_depth").Constrain(0);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4oUnit.Assert.AreEqual(1, objectSet.Size());
			Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList head = (Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList
				)objectSet.Next();
			Db().Activate(head, int.MaxValue);
			AssertListIsComplete(head);
			Db().Deactivate(head, int.MaxValue);
			Db().Activate(head, int.MaxValue);
			AssertListIsComplete(head);
			Db().Deactivate(head, int.MaxValue);
			Db().Refresh(head, int.MaxValue);
			AssertListIsComplete(head);
		}

		private void AssertListIsComplete(Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList
			 head)
		{
			int count = 1;
			Db4objects.Db4o.Tests.Common.Assorted.LongLinkedListTestCase.LinkedList tail = head
				._next;
			while (tail != head)
			{
				count++;
				tail = tail._next;
			}
			Db4oUnit.Assert.AreEqual(COUNT, count);
		}
	}
}
