namespace Db4objects.Db4o.Tests.Common.Classindex
{
	public class ClassIndexOffTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public class Item
		{
			public string name;

			public Item(string _name)
			{
				this.name = _name;
			}
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Classindex.ClassIndexOffTestCase().RunSolo();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Classindex.ClassIndexOffTestCase.Item)
				).Indexed(false);
		}

		public virtual void Test()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Classindex.ClassIndexOffTestCase.Item("1"
				));
			Db4objects.Db4o.Internal.ClassMetadata yc = (Db4objects.Db4o.Internal.ClassMetadata
				)Db().StoredClass(typeof(Db4objects.Db4o.Tests.Common.Classindex.ClassIndexOffTestCase.Item)
				);
			Db4oUnit.Assert.IsFalse(yc.HasIndex());
			AssertNoItemFound();
			Db().Commit();
			AssertNoItemFound();
		}

		private void AssertNoItemFound()
		{
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Classindex.ClassIndexOffTestCase.Item)
				);
			Db4oUnit.Assert.AreEqual(0, q.Execute().Size());
		}
	}
}
