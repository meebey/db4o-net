using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class AliasesTestCase : AbstractDb4oTestCase, IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new AliasesTestCase().RunSolo();
		}

		private int id;

		private IAlias alias;

		public class AFoo
		{
			public string foo;
		}

		public class ABar : AliasesTestCase.AFoo
		{
			public string bar;
		}

		public class BFoo
		{
			public string foo;
		}

		public class BBar : AliasesTestCase.BFoo
		{
			public string bar;
		}

		public class CFoo
		{
			public string foo;
		}

		public class CBar : AliasesTestCase.CFoo
		{
			public string bar;
		}

		protected override void Store()
		{
			AddACAlias();
			AliasesTestCase.CBar bar = new AliasesTestCase.CBar();
			bar.foo = "foo";
			bar.bar = "bar";
			Store(bar);
			id = (int)Db().GetID(bar);
		}

		public virtual void TestAccessByChildClass()
		{
			AddABAlias();
			AliasesTestCase.BBar bar = (AliasesTestCase.BBar)RetrieveOnlyInstance(typeof(AliasesTestCase.BBar)
				);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessByParentClass()
		{
			AddABAlias();
			AliasesTestCase.BBar bar = (AliasesTestCase.BBar)RetrieveOnlyInstance(typeof(AliasesTestCase.BFoo)
				);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessById()
		{
			AddABAlias();
			AliasesTestCase.BBar bar = (AliasesTestCase.BBar)Db().GetByID(id);
			Db().Activate(bar, 2);
			AssertInstanceOK(bar);
		}

		public virtual void TestAccessWithoutAlias()
		{
			RemoveAlias();
			AliasesTestCase.ABar bar = (AliasesTestCase.ABar)RetrieveOnlyInstance(typeof(AliasesTestCase.ABar)
				);
			AssertInstanceOK(bar);
		}

		private void AssertInstanceOK(AliasesTestCase.BBar bar)
		{
			Assert.AreEqual("foo", bar.foo);
			Assert.AreEqual("bar", bar.bar);
		}

		private void AssertInstanceOK(AliasesTestCase.ABar bar)
		{
			Assert.AreEqual("foo", bar.foo);
			Assert.AreEqual("bar", bar.bar);
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

		private WildcardAlias CreateAlias(string storedLetter, string runtimeLetter)
		{
			string className = Reflector().ForObject(new AliasesTestCase.ABar()).GetName();
			string storedPattern = className.Replace("ABar", storedLetter + "*");
			string runtimePattern = className.Replace("ABar", runtimeLetter + "*");
			return new WildcardAlias(storedPattern, runtimePattern);
		}
	}
}
