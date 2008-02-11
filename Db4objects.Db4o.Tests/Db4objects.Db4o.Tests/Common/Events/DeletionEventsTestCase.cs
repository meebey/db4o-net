/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class DeletionEventsTestCase : EventsTestCaseBase
	{
		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(1);
		}

		public virtual void TestDeletionEvents()
		{
			if (IsEmbeddedClientServer())
			{
				// TODO: something wrong when embedded c/s is run as part
				// of the full test suite
				return;
			}
			EventsTestCaseBase.EventLog deletionLog = new EventsTestCaseBase.EventLog();
			ServerEventRegistry().Deleting += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_25(this, deletionLog).OnEvent);
			ServerEventRegistry().Deleted += new Db4objects.Db4o.Events.ObjectEventHandler(new 
				_IEventListener4_31(this, deletionLog).OnEvent);
			Db().Delete(RetrieveOnlyInstance(typeof(EventsTestCaseBase.Item)));
			Db().Commit();
			Assert.IsTrue(deletionLog.xing);
			Assert.IsTrue(deletionLog.xed);
		}

		private sealed class _IEventListener4_25
		{
			public _IEventListener4_25(DeletionEventsTestCase _enclosing, EventsTestCaseBase.EventLog
				 deletionLog)
			{
				this._enclosing = _enclosing;
				this.deletionLog = deletionLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				deletionLog.xing = true;
				this._enclosing.AssertItemIsActive(args);
			}

			private readonly DeletionEventsTestCase _enclosing;

			private readonly EventsTestCaseBase.EventLog deletionLog;
		}

		private sealed class _IEventListener4_31
		{
			public _IEventListener4_31(DeletionEventsTestCase _enclosing, EventsTestCaseBase.EventLog
				 deletionLog)
			{
				this._enclosing = _enclosing;
				this.deletionLog = deletionLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				deletionLog.xed = true;
				this._enclosing.AssertItemIsActive(args);
			}

			private readonly DeletionEventsTestCase _enclosing;

			private readonly EventsTestCaseBase.EventLog deletionLog;
		}

		private void AssertItemIsActive(EventArgs args)
		{
			Assert.AreEqual(1, ItemForEvent(args).id);
		}

		private EventsTestCaseBase.Item ItemForEvent(EventArgs args)
		{
			return ((EventsTestCaseBase.Item)((ObjectEventArgs)args).Object);
		}
	}
}
