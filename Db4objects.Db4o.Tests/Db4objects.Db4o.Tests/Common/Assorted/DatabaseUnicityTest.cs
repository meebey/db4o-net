namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DatabaseUnicityTest : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public virtual void Test()
		{
			Db4objects.Db4o.Foundation.Hashtable4 ht = new Db4objects.Db4o.Foundation.Hashtable4
				();
			Db4objects.Db4o.Ext.IExtObjectContainer oc = Db();
			Db4objects.Db4o.YapStream yapStream = ((Db4objects.Db4o.YapStream)oc);
			yapStream.ShowInternalClasses(true);
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Ext.Db4oDatabase));
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Ext.Db4oDatabase d4b = (Db4objects.Db4o.Ext.Db4oDatabase)objectSet
					.Next();
				Db4oUnit.Assert.IsFalse(ht.ContainsKey(d4b.i_signature));
				ht.Put(d4b.i_signature, string.Empty);
			}
			yapStream.ShowInternalClasses(false);
			oc.Close();
		}
	}
}
