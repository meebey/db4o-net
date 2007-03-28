namespace Db4objects.Db4o.Tests.Common.Ext
{
	public class Db4oDatabaseTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestGenerate()
		{
			Db4objects.Db4o.Ext.Db4oDatabase db1 = Db4objects.Db4o.Ext.Db4oDatabase.Generate(
				);
			Db4objects.Db4o.Ext.Db4oDatabase db2 = Db4objects.Db4o.Ext.Db4oDatabase.Generate(
				);
			Db4objects.Db4o.Ext.Db4oDatabase db3 = Db4objects.Db4o.Ext.Db4oDatabase.Generate(
				);
			Db4oUnit.Assert.AreNotEqual(db1, db2);
			Db4oUnit.Assert.AreNotEqual(db1, db3);
			Db4oUnit.Assert.AreNotEqual(db2, db3);
		}
	}
}
