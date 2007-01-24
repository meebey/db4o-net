namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class LazyQueryDeleteTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private const int COUNT = 3;

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.Queries().EvaluationMode(Db4objects.Db4o.Config.QueryEvaluationMode.LAZY);
		}

		protected override void Store()
		{
			for (int i = 0; i < COUNT; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Staging.LazyQueryDeleteTestCase.Item(i.ToString
					()));
				Db().Commit();
			}
		}

		public virtual void Test()
		{
			Db4objects.Db4o.IObjectSet objectSet = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Staging.LazyQueryDeleteTestCase.Item)
				).Execute();
			for (int i = 0; i < COUNT; i++)
			{
				Db().Delete(objectSet.Next());
				Db().Commit();
			}
		}

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Staging.LazyQueryDeleteTestCase().RunSolo();
		}
	}
}
