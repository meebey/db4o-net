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
	public class ComposedQueryTestCase : AbstractDb4oLinqTestCase
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
			var people = new[] 
			{
				new Person { Name = "Malkovitch", Age = 24 },
				new Person { Name = "Malkovitch", Age = 20 },
				new Person { Name = "Malkovitch", Age = 25 },
				new Person { Name = "Malkovitch", Age = 32 },
				new Person { Name = "Malkovitch", Age = 7 },
			};
			foreach (var person in people)
			{
				Store(person);
			}
		}

		public void TestWhereComposition()
		{
			var adults = from Person p in Db()
						 where p.Age > 18
						 select p;

			var johns = from p in adults
						where p.Age < 30
						select p;

			AssertQuery("(Person(Age < 30)(Age > 18))",
				delegate
				{
					AssertSet(new[]
						{
							new Person { Name = "Malkovitch", Age = 24 },
							new Person { Name = "Malkovitch", Age = 20 },
							new Person { Name = "Malkovitch", Age = 25 }
						}, johns);
				});

			AssertQuery("(Person(Age > 18))",
				delegate
				{
					AssertSet(new[]
					{
						new Person { Name = "Malkovitch", Age = 24 },
						new Person { Name = "Malkovitch", Age = 20 },
						new Person { Name = "Malkovitch", Age = 25 },
						new Person { Name = "Malkovitch", Age = 32 },
					}, adults);
				});
		}

		public void TestOrderedWhereComposition()
		{
			var adults = from Person p in Db()
						 where p.Age > 21
						 orderby p.Age
						 select p;

			var johns = from p in adults
						where p.Age < 31
						select p;

			AssertQuery("(Person(Age < 31)(Age > 21)(orderby Age asc))",
				delegate
				{
					AssertSequence(new[]
						{	
							new Person { Name = "Malkovitch", Age = 24 },
							new Person { Name = "Malkovitch", Age = 25 }
						}, johns);
				});

			AssertQuery("(Person(Age > 21)(orderby Age asc))",
				delegate
				{
					AssertSequence(new[]
					{
						new Person { Name = "Malkovitch", Age = 24 },
						new Person { Name = "Malkovitch", Age = 25 },
						new Person { Name = "Malkovitch", Age = 32 },
					}, adults);
				});
		}
	}
}
