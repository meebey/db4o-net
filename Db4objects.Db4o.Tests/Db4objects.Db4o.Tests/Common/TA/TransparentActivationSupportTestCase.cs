/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Events;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA
{
	public partial class TransparentActivationSupportTestCase : TransparentActivationTestCaseBase
	{
		public static void Main(string[] args)
		{
			new TransparentActivationSupportTestCase().RunAll();
		}

		public virtual void TestActivationDepth()
		{
			Assert.IsInstanceOf(typeof(TransparentActivationDepthProviderImpl), Stream().ConfigImpl
				().ActivationDepthProvider());
		}

		public sealed partial class Item : ActivatableImpl
		{
			public void Update()
			{
				this.Activate(ActivationPurpose.Write);
			}

			internal Item(TransparentActivationSupportTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly TransparentActivationSupportTestCase _enclosing;
		}

		public virtual void TestTransparentActivationDoesNotImplyTransparentUpdate()
		{
			TransparentActivationSupportTestCase.Item item = new TransparentActivationSupportTestCase.Item
				(this);
			Db().Store(item);
			Db().Commit();
			item.Update();
			Collection4 updated = CommitCapturingUpdatedObjects(Db());
			Assert.AreEqual(0, updated.Size());
		}

		private Collection4 CommitCapturingUpdatedObjects(IExtObjectContainer container)
		{
			Collection4 updated = new Collection4();
			EventRegistryFor(container).Updated += new Db4objects.Db4o.Events.ObjectEventHandler
				(new _IEventListener4_54(updated).OnEvent);
			container.Commit();
			return updated;
		}

		private sealed class _IEventListener4_54
		{
			public _IEventListener4_54(Collection4 updated)
			{
				this.updated = updated;
			}

			public void OnEvent(object sender, Db4objects.Db4o.Events.ObjectEventArgs args)
			{
				ObjectEventArgs objectArgs = (ObjectEventArgs)args;
				updated.Add(objectArgs.Object);
			}

			private readonly Collection4 updated;
		}
	}
}
