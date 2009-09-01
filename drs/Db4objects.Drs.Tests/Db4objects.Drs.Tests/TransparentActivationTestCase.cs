/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class TransparentActivationTestCase : DrsTestCase
	{
		public class Item : IActivatable
		{
			private string _name;

			[System.NonSerialized]
			private IActivator _activator;

			public Item(string name)
			{
				_name = name;
			}

			public virtual void Activate(ActivationPurpose purpose)
			{
				if (_activator != null)
				{
					_activator.Activate(purpose);
				}
			}

			public virtual void Bind(IActivator activator)
			{
				_activator = activator;
			}

			public virtual object Name()
			{
				return _name;
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Test()
		{
			TransparentActivationTestCase.Item item = new TransparentActivationTestCase.Item(
				"foo");
			A().Provider().StoreNew(item);
			Reopen();
			ReplicateAll(A().Provider(), B().Provider());
			IObjectSet items = B().Provider().GetStoredObjects(typeof(TransparentActivationTestCase.Item
				));
			Assert.IsTrue(items.HasNext());
			TransparentActivationTestCase.Item replicatedItem = (TransparentActivationTestCase.Item
				)items.Next();
			Assert.AreEqual(item.Name(), replicatedItem.Name());
		}
	}
}
