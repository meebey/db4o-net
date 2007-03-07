namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class DebugBTreeNodeMarshalledLength : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public int _int;

			public string _string;
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength().RunSolo();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength.Item)
				).ObjectField("_int").Indexed(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength.Item)
				).ObjectField("_string").Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < 50000; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength.Item(
					));
			}
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Internal.Btree.BTree btree = Btree().DebugLoadFully(SystemTrans()
				);
			Store(new Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength.Item(
				));
			btree.Write(SystemTrans());
		}

		private Db4objects.Db4o.Internal.Btree.BTree Btree()
		{
			Db4objects.Db4o.Internal.ClassMetadata clazz = Stream().GetYapClass(Reflector().ForClass
				(typeof(Db4objects.Db4o.Tests.Common.Btree.DebugBTreeNodeMarshalledLength.Item))
				);
			Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy index = clazz.Index();
			return ((Db4objects.Db4o.Internal.Classindex.BTreeClassIndexStrategy)index).Btree
				();
		}
	}
}
