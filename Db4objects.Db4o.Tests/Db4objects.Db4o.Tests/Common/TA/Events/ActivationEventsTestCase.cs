/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA;
using Db4objects.Db4o.Tests.Common.TA.Events;

namespace Db4objects.Db4o.Tests.Common.TA.Events
{
	public class ActivationEventsTestCase : TransparentActivationTestCaseBase
	{
		public class NonActivatableItem
		{
			public string name;

			public NonActivatableItem(string name_)
			{
				name = name_;
			}

			public NonActivatableItem()
			{
			}
		}

		public class ActivatableItem : IActivatable
		{
			public string name;

			public ActivationEventsTestCase.NonActivatableItem child;

			[System.NonSerialized]
			private IActivator _activator;

			public ActivatableItem(string name_, ActivationEventsTestCase.NonActivatableItem 
				child_)
			{
				name = name_;
				child = child_;
			}

			public ActivatableItem()
			{
			}

			public virtual void Activate(ActivationPurpose purpose)
			{
				_activator.Activate(purpose);
			}

			public virtual void Bind(IActivator activator)
			{
				_activator = activator;
			}
		}

		/// <exception cref="Exception"></exception>
		protected override void Store()
		{
			ActivationEventsTestCase.NonActivatableItem nonActivatable = new ActivationEventsTestCase.NonActivatableItem
				("Eric Idle");
			Store(nonActivatable);
			Store(new ActivationEventsTestCase.ActivatableItem("John Cleese", nonActivatable)
				);
		}

		public virtual void TestActivatingCancelNonActivatableDepth0()
		{
			AddCancelAnyListener();
			ActivationEventsTestCase.NonActivatableItem item = QueryNonActivatableItem();
			Assert.IsNull(item.name);
		}

		public virtual void TestActivatingCancelActivatableDepth0()
		{
			AddCancelAnyListener();
			ActivationEventsTestCase.ActivatableItem item = QueryActivatableItem();
			item.Activate(ActivationPurpose.READ);
			Assert.IsNull(item.name);
		}

		public virtual void TestActivatingCancelDepth1()
		{
			AddCancelNonActivatableListener();
			ActivationEventsTestCase.ActivatableItem item = QueryActivatableItem();
			item.Activate(ActivationPurpose.READ);
			Assert.IsNotNull(item.name);
			Assert.IsNotNull(item.child);
			Assert.IsNull(item.child.name);
		}

		private void AddCancelNonActivatableListener()
		{
			EventRegistry().Activating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_79(this).OnEvent);
		}

		private sealed class _IEventListener4_79
		{
			public _IEventListener4_79(ActivationEventsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				object obj = ((ObjectEventArgs)args).Object;
				if (obj is ActivationEventsTestCase.NonActivatableItem)
				{
					((ICancellableEventArgs)args).Cancel();
				}
			}

			private readonly ActivationEventsTestCase _enclosing;
		}

		private void AddCancelAnyListener()
		{
			EventRegistry().Activating += new Db4objects.Db4o.Events.CancellableObjectEventHandler
				(new _IEventListener4_90(this).OnEvent);
		}

		private sealed class _IEventListener4_90
		{
			public _IEventListener4_90(ActivationEventsTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.CancellableObjectEventArgs
				 args)
			{
				((ICancellableEventArgs)args).Cancel();
			}

			private readonly ActivationEventsTestCase _enclosing;
		}

		private ActivationEventsTestCase.NonActivatableItem QueryNonActivatableItem()
		{
			return (ActivationEventsTestCase.NonActivatableItem)RetrieveOnlyInstance(typeof(ActivationEventsTestCase.NonActivatableItem
				));
		}

		private ActivationEventsTestCase.ActivatableItem QueryActivatableItem()
		{
			return (ActivationEventsTestCase.ActivatableItem)RetrieveOnlyInstance(typeof(ActivationEventsTestCase.ActivatableItem
				));
		}
	}
}
