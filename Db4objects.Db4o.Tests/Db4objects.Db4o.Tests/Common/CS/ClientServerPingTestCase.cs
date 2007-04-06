using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Tests.Common.CS;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.CS
{
	/// <exclude></exclude>
	public class ClientServerPingTestCase : ClientServerTestCaseBase
	{
		private const int ITEM_COUNT = 100;

		public static void Main(string[] arguments)
		{
			new ClientServerPingTestCase().RunClientServer();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ClientServer().BatchMessages(false);
		}

		public virtual void Test()
		{
			IServerMessageDispatcher dispatcher = ServerDispatcher();
			ClientServerPingTestCase.PingThread pingThread = new ClientServerPingTestCase.PingThread
				(dispatcher);
			pingThread.Start();
			for (int i = 0; i < ITEM_COUNT; i++)
			{
				ClientServerPingTestCase.Item item = new ClientServerPingTestCase.Item(i);
				Store(item);
			}
			Assert.AreEqual(ITEM_COUNT, Db().Get(typeof(ClientServerPingTestCase.Item)).Size(
				));
			pingThread.Close();
		}

		public class Item
		{
			public int data;

			public Item(int i)
			{
				data = i;
			}
		}

		internal class PingThread : Thread
		{
			internal IServerMessageDispatcher _dispatcher;

			internal bool _stop;

			public PingThread(IServerMessageDispatcher dispatcher)
			{
				_dispatcher = dispatcher;
			}

			public virtual void Close()
			{
				_stop = true;
			}

			public override void Run()
			{
				while (!_stop)
				{
					_dispatcher.Write(Msg.PING);
					Cool.SleepIgnoringInterruption(1);
				}
			}
		}
	}
}
