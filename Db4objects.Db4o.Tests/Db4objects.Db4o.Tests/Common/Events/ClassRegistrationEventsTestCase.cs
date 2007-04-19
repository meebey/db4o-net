using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ClassRegistrationEventsTestCase : AbstractDb4oTestCase
	{
		public class Data
		{
		}

		private class EventFlag
		{
			public bool _eventOccurred = false;
		}

		public virtual void TestClassRegistrationEvents()
		{
			ClassRegistrationEventsTestCase.EventFlag eventFlag = new ClassRegistrationEventsTestCase.EventFlag
				();
			IEventRegistry registry = EventRegistryFactory.ForObjectContainer(Db());
			registry.ClassRegistered += new Db4objects.Db4o.Events.ClassEventHandler(new _AnonymousInnerClass21
				(this, eventFlag).OnEvent);
			Store(new ClassRegistrationEventsTestCase.Data());
			Assert.IsTrue(eventFlag._eventOccurred);
		}

		private sealed class _AnonymousInnerClass21
		{
			public _AnonymousInnerClass21(ClassRegistrationEventsTestCase _enclosing, ClassRegistrationEventsTestCase.EventFlag
				 eventFlag)
			{
				this._enclosing = _enclosing;
				this.eventFlag = eventFlag;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ClassEventArgs args)
			{
				ClassEventArgs classEventArgs = (ClassEventArgs)args;
				Assert.AreEqual(typeof(ClassRegistrationEventsTestCase.Data).FullName, classEventArgs
					.ClassMetadata().GetName());
				eventFlag._eventOccurred = true;
			}

			private readonly ClassRegistrationEventsTestCase _enclosing;

			private readonly ClassRegistrationEventsTestCase.EventFlag eventFlag;
		}
	}
}
