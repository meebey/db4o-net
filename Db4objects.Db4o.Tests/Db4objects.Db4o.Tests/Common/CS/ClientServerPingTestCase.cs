namespace Db4objects.Db4o.Tests.Common.CS
{
	/// <exclude></exclude>
	public class ClientServerPingTestCase : Db4objects.Db4o.Tests.Common.CS.ClientServerTestCaseBase
	{
		private const int ITEM_COUNT = 100;

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase().RunClientServer();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ClientServer().BatchMessages(false);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Internal.CS.IServerMessageDispatcher dispatcher = ServerDispatcher
				();
			Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase.PingThread pingThread = 
				new Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase.PingThread(dispatcher
				);
			pingThread.Start();
			for (int i = 0; i < ITEM_COUNT; i++)
			{
				Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase.Item item = new Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase.Item
					(i);
				Store(item);
			}
			Db4oUnit.Assert.AreEqual(ITEM_COUNT, Db().Get(typeof(Db4objects.Db4o.Tests.Common.CS.ClientServerPingTestCase.Item)
				).Size());
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

		internal class PingThread : Sharpen.Lang.Thread
		{
			internal Db4objects.Db4o.Internal.CS.IServerMessageDispatcher _dispatcher;

			internal bool _stop;

			public PingThread(Db4objects.Db4o.Internal.CS.IServerMessageDispatcher dispatcher
				)
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
					_dispatcher.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.PING);
					Db4objects.Db4o.Foundation.Cool.SleepIgnoringInterruption(1);
				}
			}
		}
	}
}
