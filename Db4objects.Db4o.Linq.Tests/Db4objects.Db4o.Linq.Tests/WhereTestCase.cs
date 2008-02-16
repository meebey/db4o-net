/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Linq;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Linq.Tests
{
	public class WhereTestCase : AbstractDb4oLinqTestCase
	{
		public class Person
		{
			public string Name;
			public int Age;

			public override bool Equals(object obj)
			{
				Person p = obj as Person;
				if (p == null) return false;

				return p.Name == this.Name && p.Age == this.Age;
			}

			public override int GetHashCode()
			{
				return this.Age ^ this.Name.GetHashCode();
			}
		}

		public enum Kind
		{
			Other = 0,
			Plane = 1,
			Car = 2,
		}

		public class Thing
		{
			public Kind Kind;

			public override bool Equals(object obj)
			{
				Thing t = obj as Thing;
				if (t == null) return false;

				return t.Kind == this.Kind;
			}

			public override int GetHashCode()
			{
				return this.Kind.GetHashCode();
			}
		}

		protected override void Store()
		{
			Store(new Person { Name = "jb", Age = 24 });
			Store(new Person { Name = "ana", Age = 20 });
			Store(new Person { Name = "reg", Age = 25 });
			Store(new Person { Name = "ro", Age = 32 });
			Store(new Person { Name = "jb", Age = 7 });
			Store(new Person { Name = "jb", Age = 28 });
			Store(new Person { Name = "jb", Age = 34 });

			Store(new Thing { Kind = Kind.Plane });
			Store(new Thing { Kind = Kind.Car });
			Store(new Thing { Kind = Kind.Plane });
			Store(new Thing { Kind = Kind.Other });
		}

		public void TestEqualsInWhere()
		{
			AssertQuery("(Person(Name == 'jb'))",
				delegate
				{
					var jbs = from Person p in Db()
							  where p.Name == "jb"
							  select p;

					AssertSet(new[]
						{
							new Person { Name = "jb", Age = 24 },
							new Person { Name = "jb", Age = 7 },
							new Person { Name = "jb", Age = 28 },
							new Person { Name = "jb", Age = 34 },
						}, jbs);
				});
		}

		//TODO: not working
		public void _TestEqualsEnum()
		{
			AssertQuery ("(Thing(Kind = 1))",
				delegate {
					var planes = from Thing t in Db ()
								 where t.Kind == Kind.Plane
								 select t;

					AssertSet (new []
					    {
					        new Thing { Kind = Kind.Plane },
					        new Thing { Kind = Kind.Plane },
					    }, planes);
				});
		}

		public void TestInversedEqualsInWhere()
		{
			AssertQuery("(Person(Name == 'jb'))",
				delegate
				{
					var jbs = from Person p in Db()
							  where "jb" == p.Name
							  select p;

					AssertSet(new[]
						{
							new Person { Name = "jb", Age = 24 },
							new Person { Name = "jb", Age = 7 },
							new Person { Name = "jb", Age = 28 },
							new Person { Name = "jb", Age = 34 },
						}, jbs);
				});
		}

		public void TestLessThanInWhere()
		{
			AssertQuery("(Person(Age < 25))",
				delegate
				{
					var youngs = from Person p in Db()
								 where p.Age < 25
								 select p;

					AssertSet(new[]
						{
							new Person { Name = "jb", Age = 24 },
							new Person { Name = "ana", Age = 20 },
							new Person { Name = "jb", Age = 7 }
						}, youngs);
				});
		}

		public void TestSimpleAnd()
		{
			AssertQuery("(Person(((Name == 'jb') and (Age > 10)) and (Age < 30)))",
				delegate
				{
					var ages = from Person p in Db()
							   where p.Name == "jb" && p.Age > 10 && p.Age < 30
							   select p.Age;

					AssertSet(new[] { 24, 28 }, ages);
				});
		}

		public void TestSimpleOr()
		{
			AssertQuery("(Person((Age < 10) or (Age > 30)))",
				delegate
				{
					var ages = from Person p in Db()
							   where p.Age < 10 || p.Age > 30
							   select p.Age;

					AssertSet(new[] { 7, 32, 34 }, ages);
				});
		}

		public void TestSimpleNot()
		{
			AssertQuery("(Person(Name not 'jb'))",
				delegate
				{
					var notjb = from Person p in Db()
								where !(p.Name == "jb")
								select p;

					AssertSet(new[]
					{
						new Person { Name = "ana", Age = 20 },
						new Person { Name = "reg", Age = 25 },
						new Person { Name = "ro", Age = 32 },
					}, notjb);
				});
		}

		public void TestSimpleDifferent()
		{
			AssertQuery("(Person(Name not 'jb'))",
				delegate
				{
					var notjb = from Person p in Db()
								where p.Name != "jb"
								select p;

					AssertSet(new[]
						{
							new Person { Name = "ana", Age = 20 },
							new Person { Name = "reg", Age = 25 },
							new Person { Name = "ro", Age = 32 },
						}, notjb);
				});
		}

		public void TestConvolutedConditionals()
		{
			AssertQuery("(Person((((Age > 30) and (Age < 34)) or ((Age > 10) and (Age < 22))) or (Age == 25)))",
				delegate
				{
					var notjb = from Person p in Db()
								where (((p.Age > 30 && p.Age < 34) || (p.Age > 10 && p.Age < 22)) || p.Age == 25)
								select p;

					AssertSet(new[]
						{
							new Person { Name = "ana", Age = 20 },
							new Person { Name = "reg", Age = 25 },
							new Person { Name = "ro", Age = 32 },
						}, notjb);
				});
		}
	}
}
