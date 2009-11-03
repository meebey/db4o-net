/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class DeleteOnDeletingCallbackTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
		}

		public class RootItem
		{
			public DeleteOnDeletingCallbackTestCase.Item child;

			public RootItem()
			{
			}

			public virtual void ObjectOnDelete(IObjectContainer container)
			{
				container.Delete(child);
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new DeleteOnDeletingCallbackTestCase.RootItem());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			DeleteOnDeletingCallbackTestCase.RootItem root = ((DeleteOnDeletingCallbackTestCase.RootItem
				)RetrieveOnlyInstance(typeof(DeleteOnDeletingCallbackTestCase.RootItem)));
			root.child = new DeleteOnDeletingCallbackTestCase.Item();
			Db().Store(root);
			Db().Delete(root);
			Reopen();
			AssertClassIndexIsEmpty();
		}

		private void AssertClassIndexIsEmpty()
		{
			Iterator4Assert.AreEqual(new object[0], GetAllIds());
		}

		private IIntIterator4 GetAllIds()
		{
			return FileSession().GetAll(FileSession().Transaction, QueryEvaluationMode.Immediate
				).IterateIDs();
		}
	}
}
