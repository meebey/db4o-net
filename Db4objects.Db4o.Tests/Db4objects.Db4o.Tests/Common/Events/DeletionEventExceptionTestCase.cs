/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Events;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class DeletionEventExceptionTestCase : EventsTestCaseBase, IOptOutSolo
	{
		public static void Main(string[] args)
		{
			new DeletionEventExceptionTestCase().RunAll();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ActivationDepth(1);
		}

		public virtual void TestDeletionEvents()
		{
			ServerEventRegistry().Deleting += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_25().OnEvent);
			object item = RetrieveOnlyInstance(typeof(EventsTestCaseBase.Item));
			if (IsMTOC())
			{
				Assert.Expect(typeof(ReflectException), new _ICodeBlock_32(this, item));
			}
			else
			{
				Db().Delete(item);
			}
			Db().Commit();
		}

		private sealed class _IEventListener4_25
		{
			public _IEventListener4_25()
			{
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				throw new Exception();
			}
		}

		private sealed class _ICodeBlock_32 : ICodeBlock
		{
			public _ICodeBlock_32(DeletionEventExceptionTestCase _enclosing, object item)
			{
				this._enclosing = _enclosing;
				this.item = item;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Delete(item);
			}

			private readonly DeletionEventExceptionTestCase _enclosing;

			private readonly object item;
		}
	}
}
