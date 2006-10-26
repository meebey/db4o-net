namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy
{
	/// <summary>SDFT: Same descendant field typed</summary>
	public class STSDFT1TestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public STSDFT1TestCase()
		{
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase
				(), new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT2(), new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT2("str1"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT2
				("str2"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT3()
				, new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT3("str1"), 
				new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT3("str3") };
		}

		public virtual void TestStrNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase
				());
			q.Descend("foo").Constrain(null);
			Expect(q, new int[] { 0, 1, 4 });
		}

		public virtual void TestStrVal()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase)
				);
			q.Descend("foo").Constrain("str1");
			Expect(q, new int[] { 2, 5 });
		}

		public virtual void TestOrValue()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase)
				);
			Db4objects.Db4o.Query.IQuery foo = q.Descend("foo");
			foo.Constrain("str1").Or(foo.Constrain("str2"));
			Expect(q, new int[] { 2, 3, 5 });
		}

		public virtual void TestOrNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase)
				);
			Db4objects.Db4o.Query.IQuery foo = q.Descend("foo");
			foo.Constrain("str1").Or(foo.Constrain(null));
			Expect(q, new int[] { 0, 1, 2, 4, 5 });
		}

		public virtual void TestTripleOrNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STSDFT1TestCase)
				);
			Db4objects.Db4o.Query.IQuery foo = q.Descend("foo");
			foo.Constrain("str1").Or(foo.Constrain(null)).Or(foo.Constrain("str2"));
			Expect(q, new int[] { 0, 1, 2, 3, 4, 5 });
		}
	}
}
