/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class DeleteEventOnClient : EventsTestCaseBase, IOptOutSolo
	{
		public static void Main(string[] args)
		{
			new DeleteEventOnClient().RunAll();
		}

		public virtual void TestAttachingToDeletingEventThrows()
		{
			if (IsMTOC())
			{
				return;
			}
			Assert.Expect(typeof(ArgumentException), new _ICodeBlock_20(this));
		}

		private sealed class _ICodeBlock_20 : ICodeBlock
		{
			public _ICodeBlock_20(DeleteEventOnClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.EventRegistry().Deleting += new Db4objects.Db4o.Events.CancellableObjectEventHandler
					(new _IEventListener4_22().OnEvent);
			}

			private sealed class _IEventListener4_22
			{
				public _IEventListener4_22()
				{
				}

				public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
					 args)
				{
				}
			}

			private readonly DeleteEventOnClient _enclosing;
		}

		public virtual void TestAttachingToDeleteEventThrows()
		{
			if (IsMTOC())
			{
				return;
			}
			Assert.Expect(typeof(ArgumentException), new _ICodeBlock_33(this));
		}

		private sealed class _ICodeBlock_33 : ICodeBlock
		{
			public _ICodeBlock_33(DeleteEventOnClient _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.EventRegistry().Deleted += new Db4objects.Db4o.Events.ObjectEventHandler
					(new _IEventListener4_35().OnEvent);
			}

			private sealed class _IEventListener4_35
			{
				public _IEventListener4_35()
				{
				}

				public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
				{
				}
			}

			private readonly DeleteEventOnClient _enclosing;
		}
	}
}
