/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class DispatchPendingMessagesTestCase : MessagingTestCaseBase
	{
		public virtual void TestReturnsImmediatelyWithNoMessages()
		{
			IObjectServer server = OpenServer(MemoryIoConfiguration());
			try
			{
				IObjectContainer client = OpenClient("client", server);
				try
				{
					AutoStopWatch watch = new AutoStopWatch();
					((IExtClient)client).DispatchPendingMessages(long.MaxValue);
					Assert.IsTrue(watch.Peek() < 100);
				}
				finally
				{
					client.Close();
				}
			}
			finally
			{
				server.Close();
			}
		}
	}
}
