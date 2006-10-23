namespace Db4objects.Db4o.Tests.Common.Assorted
{
	/// <exclude></exclude>
	public class UUIDTestItem
	{
		public string name;

		public UUIDTestItem()
		{
		}

		public UUIDTestItem(string name)
		{
			this.name = name;
		}

		public static void AssertItemsCanBeRetrievedByUUID(Db4objects.Db4o.Ext.IExtObjectContainer
			 container, Db4objects.Db4o.Foundation.Hashtable4 uuidCache)
		{
			Db4objects.Db4o.Query.IQuery q = container.Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem));
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem item = (Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem
					)objectSet.Next();
				Db4objects.Db4o.Ext.Db4oUUID uuid = container.GetObjectInfo(item).GetUUID();
				Db4oUnit.Assert.IsNotNull(uuid);
				Db4oUnit.Assert.AreSame(item, container.GetByUUID(uuid));
				Db4objects.Db4o.Ext.Db4oUUID cached = (Db4objects.Db4o.Ext.Db4oUUID)uuidCache.Get
					(item.name);
				if (cached != null)
				{
					Db4oUnit.Assert.AreEqual(cached, uuid);
				}
				else
				{
					uuidCache.Put(item.name, uuid);
				}
			}
		}
	}
}
