/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Fieldindex;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class StringIndexWithSuperClassTestCase : AbstractDb4oTestCase
	{
		private static readonly string FIELD_NAME = "_name";

		private static readonly string FIELD_VALUE = "test";

		public class ItemParent
		{
			public int _id;
		}

		public class Item : StringIndexWithSuperClassTestCase.ItemParent
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(StringIndexWithSuperClassTestCase.Item)).ObjectField(FIELD_NAME
				).Indexed(true);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			Store(new StringIndexWithSuperClassTestCase.Item(FIELD_VALUE));
			Store(new StringIndexWithSuperClassTestCase.Item(FIELD_VALUE + "X"));
		}

		public virtual void TestIndexAccess()
		{
			IQuery query = NewQuery(typeof(StringIndexWithSuperClassTestCase.Item));
			query.Descend(FIELD_NAME).Constrain(FIELD_VALUE);
			Assert.AreEqual(1, query.Execute().Size());
		}
	}
}
