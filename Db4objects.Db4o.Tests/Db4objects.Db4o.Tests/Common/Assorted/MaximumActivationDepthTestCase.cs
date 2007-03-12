namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class MaximumActivationDepthTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Data
		{
			public int _id;

			public Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data 
				_prev;

			public Data(int id, Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data
				 prev)
			{
				_id = id;
				_prev = prev;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ActivationDepth(int.MaxValue);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data)
				).MaximumActivationDepth(1);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data data = 
				new Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data(2, 
				null);
			data = new Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data
				(1, data);
			data = new Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data
				(0, data);
			Store(data);
		}

		public virtual void TestActivationRestricted()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data)
				);
			query.Descend("_id").Constrain(0);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			Db4oUnit.Assert.AreEqual(1, result.Size());
			Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data data = 
				(Db4objects.Db4o.Tests.Common.Assorted.MaximumActivationDepthTestCase.Data)result
				.Next();
			Db4oUnit.Assert.IsNotNull(data._prev);
			Db4oUnit.Assert.IsNull(data._prev._prev);
		}
	}
}
