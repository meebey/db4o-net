/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA
{
	/// <summary>Enables the Transparent Update and Transparent Activation behaviors.</summary>
	/// <remarks>Enables the Transparent Update and Transparent Activation behaviors.</remarks>
	public class TransparentPersistenceSupport : IConfigurationItem
	{
		public virtual void Apply(IInternalObjectContainer container)
		{
		}

		public virtual void Prepare(IConfiguration configuration)
		{
			configuration.Add(new TransparentActivationSupport());
		}
	}
}
