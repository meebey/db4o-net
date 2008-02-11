/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Internal.Activation
{
	public class DescendingActivationDepth : ActivationDepthImpl
	{
		private readonly TransparentActivationDepthProvider _provider;

		public DescendingActivationDepth(TransparentActivationDepthProvider provider, ActivationMode
			 mode) : base(mode)
		{
			_provider = provider;
		}

		public override IActivationDepth Descend(ClassMetadata metadata)
		{
			return _provider.ActivationDepthFor(metadata, _mode);
		}

		public override bool RequiresActivation()
		{
			return true;
		}
	}
}
