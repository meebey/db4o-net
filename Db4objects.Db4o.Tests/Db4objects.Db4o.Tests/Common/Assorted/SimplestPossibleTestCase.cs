/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SimplestPossibleTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new SimplestPossibleTestCase().RunSolo();
		}

		protected override void Store()
		{
			Db().Set(new SimplestPossibleItem("one"));
		}

		public virtual void Test()
		{
			IQuery q = Db().Query();
			q.Constrain(typeof(SimplestPossibleItem));
			q.Descend("name").Constrain("one");
			IObjectSet objectSet = q.Execute();
			SimplestPossibleItem item = (SimplestPossibleItem)objectSet.Next();
			Assert.IsNotNull(item);
			Assert.AreEqual("one", item.GetName());
		}
	}
}
