/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ActivationEventsTestCase : EventsTestCaseBase
	{
		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(1);
		}

		public virtual void TestActivationEvents()
		{
			EventsTestCaseBase.EventLog activationLog = new EventsTestCaseBase.EventLog();
			EventRegistry().Activating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_19(activationLog).OnEvent);
			EventRegistry().Activated += new Db4objects.Db4o.Events.ObjectEventHandler(new _IEventListener4_24
				(activationLog).OnEvent);
			RetrieveOnlyInstance(typeof(EventsTestCaseBase.Item));
			Assert.IsTrue(activationLog.xing);
			Assert.IsTrue(activationLog.xed);
		}

		private sealed class _IEventListener4_19
		{
			public _IEventListener4_19(EventsTestCaseBase.EventLog activationLog)
			{
				this.activationLog = activationLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				activationLog.xing = true;
			}

			private readonly EventsTestCaseBase.EventLog activationLog;
		}

		private sealed class _IEventListener4_24
		{
			public _IEventListener4_24(EventsTestCaseBase.EventLog activationLog)
			{
				this.activationLog = activationLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				activationLog.xed = true;
			}

			private readonly EventsTestCaseBase.EventLog activationLog;
		}
	}
}
