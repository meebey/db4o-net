using Db4objects.Db4o.Ext;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.NativeQueries
{
#if NET_2_0
	using System.Collections.Generic;

	public class ListElementByIdentity : AbstractDb4oTestCase
	{
		IList<LebiElement> _list;

		override protected void Store()
		{
			StoreElement("1");
			StoreElement("2");
			StoreElement("3");
			StoreElement("4");
		}

		public void Test()
		{
			LebiElement elem = (LebiElement)Db().Get(new LebiElement("23"))[0];

			IList<ListElementByIdentity> res = Db().Query<ListElementByIdentity>(delegate(ListElementByIdentity lebi)
			{
				return lebi._list.Contains(elem);
			});

			Assert.AreEqual(1, res.Count);
			Assert.AreEqual("23", res[0]._list[3]._name);

		}

		private void StoreElement(string prefix)
		{
			ListElementByIdentity lebi = new ListElementByIdentity();
			lebi.CreateListElements(prefix);
			Store(lebi);
		}

		private void CreateListElements(string prefix)
		{
			_list = new List<LebiElement>();
			_list.Add(new LebiElement(prefix + "0"));
			_list.Add(new LebiElement(prefix + "1"));
			_list.Add(new LebiElement(prefix + "2"));
			_list.Add(new LebiElement(prefix + "3"));
		}

	}

	public class LebiElement
	{
		public string _name;

		public LebiElement(string name)
		{
			_name = name;
		}
	}
#endif

}
