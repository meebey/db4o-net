namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadeOnDelete : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public string item;
		}

		public Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item[] items;

		public virtual void Test()
		{
			NoAccidentalDeletes();
		}

		private void NoAccidentalDeletes()
		{
			NoAccidentalDeletes1(true, true);
			NoAccidentalDeletes1(true, false);
			NoAccidentalDeletes1(false, true);
			NoAccidentalDeletes1(false, false);
		}

		private void NoAccidentalDeletes1(bool cascadeOnUpdate, bool cascadeOnDelete)
		{
			DeleteAll(GetType());
			DeleteAll(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item));
			Db4objects.Db4o.Config.IObjectClass oc = Db4objects.Db4o.Db4oFactory.Configure().
				ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete));
			oc.CascadeOnDelete(cascadeOnDelete);
			oc.CascadeOnUpdate(cascadeOnUpdate);
			Reopen();
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item i = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item
				();
			Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete cod = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete
				();
			cod.items = new Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item[] { i };
			Db().Set(cod);
			Db().Commit();
			cod.items[0].item = "abrakadabra";
			Db().Set(cod);
			if (!cascadeOnDelete && !cascadeOnUpdate)
			{
				Db().Set(cod.items[0]);
			}
			Db4oUnit.Assert.AreEqual(1, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item)
				));
			Db().Commit();
			Db4oUnit.Assert.AreEqual(1, CountOccurences(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadeOnDelete.Item)
				));
		}
	}
}
