namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy
{
	/// <summary>UH: Untyped Hierarchy</summary>
	public class STUH1TestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public object h2;

		public object foo1;

		public STUH1TestCase()
		{
		}

		public STUH1TestCase(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2
			 a2)
		{
			h2 = a2;
		}

		public STUH1TestCase(string str)
		{
			foo1 = str;
		}

		public STUH1TestCase(Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2
			 a2, string str)
		{
			h2 = a2;
			foo1 = str;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(), new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				("str1"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2()), new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2("str2")), 
				new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase(new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3
				("str3"))), new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3
				("str3"), "str2")) };
		}

		public virtual void TestStrNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			q.Descend("foo1").Constrain(null);
			Expect(q, new int[] { 0, 2, 3, 4, 5 });
		}

		public virtual void TestBothNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			q.Descend("foo1").Constrain(null);
			q.Descend("h2").Constrain(null);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestDescendantNotNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			q.Descend("h2").Constrain(null).Not();
			Expect(q, new int[] { 2, 3, 4, 5 });
		}

		public virtual void TestDescendantDescendantNotNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			q.Descend("h2").Descend("h3").Constrain(null).Not();
			Expect(q, new int[] { 4, 5 });
		}

		public virtual void TestDescendantExists()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[2]);
			Expect(q, new int[] { 2, 3, 4, 5 });
		}

		public virtual void TestDescendantValue()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[3]);
			Expect(q, new int[] { 3, 5 });
		}

		public virtual void TestDescendantDescendantExists()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3
				())));
			Expect(q, new int[] { 4, 5 });
		}

		public virtual void TestDescendantDescendantValue()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3
				("str3"))));
			Expect(q, new int[] { 4, 5 });
		}

		public virtual void TestDescendantDescendantStringPath()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			q.Descend("h2").Descend("h3").Descend("foo3").Constrain("str3");
			Expect(q, new int[] { 4, 5 });
		}

		public virtual void TestSequentialAddition()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				());
			Db4objects.Db4o.Query.IQuery cur = q.Descend("h2");
			cur.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH2
				());
			cur.Descend("foo2").Constrain("str2");
			cur = cur.Descend("h3");
			cur.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH3
				());
			cur.Descend("foo3").Constrain("str3");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[5]);
		}

		public virtual void TestTwoLevelOr()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				("str1"));
			q.Descend("foo1").Constraints().Or(q.Descend("h2").Descend("h3").Descend("foo3").
				Constrain("str3"));
			Expect(q, new int[] { 1, 4, 5 });
		}

		public virtual void TestThreeLevelOr()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase
				("str1"));
			q.Descend("foo1").Constraints().Or(q.Descend("h2").Descend("foo2").Constrain("str2"
				)).Or(q.Descend("h2").Descend("h3").Descend("foo3").Constrain("str3"));
			Expect(q, new int[] { 1, 3, 4, 5 });
		}

		public virtual void TestNonExistentDescendant()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase constraint
				 = new Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy.STUH1TestCase(
				);
			constraint.foo1 = new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STETH2
				();
			q.Constrain(constraint);
			Expect(q, new int[] {  });
		}
	}
}
