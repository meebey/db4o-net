namespace Db4objects.Db4o.Tests.Common.Soda.Experiments
{
	public class STNullOnPathTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public bool @bool;

		public STNullOnPathTestCase()
		{
		}

		public STNullOnPathTestCase(bool @bool)
		{
			this.@bool = @bool;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Experiments.STNullOnPathTestCase
				(false) };
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Experiments.STNullOnPathTestCase
				());
			q.Descend("bool").Constrain(null);
			Expect(q, new int[] {  });
		}
	}
}
