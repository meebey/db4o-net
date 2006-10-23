namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	/// <exclude></exclude>
	public class DoubleFieldIndexTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase().RunSolo();
		}

		public class Item
		{
			public double value;

			public Item()
			{
			}

			public Item(double value_)
			{
				value = value_;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item)
				, "value");
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item
				(0.5));
			Db().Set(new Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item
				(1.1));
			Db().Set(new Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item
				(2));
		}

		public virtual void TestEqual()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item)
				);
			query.Descend("value").Constrain(1.1);
			AssertItems(new double[] { 1.1 }, query.Execute());
		}

		public virtual void TestGreater()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item)
				);
			Db4objects.Db4o.Query.IQuery descend = query.Descend("value");
			descend.Constrain(System.Convert.ToDouble(1)).Greater();
			descend.OrderAscending();
			AssertItems(new double[] { 1.1, 2 }, query.Execute());
		}

		private void AssertItems(double[] expected, Db4objects.Db4o.IObjectSet set)
		{
			Db4oUnit.ArrayAssert.AreEqual(expected, ToDoubleArray(set));
		}

		private double[] ToDoubleArray(Db4objects.Db4o.IObjectSet set)
		{
			double[] array = new double[set.Size()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((Db4objects.Db4o.Tests.Common.Fieldindex.DoubleFieldIndexTestCase.Item
					)set.Next()).value;
			}
			return array;
		}
	}
}
