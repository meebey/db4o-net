/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Internal.Activation
{
	public class TransparentActivationDepthProvider : IActivationDepthProvider
	{
		public virtual IActivationDepth ActivationDepth(int depth, ActivationMode mode)
		{
			if (int.MaxValue == depth)
			{
				return new FullActivationDepth(mode);
			}
			return new FixedActivationDepth(depth, mode);
		}

		public virtual IActivationDepth ActivationDepthFor(ClassMetadata classMetadata, ActivationMode
			 mode)
		{
			if (IsTAAware(classMetadata))
			{
				return new NonDescendingActivationDepth(mode);
			}
			if (mode.IsPrefetch())
			{
				return new FixedActivationDepth(classMetadata.PrefetchActivationDepth(), mode);
			}
			return new DescendingActivationDepth(this, mode);
		}

		private bool IsTAAware(ClassMetadata classMetadata)
		{
			GenericReflector reflector = classMetadata.Reflector();
			return reflector.ForClass(typeof(IActivatable)).IsAssignableFrom(classMetadata.ClassReflector
				());
		}
	}
}
