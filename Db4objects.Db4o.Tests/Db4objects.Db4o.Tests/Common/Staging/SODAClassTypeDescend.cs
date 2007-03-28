namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class SODAClassTypeDescend : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class DataA
		{
			public Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataB _val;
		}

		public class DataB
		{
			public Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataA _val;
		}

		public class DataC
		{
			public Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataC _next;
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataA objectA = new Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataA
				();
			Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataB objectB = new Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataB
				();
			objectA._val = objectB;
			objectB._val = objectA;
			Store(objectB);
			Store(new Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataC());
		}

		public virtual void TestFieldConstrainedToType()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery();
			query.Descend("_val").Constrain(typeof(Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataA)
				);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			Db4oUnit.Assert.IsInstanceOf(typeof(Db4objects.Db4o.Tests.Common.Staging.SODAClassTypeDescend.DataB)
				, result.Next());
		}
	}
}
