/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class CascadeDeleteFalseTestCase : Db4oClientServerTestCase
	{
		public static void Main(string[] args)
		{
			new CascadeDeleteFalseTestCase().RunConcurrency();
		}

		public class Item
		{
			public CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper h1;

			public CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper h2;

			public CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper h3;
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(CascadeDeleteFalseTestCase.Item)).CascadeOnDelete(true);
			config.ObjectClass(typeof(CascadeDeleteFalseTestCase.Item)).ObjectField("h3").CascadeOnDelete
				(false);
		}

		protected override void Store()
		{
			CascadeDeleteFalseTestCase.Item item = new CascadeDeleteFalseTestCase.Item();
			item.h1 = new CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper();
			item.h2 = new CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper();
			item.h3 = new CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper();
			Store(item);
		}

		/// <exception cref="Exception"></exception>
		public virtual void ConcDelete(IExtObjectContainer oc)
		{
			IObjectSet os = oc.Query(typeof(CascadeDeleteFalseTestCase.Item));
			if (os.Size() == 0)
			{
				return;
			}
			if (!os.HasNext())
			{
				return;
			}
			CascadeDeleteFalseTestCase.Item cdf = (CascadeDeleteFalseTestCase.Item)os.Next();
			oc.Delete(cdf);
			oc.Commit();
			AssertOccurrences(oc, typeof(CascadeDeleteFalseTestCase.Item), 0);
			AssertOccurrences(oc, typeof(CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper)
				, 1);
		}

		public virtual void CheckDelete(IExtObjectContainer oc)
		{
			AssertOccurrences(oc, typeof(CascadeDeleteFalseTestCase), 0);
			AssertOccurrences(oc, typeof(CascadeDeleteFalseTestCase.CascadeDeleteFalseHelper)
				, 1);
		}

		public class CascadeDeleteFalseHelper
		{
		}
	}
}
