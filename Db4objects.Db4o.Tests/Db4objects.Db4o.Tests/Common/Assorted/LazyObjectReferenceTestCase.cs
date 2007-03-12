namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class LazyObjectReferenceTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase().RunSolo();
		}

		public class Item
		{
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item)
				).GenerateUUIDs(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < 10; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item(
					));
			}
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item)
				);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			long[] ids = objectSet.Ext().GetIDs();
			Db4objects.Db4o.Ext.IObjectInfo[] infos = new Db4objects.Db4o.Ext.IObjectInfo[ids
				.Length];
			Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item[] items = 
				new Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item[ids.Length
				];
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = (Db4objects.Db4o.Tests.Common.Assorted.LazyObjectReferenceTestCase.Item
					)Db().GetByID(ids[i]);
				infos[i] = new Db4objects.Db4o.Internal.LazyObjectReference(Stream(), (int)ids[i]
					);
			}
			AssertInfosAreConsistent(ids, infos);
			for (int i = 0; i < items.Length; i++)
			{
				Db().Purge(items[i]);
			}
			Db().Purge();
			AssertInfosAreConsistent(ids, infos);
		}

		private void AssertInfosAreConsistent(long[] ids, Db4objects.Db4o.Ext.IObjectInfo[]
			 infos)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				Db4objects.Db4o.Ext.IObjectInfo info = Db().GetObjectInfo(Db().GetByID(ids[i]));
				Db4oUnit.Assert.AreEqual(info.GetInternalID(), infos[i].GetInternalID());
				Db4oUnit.Assert.AreEqual(info.GetUUID().GetLongPart(), infos[i].GetUUID().GetLongPart
					());
				Db4oUnit.Assert.AreSame(info.GetObject(), infos[i].GetObject());
			}
		}
	}
}
