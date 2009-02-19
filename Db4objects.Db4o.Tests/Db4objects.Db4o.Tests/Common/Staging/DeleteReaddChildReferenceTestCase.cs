/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	/// <summary>COR-1539  Readding a deleted object from a different client changes database ID in embedded mode
	/// 	</summary>
	public class DeleteReaddChildReferenceTestCase : Db4oClientServerTestCase
	{
		public class ItemParent
		{
			public DeleteReaddChildReferenceTestCase.Item child;
		}

		public class Item
		{
			public string name;

			public Item(string name_)
			{
				name = name_;
			}
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			DeleteReaddChildReferenceTestCase.Item child = new DeleteReaddChildReferenceTestCase.Item
				("child");
			DeleteReaddChildReferenceTestCase.ItemParent parent = new DeleteReaddChildReferenceTestCase.ItemParent
				();
			parent.child = child;
			Store(parent);
		}

		public virtual void TestDeleteReadd()
		{
			IExtObjectContainer client1 = Db();
			IExtObjectContainer client2 = OpenNewClient();
			DeleteReaddChildReferenceTestCase.ItemParent parent1 = ((DeleteReaddChildReferenceTestCase.ItemParent
				)RetrieveOnlyInstance(client1, typeof(DeleteReaddChildReferenceTestCase.ItemParent
				)));
			DeleteReaddChildReferenceTestCase.ItemParent parent2 = ((DeleteReaddChildReferenceTestCase.ItemParent
				)RetrieveOnlyInstance(client2, typeof(DeleteReaddChildReferenceTestCase.ItemParent
				)));
			client1.Delete(parent1.child);
			client1.Commit();
			client2.Store(parent2.child);
			client2.Commit();
			client2.Close();
			DeleteReaddChildReferenceTestCase.ItemParent parent3 = ((DeleteReaddChildReferenceTestCase.ItemParent
				)RetrieveOnlyInstance(client1, typeof(DeleteReaddChildReferenceTestCase.ItemParent
				)));
			Db().Refresh(parent3, int.MaxValue);
			Assert.IsNotNull(parent3.child);
		}

		public static void Main(string[] arguments)
		{
			new DeleteReaddChildReferenceTestCase().RunAll();
		}
	}
}
