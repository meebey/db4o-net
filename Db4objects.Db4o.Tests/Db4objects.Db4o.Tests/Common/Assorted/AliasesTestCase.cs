namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class AliasesTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase().RunSolo();
		}

		private int id;

		private Db4objects.Db4o.Config.IAlias alias;

		public class AFoo
		{
			public string foo;
		}

		public class ABar : Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.AFoo
		{
			public string bar;
		}

		public class BFoo
		{
			public string foo;
		}

		public class BBar : Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BFoo
		{
			public string bar;
		}

		public class CFoo
		{
			public string foo;
		}

		public class CBar : Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.CFoo
		{
			public string bar;
		}

		protected override void Store()
		{
			AddACAlias();
			Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.CBar bar = new Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.CBar
				();
			bar.foo = "foo";
			bar.bar = "bar";
			Store(bar);
			id = (int)Db().GetID(bar);
		}

		public virtual void TestAccessByChildClass()
		{
			AddABAlias();
			Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar bar = (Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar)
				);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessByParentClass()
		{
			AddABAlias();
			Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar bar = (Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BFoo)
				);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessById()
		{
			AddABAlias();
			Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar bar = (Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar
				)Db().GetByID(id);
			Db().Activate(bar, 2);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessWithoutAlias()
		{
			RemoveAlias();
			Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.ABar bar = (Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.ABar
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.ABar)
				);
			AssertInstanceOK(bar);
		}

		private void AssertInstanceOK(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.BBar
			 bar)
		{
			Db4oUnit.Assert.AreEqual("foo", bar.foo);
			Db4oUnit.Assert.AreEqual("bar", bar.bar);
		}

		private void AssertInstanceOK(Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.ABar
			 bar)
		{
			Db4oUnit.Assert.AreEqual("foo", bar.foo);
			Db4oUnit.Assert.AreEqual("bar", bar.bar);
		}

		private void AddABAlias()
		{
			AddAlias("A", "B");
		}

		private void AddACAlias()
		{
			AddAlias("A", "C");
		}

		private void AddAlias(string storedLetter, string runtimeLetter)
		{
			RemoveAlias();
			alias = CreateAlias(storedLetter, runtimeLetter);
			Db().Configure().AddAlias(alias);
		}

		private void RemoveAlias()
		{
			if (alias != null)
			{
				Db().Configure().RemoveAlias(alias);
				alias = null;
			}
		}

		private Db4objects.Db4o.Config.WildcardAlias CreateAlias(string storedLetter, string
			 runtimeLetter)
		{
			string className = Reflector().ForObject(new Db4objects.Db4o.Tests.Common.Assorted.AliasesTestCase.ABar
				()).GetName();
			string storedPattern = className.Replace("ABar", storedLetter + "*");
			string runtimePattern = className.Replace("ABar", runtimeLetter + "*");
			return new Db4objects.Db4o.Config.WildcardAlias(storedPattern, runtimePattern);
		}
	}
}
