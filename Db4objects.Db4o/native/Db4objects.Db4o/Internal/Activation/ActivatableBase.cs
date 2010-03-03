/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */
using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Internal.Activation
{
	public class ActivatableBase : IActivatable
	{
		public void Bind(IActivator activator)
		{
			if (activator == _activator)
			{
				return;
			}

			if (_activator != null && activator != null)
			{
				throw new InvalidOperationException();
			}

			_activator = activator;
		}

		public void Activate(ActivationPurpose purpose)
		{
			if (_activator == null) return;
			_activator.Activate(purpose);
		}

		protected void ActivateForWrite()
		{
			Activate(ActivationPurpose.Write);
		}

		protected void ActivateForRead()
		{
			Activate(ActivationPurpose.Read);
		}

		[NonSerialized]
		private IActivator _activator;
	}
}