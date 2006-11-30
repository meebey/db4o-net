namespace Db4objects.Db4o.Tests.Common.Staging
{
	/// <exclude></exclude>
	public class AliasesTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class AFoo
		{
			public string foo;
		}

		public class ABar : Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.AFoo
		{
			public string bar;
		}

		public class BFoo
		{
			public string foo;
		}

		public class BBar : Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.AFoo
		{
			public string bar;
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase().RunSolo();
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.ABar bar = new Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.ABar
				();
			bar.foo = "foo";
			bar.bar = "bar";
		}

		private Db4objects.Db4o.Config.WildcardAlias CreateAlias()
		{
			string className = Reflector().ForObject(new Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.ABar
				()).GetName();
			string storedPattern = className.Replace("ABar", "A*");
			string runtimePattern = className.Replace("ABar", "B*");
			return new Db4objects.Db4o.Config.WildcardAlias(storedPattern, runtimePattern);
		}

		public virtual void Test()
		{
			Db().Configure().AddAlias(CreateAlias());
			Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.BBar bar = (Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.BBar
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Staging.AliasesTestCase.BBar)
				);
			Db4oUnit.Assert.AreEqual("foo", bar.foo);
			Db4oUnit.Assert.AreEqual("bar", bar.bar);
		}
	}
}
