/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DatabaseGrowthSizeTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		private const int Size = 10000;

		private const int ApproximateHeaderSize = 100;

		public static void Main(string[] args)
		{
			new DatabaseGrowthSizeTestCase().RunSolo();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.DatabaseGrowthSize(Size);
			config.BlockSize(3);
		}

		public virtual void Test()
		{
			Assert.IsGreater(Size, FileSession().FileLength());
			Assert.IsSmaller(Size + ApproximateHeaderSize, FileSession().FileLength());
			DatabaseGrowthSizeTestCase.Item item = DatabaseGrowthSizeTestCase.Item.NewItem(Size
				);
			Store(item);
			Assert.IsGreater(Size * 2, FileSession().FileLength());
			Assert.IsSmaller(Size * 2 + ApproximateHeaderSize, FileSession().FileLength());
			object retrievedItem = ((DatabaseGrowthSizeTestCase.Item)RetrieveOnlyInstance(typeof(
				DatabaseGrowthSizeTestCase.Item)));
			Assert.AreSame(item, retrievedItem);
		}

		public class Item
		{
			public byte[] _payload;

			public Item()
			{
			}

			public static DatabaseGrowthSizeTestCase.Item NewItem(int payloadSize)
			{
				DatabaseGrowthSizeTestCase.Item item = new DatabaseGrowthSizeTestCase.Item();
				item._payload = new byte[payloadSize];
				return item;
			}
		}
	}
}
