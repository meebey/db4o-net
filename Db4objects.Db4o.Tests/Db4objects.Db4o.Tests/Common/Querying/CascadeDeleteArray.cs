namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeDeleteArray : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class ArrayElem
		{
			public string name;

			public ArrayElem(string name)
			{
				this.name = name;
			}
		}

		public Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem[] array;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(this).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray cda = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray
				();
			cda.array = new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem
				[] { new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem("one"
				), new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem("two")
				, new Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem("three"
				) };
			Db().Set(cda);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray cda = (Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray
				)RetrieveOnlyInstance(GetType());
			Db4oUnit.Assert.AreEqual(3, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem)
				));
			Db().Delete(cda);
			Db4oUnit.Assert.AreEqual(0, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem)
				));
			Db().Rollback();
			Db4oUnit.Assert.AreEqual(3, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem)
				));
			Db().Delete(cda);
			Db4oUnit.Assert.AreEqual(0, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem)
				));
			Db().Commit();
			Db4oUnit.Assert.AreEqual(0, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeDeleteArray.ArrayElem)
				));
		}
	}
}
