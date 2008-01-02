/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tests.Common.TA.Sample
{
	public class City : IActivatable
	{
		public string _name;

		[System.NonSerialized]
		private IActivator _activator;

		public virtual void Activate(ActivationPurpose purpose)
		{
			if (_activator != null)
			{
				_activator.Activate(purpose);
			}
		}

		public virtual void Bind(IActivator activator)
		{
			if (_activator != null || activator == null)
			{
				throw new InvalidOperationException();
			}
			_activator = activator;
		}
	}
}
