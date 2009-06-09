/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Db4objects.Db4o.Linq.Tests
{
	public class OrderByTestCase : AbstractDb4oLinqTestCase
	{
		public class Person
		{
			public string Name;
			public int Age;
			public Person Parent;
			public DateTimeOffset BirthDate;

			public int UnoptimizableAgeProperty
			{
				get
				{
					return Age + 1;
				}
			}

			public string UnoptimizableNameProperty
			{
				get
				{
					if (string.IsNullOrEmpty(Name))
					{
						return Age.ToString();
					}
					return Name + " (" + Age + ")";
				}
			}

			public string OptimizableNameProperty
			{
				get { return Name; }
			}

			public int OptimizableAgeProperty
			{
				get { return Age; }
			}

			public override bool Equals(object obj)
			{
				Person p = obj as Person;
				if (p == null) return false;

				return p.Name == Name && p.Age == Age && p.BirthDate == BirthDate;
			}

			public override int GetHashCode()
			{
				return Age ^ Name.GetHashCode() ^ BirthDate.GetHashCode();
			}

			public override string ToString()
			{
				return "Person(" + Name + ", " + Age + ")";
			}

			public int GetAge()
			{
				return Age;
			}

			public string GetName()
			{
				return Name;
			}
		}

		protected override void Store()
		{
			foreach (var person in People())
			{
				Store(person);
			}
		}

		private static Person[] People()
		{
			return new[] {
			             	new Person { Name = "jb", Age = 24 , BirthDate =  new DateTime(2009, 01, 01)},
			             	new Person { Name = "ana", Age = 24, BirthDate =  new DateTime(2009, 02, 01) },
			             	new Person { Name = "reg", Age = 25, BirthDate =  new DateTime(2009, 03, 01) },
			             	new Person { Name = "ro", Age = 25, BirthDate =  new DateTime(2009, 04, 01) },
			             	new Person { Name = "jb", Age = 7, BirthDate =  new DateTime(2009, 05, 01) }
			             };
		}

		public void TestOrderByValueType()
		{
			AssertQuerySequence(
				from Person p in Db()
				orderby p.BirthDate
				select p,

				"(Person(orderby BirthDate asc))",
				
				from p in People()
				orderby p.BirthDate
				select p);
		}

		public void TestOrderByOnUnoptimizableStringProperty()
		{
			AssertQuerySequence(
				from Person p in Db()
				where p.Name != "jb"
				orderby p.UnoptimizableNameProperty
				select p,

				"(Person(Name not 'jb'))",

				from p in People()
				where p.Name != "jb"
				orderby p.UnoptimizableNameProperty
				select p);
		}

		public void TestOrderByOnUnoptimizableProperty()
		{
			AssertQuerySequence(
				from Person p in Db()
				where p.Name == "jb"
				orderby p.UnoptimizableAgeProperty
				select p,

				"(Person(Name == 'jb'))",

				from p in People()
				where p.Name == "jb"
				orderby p.UnoptimizableAgeProperty
				select p);
		}

		public void TestOrderByDescendingOnWhere()
		{
			AssertQuerySequence(
				from Person p in Db()
				where p.Name == "jb"
				orderby p.Age descending
				select p,

				"(Person(Name == 'jb')(orderby Age desc))",

				from p in People()
				where p.Name == "jb"
				orderby p.Age descending
				select p);
		}

		public void TestOrderByDescendingOnUnoptimizableProperty()
		{
			AssertQuerySequence(
				from Person p1 in Db()
			    where p1.Name == "jb"
			    orderby p1.UnoptimizableAgeProperty descending
			    select p1, 
				
				"(Person(Name == 'jb'))", 
				
				from p2 in People()
				where p2.Name == "jb"
				orderby p2.UnoptimizableAgeProperty descending
				select p2);
		}

		public void _TestUnoptimizableThenByOnOptimizedOrderBy()
		{
			var query = from Person p in Db()
						orderby p.OptimizableAgeProperty ascending,
							p.UnoptimizableNameProperty descending
						select p;
			AssertOrderByNameDescAgeAsc("(Person)(orderby Age asc)", query);
		}

		public void TestUnoptimizableOrderByAscendingThenDescendingOnProperties()
		{
			var query = from Person p in Db()
						orderby p.UnoptimizableAgeProperty ascending,
							p.UnoptimizableNameProperty descending
						select p;

			AssertOrderByNameDescAgeAsc("(Person)", query);
		}

		public void TestSimpleOrderByAscendingThenDescendingProperties()
		{
			var query = from Person p in Db()
						orderby p.OptimizableAgeProperty ascending,
							p.OptimizableNameProperty descending
						select p;

			AssertOrderByNameDescAgeAsc(query);
		}

		public void TestSimpleOrderByAscendingThenDescendingMethods()
		{
			var query = from Person p in Db()
						orderby p.GetAge() ascending,
							p.GetName() descending
						select p;

			AssertOrderByNameDescAgeAsc(query);
		}

		public void TestSimpleOrderByAscendingThenDescendingFields()
		{
			var query = from Person p in Db()
					  orderby p.Age ascending, p.Name descending
					  select p;

			AssertOrderByNameDescAgeAsc(query);
		}

		private void AssertOrderByNameDescAgeAsc(IDb4oLinqQuery<Person> query)
		{
			string expectedQuery = "(Person(orderby Name desc)(orderby Age asc))";
			AssertOrderByNameDescAgeAsc(expectedQuery, query);
		}

		private void AssertOrderByNameDescAgeAsc(string expectedQuery, IDb4oLinqQuery<Person> query)
		{
			AssertQuerySequence(
				query, 
				expectedQuery, 

				from p in People()
				orderby p.Age ascending, p.Name descending
				select p);
		}

		public void TestSimpleOrderByDescendingThenAscending()
		{
			AssertQuerySequence(
				from Person p1 in Db()
				orderby p1.Age descending, p1.Name ascending
				select p1,

				"(Person(orderby Name asc)(orderby Age desc))",

				from p2 in People()
				orderby p2.Age descending, p2.Name ascending
				select p2);
		}

		public void TestOrderByDescendingThenAscendingOnCompositeFieldAccess()
		{
			AssertQueryTranslation(
				from Person p in Db()
				orderby p.Parent.Age descending, p.Parent.Name ascending
				select p,
				
				"(Person(orderby Parent.Name asc)(orderby Parent.Age desc))");
		}

		private void AssertQueryTranslation<T>(IEnumerable<T> query, string expectedRepresentation)
		{
			AssertQuery(expectedRepresentation, () => query.ToList());
		}
		
		private void AssertQuerySequence(IDb4oLinqQuery<Person> query, string expectedQueryString, IEnumerable<Person> expectedSequence)
		{
			AssertQuery(query, expectedQueryString, actualSequence => AssertSequence(expectedSequence, actualSequence));
		}
	}
}
