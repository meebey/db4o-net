/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Mocking;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o.Tests.Common.Activation
{
	/// <summary>
	/// An ActivationDepthProvider that records ActivationDepthProvider calls and
	/// delegates to another provider.
	/// </summary>
	/// <remarks>
	/// An ActivationDepthProvider that records ActivationDepthProvider calls and
	/// delegates to another provider.
	/// </remarks>
	public class MockActivationDepthProvider : MethodCallRecorder, IActivationDepthProvider
	{
		private readonly IActivationDepthProvider _delegate;

		public MockActivationDepthProvider()
		{
			_delegate = LegacyActivationDepthProvider.Instance;
		}

		public virtual IActivationDepth ActivationDepthFor(ClassMetadata classMetadata, ActivationMode
			 mode)
		{
			Record(new MethodCall("activationDepthFor", classMetadata, mode));
			return _delegate.ActivationDepthFor(classMetadata, mode);
		}

		public virtual IActivationDepth ActivationDepth(int depth, ActivationMode mode)
		{
			Record(new MethodCall("activationDepth", depth, mode));
			return _delegate.ActivationDepth(depth, mode);
		}
	}
}
