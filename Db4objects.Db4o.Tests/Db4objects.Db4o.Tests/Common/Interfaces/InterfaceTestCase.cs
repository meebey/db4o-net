/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Interfaces;

namespace Db4objects.Db4o.Tests.Common.Interfaces
{
	public class InterfaceTestCase : AbstractDb4oTestCase
	{
		protected override void Store()
		{
			Store(new ThreeSomeParent());
			Store(new ThreeSomeLeftChild());
			Store(new ThreeSomeRightChild());
		}

		public virtual void Test()
		{
			IQuery q = NewQuery(typeof(IThreeSomeInterface));
			Assert.AreEqual(2, q.Execute().Size());
		}
	}
}
