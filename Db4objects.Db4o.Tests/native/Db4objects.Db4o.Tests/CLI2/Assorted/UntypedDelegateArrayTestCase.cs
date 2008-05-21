using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
	class UntypedDelegateArrayTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public Action<string> typed;
			public Action<string> untyped;
			public Action<string>[] typedArray;
			public object[] untypedArray;

			public Item(System.Action<string> action)
			{
				typed = action;
				untyped = action;
				typedArray = new System.Action<string>[] {action};
				untypedArray = new object[] {action};
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ExceptionsOnNotStorable(true);
		}

		protected override void Store()
		{
			Store(new Item(StringAction));
		}

		public void Test()
		{
			Item item = RetrieveOnlyInstance<Item>();
			Assert.IsNull(item.typed);
			Assert.IsNull(item.untyped);
			ArrayAssert.AreEqual(new object[1], item.untypedArray);
			Assert.IsNull(item.typedArray);
		}

		private static void StringAction(string s)
		{
			throw new NotImplementedException();
		}
	}
}
