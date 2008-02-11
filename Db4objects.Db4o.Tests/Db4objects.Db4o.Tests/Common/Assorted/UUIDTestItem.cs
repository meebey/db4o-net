/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;

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

		public static void AssertItemsCanBeRetrievedByUUID(IExtObjectContainer container, 
			Hashtable4 uuidCache)
		{
			IQuery q = container.Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem));
			IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem item = (Db4objects.Db4o.Tests.Common.Assorted.UUIDTestItem
					)objectSet.Next();
				Db4oUUID uuid = container.GetObjectInfo(item).GetUUID();
				Assert.IsNotNull(uuid);
				Assert.AreSame(item, container.GetByUUID(uuid));
				Db4oUUID cached = (Db4oUUID)uuidCache.Get(item.name);
				if (cached != null)
				{
					Assert.AreEqual(cached, uuid);
				}
				else
				{
					uuidCache.Put(item.name, uuid);
				}
			}
		}
	}
}
