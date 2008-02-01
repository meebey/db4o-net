/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	/// <summary>Enables the Transparent Update and Transparent Activation behaviors.</summary>
	/// <remarks>Enables the Transparent Update and Transparent Activation behaviors.</remarks>
	public class TransparentPersistenceSupport : IConfigurationItem
	{
		private readonly IRollbackStrategy _rollbackStrategy;

		public TransparentPersistenceSupport(IRollbackStrategy rollbackStrategy)
		{
			_rollbackStrategy = rollbackStrategy;
		}

		public TransparentPersistenceSupport() : this(null)
		{
		}

		public virtual void Apply(IInternalObjectContainer container)
		{
		}

		public virtual void Prepare(IConfiguration configuration)
		{
			configuration.Add(new TransparentActivationSupport());
		}

		public virtual void Rollback(IObjectContainer container, object obj)
		{
			if (null == _rollbackStrategy)
			{
				return;
			}
			_rollbackStrategy.Rollback(container, obj);
		}
	}
}
