using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class InstantiationEventsTestCase : EventsTestCaseBase
	{
		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(0);
		}

		public virtual void TestInstantiationEvents()
		{
			EventsTestCaseBase.EventLog instantiatedLog = new EventsTestCaseBase.EventLog();
			EventRegistry().Instantiated += new Db4objects.Db4o.Events.ObjectEventHandler(new 
				_AnonymousInnerClass18(this, instantiatedLog).OnEvent);
			RetrieveOnlyInstance(typeof(EventsTestCaseBase.Item));
			Assert.IsFalse(instantiatedLog.xing);
			Assert.IsTrue(instantiatedLog.xed);
		}

		private sealed class _AnonymousInnerClass18
		{
			public _AnonymousInnerClass18(InstantiationEventsTestCase _enclosing, EventsTestCaseBase.EventLog
				 instantiatedLog)
			{
				this._enclosing = _enclosing;
				this.instantiatedLog = instantiatedLog;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				instantiatedLog.xed = true;
			}

			private readonly InstantiationEventsTestCase _enclosing;

			private readonly EventsTestCaseBase.EventLog instantiatedLog;
		}
	}
}
