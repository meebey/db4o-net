/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ExceptionPropagationInEventsTestUnit : EventsTestCaseBase
	{
		public ExceptionPropagationInEventsTestUnit()
		{
			_eventFirer["insert"] = NewObjectInserter();
			_eventFirer["query"] = NewQueryRunner();
			_eventFirer["update"] = NewObjectUpdater();
			_eventFirer["delete"] = NewObjectDeleter();
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new EventsTestCaseBase.Item(1));
			Store(new EventsTestCaseBase.Item(2));
		}

		public virtual void TestEvents()
		{
			EventInfo @event = (EventInfo)ExceptionPropagationInEventsTestVariables.EventSelector
				.Value;
			AssertEventThrows(((ICodeBlock)_eventFirer[@event.EventFirerName()]), @event.ListenerSetter
				());
		}

		private void AssertEventThrows(ICodeBlock codeBlock, IProcedure4 listenerSetter)
		{
			IEventRegistry eventRegistry = EventRegistryFactory.ForObjectContainer(Db());
			listenerSetter.Apply(eventRegistry);
			Assert.Expect(typeof(EventException), typeof(NotImplementedException), codeBlock);
		}

		private ICodeBlock NewObjectUpdater()
		{
			return new _ICodeBlock_39(this);
		}

		private sealed class _ICodeBlock_39 : ICodeBlock
		{
			public _ICodeBlock_39(ExceptionPropagationInEventsTestUnit _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				EventsTestCaseBase.Item item = this._enclosing.RetrieveItem(1);
				item.id = 10;
				this._enclosing.Db().Store(item);
				this._enclosing.Db().Commit();
			}

			private readonly ExceptionPropagationInEventsTestUnit _enclosing;
		}

		private ICodeBlock NewObjectDeleter()
		{
			return new _ICodeBlock_53(this);
		}

		private sealed class _ICodeBlock_53 : ICodeBlock
		{
			public _ICodeBlock_53(ExceptionPropagationInEventsTestUnit _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Delete(this._enclosing.RetrieveItem(1));
				this._enclosing.Db().Commit();
			}

			private readonly ExceptionPropagationInEventsTestUnit _enclosing;
		}

		private ICodeBlock NewQueryRunner()
		{
			return new _ICodeBlock_62(this);
		}

		private sealed class _ICodeBlock_62 : ICodeBlock
		{
			public _ICodeBlock_62(ExceptionPropagationInEventsTestUnit _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.RetrieveItem(1);
			}

			private readonly ExceptionPropagationInEventsTestUnit _enclosing;
		}

		private ICodeBlock NewObjectInserter()
		{
			return new _ICodeBlock_70(this);
		}

		private sealed class _ICodeBlock_70 : ICodeBlock
		{
			public _ICodeBlock_70(ExceptionPropagationInEventsTestUnit _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="System.Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Store(new EventsTestCaseBase.Item());
				this._enclosing.Db().Commit();
			}

			private readonly ExceptionPropagationInEventsTestUnit _enclosing;
		}

		private EventsTestCaseBase.Item RetrieveItem(int id)
		{
			IQuery query = NewQuery(typeof(EventsTestCaseBase.Item));
			query.Descend("id").Constrain(id);
			IObjectSet results = query.Execute();
			Assert.AreEqual(1, results.Count);
			return ((EventsTestCaseBase.Item)results.Next());
		}

		private Hashtable _eventFirer = new Hashtable();
	}
}
