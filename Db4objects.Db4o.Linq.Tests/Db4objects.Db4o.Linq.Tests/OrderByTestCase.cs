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
	public class OrderByTestCase : AbstractDb4oLinqTestCase
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

		protected override void Store()
		{
			Store(new Person { Name = "jb", Age = 24 });
			Store(new Person { Name = "ana", Age = 24 });
			Store(new Person { Name = "reg", Age = 25 });
			Store(new Person { Name = "ro", Age = 25 });
			Store(new Person { Name = "jb", Age = 7 });
		}

		public void TestSimpleOrderByAscendingThenDescending()
		{
			AssertQuery("(Person(orderby Name desc)(orderby Age asc))",
				delegate
				{
					var jbs = from Person p in Db()
							  orderby p.Age ascending, p.Name descending
							  select p;

					AssertSequence(new[]
						{
							new Person { Name = "jb", Age = 7 },
							new Person { Name = "jb", Age = 24 },
							new Person { Name = "ana", Age = 24 },
							new Person { Name = "ro", Age = 25 },
							new Person { Name = "reg", Age = 25 }
						}, jbs);
				});
		}

		public void TestSimpleOrderByDescendingThenAscending()
		{
			AssertQuery("(Person(orderby Name asc)(orderby Age desc))",
				delegate
				{
					var jbs = from Person p in Db()
							  orderby p.Age descending, p.Name ascending
							  select p;

					AssertSequence(new[]
						{
							new Person { Name = "reg", Age = 25 },
							new Person { Name = "ro", Age = 25 },
							new Person { Name = "ana", Age = 24 },
							new Person { Name = "jb", Age = 24 },
							new Person { Name = "jb", Age = 7 },
						}, jbs);
				});
		}
	}
}
