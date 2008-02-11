/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions.Mocking;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.Tests.Common.TA
{
	public class MockActivatable : IActivatable
	{
		[System.NonSerialized]
		private MethodCallRecorder _recorder;

		public virtual MethodCallRecorder Recorder()
		{
			if (null == _recorder)
			{
				_recorder = new MethodCallRecorder();
			}
			return _recorder;
		}

		public virtual void Bind(IActivator activator)
		{
			Record(new MethodCall("bind", activator));
		}

		public virtual void Activate(ActivationPurpose purpose)
		{
			Record(new MethodCall("activate", purpose));
		}

		private void Record(MethodCall methodCall)
		{
			Recorder().Record(methodCall);
		}
	}
}
