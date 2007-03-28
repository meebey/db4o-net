namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientServerTestCaseBase : Db4oUnit.Extensions.AbstractDb4oTestCase, 
		Db4oUnit.Extensions.Fixtures.IOptOutSolo
	{
		protected virtual Db4objects.Db4o.Internal.CS.IServerMessageDispatcher ServerDispatcher
			()
		{
			Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture fixture = (Db4oUnit.Extensions.Fixtures.AbstractClientServerDb4oFixture
				)Fixture();
			Db4objects.Db4o.Internal.CS.ObjectServerImpl serverImpl = (Db4objects.Db4o.Internal.CS.ObjectServerImpl
				)fixture.Server();
			System.Collections.IEnumerator iter = serverImpl.IterateDispatchers();
			iter.MoveNext();
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher dispatcher = (Db4objects.Db4o.Internal.CS.IServerMessageDispatcher
				)iter.Current;
			return dispatcher;
		}

		protected virtual Db4objects.Db4o.Internal.CS.ClientObjectContainer Client()
		{
			return (Db4objects.Db4o.Internal.CS.ClientObjectContainer)Db();
		}
	}
}
