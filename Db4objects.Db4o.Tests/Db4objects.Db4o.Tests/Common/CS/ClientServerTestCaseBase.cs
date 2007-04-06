using System.Collections;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Internal.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class ClientServerTestCaseBase : AbstractDb4oTestCase, IOptOutSolo
	{
		protected virtual IServerMessageDispatcher ServerDispatcher()
		{
			AbstractClientServerDb4oFixture fixture = (AbstractClientServerDb4oFixture)Fixture
				();
			ObjectServerImpl serverImpl = (ObjectServerImpl)fixture.Server();
			IEnumerator iter = serverImpl.IterateDispatchers();
			iter.MoveNext();
			IServerMessageDispatcher dispatcher = (IServerMessageDispatcher)iter.Current;
			return dispatcher;
		}

		protected virtual ClientObjectContainer Client()
		{
			return (ClientObjectContainer)Db();
		}
	}
}
