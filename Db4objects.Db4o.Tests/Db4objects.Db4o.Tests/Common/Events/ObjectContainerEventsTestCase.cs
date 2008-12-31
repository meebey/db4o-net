/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Events
{
	public class ObjectContainerEventsTestCase : AbstractDb4oTestCase
	{
		/// <exception cref="System.Exception"></exception>
		public virtual void TestClose()
		{
			IExtObjectContainer container = Db();
			LocalObjectContainer session = FileSession();
			Collection4 actual = new Collection4();
			EventRegistry().Closing += new Db4objects.Db4o.Events.ObjectContainerEventHandler
				(new _IEventListener4_20(actual).OnEvent);
			Fixture().Close();
			if (IsEmbeddedClientServer())
			{
				Iterator4Assert.AreEqual(new object[] { container, session }, actual.GetEnumerator
					());
			}
			else
			{
				Assert.AreSame(container, actual.SingleElement());
			}
		}

		private sealed class _IEventListener4_20
		{
			public _IEventListener4_20(Collection4 actual)
			{
				this.actual = actual;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectContainerEventArgs
				 args)
			{
				actual.Add(((ObjectContainerEventArgs)args).ObjectContainer);
			}

			private readonly Collection4 actual;
		}
	}
}
