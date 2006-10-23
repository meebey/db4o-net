namespace Db4objects.Db4o.Tests.Common.Querying
{
	/// <exclude></exclude>
	public class MultiFieldIndexQueryTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase().RunSolo(
				);
		}

		public class Book
		{
			public Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person[]
				 authors;

			public string title;

			public Book()
			{
			}

			public Book(string title, Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person[]
				 authors)
			{
				this.title = title;
				this.authors = authors;
			}

			public override string ToString()
			{
				string ret = title;
				if (authors != null)
				{
					for (int i = 0; i < authors.Length; i++)
					{
						ret += "\n  " + authors[i].ToString();
					}
				}
				return ret;
			}
		}

		public class Person
		{
			public string firstName;

			public string lastName;

			public Person()
			{
			}

			public Person(string firstName, string lastName)
			{
				this.firstName = firstName;
				this.lastName = lastName;
			}

			public override string ToString()
			{
				return "Person " + firstName + " " + lastName;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexAllFields(config, typeof(Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book)
				);
			IndexAllFields(config, typeof(Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person)
				);
		}

		protected virtual void IndexAllFields(Db4objects.Db4o.Config.IConfiguration config
			, System.Type clazz)
		{
			System.Reflection.FieldInfo[] fields = Sharpen.Runtime.GetDeclaredFields(clazz);
			for (int i = 0; i < fields.Length; i++)
			{
				IndexField(config, clazz, fields[i].Name);
			}
			System.Type superclass = clazz.BaseType;
			if (superclass != null)
			{
				IndexAllFields(config, superclass);
			}
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person aaron = 
				new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Aaron"
				, "OneOK");
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person bill = 
				new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Bill"
				, "TwoOK");
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person chris = 
				new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Chris"
				, "ThreeOK");
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person dave = 
				new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Dave"
				, "FourOK");
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person neil = 
				new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Neil"
				, "Notwanted");
			Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person nat = new 
				Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person("Nat", 
				"Neverwanted");
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book
				("Persistence possibilities", new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person
				[] { aaron, bill, chris }));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book
				("Persistence using S.O.D.A.", new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person
				[] { aaron }));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book
				("Persistence using JDO", new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person
				[] { bill, dave }));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book
				("Don't want to find Phil", new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person
				[] { aaron, bill, neil }));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book
				("Persistence by Jeff", new Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person
				[] { nat }));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery qBooks = NewQuery();
			qBooks.Constrain(typeof(Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Book)
				);
			qBooks.Descend("title").Constrain("Persistence").Like();
			Db4objects.Db4o.Query.IQuery qAuthors = qBooks.Descend("authors");
			Db4objects.Db4o.Query.IQuery qFirstName = qAuthors.Descend("firstName");
			Db4objects.Db4o.Query.IQuery qLastName = qAuthors.Descend("lastName");
			Db4objects.Db4o.Query.IConstraint cAaron = qFirstName.Constrain("Aaron").And(qLastName
				.Constrain("OneOK"));
			Db4objects.Db4o.Query.IConstraint cBill = qFirstName.Constrain("Bill").And(qLastName
				.Constrain("TwoOK"));
			cAaron.Or(cBill);
			Db4objects.Db4o.IObjectSet results = qAuthors.Execute();
			Db4oUnit.Assert.AreEqual(4, results.Size());
			while (results.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person person = 
					(Db4objects.Db4o.Tests.Common.Querying.MultiFieldIndexQueryTestCase.Person)results
					.Next();
				Db4oUnit.Assert.IsTrue(person.lastName.EndsWith("OK"));
			}
		}
	}
}
