namespace Db4objects.Db4o.Tests.Common.CS
{
	public class SendMessageToClientTestCase : Db4objects.Db4o.Tests.Common.CS.ClientServerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.CS.SendMessageToClientTestCase().RunClientServer
				();
		}

		public virtual void Test()
		{
			ServerDispatcher().Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OK);
			Db4objects.Db4o.Internal.CS.Messages.Msg msg = Client().GetResponse();
			Db4oUnit.Assert.AreEqual(Db4objects.Db4o.Internal.CS.Messages.Msg.OK, msg);
		}
	}
}
