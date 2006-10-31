using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class CsStructsRegression : AbstractDb4oTestCase
	{
		override protected void Store()
		{
			Store(new Item());
			Store(new Item(1));
			Store(new Item(2));
		}

		public void TestConstrainOnNullableValue()
		{
			CheckQueryById(1);
			CheckQueryById(2);
		}

		private void CheckQueryById(int id)
		{
			IObjectSet os = QueryById(id);
			Assert.AreEqual(1, os.Size());
			Assert.AreEqual(id, ((Item)os.Next()).Id);
		}

		private IObjectSet QueryById(int id)
		{
            IQuery q = NewQuery(typeof(Item));
			q.Descend("_id").Descend("_value").Constrain(id);
			return q.Execute();
		}
	}

	public class Item
	{
		NullableInt32 _id;

		public Item(int id)
		{
			_id = new NullableInt32(id);
		}

		public Item()
		{	
		}

		public int Id
		{
			get
			{
				return _id.Value;
			}
		}
	}

	public struct NullableInt32
	{
		private int _value;
		private bool _hasValue;

		public NullableInt32(int value)
		{
			_value = value;
			_hasValue = true;
		}

		public int Value
		{
			get { return _value; }
		}

		public bool HasValue
		{
			get { return _hasValue; }
		}
	}
}
