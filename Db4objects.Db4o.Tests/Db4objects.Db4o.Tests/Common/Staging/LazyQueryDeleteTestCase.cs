/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class LazyQueryDeleteTestCase : AbstractDb4oTestCase
	{
		private const int COUNT = 3;

		public class Item
		{
			public string _name;

			public Item(string name)
			{
				_name = name;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.Queries().EvaluationMode(QueryEvaluationMode.LAZY);
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			for (int i = 0; i < COUNT; i++)
			{
				Store(new LazyQueryDeleteTestCase.Item(i.ToString()));
				Db().Commit();
			}
		}

		public virtual void Test()
		{
			IObjectSet objectSet = NewQuery(typeof(LazyQueryDeleteTestCase.Item)).Execute();
			for (int i = 0; i < COUNT; i++)
			{
				Db().Delete(objectSet.Next());
				Db().Commit();
			}
		}

		public static void Main(string[] arguments)
		{
			new LazyQueryDeleteTestCase().RunSolo();
		}
	}
}
