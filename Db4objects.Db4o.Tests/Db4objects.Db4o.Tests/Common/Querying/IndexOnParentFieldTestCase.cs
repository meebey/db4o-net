/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	/// <exclude></exclude>
	public class IndexOnParentFieldTestCase : AbstractDb4oTestCase
	{
		public class Parent
		{
			public Parent(string name)
			{
				_name = name;
			}

			public string _name;
		}

		public class Child : IndexOnParentFieldTestCase.Parent
		{
			public Child(string name) : base(name)
			{
			}
		}

		protected override void Store()
		{
			Store(new IndexOnParentFieldTestCase.Parent("one"));
			Store(new IndexOnParentFieldTestCase.Child("one"));
		}

		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(IndexOnParentFieldTestCase.Parent)).ObjectField("_name"
				).Indexed(true);
		}

		public virtual void Test()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(IndexOnParentFieldTestCase.Child));
			q.Descend("_name").Constrain("one");
			Assert.AreEqual(1, q.Execute().Count);
		}
	}
}
