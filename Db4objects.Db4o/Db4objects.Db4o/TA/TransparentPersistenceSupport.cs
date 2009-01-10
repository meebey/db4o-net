/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	/// <summary>Enables the Transparent Update and Transparent Activation behaviors.</summary>
	/// <remarks>Enables the Transparent Update and Transparent Activation behaviors.</remarks>
	public class TransparentPersistenceSupport : TransparentActivationSupport
	{
		private readonly IRollbackStrategy _rollbackStrategy;

		public TransparentPersistenceSupport(IRollbackStrategy rollbackStrategy)
		{
			_rollbackStrategy = rollbackStrategy;
		}

		public TransparentPersistenceSupport() : this(null)
		{
		}

		public override void Apply(IInternalObjectContainer container)
		{
			base.Apply(container);
			EnableTransparentPersistenceFor(container);
		}

		private void EnableTransparentPersistenceFor(IInternalObjectContainer container)
		{
			ITransparentActivationDepthProvider provider = (ITransparentActivationDepthProvider
				)ActivationProvider(container);
			provider.EnableTransparentPersistenceSupportFor(container, _rollbackStrategy);
		}
	}
}
