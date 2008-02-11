/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class DebugBTreeNodeMarshalledLength : AbstractDb4oTestCase
	{
		public class Item
		{
			public int _int;

			public string _string;
		}

		public static void Main(string[] args)
		{
			new DebugBTreeNodeMarshalledLength().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(DebugBTreeNodeMarshalledLength.Item)).ObjectField("_int"
				).Indexed(true);
			config.ObjectClass(typeof(DebugBTreeNodeMarshalledLength.Item)).ObjectField("_string"
				).Indexed(true);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			for (int i = 0; i < 50000; i++)
			{
				Store(new DebugBTreeNodeMarshalledLength.Item());
			}
		}

		public virtual void Test()
		{
			BTree btree = Btree().DebugLoadFully(SystemTrans());
			Store(new DebugBTreeNodeMarshalledLength.Item());
			btree.Write(SystemTrans());
		}

		private BTree Btree()
		{
			IClassIndexStrategy index = ClassMetadataFor(typeof(DebugBTreeNodeMarshalledLength.Item
				)).Index();
			return ((BTreeClassIndexStrategy)index).Btree();
		}
	}
}
