using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class CsDisposableTestCase : AbstractDb4oTestCase
	{
		public void TestDispose()
		{
			Assert.IsTrue(!Db().IsClosed());
			(Db() as System.IDisposable).Dispose();
			Assert.IsTrue(Db().IsClosed());
		}
	}
}
