namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SimplestPossibleTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleTestCase().RunSolo();
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem("one"));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem));
			q.Descend("name").Constrain("one");
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem item = (Db4objects.Db4o.Tests.Common.Assorted.SimplestPossibleItem
				)objectSet.Next();
			Db4oUnit.Assert.IsNotNull(item);
			Db4oUnit.Assert.AreEqual("one", item.GetName());
		}
	}
}
