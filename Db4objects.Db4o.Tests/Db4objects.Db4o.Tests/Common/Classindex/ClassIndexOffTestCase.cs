using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Classindex;

namespace Db4objects.Db4o.Tests.Common.Classindex
{
	public class ClassIndexOffTestCase : AbstractDb4oTestCase, IOptOutCS
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
			new ClassIndexOffTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(ClassIndexOffTestCase.Item)).Indexed(false);
		}

		public virtual void Test()
		{
			Db().Set(new ClassIndexOffTestCase.Item("1"));
			ClassMetadata yc = (ClassMetadata)Db().StoredClass(typeof(ClassIndexOffTestCase.Item)
				);
			Assert.IsFalse(yc.HasIndex());
			AssertNoItemFound();
			Db().Commit();
			AssertNoItemFound();
		}

		private void AssertNoItemFound()
		{
			IQuery q = Db().Query();
			q.Constrain(typeof(ClassIndexOffTestCase.Item));
			Assert.AreEqual(0, q.Execute().Size());
		}
	}
}
