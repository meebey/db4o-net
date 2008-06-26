/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Common.TA
{
	public class MockActivator : IActivator
	{
		private int _readCount;

		private int _writeCount;

		public MockActivator()
		{
		}

		public virtual int Count()
		{
			return _readCount + _writeCount;
		}

		public virtual void Activate(ActivationPurpose purpose)
		{
			if (purpose == ActivationPurpose.Read)
			{
				++_readCount;
			}
			else
			{
				++_writeCount;
			}
		}

		public virtual int WriteCount()
		{
			return _writeCount;
		}

		public virtual int ReadCount()
		{
			return _readCount;
		}

		public static MockActivator ActivatorFor(IActivatable obj)
		{
			MockActivator activator = new MockActivator();
			obj.Bind(activator);
			return activator;
		}
	}
}
