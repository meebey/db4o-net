namespace Db4objects.Db4o.Tests.Common.Regression
{
	/// <exclude></exclude>
	public class COR57TestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase().RunSolo();
		}

		public class Base
		{
			public string name;

			public Base()
			{
			}

			public Base(string name_)
			{
				name = name_;
			}

			public override string ToString()
			{
				return GetType() + ":" + name;
			}
		}

		public class BaseExt : Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
		{
			public BaseExt()
			{
			}

			public BaseExt(string name_) : base(name_)
			{
			}
		}

		public class BaseExtExt : Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExt
		{
			public BaseExtExt()
			{
			}

			public BaseExtExt(string name_) : base(name_)
			{
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base)
				).ObjectField("name").Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < 5; i++)
			{
				string name = i.ToString();
				Db().Set(new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base(name));
				Db().Set(new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExt(name));
				Db().Set(new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExtExt(name
					));
			}
		}

		public virtual void TestQBE()
		{
			AssertQBE(1, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExtExt
				("1"));
			AssertQBE(2, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExt("1"
				));
			AssertQBE(3, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base("1"));
		}

		public virtual void TestSODA()
		{
			AssertSODA(1, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExtExt
				("1"));
			AssertSODA(2, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.BaseExt("1"
				));
			AssertSODA(3, new Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base("1")
				);
		}

		private void AssertSODA(int expectedCount, Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
			 template)
		{
			AssertQueryResult(expectedCount, template, CreateSODA(template).Execute());
		}

		private Db4objects.Db4o.Query.IQuery CreateSODA(Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
			 template)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(template.GetType());
			q.Descend("name").Constrain(template.name);
			return q;
		}

		private void AssertQBE(int expectedCount, Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
			 template)
		{
			AssertQueryResult(expectedCount, template, Db().Get(template));
		}

		private void AssertQueryResult(int expectedCount, Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
			 expectedTemplate, Db4objects.Db4o.IObjectSet result)
		{
			Db4oUnit.Assert.AreEqual(expectedCount, result.Size(), SimpleName(expectedTemplate
				.GetType()));
			while (result.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base actual = (Db4objects.Db4o.Tests.Common.Regression.COR57TestCase.Base
					)result.Next();
				Db4oUnit.Assert.AreEqual(expectedTemplate.name, actual.name);
				Db4oUnit.Assert.IsInstanceOf(expectedTemplate.GetType(), actual);
			}
		}

		private string SimpleName(System.Type c)
		{
			string name = c.FullName;
			return Sharpen.Runtime.Substring(name, name.LastIndexOf('$') + 1);
		}
	}
}
