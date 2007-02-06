namespace Db4objects.Db4o.Tests.Common.Interfaces
{
	public class InterfaceTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Interfaces.ThreeSomeParent());
			Store(new Db4objects.Db4o.Tests.Common.Interfaces.ThreeSomeLeftChild());
			Store(new Db4objects.Db4o.Tests.Common.Interfaces.ThreeSomeRightChild());
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Interfaces.IThreeSomeInterface)
				);
			Db4oUnit.Assert.AreEqual(2, q.Execute().Size());
		}
	}
}
