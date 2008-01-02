/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tests.Jre5.Collections
{
	public class Product : IActivatable
	{
		private string _code;

		private string _description;

		public Product(string code, string description)
		{
			_code = code;
			_description = description;
		}

		public virtual string Code()
		{
			Activate(ActivationPurpose.READ);
			return _code;
		}

		public virtual string Description()
		{
			Activate(ActivationPurpose.READ);
			return _description;
		}

		public override bool Equals(object p)
		{
			Activate(ActivationPurpose.READ);
			if (p == null)
			{
				return false;
			}
			if (p.GetType() != this.GetType())
			{
				return false;
			}
			Db4objects.Db4o.Tests.Jre5.Collections.Product rhs = (Db4objects.Db4o.Tests.Jre5.Collections.Product
				)p;
			return rhs._code == _code;
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
