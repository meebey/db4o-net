namespace Db4objects.Db4o.Tests.Common.Constraints
{
	public class UniqueFieldIndexTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase().RunClientServer
				();
		}

		public class Item
		{
			public string _str;

			public Item()
			{
			}

			public Item(string str)
			{
				_str = str;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item)
				).ObjectField("_str").Indexed(true);
			config.Add(new Db4objects.Db4o.Constraints.UniqueFieldValueConstraint(typeof(Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item)
				, "_str"));
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item(
				"1"));
			Store(new Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item(
				"2"));
			Store(new Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item(
				"3"));
		}

		public virtual void TestNewViolates()
		{
			Store(new Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item(
				"2"));
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Constraints.UniqueFieldValueConstraintViolationException)
				, new _AnonymousInnerClass46(this));
			Db().Rollback();
		}

		private sealed class _AnonymousInnerClass46 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass46(UniqueFieldIndexTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly UniqueFieldIndexTestCase _enclosing;
		}

		public virtual void TestUpdateViolates()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item)
				);
			q.Descend("_str").Constrain("2");
			Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item item = (Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item
				)q.Execute().Next();
			item._str = "3";
			Store(item);
			Db4oUnit.Assert.Expect(typeof(Db4objects.Db4o.Constraints.UniqueFieldValueConstraintViolationException)
				, new _AnonymousInnerClass60(this));
			Db().Rollback();
		}

		private sealed class _AnonymousInnerClass60 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass60(UniqueFieldIndexTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly UniqueFieldIndexTestCase _enclosing;
		}

		public virtual void TestUpdateDoesNotViolate()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item)
				);
			q.Descend("_str").Constrain("2");
			Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item item = (Db4objects.Db4o.Tests.Common.Constraints.UniqueFieldIndexTestCase.Item
				)q.Execute().Next();
			item._str = "4";
			Store(item);
			Db().Commit();
		}
	}
}
