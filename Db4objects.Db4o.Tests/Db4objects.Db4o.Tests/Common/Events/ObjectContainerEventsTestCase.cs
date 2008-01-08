/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ObjectContainerEventsTestCase : AbstractDb4oTestCase
	{
		private class EventFlag
		{
			public bool eventOccurred = false;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestClassRegistrationEvents()
		{
			ObjectContainerEventsTestCase.EventFlag eventFlag = new ObjectContainerEventsTestCase.EventFlag
				();
			EventRegistry().Closing += new Db4objects.Db4o.Events.ObjectContainerEventHandler
				(new _IEventListener4_18(this, eventFlag).OnEvent);
			Fixture().Close();
			Assert.IsTrue(eventFlag.eventOccurred);
		}

		private sealed class _IEventListener4_18
		{
			public _IEventListener4_18(ObjectContainerEventsTestCase _enclosing, ObjectContainerEventsTestCase.EventFlag
				 eventFlag)
			{
				this._enclosing = _enclosing;
				this.eventFlag = eventFlag;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
				 args)
			{
				eventFlag.eventOccurred = true;
			}

			private readonly ObjectContainerEventsTestCase _enclosing;

			private readonly ObjectContainerEventsTestCase.EventFlag eventFlag;
		}
	}
}
