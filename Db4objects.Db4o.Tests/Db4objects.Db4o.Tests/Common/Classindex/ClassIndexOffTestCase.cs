/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Classindex;

namespace Db4objects.Db4o.Tests.Common.Classindex
{
	public class ClassIndexOffTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public class Item
		{
			public string name;

			public Item(string _name)
			{
				this.name = _name;
			}
		}

		public static void Main(string[] args)
		{
			new ClassIndexOffTestCase().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(ClassIndexOffTestCase.Item)).Indexed(false);
		}

		public virtual void Test()
		{
			Db().Store(new ClassIndexOffTestCase.Item("1"));
			IStoredClass yc = Db().StoredClass(typeof(ClassIndexOffTestCase.Item));
			Assert.IsFalse(yc.HasClassIndex());
			AssertNoItemFound();
			Db().Commit();
			AssertNoItemFound();
		}

		private void AssertNoItemFound()
		{
			IQuery q = Db().Query();
			q.Constrain(typeof(ClassIndexOffTestCase.Item));
			Assert.AreEqual(0, q.Execute().Size());
		}
	}
}
