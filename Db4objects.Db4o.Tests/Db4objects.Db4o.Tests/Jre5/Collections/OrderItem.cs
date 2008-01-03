/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tests.Jre5.Collections
{
	public class OrderItem : IActivatable
	{
		private Db4objects.Db4o.Tests.Jre5.Collections.Product _product;

		private int _quantity;

		public OrderItem(Db4objects.Db4o.Tests.Jre5.Collections.Product product, int quantity
			)
		{
			_product = product;
			_quantity = quantity;
		}

		public virtual Db4objects.Db4o.Tests.Jre5.Collections.Product Product()
		{
			Activate(ActivationPurpose.Read);
			return _product;
		}

		public virtual int Quantity()
		{
			Activate(ActivationPurpose.Read);
			return _quantity;
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
