/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Jre5.Collections;

namespace Db4objects.Db4o.Tests.Jre5.Collections
{
	public class Order : IActivatable
	{
		private ArrayList4<OrderItem> _items;

		public Order()
		{
			_items = new ArrayList4<OrderItem>();
		}

		public virtual void AddItem(OrderItem item)
		{
			Activate();
			_items.Add(item);
		}

		public virtual OrderItem Item(int i)
		{
			Activate();
			return _items.Get(i);
		}

		public virtual int Size()
		{
			Activate();
			return _items.Count;
		}

		public virtual void Activate()
		{
			if (_activator != null)
			{
				_activator.Activate();
			}
		}

		public virtual void Bind(IActivator activator)
		{
			if (activator == null || _activator != null)
			{
				throw new ArgumentNullException();
			}
			_activator = activator;
		}

		[System.NonSerialized]
		private IActivator _activator;
	}
}
