namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class PersistTypeTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public sealed class Item
		{
			public System.Type type;

			public Item()
			{
			}

			public Item(System.Type type_)
			{
				type = type_;
			}
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Assorted.PersistTypeTestCase.Item(typeof(string)
				));
		}

		public virtual void Test()
		{
			Db4oUnit.Assert.AreEqual(typeof(string), ((Db4objects.Db4o.Tests.Common.Assorted.PersistTypeTestCase.Item
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Assorted.PersistTypeTestCase.Item)
				)).type);
		}
	}
}
