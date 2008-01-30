using System.Collections.Generic;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Types
{
	class ArrayAsGenericListTestCase : AbstractDb4oTestCase
	{
		public class Item<T>
		{
			private IList<T> _list;

			public Item(IList<T> list)
			{
				_list = list;
			}

			public IList<T> List
			{
				get { return _list;  }
			}
		}

		static readonly string[] Elements = new string[] { "foo", "bar" };

		protected override void Store()
		{	
			Store(new Item<string>(Elements));
		}

		public void Test()
		{
			Item<string> item = RetrieveOnlyInstance<Item<string>>();
			ArrayAssert.AreEqual(Elements, (string[])item.List);
		}
	}
}
