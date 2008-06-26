/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public class UnknownActivationDepth : IActivationDepth
	{
		public static readonly IActivationDepth Instance = new UnknownActivationDepth();

		private UnknownActivationDepth()
		{
		}

		public virtual ActivationMode Mode()
		{
			throw new InvalidOperationException();
		}

		public virtual IActivationDepth Descend(ClassMetadata metadata)
		{
			throw new InvalidOperationException();
		}

		public virtual bool RequiresActivation()
		{
			throw new InvalidOperationException();
		}
	}
}
