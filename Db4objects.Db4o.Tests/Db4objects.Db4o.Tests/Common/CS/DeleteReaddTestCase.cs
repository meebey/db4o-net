/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class DeleteReaddTestCase : Db4oClientServerTestCase
	{
		public class ItemParent
		{
		}

		public class Item : DeleteReaddTestCase.ItemParent
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
			Store(new DeleteReaddTestCase.Item("one"));
		}

		public virtual void TestDeleteReadd()
		{
			IExtObjectContainer client1 = Db();
			IExtObjectContainer client2 = OpenNewClient();
			DeleteReaddTestCase.Item item1 = (DeleteReaddTestCase.Item)((DeleteReaddTestCase.Item
				)RetrieveOnlyInstance(client1, typeof(DeleteReaddTestCase.Item)));
			DeleteReaddTestCase.Item item2 = (DeleteReaddTestCase.Item)((DeleteReaddTestCase.Item
				)RetrieveOnlyInstance(client2, typeof(DeleteReaddTestCase.Item)));
			client1.Delete(item1);
			client1.Commit();
			client2.Store(item2);
			client2.Commit();
			client2.Close();
			RetrieveOnlyInstance(client1, typeof(DeleteReaddTestCase.Item));
			RetrieveOnlyInstance(client1, typeof(DeleteReaddTestCase.ItemParent));
		}

		public static void Main(string[] arguments)
		{
			new DeleteReaddTestCase().RunClientServer();
		}
	}
}
