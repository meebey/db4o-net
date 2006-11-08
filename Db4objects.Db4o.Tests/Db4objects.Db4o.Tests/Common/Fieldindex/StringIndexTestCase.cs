namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	/// <exclude></exclude>
	public class StringIndexTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase().RunSolo();
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name_)
			{
				name = name_;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item)
				, "name");
		}

		public virtual void TestNotEquals()
		{
			Add("foo");
			Add("bar");
			Add("baz");
			Add(null);
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item)
				);
			query.Descend("name").Constrain("bar").Not();
			AssertItems(new string[] { "foo", "baz", null }, query.Execute());
		}

		private void AssertItems(string[] expected, Db4objects.Db4o.IObjectSet result)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(ToObjectArray(expected));
			while (result.HasNext())
			{
				expectingVisitor.Visit(((Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item
					)result.Next()).name);
			}
			expectingVisitor.AssertExpectations();
		}

		private object[] ToObjectArray(string[] source)
		{
			object[] array = new object[source.Length];
			System.Array.Copy(source, 0, array, 0, source.Length);
			return array;
		}

		public virtual void TestCancelRemovalRollback()
		{
			PrepareCancelRemoval(Trans(), "original");
			Rename("original", "updated");
			Db().Rollback();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		public virtual void TestCancelRemovalRollbackForMultipleTransactions()
		{
			Db4objects.Db4o.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Transaction trans2 = NewTransaction();
			PrepareCancelRemoval(trans1, "original");
			AssertExists(trans2, "original");
			trans1.Rollback();
			AssertExists(trans2, "original");
			Add(trans2, "second");
			AssertExists(trans2, "original");
			trans2.Commit();
			AssertExists(trans2, "original");
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		public virtual void TestCancelRemoval()
		{
			PrepareCancelRemoval(Trans(), "original");
			Db().Commit();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		private void PrepareCancelRemoval(Db4objects.Db4o.Transaction transaction, string
			 itemName)
		{
			Add(itemName);
			Db().Commit();
			Rename(transaction, itemName, "updated");
			AssertExists(transaction, "updated");
			Rename(transaction, "updated", itemName);
			AssertExists(transaction, itemName);
		}

		public virtual void TestCancelRemovalForMultipleTransactions()
		{
			Db4objects.Db4o.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Transaction trans2 = NewTransaction();
			PrepareCancelRemoval(trans1, "original");
			Rename(trans2, "original", "updated");
			trans1.Commit();
			GrafittiFreeSpace();
			Reopen();
			AssertExists("original");
		}

		private void GrafittiFreeSpace()
		{
			Db4objects.Db4o.YapRandomAccessFile file = ((Db4objects.Db4o.YapRandomAccessFile)
				Db());
			Db4objects.Db4o.Inside.Freespace.FreespaceManagerRam fm = (Db4objects.Db4o.Inside.Freespace.FreespaceManagerRam
				)file.FreespaceManager();
			fm.TraverseFreeSlots(new _AnonymousInnerClass134(this, file));
		}

		private sealed class _AnonymousInnerClass134 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass134(StringIndexTestCase _enclosing, Db4objects.Db4o.YapRandomAccessFile
				 file)
			{
				this._enclosing = _enclosing;
				this.file = file;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Inside.Slots.Slot slot = (Db4objects.Db4o.Inside.Slots.Slot)obj;
				file.WriteXBytes(slot.GetAddress(), slot.GetLength());
			}

			private readonly StringIndexTestCase _enclosing;

			private readonly Db4objects.Db4o.YapRandomAccessFile file;
		}

		public virtual void TestDeletingAndReaddingMember()
		{
			Add("original");
			AssertExists("original");
			Rename("original", "updated");
			AssertExists("updated");
			Db4oUnit.Assert.IsNull(Query("original"));
			Reopen();
			AssertExists("updated");
			Db4oUnit.Assert.IsNull(Query("original"));
		}

		private void AssertExists(string itemName)
		{
			AssertExists(Trans(), itemName);
		}

		private void Add(string itemName)
		{
			Add(Trans(), itemName);
		}

		private void Add(Db4objects.Db4o.Transaction transaction, string itemName)
		{
			Stream().Set(transaction, new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item
				(itemName));
		}

		private void AssertExists(Db4objects.Db4o.Transaction transaction, string itemName
			)
		{
			Db4oUnit.Assert.IsNotNull(Query(transaction, itemName));
		}

		private void Rename(Db4objects.Db4o.Transaction transaction, string from, string 
			to)
		{
			Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item item = Query(transaction
				, from);
			Db4oUnit.Assert.IsNotNull(item);
			item.name = to;
			Stream().Set(transaction, item);
		}

		private void Rename(string from, string to)
		{
			Rename(Trans(), from, to);
		}

		private Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item Query(string
			 name)
		{
			return Query(Trans(), name);
		}

		private Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item Query(Db4objects.Db4o.Transaction
			 transaction, string name)
		{
			Db4objects.Db4o.IObjectSet objectSet = NewQuery(transaction, name).Execute();
			if (!objectSet.HasNext())
			{
				return null;
			}
			return (Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item)objectSet
				.Next();
		}

		private Db4objects.Db4o.Query.IQuery NewQuery(Db4objects.Db4o.Transaction transaction
			, string itemName)
		{
			Db4objects.Db4o.Query.IQuery query = Stream().Query(transaction);
			query.Constrain(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCase.Item)
				);
			query.Descend("name").Constrain(itemName);
			return query;
		}
	}
}
