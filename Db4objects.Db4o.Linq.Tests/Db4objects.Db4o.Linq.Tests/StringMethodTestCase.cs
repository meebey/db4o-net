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
	public class StringMethodTestCase : AbstractDb4oLinqTestCase
	{
		public class Person
		{
			public string Name;

			public override bool Equals(object obj)
			{
				Person p = obj as Person;
				if (p == null) return false;

				return p.Name == this.Name;
			}

			public override int GetHashCode()
			{
				return this.Name.GetHashCode();
			}
		}

		protected override void Store()
		{
			Store(new Person { Name = "BiroBiro" });
			Store(new Person { Name = "Luna" });
			Store(new Person { Name = "Loustic" });
			Store(new Person { Name = "Loupiot" });
			Store(new Person { Name = "LeMiro" });
			Store(new Person { Name = "Tounage" });
		}

		public void TestStartsWith()
		{
			AssertQuery("(Person(Name startswith 'Lo'))",
				delegate
				{
					var los = from Person p in Db()
								where p.Name.StartsWith("Lo")
								select p;

					AssertSet(new[]
						{
							new Person { Name = "Loustic" },
							new Person { Name = "Loupiot" }
						}, los);
				});
		}

		public void TestEndsWith()
		{
			AssertQuery("(Person(Name endswith 'iro'))",
				delegate
				{
					var los = from Person p in Db()
							  where p.Name.EndsWith("iro")
							  select p;

					AssertSet(new[]
						{
							new Person { Name = "BiroBiro" },
							new Person { Name = "LeMiro" }
						}, los);
				});
		}

		public void TestContains()
		{
			AssertQuery("(Person(Name contains 'una'))",
				delegate
				{
					var los = from Person p in Db()
							  where p.Name.Contains("una")
							  select p;

					AssertSet(new[]
						{
							new Person { Name = "Luna" },
							new Person { Name = "Tounage" }
						}, los);
				});
		}
	}
}
