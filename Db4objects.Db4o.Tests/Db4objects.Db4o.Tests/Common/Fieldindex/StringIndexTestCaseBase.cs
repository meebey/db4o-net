namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	/// <exclude></exclude>
	public abstract class StringIndexTestCaseBase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
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

		public StringIndexTestCaseBase() : base()
		{
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item)
				, "name");
		}

		protected virtual void AssertItems(string[] expected, Db4objects.Db4o.IObjectSet 
			result)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(ToObjectArray(expected));
			while (result.HasNext())
			{
				expectingVisitor.Visit(((Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item
					)result.Next()).name);
			}
			expectingVisitor.AssertExpectations();
		}

		protected virtual object[] ToObjectArray(string[] source)
		{
			object[] array = new object[source.Length];
			System.Array.Copy(source, 0, array, 0, source.Length);
			return array;
		}

		protected virtual void GrafittiFreeSpace()
		{
			Db4objects.Db4o.Internal.IoAdaptedObjectContainer file = ((Db4objects.Db4o.Internal.IoAdaptedObjectContainer
				)Db());
			Db4objects.Db4o.Internal.Freespace.FreespaceManagerRam fm = (Db4objects.Db4o.Internal.Freespace.FreespaceManagerRam
				)file.FreespaceManager();
			fm.TraverseFreeSlots(new _AnonymousInnerClass58(this, file));
		}

		private sealed class _AnonymousInnerClass58 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass58(StringIndexTestCaseBase _enclosing, Db4objects.Db4o.Internal.IoAdaptedObjectContainer
				 file)
			{
				this._enclosing = _enclosing;
				this.file = file;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Slots.Slot slot = (Db4objects.Db4o.Internal.Slots.Slot)obj;
				file.OverwriteDeletedBytes(slot.GetAddress(), slot.GetLength());
			}

			private readonly StringIndexTestCaseBase _enclosing;

			private readonly Db4objects.Db4o.Internal.IoAdaptedObjectContainer file;
		}

		protected virtual void AssertExists(string itemName)
		{
			AssertExists(Trans(), itemName);
		}

		protected virtual void Add(string itemName)
		{
			Add(Trans(), itemName);
		}

		protected virtual void Add(Db4objects.Db4o.Internal.Transaction transaction, string
			 itemName)
		{
			Stream().Set(transaction, new Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item
				(itemName));
		}

		protected virtual void AssertExists(Db4objects.Db4o.Internal.Transaction transaction
			, string itemName)
		{
			Db4oUnit.Assert.IsNotNull(Query(transaction, itemName));
		}

		protected virtual void Rename(Db4objects.Db4o.Internal.Transaction transaction, string
			 from, string to)
		{
			Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item item = Query
				(transaction, from);
			Db4oUnit.Assert.IsNotNull(item);
			item.name = to;
			Stream().Set(transaction, item);
		}

		protected virtual void Rename(string from, string to)
		{
			Rename(Trans(), from, to);
		}

		protected virtual Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item
			 Query(string name)
		{
			return Query(Trans(), name);
		}

		protected virtual Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item
			 Query(Db4objects.Db4o.Internal.Transaction transaction, string name)
		{
			Db4objects.Db4o.IObjectSet objectSet = NewQuery(transaction, name).Execute();
			if (!objectSet.HasNext())
			{
				return null;
			}
			return (Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item)objectSet
				.Next();
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewQuery(Db4objects.Db4o.Internal.Transaction
			 transaction, string itemName)
		{
			Db4objects.Db4o.Query.IQuery query = Stream().Query(transaction);
			query.Constrain(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.StringIndexTestCaseBase.Item)
				);
			query.Descend("name").Constrain(itemName);
			return query;
		}
	}
}
