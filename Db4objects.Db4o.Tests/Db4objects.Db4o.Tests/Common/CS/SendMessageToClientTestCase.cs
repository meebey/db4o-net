using Db4oUnit;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SendMessageToClientTestCase : ClientServerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new SendMessageToClientTestCase().RunClientServer();
		}

		public virtual void Test()
		{
			ServerDispatcher().Write(Msg.OK);
			Msg msg = Client().GetResponse();
			Assert.AreEqual(Msg.OK, msg);
		}
	}
}
